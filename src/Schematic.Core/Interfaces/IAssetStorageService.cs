using System.IO;
using System.Threading.Tasks;

namespace Schematic.Core
{
    public interface IAssetStorageService
    {
        Task<byte[]> GetAssetAsync(AssetDownloadRequest asset);

        Task<AssetUploadResult> SaveAssetAsync(AssetUploadRequest asset);
    }
}