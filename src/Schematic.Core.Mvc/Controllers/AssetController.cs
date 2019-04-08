using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Ansa.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Options;
using Schematic.Core;
using Schematic.Core.Mvc;

namespace Schematic.Controllers
{
    [Route("[controller]")]
    [Authorize]
    public class AssetController : Controller
    {
        private readonly IOptionsMonitor<SchematicSettings> _settings;
        private readonly IAssetRepository _assetRepository;
        private readonly IImageAssetRepository _imageRepository;
        private readonly IAssetStorageService _assetStorageService;

        public AssetController(
            IOptionsMonitor<SchematicSettings> settings,
            IAssetRepository assetRepository,
            IImageAssetRepository imageRepository,
            IAssetStorageService assetStorageService)
        {
            _settings = settings;
            _assetRepository = assetRepository;
            _imageRepository = imageRepository;
            _assetStorageService = assetStorageService;
        }

        protected string CloudContainerName { get; set; }
        protected string FileName { get; set; }
        protected string FileExtension { get; set; }
        protected string FilePath { get; set; }
        protected long TotalSize { get; set; }

        protected ClaimsIdentity ClaimsIdentity => User.Identity as ClaimsIdentity;
        protected int UserID => int.Parse(ClaimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        [Route("{fileName}")]
        [HttpGet]
        public async Task<IActionResult> DownloadAsync(string fileName, string container = "", string attachment = "")
        {
            FilePath = Path.Combine(_settings.CurrentValue.AssetDirectory, fileName);

            if (container.HasValue())
                CloudContainerName = container;

            var provider = new FileExtensionContentTypeProvider();

            if (!provider.TryGetContentType(fileName, out string contentType))
                contentType = "application/octet-stream";

            var downloadRequest = new AssetDownloadRequest()
            {
                ContainerName = CloudContainerName,
                FileName = fileName,
                FilePath = this.FilePath,
            };

            var stream = await _assetStorageService.GetAssetAsync(downloadRequest);

            if (stream is null)
                return NotFound();

            if (attachment.HasValue() && attachment == "true")
            {
                var fileNameEncoded = HttpUtility.UrlEncode(fileName, System.Text.Encoding.UTF8);
                Response.Headers["Content-Disposition"] = "attachment; filename=\"" + fileNameEncoded  + "\"";
            }

            return File(stream, contentType);
        }

        [Route("upload")]
        [HttpPost]
        public async Task<IActionResult> UploadAsync(List<IFormFile> files, string container = "")
        {
            var response = new List<AssetUploadResponse>();
            TotalSize = files.Sum(f => f.Length);

            if (container.HasValue())
                CloudContainerName = container;

            foreach (var file in files)
            {
                if (file.Length == 0)
                    continue;

                var assetDirectory = _settings.CurrentValue.AssetDirectory;
                var assetWebPath = _settings.CurrentValue.AssetWebPath;
                FileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                FileName = (FileName.HasValue()) ? FileName : Convert.ToString(Guid.NewGuid());
                FilePath = Path.Combine(assetDirectory, FileName);

                var uploadRequest = new AssetUploadRequest()
                {
                    ContainerName = CloudContainerName,
                    File = file,
                    FileName = this.FileName,
                    FilePath = this.FilePath
                };

                //TODO: TryGetAudioAsset(out AudioAsset audio), TryGetVideoAsset(out VideoAsset video)
                
                if (file.TryGetImageAsset(out ImageAsset image))
                {
                    // save the file to storage
                    var saveImageAsset = await _assetStorageService.SaveAssetAsync(uploadRequest);

                    if (saveImageAsset != AssetUploadResult.Success)
                        continue;

                    // save image metadata to data store
                    image.FileName = this.FileName;
                    image.ContentType = file.ContentType.ToLower();
                    image.DateCreated = DateTime.UtcNow;
                    image.CreatedBy = UserID;

                    var imageID = await _imageRepository.CreateAsync(image, UserID);

                    // return upload report to client
                    var imageAssetResponse = new AssetUploadResponse()
                    {
                        ID = imageID,
                        FileName = this.FileName,
                        Size = file.Length,
                        Uri = file.GetAssetUri(assetWebPath, this.FileName)
                    };

                    response.Add(imageAssetResponse);
                }
                else
                {
                    // save the file to storage
                    var saveAsset = await _assetStorageService.SaveAssetAsync(uploadRequest);

                    if (saveAsset != AssetUploadResult.Success)
                        continue;
                    
                    // save file metadata to data store
                    var asset = new Asset()
                    {
                        FileName = this.FileName,
                        ContentType = file.ContentType.ToLower(),
                        DateCreated = DateTime.UtcNow,
                        CreatedBy = UserID
                    };

                    var assetID = await _assetRepository.CreateAsync(asset, UserID);
                    
                    // return upload report to client
                    var assetResponse = new AssetUploadResponse()
                    {
                        ID = assetID,
                        FileName = this.FileName,
                        Size = file.Length,
                        Uri = file.GetAssetUri(assetWebPath, this.FileName)
                    };

                    response.Add(assetResponse);
                }
            }

            var assetUri = HttpUtility.UrlEncode(response[0].Uri, System.Text.Encoding.UTF8);
            return Created(assetUri, response);
        }
    }
}