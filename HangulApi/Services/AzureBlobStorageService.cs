//using Azure.Storage.Blobs;
//using System.Text;
//using System.Text.Json;

//namespace HangulApi.Services;

//public class AzureBlobStorageService : IFileStorageService
//{
//    private readonly BlobContainerClient _containerClient;
//    private readonly JsonSerializerOptions _metadataOptions;

//    public AzureBlobStorageService(IConfiguration configuration)
//    {
//        var connectionString = configuration["AzureBlobStorage:ConnectionString"];
//        var containerName = configuration["AzureBlobStorage:ContainerName"];
//        _containerClient = new BlobContainerClient(connectionString, containerName);
//        _containerClient.CreateIfNotExists();
//        _metadataOptions = new JsonSerializerOptions { WriteIndented = true };
//    }

//    public async Task<string> UploadAudioAsync(Guid jamoId, IFormFile file, CancellationToken cancellationToken = default)
//    {
//        var folderPath = $"{jamoId}/audios";
//        return await UploadFileAsync(folderPath, file);
//    }

//    public async Task<string> UploadImageAsync(Guid jamoId, IFormFile file, CancellationToken cancellationToken = default)
//    {
//        var folderPath = $"{jamoId}/images";
//        return await UploadFileAsync(folderPath, file);
//    }

//    public async Task<string> UploadJamoMetadataAsync(Guid jamoId, object jamoMetadata, CancellationToken cancellationToken = default)
//    {
//        var folderPath = $"{jamoId}";
//        var fileName = "jamo.json";

//        var blobClient = _containerClient.GetBlobClient($"{folderPath}/{fileName}");

//        var json = JsonSerializer.Serialize(jamoMetadata, _metadataOptions);
//        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));

//        await blobClient.UploadAsync(stream, overwrite: true, cancellationToken);
//        return blobClient.Uri.ToString();
//    }

//    private async Task<string> UploadFileAsync(string folderPath, IFormFile file, CancellationToken cancellationToken = default)
//    {
//        var fileName = file.FileName;
//        var blobClient = _containerClient.GetBlobClient($"{folderPath}/{fileName}");

//        await using var stream = file.OpenReadStream();
//        await blobClient.UploadAsync(stream, overwrite: true, cancellationToken);

//        return blobClient.Uri.ToString();
//    }
//}
