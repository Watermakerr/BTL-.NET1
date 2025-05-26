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
                        return count > 0;
                    }
                }
            }
            catch (Exception)
            {
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
                        return result != null ? Convert.ToInt32(result) : (int?)null;
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public bool RegisterUser(string username, string password, string phoneNumber)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string checkQuery = "SELECT COUNT(*) FROM Users WHERE Username = @Username";
                    using (SqlCommand checkCommand = new SqlCommand(checkQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@Username", username);
                        int count = (int)checkCommand.ExecuteScalar();
                        if (count > 0)
                            return false;
                    }

                    string insertQuery = "INSERT INTO Users (Username, Password, PhoneNumber) VALUES (@Username, @Password, @PhoneNumber)";
                    using (SqlCommand insertCommand = new SqlCommand(insertQuery, connection))
                    {
                        insertCommand.Parameters.AddWithValue("@Username", username);
                        insertCommand.Parameters.AddWithValue("@Password", password);
                        insertCommand.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
                        insertCommand.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch (Exception)
            {
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
            }
            catch (Exception)
            {
                // Log lỗi nếu cần
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
            }
            catch (Exception)
            {
                // Log lỗi nếu cần
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
            }
            catch (Exception)
            {
                // Log lỗi nếu cần
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
                        return count > 0;
                    }
                }
            }
            catch (Exception)
            {
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
                        return true;
                    }
                }
            }
            catch (Exception)
            {
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
                        return true;
                    }
                }
            }
            catch (Exception)
            {
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
                System.Diagnostics.Debug.WriteLine($"GetWishlistProducts: UserID={userId}, Found {products.Count} products.");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetWishlistProducts Error: {ex.Message}");
                throw; // Throw to catch errors during debugging
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
                System.Diagnostics.Debug.WriteLine($"GetCategories: Found {categories.Count} categories.");
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
                System.Diagnostics.Debug.WriteLine($"GetProductsBySearch: Query='{searchQuery}', Found {products.Count} products.");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetProductsBySearch Error: {ex.Message}");
                throw;
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
                System.Diagnostics.Debug.WriteLine($"GetProductsByCategory: CategoryID={categoryId}, Found {products.Count} products.");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetProductsByCategory Error: {ex.Message}");
                throw;
            }
            return products;
        }
    }
}