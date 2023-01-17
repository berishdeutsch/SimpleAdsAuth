using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace SimpleAdsAuth.Data
{
    public class SimpleAdsAuteRepository
    {
        private string _connectionString;
        public SimpleAdsAuteRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
        public void AddUser(User user, string password)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"INSERT INTO Users (Name, Email, PasswordHash)
                                VALUES(@name, @email, @password)";
            cmd.Parameters.AddWithValue("@name", user.Name);
            cmd.Parameters.AddWithValue("@email", user.Email);
            cmd.Parameters.AddWithValue("@password", BCrypt.Net.BCrypt.HashPassword(password));
            connection.Open();
            cmd.ExecuteNonQuery();
        }
        public User Login(string email, string password)
        {
            var user = GetUserByEmail(email);
            if (user == null)
            {
                return null;
            }
            return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash) ? user : null;
        }
        public User GetUserByEmail(string email)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT * FROM Users WHERE Email = @email";
            cmd.Parameters.AddWithValue("@email", email);
            connection.Open();
            var reader = cmd.ExecuteReader();
            if (!reader.Read())
            {
                return null;
            }

            return new User
            {
                Id = (int)reader["Id"],
                Name = (string)reader["Name"],
                Email = (string)reader["Email"],  
                PasswordHash = (string)reader["PasswordHash"],
            };
        }
        public void AddAd(Ad ad)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"INSERT INTO Ads (Description, PhoneNumber, Date, UserId)
                                VALUES(@description, @phonenumber, @date, @userid)";
            cmd.Parameters.AddWithValue("@description", ad.Description);
            cmd.Parameters.AddWithValue("@phonenumber", ad.PhoneNumber);
            cmd.Parameters.AddWithValue("@date", DateTime.Now);
            cmd.Parameters.AddWithValue("@userid", ad.UserId);
            connection.Open();
            cmd.ExecuteNonQuery();
        }
        public List<Ad> GetAdsByUserId(int userId)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT * FROM Ads WHERE UserId = @userid ORDER BY Id DESC";
            cmd.Parameters.AddWithValue("@userid", userId);
            connection.Open();
            var result = new List<Ad>();
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                result.Add(new Ad
                {
                    Id = (int)reader["Id"],
                    PhoneNumber = (string)reader["PhoneNumber"],
                    Description = (string)reader["Description"],
                    Date = (DateTime)reader["Date"],
                    UserId = (int)reader["UserId"]
                });
            }
            return result;
        }
        public List<Ad> GetAllAds()
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT * FROM Ads ORDER BY Id DESC";
            connection.Open();
            var result = new List<Ad>();
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                result.Add(new Ad
                {
                    Id = (int)reader["Id"],
                    PhoneNumber = (string)reader["PhoneNumber"],
                    Description = (string)reader["Description"],
                    Date = (DateTime)reader["Date"],
                    UserId = (int)reader["UserId"]
                });
            }
            return result;

        }

        public void DeleteAd(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"DELETE FROM Ads WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", id);
            connection.Open();
            cmd.ExecuteNonQuery();
        }
    }
}
