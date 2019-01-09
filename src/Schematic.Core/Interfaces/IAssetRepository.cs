using System.Collections.Generic;
using System.Threading.Tasks;

namespace Schematic.Core
{
    public interface IAssetRepository
    {
        Task<int> CreateAsync(Asset asset, int userID);

        Task<Asset> ReadAsync(int id);

        Task<int> UpdateAsync(Asset asset, int userID);
        
        Task<int> DeleteAsync(int id, int userID);
    }
}