using LobasOrdersApi.Models;
using LobasOrdersApi.Repositories.Interfaces;
using Microsoft.Data.SqlClient;

namespace LobasOrdersApi.Repositories
{
    public class AddressRepository : IAddressRepository
    {
        private readonly IConfiguration _configuration;

        public AddressRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public List<Address> GetAll()
        {
            List<Address> addresses = new List<Address>();

            string connectionString = _configuration.GetConnectionString("DefaultConnection")!;

            using SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            string query = @"
                SELECT Id, CustomerId, Street, City, State, ZipCode, IsMain
                FROM Addresses
                ORDER BY Id DESC";

            using SqlCommand command = new SqlCommand(query, connection);
            using SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                addresses.Add(MapAddress(reader));
            }

            return addresses;
        }

        public Address? GetById(int id)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection")!;

            using SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            string query = @"
                SELECT Id, CustomerId, Street, City, State, ZipCode, IsMain
                FROM Addresses
                WHERE Id = @Id";

            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            using SqlDataReader reader = command.ExecuteReader();

            if (!reader.Read())
            {
                return null;
            }

            return MapAddress(reader);
        }

        public List<Address> GetByCustomerId(int customerId)
        {
            List<Address> addresses = new List<Address>();

            string connectionString = _configuration.GetConnectionString("DefaultConnection")!;

            using SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            string query = @"
                SELECT Id, CustomerId, Street, City, State, ZipCode, IsMain
                FROM Addresses
                WHERE CustomerId = @CustomerId
                ORDER BY IsMain DESC, Id DESC";

            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@CustomerId", customerId);

            using SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                addresses.Add(MapAddress(reader));
            }

            return addresses;
        }

        public int CountByCustomerId(int customerId)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection")!;

            using SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            string query = @"
                SELECT COUNT(*)
                FROM Addresses
                WHERE CustomerId = @CustomerId";

            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@CustomerId", customerId);

            return Convert.ToInt32(command.ExecuteScalar());
        }

        public int Create(Address address)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection")!;

            using SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            string query = @"
                INSERT INTO Addresses (CustomerId, Street, City, State, ZipCode, IsMain)
                VALUES (@CustomerId, @Street, @City, @State, @ZipCode, @IsMain);

                SELECT SCOPE_IDENTITY();";

            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@CustomerId", address.CustomerId);
            command.Parameters.AddWithValue("@Street", address.Street);
            command.Parameters.AddWithValue("@City", address.City);
            command.Parameters.AddWithValue("@State", address.State);
            command.Parameters.AddWithValue("@ZipCode", address.ZipCode);
            command.Parameters.AddWithValue("@IsMain", address.IsMain);

            return Convert.ToInt32(command.ExecuteScalar());
        }

        public bool Update(int id, Address address)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection")!;

            using SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            string query = @"
                UPDATE Addresses
                SET Street = @Street,
                    City = @City,
                    State = @State,
                    ZipCode = @ZipCode,
                    IsMain = @IsMain
                WHERE Id = @Id";

            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@Street", address.Street);
            command.Parameters.AddWithValue("@City", address.City);
            command.Parameters.AddWithValue("@State", address.State);
            command.Parameters.AddWithValue("@ZipCode", address.ZipCode);
            command.Parameters.AddWithValue("@IsMain", address.IsMain);

            int rowsAffected = command.ExecuteNonQuery();

            return rowsAffected > 0;
        }

        public bool Delete(int id)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection")!;

            using SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            string query = @"
                DELETE FROM Addresses
                WHERE Id = @Id";

            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            int rowsAffected = command.ExecuteNonQuery();

            return rowsAffected > 0;
        }

        public bool ClearMainAddress(int customerId)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection")!;

            using SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            string query = @"
                UPDATE Addresses
                SET IsMain = 0
                WHERE CustomerId = @CustomerId";

            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@CustomerId", customerId);

            command.ExecuteNonQuery();

            return true;
        }

        private Address MapAddress(SqlDataReader reader)
        {
            return new Address
            {
                Id = Convert.ToInt32(reader["Id"]),
                CustomerId = Convert.ToInt32(reader["CustomerId"]),
                Street = reader["Street"].ToString() ?? "",
                City = reader["City"].ToString() ?? "",
                State = reader["State"].ToString() ?? "",
                ZipCode = reader["ZipCode"].ToString() ?? "",
                IsMain = Convert.ToBoolean(reader["IsMain"])
            };
        }
    }
}
