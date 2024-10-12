namespace Proyect_1.Services;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

    public class BlobService
    {
        private readonly string _connectionString;
        private readonly string _containerName;

        public BlobService(IConfiguration configuration)
        {
            // Obtener la cadena de conexión de la configuración
            _connectionString = configuration.GetValue<string>("AzureBlobStorage:ConnectionString");
            // Obtener el nombre del contenedor de la configuración
            _containerName = configuration.GetValue<string>("AzureBlobStorage:ContainerName"); // Asegúrate de tener esta clave en tu appsettings.json
        }

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            // Crear el cliente de BlobService
            BlobServiceClient blobServiceClient = new BlobServiceClient(_connectionString);

            // Obtén una referencia al contenedor sin crearlo
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(_containerName);

            // Nombre del archivo (puedes cambiarlo según tus necesidades)
            string blobName = Guid.NewGuid().ToString() + "_" + file.FileName;

            // Obtén una referencia al blob
            BlobClient blobClient = containerClient.GetBlobClient(blobName);

            // Sube el archivo
            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream);
            }

            // Retorna la URL del blob
            return blobClient.Uri.ToString();
        }
    }




