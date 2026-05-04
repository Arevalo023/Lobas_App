using LobasOrdersApi.Models;
using LobasOrdersApi.Repositories.Interfaces;
using Microsoft.Data.SqlClient;

namespace LobasOrdersApi.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly IConfiguration _configuration;

        public OrderRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public List<Order> GetAll()
        {
            List<Order> orders = new List<Order>();

            string connectionString = _configuration.GetConnectionString("DefaultConnection")!;

            using SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            string query = @"
                SELECT Id, CustomerId, OrderDate, Total, Status, CreatedAt, IsActive
                FROM Orders
                WHERE IsActive = 1
                ORDER BY Id DESC";

            using SqlCommand command = new SqlCommand(query, connection);
            using SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                orders.Add(MapOrder(reader));
            }

            return orders;
        }

        public Order? GetById(int id)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection")!;

            using SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            string query = @"
                SELECT Id, CustomerId, OrderDate, Total, Status, CreatedAt, IsActive
                FROM Orders
                WHERE Id = @Id AND IsActive = 1";

            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            using SqlDataReader reader = command.ExecuteReader();

            if (!reader.Read())
            {
                return null;
            }

            return MapOrder(reader);
        }

        public List<Order> GetByCustomerId(int customerId)
        {
            List<Order> orders = new List<Order>();

            string connectionString = _configuration.GetConnectionString("DefaultConnection")!;

            using SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            string query = @"
                SELECT Id, CustomerId, OrderDate, Total, Status, CreatedAt, IsActive
                FROM Orders
                WHERE CustomerId = @CustomerId
                AND IsActive = 1
                ORDER BY Id DESC";

            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@CustomerId", customerId);

            using SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                orders.Add(MapOrder(reader));
            }

            return orders;
        }

        public List<Order> Search(string searchTerm)
        {
            List<Order> orders = new List<Order>();

            string connectionString = _configuration.GetConnectionString("DefaultConnection")!;

            using SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            string query = @"
                SELECT O.Id, O.CustomerId, O.OrderDate, O.Total, O.Status, O.CreatedAt, O.IsActive
                FROM Orders O
                INNER JOIN Customers C
                    ON C.Id = O.CustomerId
                WHERE O.IsActive = 1
                AND (
                    C.Name LIKE @SearchTerm
                    OR O.Status LIKE @SearchTerm
                    OR CONVERT(NVARCHAR(20), O.Id) LIKE @SearchTerm
                )
                ORDER BY O.Id DESC";

            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@SearchTerm", $"%{searchTerm}%");

            using SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                orders.Add(MapOrder(reader));
            }

            return orders;
        }

        public int Create(Order order)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection")!;

            using SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            string query = @"
                INSERT INTO Orders (CustomerId, Total, Status)
                VALUES (@CustomerId, @Total, @Status);

                SELECT SCOPE_IDENTITY();";

            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@CustomerId", order.CustomerId);
            command.Parameters.AddWithValue("@Total", order.Total);
            command.Parameters.AddWithValue("@Status", order.Status);

            return Convert.ToInt32(command.ExecuteScalar());
        }

        public bool Update(int id, Order order)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection")!;

            using SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            string query = @"
                UPDATE Orders
                SET CustomerId = @CustomerId,
                    Status = @Status
                WHERE Id = @Id
                AND IsActive = 1";

            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@CustomerId", order.CustomerId);
            command.Parameters.AddWithValue("@Status", order.Status);

            int rowsAffected = command.ExecuteNonQuery();

            return rowsAffected > 0;
        }

        public bool UpdateStatus(int id, string status)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection")!;

            using SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            string query = @"
                UPDATE Orders
                SET Status = @Status
                WHERE Id = @Id
                AND IsActive = 1";

            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@Status", status);

            int rowsAffected = command.ExecuteNonQuery();

            return rowsAffected > 0;
        }

        public bool UpdateTotal(int id, decimal total)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection")!;

            using SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            string query = @"
                UPDATE Orders
                SET Total = @Total
                WHERE Id = @Id
                AND IsActive = 1";

            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@Total", total);

            int rowsAffected = command.ExecuteNonQuery();

            return rowsAffected > 0;
        }

        public bool Delete(int id)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection")!;

            using SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            string query = @"
                UPDATE Orders
                SET IsActive = 0
                WHERE Id = @Id
                AND IsActive = 1";

            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            int rowsAffected = command.ExecuteNonQuery();

            return rowsAffected > 0;
        }

        private Order MapOrder(SqlDataReader reader)
        {
            return new Order
            {
                Id = Convert.ToInt32(reader["Id"]),
                CustomerId = Convert.ToInt32(reader["CustomerId"]),
                OrderDate = Convert.ToDateTime(reader["OrderDate"]),
                Total = Convert.ToDecimal(reader["Total"]),
                Status = reader["Status"].ToString() ?? "",
                CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                IsActive = Convert.ToBoolean(reader["IsActive"])
            };
        }
    }
}