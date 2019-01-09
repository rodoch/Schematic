using System.Collections.Generic;
using System.Threading.Tasks;

namespace Schematic.Core
{
    public interface IImageAssetRepository
    {
        Task<int> CreateAsync(ImageAsset asset, int userID);

        Task<ImageAsset> ReadAsync(int id);

        Task<int> UpdateAsync(ImageAsset asset, int userID);
        
        Task<int> DeleteAsync(int id, int userID);
    }
}