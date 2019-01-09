using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using Dapper;
using Schematic.Core;
using Schematic.Identity;

namespace Schematic.BaseInfrastructure.Sqlite
{
    public class UserPasswordRepository : IUserPasswordRepository<UserDTO>
    {
        private readonly IOptionsMonitor<SchematicSettings> _settings;
        private readonly string _connectionString;

        public UserPasswordRepository(IOptionsMonitor<SchematicSettings> settings)
        {
            _settings = settings;
            _connectionString = _settings.CurrentValue.DataStore.ConnectionString;
        }

        public async Task<bool> CreatePasswordTokenAsync(UserDTO user, string token)
        {
            const string sql = @"INSERT INTO UsersPasswordToken (UserID, Email, Token, DateCreated) 
                VALUES (@UserID, @Email, @Token, @DateCreated)";
            
            using (var db = new SqliteConnection(_connectionString))
            {
                var createToken = await db.ExecuteAsync(sql, new { UserID = user.ID, Email = user.Email, Token = token, 
                    DateCreated = DateTime.UtcNow });
                return (createToken > 0) ? true : false;
            }
        }

        public async Task<bool> SavePasswordTokenAsync(UserDTO user, string token)
        {
            const string sql = @"INSERT INTO UsersPasswordToken (UserID, Email, Token, NotificationsSent, DateCreated) 
                VALUES (@UserID, @Email, @Token, @NotificationsSent, @DateCreated)";

            using (var db = new SqliteConnection(_connectionString))
            {
                var tokenSaved = await db.ExecuteAsync(sql, new { UserID = user.ID, Email = user.Email, Token = token, 
                    NotificationsSent = 0, DateCreated = DateTime.UtcNow });
                return (tokenSaved > 0) ? true : false;
            }
        }

        public async Task<bool> SetPasswordAsync(UserDTO user, string passHash)
        {
            const string sql = @"UPDATE Users SET PassHash = @PassHash WHERE ID = @ID";

            using (var db = new SqliteConnection(_connectionString))
            {
                var passwordSet = await db.ExecuteAsync(sql, new { ID = user.ID, PassHash = passHash });
                return (passwordSet > 0) ? true : false;
            }
        }

        public async Task<TokenVerificationResult> ValidatePasswordTokenAsync(string email, string token)
        {
            const string sql = @"SELECT ID, DateCreated FROM UsersPasswordToken 
                WHERE Email = @Email AND Token = @Token LIMIT 1";

            using (var db = new SqliteConnection(_connectionString))
            {
                var result = await db.QueryAsync(sql, new { Email = email, Token = token });
                var tokenResult = result.FirstOrDefault();

                if (tokenResult is null || tokenResult.Count <= 0)
                {
                    return TokenVerificationResult.Invalid;
                }

                var timeCreated = DateTime.Parse(tokenResult.DateCreated);
                var timeLimit = _settings.CurrentValue.SetPasswordTimeLimitHours;

                return (timeCreated < DateTime.UtcNow.Subtract(TimeSpan.FromHours(timeLimit)))
                    ? TokenVerificationResult.Expired
                    : TokenVerificationResult.Success;
            }
        }
    }
}