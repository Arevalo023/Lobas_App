using LobasOrdersApi.Models;
using LobasOrdersApi.Repositories.Interfaces;
using Microsoft.Data.SqlClient;

namespace LobasOrdersApi.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IConfiguration _configuration;

        public UserRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public List<User> GetAll()
        {
            List<User> users = new List<User>();

            string connectionString = _configuration.GetConnectionString("DefaultConnection")!;

            using SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            string query = @"
                SELECT Id, Name, Email, PasswordHash, AuthProvider, ProviderUserId,
                       BiometricEnabled, CreatedAt, LastNameChangedAt, IsActive
                FROM Users
                WHERE IsActive = 1
                ORDER BY Id DESC";

            using SqlCommand command = new SqlCommand(query, connection);
            using SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                users.Add(MapUser(reader));
            }

            return users;
        }

        public User? GetById(int id)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection")!;

            using SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            string query = @"
                SELECT Id, Name, Email, PasswordHash, AuthProvider, ProviderUserId,
                       BiometricEnabled, CreatedAt, LastNameChangedAt, IsActive
                FROM Users
                WHERE Id = @Id AND IsActive = 1";

            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            using SqlDataReader reader = command.ExecuteReader();

            if (!reader.Read())
            {
                return null;
            }

            return MapUser(reader);
        }

        public User? GetByEmail(string email)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection")!;

            using SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            string query = @"
                SELECT Id, Name, Email, PasswordHash, AuthProvider, ProviderUserId,
                       BiometricEnabled, CreatedAt, LastNameChangedAt, IsActive
                FROM Users
                WHERE Email = @Email AND IsActive = 1";

            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Email", email);

            using SqlDataReader reader = command.ExecuteReader();

            if (!reader.Read())
            {
                return null;
            }

            return MapUser(reader);
        }

        public User? GetByProvider(string authProvider, string providerUserId)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection")!;

            using SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            string query = @"
                SELECT Id, Name, Email, PasswordHash, AuthProvider, ProviderUserId,
                       BiometricEnabled, CreatedAt, LastNameChangedAt, IsActive
                FROM Users
                WHERE AuthProvider = @AuthProvider
                  AND ProviderUserId = @ProviderUserId
                  AND IsActive = 1";

            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@AuthProvider", authProvider);
            command.Parameters.AddWithValue("@ProviderUserId", providerUserId);

            using SqlDataReader reader = command.ExecuteReader();

            if (!reader.Read())
            {
                return null;
            }

            return MapUser(reader);
        }

        public int Create(User user)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection")!;

            using SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            string query = @"
                INSERT INTO Users (Name, Email, PasswordHash, AuthProvider, ProviderUserId, BiometricEnabled)
                VALUES (@Name, @Email, @PasswordHash, @AuthProvider, @ProviderUserId, @BiometricEnabled);

                SELECT SCOPE_IDENTITY();";

            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Name", user.Name);
            command.Parameters.AddWithValue("@Email", user.Email);
            command.Parameters.AddWithValue("@PasswordHash", (object?)user.PasswordHash ?? DBNull.Value);
            command.Parameters.AddWithValue("@AuthProvider", user.AuthProvider);
            command.Parameters.AddWithValue("@ProviderUserId", (object?)user.ProviderUserId ?? DBNull.Value);
            command.Parameters.AddWithValue("@BiometricEnabled", user.BiometricEnabled);

            return Convert.ToInt32(command.ExecuteScalar());
        }

        public bool Update(int id, User user)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection")!;

            using SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            string query = @"
                UPDATE Users
                SET Name = @Name,
                    Email = @Email,
                    BiometricEnabled = @BiometricEnabled
                WHERE Id = @Id AND IsActive = 1";

            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@Name", user.Name);
            command.Parameters.AddWithValue("@Email", user.Email);
            command.Parameters.AddWithValue("@BiometricEnabled", user.BiometricEnabled);

            int rowsAffected = command.ExecuteNonQuery();

            return rowsAffected > 0;
        }

        public bool UpdateName(int id, string name, DateTime changedAt)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection")!;

            using SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            string query = @"
                UPDATE Users
                SET Name = @Name,
                    LastNameChangedAt = @LastNameChangedAt
                WHERE Id = @Id AND IsActive = 1";

            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@Name", name);
            command.Parameters.AddWithValue("@LastNameChangedAt", changedAt);

            int rowsAffected = command.ExecuteNonQuery();

            return rowsAffected > 0;
        }

        public bool Delete(int id)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection")!;

            using SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            string query = @"
                UPDATE Users
                SET IsActive = 0
                WHERE Id = @Id AND IsActive = 1";

            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            int rowsAffected = command.ExecuteNonQuery();

            return rowsAffected > 0;
        }

        public bool UpdateBiometricStatus(int id, bool biometricEnabled)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection")!;

            using SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            string query = @"
                UPDATE Users
                SET BiometricEnabled = @BiometricEnabled
                WHERE Id = @Id AND IsActive = 1";

            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@BiometricEnabled", biometricEnabled);

            int rowsAffected = command.ExecuteNonQuery();

            return rowsAffected > 0;
        }

        private User MapUser(SqlDataReader reader)
        {
            return new User
            {
                Id = Convert.ToInt32(reader["Id"]),
                Name = reader["Name"].ToString() ?? "",
                Email = reader["Email"].ToString() ?? "",
                PasswordHash = reader["PasswordHash"] == DBNull.Value ? null : reader["PasswordHash"].ToString(),
                AuthProvider = reader["AuthProvider"].ToString() ?? "Local",
                ProviderUserId = reader["ProviderUserId"] == DBNull.Value ? null : reader["ProviderUserId"].ToString(),
                BiometricEnabled = Convert.ToBoolean(reader["BiometricEnabled"]),
                CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                LastNameChangedAt = reader["LastNameChangedAt"] == DBNull.Value
                    ? null
                    : Convert.ToDateTime(reader["LastNameChangedAt"]),
                IsActive = Convert.ToBoolean(reader["IsActive"])
            };
        }
    }
}
