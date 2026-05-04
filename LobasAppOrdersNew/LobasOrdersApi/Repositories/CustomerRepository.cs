using LobasOrdersApi.Models;
using LobasOrdersApi.Repositories.Interfaces;
using Microsoft.Data.SqlClient;

namespace LobasOrdersApi.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly IConfiguration _configuration;

        public CustomerRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public List<Customer> GetAll()
        {
            List<Customer> customers = new List<Customer>();

            string connectionString = _configuration.GetConnectionString("DefaultConnection")!;

            using SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            string query = @"
                SELECT Id, Name, Email, Phone, CreatedAt, IsActive
                FROM Customers
                WHERE IsActive = 1
                ORDER BY Id DESC";

            using SqlCommand command = new SqlCommand(query, connection);
            using SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                customers.Add(MapCustomer(reader));
            }

            return customers;
        }

        public Customer? GetById(int id)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection")!;

            using SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            string query = @"
                SELECT Id, Name, Email, Phone, CreatedAt, IsActive
                FROM Customers
                WHERE Id = @Id AND IsActive = 1";

            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            using SqlDataReader reader = command.ExecuteReader();

            if (!reader.Read())
            {
                return null;
            }

            return MapCustomer(reader);
        }

        public List<Customer> Search(string searchTerm)
        {
            List<Customer> customers = new List<Customer>();

            string connectionString = _configuration.GetConnectionString("DefaultConnection")!;

            using SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            string query = @"
                SELECT Id, Name, Email, Phone, CreatedAt, IsActive
                FROM Customers
                WHERE IsActive = 1
                AND (
                    Name LIKE @SearchTerm
                    OR Email LIKE @SearchTerm
                    OR Phone LIKE @SearchTerm
                )
                ORDER BY Id DESC";

            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@SearchTerm", $"%{searchTerm}%");

            using SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                customers.Add(MapCustomer(reader));
            }

            return customers;
        }

        public int Create(Customer customer)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection")!;

            using SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            string query = @"
                INSERT INTO Customers (Name, Email, Phone)
                VALUES (@Name, @Email, @Phone);

                SELECT SCOPE_IDENTITY();";

            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Name", customer.Name);
            command.Parameters.AddWithValue("@Email", (object?)customer.Email ?? DBNull.Value);
            command.Parameters.AddWithValue("@Phone", (object?)customer.Phone ?? DBNull.Value);

            return Convert.ToInt32(command.ExecuteScalar());
        }

        public bool Update(int id, Customer customer)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection")!;

            using SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            string query = @"
                UPDATE Customers
                SET Name = @Name,
                    Email = @Email,
                    Phone = @Phone
                WHERE Id = @Id AND IsActive = 1";

            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@Name", customer.Name);
            command.Parameters.AddWithValue("@Email", (object?)customer.Email ?? DBNull.Value);
            command.Parameters.AddWithValue("@Phone", (object?)customer.Phone ?? DBNull.Value);

            int rowsAffected = command.ExecuteNonQuery();

            return rowsAffected > 0;
        }

        public bool Delete(int id)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection")!;

            using SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            string query = @"
                UPDATE Customers
                SET IsActive = 0
                WHERE Id = @Id AND IsActive = 1";

            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            int rowsAffected = command.ExecuteNonQuery();

            return rowsAffected > 0;
        }

        private Customer MapCustomer(SqlDataReader reader)
        {
            return new Customer
            {
                Id = Convert.ToInt32(reader["Id"]),
                Name = reader["Name"].ToString() ?? "",
                Email = reader["Email"] == DBNull.Value ? null : reader["Email"].ToString(),
                Phone = reader["Phone"] == DBNull.Value ? null : reader["Phone"].ToString(),
                CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                IsActive = Convert.ToBoolean(reader["IsActive"])
            };
        }
    }
}