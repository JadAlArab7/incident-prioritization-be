using System.Data;
using Incident.Infrastructure;
using Incident.Models;

namespace Incident.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IDbHelper _dbHelper;

    public UserRepository(IDbHelper dbHelper)
    {
        _dbHelper = dbHelper;
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        var results = new List<User>();
        using var connection = _dbHelper.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT 
                u.id, u.username, u.password_hash, u.password_salt,
                u.role_id, u.created_at, u.updated_at,
                r.id as role_id_ref, r.name as role_name
            FROM incident.users u
            LEFT JOIN incident.roles r ON u.role_id = r.id
            ORDER BY u.username";

        using var reader = await ExecuteReaderAsync(command);
        while (await ReadAsync(reader))
        {
            results.Add(MapUserFromReader(reader));
        }

        return results;
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        using var connection = _dbHelper.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT 
                u.id, u.username, u.password_hash, u.password_salt,
                u.role_id, u.created_at, u.updated_at,
                r.id as role_id_ref, r.name as role_name
            FROM incident.users u
            LEFT JOIN incident.roles r ON u.role_id = r.id
            WHERE u.id = @Id";

        AddParameter(command, "@Id", id);

        using var reader = await ExecuteReaderAsync(command);
        if (await ReadAsync(reader))
        {
            return MapUserFromReader(reader);
        }

        return null;
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        using var connection = _dbHelper.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT 
                u.id, u.username, u.password_hash, u.password_salt,
                u.role_id, u.created_at, u.updated_at,
                r.id as role_id_ref, r.name as role_name
            FROM incident.users u
            LEFT JOIN incident.roles r ON u.role_id = r.id
            WHERE u.username = @Username";

        AddParameter(command, "@Username", username);

        using var reader = await ExecuteReaderAsync(command);
        if (await ReadAsync(reader))
        {
            return MapUserFromReader(reader);
        }

        return null;
    }

    public async Task<IEnumerable<User>> GetByRoleAsync(string roleName)
    {
        var results = new List<User>();
        using var connection = _dbHelper.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT 
                u.id, u.username, u.password_hash, u.password_salt,
                u.role_id, u.created_at, u.updated_at,
                r.id as role_id_ref, r.name as role_name
            FROM incident.users u
            LEFT JOIN incident.roles r ON u.role_id = r.id
            WHERE r.name = @RoleName
            ORDER BY u.username";

        AddParameter(command, "@RoleName", roleName);

        using var reader = await ExecuteReaderAsync(command);
        while (await ReadAsync(reader))
        {
            results.Add(MapUserFromReader(reader));
        }

        return results;
    }

    public async Task<User?> CreateAsync(User user)
    {
        using var connection = _dbHelper.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO incident.users (id, username, password_hash, password_salt, role_id, created_at, updated_at)
            VALUES (@Id, @Username, @PasswordHash, @PasswordSalt, @RoleId, @CreatedAt, @UpdatedAt)
            RETURNING id, username, password_hash, password_salt, role_id, created_at, updated_at";

        AddParameter(command, "@Id", user.Id);
        AddParameter(command, "@Username", user.Username);
        AddParameter(command, "@PasswordHash", user.PasswordHash);
        AddParameter(command, "@PasswordSalt", user.PasswordSalt);
        AddParameter(command, "@RoleId", user.RoleId);
        AddParameter(command, "@CreatedAt", user.CreatedAt);
        AddParameter(command, "@UpdatedAt", user.UpdatedAt);

        using var reader = await ExecuteReaderAsync(command);
        if (await ReadAsync(reader))
        {
            return new User
            {
                Id = reader.GetGuid(reader.GetOrdinal("id")),
                Username = reader.GetString(reader.GetOrdinal("username")),
                PasswordHash = (byte[])reader["password_hash"],
                PasswordSalt = (byte[])reader["password_salt"],
                RoleId = reader.GetGuid(reader.GetOrdinal("role_id")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
                UpdatedAt = reader.GetDateTime(reader.GetOrdinal("updated_at"))
            };
        }

        return null;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        using var connection = _dbHelper.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM incident.users WHERE id = @Id";

        AddParameter(command, "@Id", id);

        var rowsAffected = await ExecuteNonQueryAsync(command);
        return rowsAffected > 0;
    }

    private static User MapUserFromReader(IDataReader reader)
    {
        var user = new User
        {
            Id = reader.GetGuid(reader.GetOrdinal("id")),
            Username = reader.GetString(reader.GetOrdinal("username")),
            PasswordHash = (byte[])reader["password_hash"],
            PasswordSalt = (byte[])reader["password_salt"],
            RoleId = reader.GetGuid(reader.GetOrdinal("role_id")),
            CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
            UpdatedAt = reader.GetDateTime(reader.GetOrdinal("updated_at"))
        };

        var roleIdOrdinal = reader.GetOrdinal("role_id_ref");
        if (!reader.IsDBNull(roleIdOrdinal))
        {
            user.Role = new Role
            {
                Id = reader.GetGuid(roleIdOrdinal),
                Name = reader.GetString(reader.GetOrdinal("role_name"))
            };
        }

        return user;
    }

    private static void AddParameter(IDbCommand command, string name, object value)
    {
        var param = command.CreateParameter();
        param.ParameterName = name;
        param.Value = value ?? DBNull.Value;
        command.Parameters.Add(param);
    }

    private static async Task<IDataReader> ExecuteReaderAsync(IDbCommand command)
    {
        if (command is System.Data.Common.DbCommand dbCommand)
        {
            return await dbCommand.ExecuteReaderAsync();
        }
        return command.ExecuteReader();
    }

    private static async Task<bool> ReadAsync(IDataReader reader)
    {
        if (reader is System.Data.Common.DbDataReader dbReader)
        {
            return await dbReader.ReadAsync();
        }
        return reader.Read();
    }

    private static async Task<int> ExecuteNonQueryAsync(IDbCommand command)
    {
        if (command is System.Data.Common.DbCommand dbCommand)
        {
            return await dbCommand.ExecuteNonQueryAsync();
        }
        return command.ExecuteNonQuery();
    }
}