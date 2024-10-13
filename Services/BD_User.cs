using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Proyect_1.ContextBD;
using Proyect_1.Models;

namespace Proyect_1.Services
{
    public class BD_User
    {
        static string connectionString = BD.connectionString;

        // Método para obtener el perfil de un usuario
        public UserProfileViewModel GetUserProfile(string userName)
        {
            using (SqlConnection contextBD = new SqlConnection(connectionString))
            {
                try
                {
                    contextBD.Open();

                    // Obtener información del usuario
                    var user = GetUserInfo(userName, contextBD);
                    if (user == null) return null;

                    // Obtener publicaciones del usuario
                    var userPosts = GetUserPosts(user.id_user, contextBD);
                    List<PostViewModel> posts;

                    if (userPosts is string message) // Verifica si el retorno es un mensaje
                    {
                        // Si no se encontraron publicaciones, puedes decidir cómo manejar esto
                        // Aquí se puede agregar una lógica para manejar el mensaje o dejar la lista vacía
                        posts = new List<PostViewModel>(); // Puedes dejar la lista vacía si no hay publicaciones
                    }
                    else
                    {
                        posts = (List<PostViewModel>)userPosts; // Si hay publicaciones, cast a List<PostViewModel>
                    }

                    return new UserProfileViewModel
                    {
                        UserName = user.username,
                        Bio = user.biografia,
                        ProfilePictureUrl = user.imagen_perfil_url,
                        BannerImageUrl = user.imagen_baner_url,
                        Posts = posts
                    };
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al obtener el perfil del usuario: {ex.Message}");
                    throw new Exception("Error en la conexión a la base de datos: " + ex.Message);
                }
            }
        }

        // Método para obtener información del usuario
        private dynamic GetUserInfo(string userName, SqlConnection contextBD)
        {
            string query = "SELECT id_user, username, biografia, imagen_perfil_url, imagen_baner_url FROM Usuario WHERE username = @username";
            using (SqlCommand command = new SqlCommand(query, contextBD))
            {
                command.Parameters.AddWithValue("@username", userName);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {

                        return new
                        {
                            id_user = reader.GetInt32(0),
                            username = reader.GetString(1),
                            biografia = reader.IsDBNull(2) ? null : reader.GetString(2),
                            imagen_perfil_url = reader.IsDBNull(3) ? null : reader.GetString(3),
                            imagen_baner_url = reader.IsDBNull(4) ? null : reader.GetString(4)
                        };
                    }
                }
            }
            return null; // Usuario no encontrado
        }

        // Método para obtener las publicaciones del usuario
        private List<PostViewModel> GetUserPosts(int userId, SqlConnection contextBD)
        {
            var posts = new List<PostViewModel>();
            string query = "SELECT id_publicacion, tipo_publicacion, descripcion, archivo_url, fecha_publicacion FROM Publicacion WHERE id_user = @userId";

            using (SqlCommand command = new SqlCommand(query, contextBD))
            {
                command.Parameters.AddWithValue("@userId", userId);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var postId = reader.GetInt32(0);
                        var post = new PostViewModel
                        {
                            Title = reader.GetString(1), // Suponiendo que "tipo_publicacion" es el título
                            Content = reader.IsDBNull(2) ? null : reader.GetString(2),
                            ImageUrl = reader.IsDBNull(3) ? null : reader.GetString(3),
                            CreatedAt = reader.GetDateTime(4),
                            Likes = 0, // Inicializar a 0 temporalmente
                            Comments = new List<CommentViewModel>() // Inicializar a lista vacía temporalmente
                        };

                        // Cerrar el DataReader antes de hacer más consultas
                        

                        // Obtener likes y comentarios ahora
                        post.Likes = GetPostLikes(postId, contextBD);
                        post.Comments = GetPostComments(postId, contextBD);
                        
                        posts.Add(post);
                    }
                }
            }
            return posts;
        }



        // Método para obtener los likes de una publicación
        private int GetPostLikes(int postId, SqlConnection contextBD)
        {
            string query = "SELECT COUNT(*) FROM Reaccion WHERE id_publicacion = @postId AND tipo_reaccion = 'like'";
            using (SqlCommand command = new SqlCommand(query, contextBD))
            {
                command.Parameters.AddWithValue("@postId", postId);
                return (int)command.ExecuteScalar(); // Retorna el número de 'likes'
            }
        }

        // Método para obtener los comentarios de una publicación
        private List<CommentViewModel> GetPostComments(int postId, SqlConnection contextBD)
        {
            var comments = new List<CommentViewModel>();
            string query = "SELECT id_user, contenido, fecha_comentario FROM Comentario WHERE id_publicacion = @postId";
            using (SqlCommand command = new SqlCommand(query, contextBD))
            {
                command.Parameters.AddWithValue("@postId", postId);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        comments.Add(new CommentViewModel
                        {
                            UserName = reader.GetString(0), // Suponiendo que el nombre de usuario se puede obtener directamente
                            Content = reader.GetString(1),
                            CreatedAt = reader.GetDateTime(2)
                        });
                    }
                }
            }
            return comments;
        }


        //***************************************
        //*************************************** EDITAR PERFIL DE USUARIO
        //***************************************


        public bool UpdateProfilePicture(string userName, string imageUrl)
        {
            using (SqlConnection contextBD = new SqlConnection(connectionString))
            {
                try
                {
                    contextBD.Open();

                    string query = "UPDATE Usuario SET imagen_perfil_url = @imageUrl WHERE username = @username";
                    using (SqlCommand command = new SqlCommand(query, contextBD))
                    {
                        command.Parameters.AddWithValue("@imageUrl", imageUrl);
                        command.Parameters.AddWithValue("@username", userName);

                        // Ejecuta el comando y devuelve true si se actualizó con éxito
                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0; // Retorna true si se actualizó algún registro
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al actualizar la imagen de perfil: {ex.Message}");
                    return false; // En caso de error, retorna false
                }
            }
        }

        // Método para actualizar la URL del banner de un usuario
        public bool UpdateBannerUrl(string userName, string newBannerUrl)
        {
            using (SqlConnection contextBD = new SqlConnection(connectionString))
            {
                try
                {
                    contextBD.Open();

                    // Comenzar la transacción
                    using (var transaction = contextBD.BeginTransaction())
                    {
                        // Actualizar la URL del banner
                        string query = "UPDATE Usuario SET imagen_baner_url = @newBannerUrl WHERE username = @username";
                        using (SqlCommand command = new SqlCommand(query, contextBD, transaction))
                        {
                            command.Parameters.AddWithValue("@newBannerUrl", newBannerUrl);
                            command.Parameters.AddWithValue("@username", userName);

                            int rowsAffected = command.ExecuteNonQuery();

                            // Si se actualizó al menos una fila, confirmar la transacción
                            if (rowsAffected > 0)
                            {
                                transaction.Commit();
                                return true; // Actualización exitosa
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al actualizar la URL del banner: {ex.Message}");
                    throw new Exception("Error en la conexión a la base de datos: " + ex.Message);
                }
            }

            return false; // No se realizó ninguna actualización
        }


        //***************************************
        //*************************************** EDITAR PERFIL DE USUARIO
        //***************************************

    }


}




