using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using Dapper;
using Schematic.Core;

namespace Schematic.BaseInfrastructure.Sqlite
{
    public class AssetRepository : IAssetRepository
    {
        private readonly string _connectionString;

        public AssetRepository(IOptionsMonitor<SchematicSettings> settings)
        {
            _connectionString = settings.CurrentValue.DataStore.ConnectionString;
        }

        public async Task<int> CreateAsync(Asset asset, int userID)
        {
            const string sql = @"INSERT INTO Assets (FileName, ContentType, Title, DateCreated, CreatedBy) 
                VALUES (@FileName, @ContentType, @Title, @DateCreated, @CreatedBy);
                SELECT last_insert_rowid();";

            using (IDbConnection db = new SqliteConnection(_connectionString))
            {
                var assetID = await db.QueryAsync<int>(sql, new { FileName = asset.FileName, ContentType = asset.ContentType, 
                   Title = asset.Title, DateCreated = asset.DateCreated, CreatedBy = asset.CreatedBy });
                return assetID.FirstOrDefault();
            }
        }

        public async Task<int> DeleteAsync(int id, int userID)
        {
            const string sql = @"DELETE FROM Assets WHERE ID = @ID";

            using (IDbConnection db = new SqliteConnection(_connectionString))
            {
                return await db.ExecuteAsync(sql, new { ID = id });
            }
        }

        public async Task<Asset> ReadAsync(int id)
        {
            const string sql = @"SELECT * FROM Assets WHERE FileName = @FileName LIMIT 1";

            using (IDbConnection db = new SqliteConnection(_connectionString))
            {
                var results = await db.QueryAsync<Asset>(sql, new { ID = id });
                return results.FirstOrDefault();
            }
        }

        public async Task<int> UpdateAsync(Asset asset, int userID)
        {
            const string sql = @"UPDATE SET FileName = @FileName, ContentType = @ContentType, Title = @Title 
                WHERE ID = @ID";

            using (IDbConnection db = new SqliteConnection(_connectionString))
            {
                return await db.ExecuteAsync(sql, new { ID = asset.ID, FileName = asset.FileName, 
                    ContentType = asset.ContentType, Title = asset.Title });
            }
        }
    }
}