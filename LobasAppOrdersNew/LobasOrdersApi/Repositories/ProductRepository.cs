using LobasOrdersApi.Models;
using LobasOrdersApi.Repositories.Interfaces;
using Microsoft.Data.SqlClient;

namespace LobasOrdersApi.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly IConfiguration _configuration;

        public ProductRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public List<Product> GetAll()
        {
            List<Product> products = new List<Product>();

            string connectionString = _configuration.GetConnectionString("DefaultConnection")!;

            using SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            string query = @"
                SELECT Id, Name, Description, Price, Stock, IsActive, CreatedAt
                FROM Products
                WHERE IsActive = 1
                ORDER BY Id DESC";

            using SqlCommand command = new SqlCommand(query, connection);
            using SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                products.Add(MapProduct(reader));
            }

            return products;
        }

        public Product? GetById(int id)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection")!;

            using SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            string query = @"
                SELECT Id, Name, Description, Price, Stock, IsActive, CreatedAt
                FROM Products
                WHERE Id = @Id AND IsActive = 1";

            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            using SqlDataReader reader = command.ExecuteReader();

            if (!reader.Read())
            {
                return null;
            }

            return MapProduct(reader);
        }

        public List<Product> Search(string searchTerm)
        {
            List<Product> products = new List<Product>();

            string connectionString = _configuration.GetConnectionString("DefaultConnection")!;

            using SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            string query = @"
                SELECT Id, Name, Description, Price, Stock, IsActive, CreatedAt
                FROM Products
                WHERE IsActive = 1
                AND (
                    Name LIKE @SearchTerm
                    OR Description LIKE @SearchTerm
                )
                ORDER BY Id DESC";

            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@SearchTerm", $"%{searchTerm}%");

            using SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                products.Add(MapProduct(reader));
            }

            return products;
        }

        public int Create(Product product)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection")!;

            using SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            string query = @"
                INSERT INTO Products (Name, Description, Price, Stock)
                VALUES (@Name, @Description, @Price, @Stock);

                SELECT SCOPE_IDENTITY();";

            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Name", product.Name);
            command.Parameters.AddWithValue("@Description", (object?)product.Description ?? DBNull.Value);
            command.Parameters.AddWithValue("@Price", product.Price);
            command.Parameters.AddWithValue("@Stock", product.Stock);

            return Convert.ToInt32(command.ExecuteScalar());
        }

        public bool Update(int id, Product product)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection")!;

            using SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            string query = @"
                UPDATE Products
                SET Name = @Name,
                    Description = @Description,
                    Price = @Price,
                    Stock = @Stock
                WHERE Id = @Id AND IsActive = 1";

            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@Name", product.Name);
            command.Parameters.AddWithValue("@Description", (object?)product.Description ?? DBNull.Value);
            command.Parameters.AddWithValue("@Price", product.Price);
            command.Parameters.AddWithValue("@Stock", product.Stock);

            int rowsAffected = command.ExecuteNonQuery();

            return rowsAffected > 0;
        }

        public bool Delete(int id)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection")!;

            using SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            string query = @"
                UPDATE Products
                SET IsActive = 0
                WHERE Id = @Id AND IsActive = 1";

            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            int rowsAffected = command.ExecuteNonQuery();

            return rowsAffected > 0;
        }

        public bool UpdateStock(int productId, int newStock)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection")!;

            using SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            string query = @"
                UPDATE Products
                SET Stock = @Stock
                WHERE Id = @Id AND IsActive = 1";

            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", productId);
            command.Parameters.AddWithValue("@Stock", newStock);

            int rowsAffected = command.ExecuteNonQuery();

            return rowsAffected > 0;
        }

        private Product MapProduct(SqlDataReader reader)
        {
            return new Product
            {
                Id = Convert.ToInt32(reader["Id"]),
                Name = reader["Name"].ToString() ?? "",
                Description = reader["Description"] == DBNull.Value ? null : reader["Description"].ToString(),
                Price = Convert.ToDecimal(reader["Price"]),
                Stock = Convert.ToInt32(reader["Stock"]),
                IsActive = Convert.ToBoolean(reader["IsActive"]),
                CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
            };
        }
    }
}