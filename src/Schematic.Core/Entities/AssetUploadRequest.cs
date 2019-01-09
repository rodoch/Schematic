using Microsoft.AspNetCore.Http;

namespace Schematic.Core
{
    public class AssetUploadRequest
    {
        public string ContainerName { get; set; }
        
        public IFormFile File { get; set; }

        public string FileName { get; set; }

        public string FilePath { get; set; }
    }
}