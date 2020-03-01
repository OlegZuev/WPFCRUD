using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Npgsql;
using WPFCRUD.Properties;

namespace WPFCRUD.Models {
    public static class DatabaseInteraction {
        private static readonly string SConnection = new NpgsqlConnectionStringBuilder {
            Host = DatabaseSettings.Default.Host,
            Port = DatabaseSettings.Default.Port,
            Database = DatabaseSettings.Default.Name,
            Username = Environment.GetEnvironmentVariable("POSTGRES_USERNAME"),
            Password = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD"),
            MaxAutoPrepare = 10,
            AutoPrepareMinUsages = 2
        }.ConnectionString;

        public static void InitializeToUsers(IList<User> users) {
            using (var connection = new NpgsqlConnection(SConnection)) {
                connection.Open();

                var command = new NpgsqlCommand {
                    Connection = connection,
                    CommandText = "SELECT id, login, password, registration_date FROM users;",
                };

                using (NpgsqlDataReader reader = command.ExecuteReader()) {
                    while (reader.Read()) {
                        users.Add(new User((long) reader["id"], (string) reader["login"], (string) reader["password"],
                                           ((DateTime) reader["registration_date"]).ToLongDateString()));
                    }
                }
            }
        }

        public static void ShowNewUser(IList<User> users, long id) {
            using (var connection = new NpgsqlConnection(SConnection)) {
                connection.Open();

                var command = new NpgsqlCommand {
                    Connection = connection,
                    CommandText = "SELECT login, password, registration_date FROM users WHERE id = @id;",
                };
                command.Parameters.AddWithValue("@id", id);

                using (NpgsqlDataReader reader = command.ExecuteReader()) {
                    reader.Read();
                    users.Add(new User(id, (string) reader["login"], (string) reader["password"],
                                       ((DateTime) reader["registration_date"]).ToLongDateString()));
                }
            }
        }

        public static void LoadUserInfo(IList<User> users, long id) {
            using (var connection = new NpgsqlConnection(SConnection)) {
                connection.Open();

                var command = new NpgsqlCommand {
                    Connection = connection,
                    CommandText = "SELECT login, password, registration_date FROM users WHERE id = @id;",
                };
                command.Parameters.AddWithValue("@id", id);

                using (NpgsqlDataReader reader = command.ExecuteReader()) {
                    reader.Read();
                    User changedUser = users.First(user => user.Id == id);
                    changedUser.Login = (string) reader["login"];
                    changedUser.Password = (string) reader["password"];
                    changedUser.RegistrationDate = ((DateTime) reader["registration_date"]).ToLongDateString();
                }
            }
        }

        public static void DeleteUser(IList<User> users, User currentUser) {
            using (var connection = new NpgsqlConnection(SConnection)) {
                connection.Open();

                var command = new NpgsqlCommand {
                    Connection = connection,
                    CommandText = "DELETE FROM users WHERE id = @id;",
                };
                command.Parameters.AddWithValue("@id", currentUser.Id);
                command.ExecuteNonQuery();

                users.Remove(currentUser);
            }
        }
    }
}