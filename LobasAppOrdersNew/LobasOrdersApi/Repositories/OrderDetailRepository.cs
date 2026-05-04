using LobasOrdersApi.Models;
using LobasOrdersApi.Repositories.Interfaces;
using Microsoft.Data.SqlClient;

namespace LobasOrdersApi.Repositories
{
    public class OrderDetailRepository : IOrderDetailRepository
    {
        private readonly IConfiguration _configuration;

        public OrderDetailRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public List<OrderDetail> GetAll()
        {
            List<OrderDetail> orderDetails = new List<OrderDetail>();

            string connectionString = _configuration.GetConnectionString("DefaultConnection")!;

            using SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            string query = @"
                SELECT Id, OrderId, ProductId, Quantity, UnitPrice, Subtotal
                FROM OrderDetails
                ORDER BY Id DESC";

            using SqlCommand command = new SqlCommand(query, connection);
            using SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                orderDetails.Add(MapOrderDetail(reader));
            }

            return orderDetails;
        }

        public OrderDetail? GetById(int id)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection")!;

            using SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            string query = @"
                SELECT Id, OrderId, ProductId, Quantity, UnitPrice, Subtotal
                FROM OrderDetails
                WHERE Id = @Id";

            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            using SqlDataReader reader = command.ExecuteReader();

            if (!reader.Read())
            {
                return null;
            }

            return MapOrderDetail(reader);
        }

        public List<OrderDetail> GetByOrderId(int orderId)
        {
            List<OrderDetail> orderDetails = new List<OrderDetail>();

            string connectionString = _configuration.GetConnectionString("DefaultConnection")!;

            using SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            string query = @"
                SELECT Id, OrderId, ProductId, Quantity, UnitPrice, Subtotal
                FROM OrderDetails
                WHERE OrderId = @OrderId
                ORDER BY Id ASC";

            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@OrderId", orderId);

            using SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                orderDetails.Add(MapOrderDetail(reader));
            }

            return orderDetails;
        }

        public int Create(OrderDetail orderDetail)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection")!;

            using SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            string query = @"
                INSERT INTO OrderDetails (OrderId, ProductId, Quantity, UnitPrice, Subtotal)
                VALUES (@OrderId, @ProductId, @Quantity, @UnitPrice, @Subtotal);

                SELECT SCOPE_IDENTITY();";

            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@OrderId", orderDetail.OrderId);
            command.Parameters.AddWithValue("@ProductId", orderDetail.ProductId);
            command.Parameters.AddWithValue("@Quantity", orderDetail.Quantity);
            command.Parameters.AddWithValue("@UnitPrice", orderDetail.UnitPrice);
            command.Parameters.AddWithValue("@Subtotal", orderDetail.Subtotal);

            return Convert.ToInt32(command.ExecuteScalar());
        }

        public bool Update(int id, OrderDetail orderDetail)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection")!;

            using SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            string query = @"
                UPDATE OrderDetails
                SET ProductId = @ProductId,
                    Quantity = @Quantity,
                    UnitPrice = @UnitPrice,
                    Subtotal = @Subtotal
                WHERE Id = @Id";

            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@ProductId", orderDetail.ProductId);
            command.Parameters.AddWithValue("@Quantity", orderDetail.Quantity);
            command.Parameters.AddWithValue("@UnitPrice", orderDetail.UnitPrice);
            command.Parameters.AddWithValue("@Subtotal", orderDetail.Subtotal);

            int rowsAffected = command.ExecuteNonQuery();

            return rowsAffected > 0;
        }

        public bool Delete(int id)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection")!;

            using SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            string query = @"
                DELETE FROM OrderDetails
                WHERE Id = @Id";

            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            int rowsAffected = command.ExecuteNonQuery();

            return rowsAffected > 0;
        }

        public bool DeleteByOrderId(int orderId)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection")!;

            using SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            string query = @"
                DELETE FROM OrderDetails
                WHERE OrderId = @OrderId";

            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@OrderId", orderId);

            command.ExecuteNonQuery();

            return true;
        }

        private OrderDetail MapOrderDetail(SqlDataReader reader)
        {
            return new OrderDetail
            {
                Id = Convert.ToInt32(reader["Id"]),
                OrderId = Convert.ToInt32(reader["OrderId"]),
                ProductId = Convert.ToInt32(reader["ProductId"]),
                Quantity = Convert.ToInt32(reader["Quantity"]),
                UnitPrice = Convert.ToDecimal(reader["UnitPrice"]),
                Subtotal = Convert.ToDecimal(reader["Subtotal"])
            };
        }
    }
}