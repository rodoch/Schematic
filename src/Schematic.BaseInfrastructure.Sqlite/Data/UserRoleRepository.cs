using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using Dapper;
using Schematic.Core;
using Schematic.Identity;

namespace Schematic.BaseInfrastructure.Sqlite
{
    public class UserRoleRepository : IUserRoleRepository<UserRole>
    {
        protected readonly string ConnectionString;

        public UserRoleRepository(IOptionsMonitor<SchematicSettings> settings)
        {
            ConnectionString = settings.CurrentValue.DataStore.ConnectionString;
        }

        public async Task<List<UserRole>> ListAsync()
        {
            const string sql = @"SELECT * FROM UserRoles ORDER BY DisplayTitle, Name";

            using (IDbConnection db = new SqliteConnection(ConnectionString))
            {
                var roles = await db.QueryAsync<UserRole>(sql);
                return roles.ToList();
            }
        }
    }
}