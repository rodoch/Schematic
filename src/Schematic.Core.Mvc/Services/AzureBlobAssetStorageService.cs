using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Schematic.Core.Mvc
{
    public class AzureBlobAssetStorageService : IAssetStorageService
    {
        private IOptionsMonitor<SchematicSettings> _settings;

        public AzureBlobAssetStorageService(IOptionsMonitor<SchematicSettings> settings)
        {
            _settings = settings;
        }

        protected async Task<CloudBlobContainer> GetContainerAsync(string containerName)
        {
            var storageConnectionString = _settings.CurrentValue.CloudStorage.AzureStorage.StorageAccount;

            if (!CloudStorageAccount.TryParse(storageConnectionString, out CloudStorageAccount storageAccount))
                return null;

            var cloudBlobClient = storageAccount.CreateCloudBlobClient();
            var container = cloudBlobClient.GetContainerReference(containerName);

            if (!await container.ExistsAsync())
                throw new StorageException();

            return container;
        }

        public async Task<byte[]> GetAssetAsync(AssetDownloadRequest asset)
        {
            var container = await GetContainerAsync(asset.ContainerName);
            var blockBlob = container.GetBlockBlobReference(asset.FileName);

            if (!await blockBlob.ExistsAsync())
                return null;

            using (var blobStream = blockBlob.OpenRead())
            {
                blobStream.Seek(0, SeekOrigin.Begin);
                byte[] output = new byte[blobStream.Length];
                await blobStream.ReadAsync(output, 0, output.Length);
                return output;
            }
        }

        public async Task<AssetUploadResult> SaveAssetAsync(AssetUploadRequest asset)
        {
            var container = await GetContainerAsync(asset.ContainerName);

            try
            {
                var blockBlob = container.GetBlockBlobReference(asset.FileName);

                using (var blobStream = asset.File.OpenReadStream())
                    await blockBlob.UploadFromStreamAsync(blobStream);

                return AssetUploadResult.Success;
            }
            catch
            {
                return AssetUploadResult.Failure;
            }
        }
    }
}