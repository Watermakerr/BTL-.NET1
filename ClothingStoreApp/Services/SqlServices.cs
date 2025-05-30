using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using ClothingStoreApp.Models;

namespace ClothingStoreApp.Services
{
    public class SqlService
    {
        private readonly string _connectionString = "Server=localhost;Database=ClothingStore;Trusted_Connection=True;TrustServerCertificate=True;";

        public bool AuthenticateUser(string username, string password)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "SELECT COUNT(*) FROM Users WHERE Username = @Username AND Password = @Password";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Username", username);
                        command.Parameters.AddWithValue("@Password", password);
                        int count = (int)command.ExecuteScalar();
                        System.Diagnostics.Debug.WriteLine($"AuthenticateUser: Username={username}, Success={count > 0}");
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"AuthenticateUser Error: {ex.Message}");
                return false;
            }
        }

        public int? GetUserIdByUsername(string username)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "SELECT UserID FROM Users WHERE Username = @Username";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Username", username);
                        object result = command.ExecuteScalar();
                        int? userId = result != null ? Convert.ToInt32(result) : (int?)null;
                        System.Diagnostics.Debug.WriteLine($"GetUserIdByUsername: Username={username}, UserID={userId}");
                        return userId;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetUserIdByUsername Error: {ex.Message}");
                return null;
            }
        }

        public string GetUserAddress(int userId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "SELECT Address FROM Users WHERE UserID = @UserID";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UserID", userId);
                        object result = command.ExecuteScalar();
                        string address = result != null ? result.ToString() : "Chưa có địa chỉ giao hàng";
                        System.Diagnostics.Debug.WriteLine($"GetUserAddress: UserID={userId}, Address={address}");
                        return address;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetUserAddress Error: {ex.Message}");
                return "Lỗi khi lấy địa chỉ";
            }
        }

        public bool RegisterUser(string username, string password, string phoneNumber, string address)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    // Check if username already exists
                    string checkQuery = "SELECT COUNT(*) FROM Users WHERE Username = @Username";
                    using (SqlCommand checkCommand = new SqlCommand(checkQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@Username", username);
                        int count = (int)checkCommand.ExecuteScalar();
                        if (count > 0)
                        {
                            return false; // Username already exists
                        }
                    }

                    // Insert new user
                    string insertQuery = "INSERT INTO Users (Username, Password, PhoneNumber, Address) VALUES (@Username, @Password, @PhoneNumber, @Address)";
                    using (SqlCommand insertCommand = new SqlCommand(insertQuery, connection))
                    {
                        insertCommand.Parameters.AddWithValue("@Username", username);
                        insertCommand.Parameters.AddWithValue("@Password", password);
                        insertCommand.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
                        insertCommand.Parameters.AddWithValue("@Address", (object)address ?? DBNull.Value);
                        insertCommand.ExecuteNonQuery();
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"RegisterUser Error: {ex.Message}");
                return false;
            }
        }

        public List<Product> GetTopSellingProducts()
        {
            List<Product> products = new List<Product>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "SELECT TOP 10 ProductID, ProductName, Price, QuantitySold, Description, CategoryID, ImageURL FROM Products ORDER BY QuantitySold DESC";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                products.Add(new Product
                                {
                                    ProductID = reader.GetInt32(0),
                                    ProductName = reader.GetString(1),
                                    Price = reader.GetDecimal(2),
                                    QuantitySold = reader.GetInt32(3),
                                    Description = reader.IsDBNull(4) ? null : reader.GetString(4),
                                    CategoryID = reader.GetInt32(5),
                                    ImageURL = reader.IsDBNull(6) ? null : reader.GetString(6)
                                });
                            }
                        }
                    }
                }
                System.Diagnostics.Debug.WriteLine($"GetTopSellingProducts: Found {products.Count} products");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetTopSellingProducts Error: {ex.Message}");
            }
            return products;
        }

        public Product GetProductById(int productId)
        {
            Product product = null;
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "SELECT ProductID, ProductName, Price, QuantitySold, Description, CategoryID, ImageURL FROM Products WHERE ProductID = @ProductID";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ProductID", productId);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                product = new Product
                                {
                                    ProductID = reader.GetInt32(0),
                                    ProductName = reader.GetString(1),
                                    Price = reader.GetDecimal(2),
                                    QuantitySold = reader.GetInt32(3),
                                    Description = reader.IsDBNull(4) ? null : reader.GetString(4),
                                    CategoryID = reader.GetInt32(5),
                                    ImageURL = reader.IsDBNull(6) ? null : reader.GetString(6)
                                };
                            }
                        }
                    }
                }
                System.Diagnostics.Debug.WriteLine($"GetProductById: ProductID={productId}, Found={product != null}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetProductById Error: {ex.Message}");
            }
            return product;
        }

        public List<Review> GetReviewsByProductId(int productId)
        {
            List<Review> reviews = new List<Review>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "SELECT ReviewID, UserID, ProductID, Rating, Comment, ReviewDate FROM Reviews WHERE ProductID = @ProductID";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ProductID", productId);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                reviews.Add(new Review
                                {
                                    ReviewID = reader.GetInt32(0),
                                    UserID = reader.GetInt32(1),
                                    ProductID = reader.GetInt32(2),
                                    Rating = reader.GetInt32(3),
                                    Comment = reader.IsDBNull(4) ? null : reader.GetString(4),
                                    ReviewDate = reader.GetDateTime(5)
                                });
                            }
                        }
                    }
                }
                System.Diagnostics.Debug.WriteLine($"GetReviewsByProductId: ProductID={productId}, Found {reviews.Count} reviews");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetReviewsByProductId Error: {ex.Message}");
            }
            return reviews;
        }

        public bool IsProductInWishlist(int userId, int productId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "SELECT COUNT(*) FROM Wishlist WHERE UserID = @UserID AND ProductID = @ProductID";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UserID", userId);
                        command.Parameters.AddWithValue("@ProductID", productId);
                        int count = (int)command.ExecuteScalar();
                        System.Diagnostics.Debug.WriteLine($"IsProductInWishlist: UserID={userId}, ProductID={productId}, InWishlist={count > 0}");
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"IsProductInWishlist Error: {ex.Message}");
                return false;
            }
        }

        public bool AddToWishlist(int userId, int productId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO Wishlist (UserID, ProductID, AddedDate) VALUES (@UserID, @ProductID, GETDATE())";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UserID", userId);
                        command.Parameters.AddWithValue("@ProductID", productId);
                        command.ExecuteNonQuery();
                        System.Diagnostics.Debug.WriteLine($"AddToWishlist: UserID={userId}, ProductID={productId}, Success=true");
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"AddToWishlist Error: {ex.Message}");
                return false;
            }
        }

        public bool RemoveFromWishlist(int userId, int productId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM Wishlist WHERE UserID = @UserID AND ProductID = @ProductID";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UserID", userId);
                        command.Parameters.AddWithValue("@ProductID", productId);
                        command.ExecuteNonQuery();
                        System.Diagnostics.Debug.WriteLine($"RemoveFromWishlist: UserID={userId}, ProductID={productId}, Success=true");
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"RemoveFromWishlist Error: {ex.Message}");
                return false;
            }
        }

        public List<Product> GetWishlistProducts(int userId)
        {
            List<Product> products = new List<Product>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = @"
                        SELECT p.ProductID, p.ProductName, p.Price, p.QuantitySold, p.Description, p.CategoryID, p.ImageURL
                        FROM Products p
                        INNER JOIN Wishlist w ON p.ProductID = w.ProductID
                        WHERE w.UserID = @UserID";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UserID", userId);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                products.Add(new Product
                                {
                                    ProductID = reader.GetInt32(0),
                                    ProductName = reader.GetString(1),
                                    Price = reader.GetDecimal(2),
                                    QuantitySold = reader.GetInt32(3),
                                    Description = reader.IsDBNull(4) ? null : reader.GetString(4),
                                    CategoryID = reader.GetInt32(5),
                                    ImageURL = reader.IsDBNull(6) ? null : reader.GetString(6)
                                });
                            }
                        }
                    }
                }
                System.Diagnostics.Debug.WriteLine($"GetWishlistProducts: UserID={userId}, Found {products.Count} products");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetWishlistProducts Error: {ex.Message}");
            }
            return products;
        }

        public List<Category> GetCategories()
        {
            List<Category> categories = new List<Category>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "SELECT CategoryID, CategoryName, ImagePath FROM Categories";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                categories.Add(new Category
                                {
                                    CategoryID = reader.GetInt32(0),
                                    CategoryName = reader.GetString(1),
                                    ImagePath = reader.IsDBNull(2) ? null : reader.GetString(2)
                                });
                            }
                        }
                    }
                }
                System.Diagnostics.Debug.WriteLine($"GetCategories: Found {categories.Count} categories");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetCategories Error: {ex.Message}");
            }
            return categories;
        }

        public List<Product> GetProductsBySearch(string searchQuery)
        {
            List<Product> products = new List<Product>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = @"
                        SELECT ProductID, ProductName, Price, QuantitySold, Description, CategoryID, ImageURL
                        FROM Products
                        WHERE ProductName LIKE @SearchQuery";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@SearchQuery", $"%{searchQuery}%");
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                products.Add(new Product
                                {
                                    ProductID = reader.GetInt32(0),
                                    ProductName = reader.GetString(1),
                                    Price = reader.GetDecimal(2),
                                    QuantitySold = reader.GetInt32(3),
                                    Description = reader.IsDBNull(4) ? null : reader.GetString(4),
                                    CategoryID = reader.GetInt32(5),
                                    ImageURL = reader.IsDBNull(6) ? null : reader.GetString(6)
                                });
                            }
                        }
                    }
                }
                System.Diagnostics.Debug.WriteLine($"GetProductsBySearch: Query='{searchQuery}', Found {products.Count} products");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetProductsBySearch Error: {ex.Message}");
            }
            return products;
        }

        public List<Product> GetProductsByCategory(int categoryId)
        {
            List<Product> products = new List<Product>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = @"
                        SELECT ProductID, ProductName, Price, QuantitySold, Description, CategoryID, ImageURL
                        FROM Products
                        WHERE CategoryID = @CategoryID";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@CategoryID", categoryId);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                products.Add(new Product
                                {
                                    ProductID = reader.GetInt32(0),
                                    ProductName = reader.GetString(1),
                                    Price = reader.GetDecimal(2),
                                    QuantitySold = reader.GetInt32(3),
                                    Description = reader.IsDBNull(4) ? null : reader.GetString(4),
                                    CategoryID = reader.GetInt32(5),
                                    ImageURL = reader.IsDBNull(6) ? null : reader.GetString(6)
                                });
                            }
                        }
                    }
                }
                System.Diagnostics.Debug.WriteLine($"GetProductsByCategory: CategoryID={categoryId}, Found {products.Count} products");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetProductsByCategory Error: {ex.Message}");
            }
            return products;
        }

        public bool IsProductInCart(int userId, int productId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "SELECT COUNT(*) FROM Cart WHERE UserID = @UserID AND ProductID = @ProductID";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UserID", userId);
                        command.Parameters.AddWithValue("@ProductID", productId);
                        int count = (int)command.ExecuteScalar();
                        System.Diagnostics.Debug.WriteLine($"IsProductInCart: UserID={userId}, ProductID={productId}, InCart={count > 0}");
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"IsProductInCart Error: {ex.Message}");
                return false;
            }
        }

        public bool AddToCart(int userId, int productId, int quantity)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = @"
                        IF EXISTS (SELECT 1 FROM Cart WHERE UserID = @UserID AND ProductID = @ProductID)
                            UPDATE Cart
                            SET Quantity = Quantity + @Quantity, AddedDate = GETDATE()
                            WHERE UserID = @UserID AND ProductID = @ProductID
                        ELSE
                            INSERT INTO Cart (UserID, ProductID, Quantity, AddedDate)
                            VALUES (@UserID, @ProductID, @Quantity, GETDATE())";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UserID", userId);
                        command.Parameters.AddWithValue("@ProductID", productId);
                        command.Parameters.AddWithValue("@Quantity", quantity);
                        command.ExecuteNonQuery();
                        System.Diagnostics.Debug.WriteLine($"AddToCart: UserID={userId}, ProductID={productId}, Quantity={quantity}, Success=true");
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"AddToCart Error: {ex.Message}");
                return false;
            }
        }

        public bool UpdateCartItem(int userId, int productId, int quantity)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = @"
                        UPDATE Cart
                        SET Quantity = @Quantity, AddedDate = GETDATE()
                        WHERE UserID = @UserID AND ProductID = @ProductID";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UserID", userId);
                        command.Parameters.AddWithValue("@ProductID", productId);
                        command.Parameters.AddWithValue("@Quantity", quantity);
                        int rowsAffected = command.ExecuteNonQuery();
                        System.Diagnostics.Debug.WriteLine($"UpdateCartItem: UserID={userId}, ProductID={productId}, Quantity={quantity}, RowsAffected={rowsAffected}");
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UpdateCartItem Error: {ex.Message}");
                return false;
            }
        }

        public bool RemoveFromCart(int userId, int productId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM Cart WHERE UserID = @UserID AND ProductID = @ProductID";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UserID", userId);
                        command.Parameters.AddWithValue("@ProductID", productId);
                        int rowsAffected = command.ExecuteNonQuery();
                        System.Diagnostics.Debug.WriteLine($"RemoveFromCart: UserID={userId}, ProductID={productId}, RowsAffected={rowsAffected}");
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"RemoveFromCart Error: {ex.Message}");
                return false;
            }
        }

        public List<(Cart Cart, Product Product)> GetCartItems(int userId)
        {
            List<(Cart Cart, Product Product)> cartItems = new List<(Cart, Product)>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = @"
                        SELECT c.UserID, c.ProductID, c.Quantity, c.AddedDate, 
                               p.ProductID, p.ProductName, p.Price, p.QuantitySold, p.Description, p.CategoryID, p.ImageURL
                        FROM Cart c
                        LEFT JOIN Products p ON c.ProductID = p.ProductID
                        WHERE c.UserID = @UserID";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UserID", userId);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var cart = new Cart
                                {
                                    UserID = reader.GetInt32(0),
                                    ProductID = reader.GetInt32(1),
                                    Quantity = reader.GetInt32(2),
                                    AddedDate = reader.GetDateTime(3)
                                };
                                Product product = null;
                                if (!reader.IsDBNull(4))
                                {
                                    product = new Product
                                    {
                                        ProductID = reader.GetInt32(4),
                                        ProductName = reader.GetString(5),
                                        Price = reader.GetDecimal(6),
                                        QuantitySold = reader.GetInt32(7),
                                        Description = reader.IsDBNull(8) ? null : reader.GetString(8),
                                        CategoryID = reader.GetInt32(9),
                                        ImageURL = reader.IsDBNull(10) ? null : reader.GetString(10)
                                    };
                                }
                                cartItems.Add((cart, product));
                            }
                        }
                    }
                }
                System.Diagnostics.Debug.WriteLine($"GetCartItems: UserID={userId}, Found {cartItems.Count} items, ProductsFound={cartItems.Count(i => i.Product != null)}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetCartItems Error: {ex.Message}");
            }
            return cartItems;
        }

        public int PlaceOrder(int userId, decimal totalAmount, string address)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO Orders (UserID, OrderDate, TotalAmount, Address, Status) VALUES (@UserID, GETDATE(), @TotalAmount, @Address, 0); SELECT SCOPE_IDENTITY();";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UserID", userId);
                        command.Parameters.AddWithValue("@TotalAmount", totalAmount);
                        command.Parameters.AddWithValue("@Address", address);
                        int orderId = Convert.ToInt32(command.ExecuteScalar());
                        System.Diagnostics.Debug.WriteLine($"PlaceOrder: UserID={userId}, OrderID={orderId}, TotalAmount={totalAmount}, Address={address}");
                        return orderId;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"PlaceOrder Error: {ex.Message}");
                return -1;
            }
        }

        public bool AddOrderDetail(int orderId, int productId, int quantity, decimal unitPrice)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO OrderDetails (OrderID, ProductID, Quantity, UnitPrice) VALUES (@OrderID, @ProductID, @Quantity, @UnitPrice)";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@OrderID", orderId);
                        command.Parameters.AddWithValue("@ProductID", productId);
                        command.Parameters.AddWithValue("@Quantity", quantity);
                        command.Parameters.AddWithValue("@UnitPrice", unitPrice);
                        command.ExecuteNonQuery();
                        System.Diagnostics.Debug.WriteLine($"AddOrderDetail: OrderID={orderId}, ProductID={productId}, Quantity={quantity}, UnitPrice={unitPrice}");
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"AddOrderDetail Error: {ex.Message}");
                return false;
            }
        }

        public bool ClearCart(int userId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM Cart WHERE UserID = @UserID";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UserID", userId);
                        int rowsAffected = command.ExecuteNonQuery();
                        System.Diagnostics.Debug.WriteLine($"ClearCart: UserID={userId}, RowsAffected={rowsAffected}");
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ClearCart Error: {ex.Message}");
                return false;
            }
        }

        public User GetUserInfo(int userId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "SELECT UserID, Username, PhoneNumber, Address FROM Users WHERE UserID = @UserID";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UserID", userId);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new User
                                {
                                    UserID = reader.GetInt32(0),
                                    Username = reader.GetString(1),
                                    PhoneNumber = reader.IsDBNull(2) ? null : reader.GetString(2),
                                    Address = reader.IsDBNull(3) ? null : reader.GetString(3)
                                };
                            }
                        }
                    }
                }
                System.Diagnostics.Debug.WriteLine($"GetUserInfo: UserID={userId}, Not found");
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetUserInfo Error: {ex.Message}");
                return null;
            }
        }

        public List<Order> GetUserOrders(int userId)
        {
            List<Order> orders = new List<Order>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "SELECT OrderID, UserID, OrderDate, TotalAmount, Address, Status FROM Orders WHERE UserID = @UserID ORDER BY OrderDate DESC";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UserID", userId);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                orders.Add(new Order
                                {
                                    OrderID = reader.GetInt32(0),
                                    UserID = reader.GetInt32(1),
                                    OrderDate = reader.GetDateTime(2),
                                    TotalAmount = reader.GetDecimal(3),
                                    Address = reader.GetString(4),
                                    Status = reader.GetInt32(5)
                                });
                            }
                        }
                    }
                }
                System.Diagnostics.Debug.WriteLine($"GetUserOrders: UserID={userId}, Found {orders.Count} orders");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetUserOrders Error: {ex.Message}");
            }
            return orders;
        }

        public string GetUsernameByUserId(int userId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "SELECT Username FROM Users WHERE UserID = @UserID";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UserID", userId);
                        object result = command.ExecuteScalar();
                        string username = result?.ToString();
                        System.Diagnostics.Debug.WriteLine($"GetUsernameByUserId: UserID={userId}, Username={username}");
                        return username;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetUsernameByUserId Error: {ex.Message}");
                return null;
            }
        }

        public bool UpdateUserPassword(int userId, string newPassword)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "UPDATE Users SET Password = @Password WHERE UserID = @UserID";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Password", newPassword);
                        command.Parameters.AddWithValue("@UserID", userId);
                        int rowsAffected = command.ExecuteNonQuery();
                        System.Diagnostics.Debug.WriteLine($"UpdateUserPassword: UserID={userId}, RowsAffected={rowsAffected}");
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UpdateUserPassword Error: {ex.Message}");
                return false;
            }
        }

        public List<(Cart Cart, Product Product)> GetOrderItems(int orderId)
        {
            List<(Cart Cart, Product Product)> orderItems = new List<(Cart, Product)>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = @"
                        SELECT od.OrderID, od.ProductID, od.Quantity, od.UnitPrice,
                               p.ProductID, p.ProductName, p.Price, p.QuantitySold, p.Description, p.CategoryID, p.ImageURL
                        FROM OrderDetails od
                        LEFT JOIN Products p ON od.ProductID = p.ProductID
                        WHERE od.OrderID = @OrderID";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@OrderID", orderId);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var cart = new Cart
                                {
                                    UserID = 0, // Not needed for order details
                                    ProductID = reader.IsDBNull(1) ? 0 : reader.GetInt32(1),
                                    Quantity = reader.IsDBNull(2) ? 0 : reader.GetInt32(2),
                                    AddedDate = DateTime.Now // Placeholder
                                };
                                Product product = null;
                                if (!reader.IsDBNull(4)) // Check if ProductID from Products exists
                                {
                                    product = new Product
                                    {
                                        ProductID = reader.GetInt32(4),
                                        ProductName = reader.IsDBNull(5) ? "Unknown" : reader.GetString(5),
                                        Price = reader.IsDBNull(6) ? 0m : reader.GetDecimal(6),
                                        QuantitySold = reader.IsDBNull(7) ? 0 : reader.GetInt32(7),
                                        Description = reader.IsDBNull(8) ? null : reader.GetString(8),
                                        CategoryID = reader.IsDBNull(9) ? 0 : reader.GetInt32(9),
                                        ImageURL = reader.IsDBNull(10) ? null : reader.GetString(10)
                                    };
                                }
                                orderItems.Add((Cart: cart, Product: product));
                                System.Diagnostics.Debug.WriteLine($"GetOrderItems: OrderID={orderId}, ProductID={cart.ProductID}, Quantity={cart.Quantity}, ProductName={product?.ProductName ?? "null"}");
                            }
                        }
                    }
                }
                System.Diagnostics.Debug.WriteLine($"GetOrderItems: OrderID={orderId}, Found {orderItems.Count} items");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetOrderItems Error: {ex.Message}\nStackTrace: {ex.StackTrace}");
            }
            return orderItems;
        }

        public bool UpdateOrderStatus(int orderId, int newStatus)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "UPDATE Orders SET Status = @Status WHERE OrderID = @OrderID";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Status", newStatus);
                        command.Parameters.AddWithValue("@OrderID", orderId);
                        int rowsAffected = command.ExecuteNonQuery();
                        System.Diagnostics.Debug.WriteLine($"UpdateOrderStatus: OrderID={orderId}, NewStatus={newStatus}, RowsAffected={rowsAffected}");
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UpdateOrderStatus Error: {ex.Message}");
                return false;
            }
        }
    }
}