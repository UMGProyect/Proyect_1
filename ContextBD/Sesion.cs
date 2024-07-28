using Proyect_1.Models;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

namespace Proyect_1.ContextBD
{
    public class Sesion
    {
        static string connectionString = BD.connectionString;

        public bool Authenticator(User model)
        {
            using (SqlConnection contextBD = new SqlConnection(connectionString))
            {
                try
                {
                    contextBD.Open();
                    string query = "SELECT password FROM login WHERE [user] = @name";
                    SqlCommand command = new SqlCommand(query, contextBD);
                    command.Parameters.AddWithValue("@name", model.Name);
                    var storedHash = command.ExecuteScalar()?.ToString();
                    // Example
                    if(model.Password != null)
                    {
                        model.Password = ComputeSha256Hash(model.Password);
                        // Comparamos el password
                        if (storedHash != null && VerifyPassword(model.Password, storedHash))
                        {
                            return true; // Autenticación exitosa
                        }
                    }
                    else
                    {
                        return false; // Autenticación fallida
                    }
                    
                    
                   
                }
                catch (Exception ex)
                {
                    // Manejar el error de manera más robusta
                    Console.WriteLine($"Error al autenticar al usuario: {ex.Message}");
                }

                return false; // Autenticación fallida
            }
        }

        private bool VerifyPassword(string password, string storedHash)
        {

            // SHA-256
            var hash = password; //= ComputeSha256Hash(password);
            return hash == storedHash;
        }

        private string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // hash de la contraseña
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                var builder = new StringBuilder();
                foreach (byte t in bytes)
                {
                    builder.Append(t.ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
