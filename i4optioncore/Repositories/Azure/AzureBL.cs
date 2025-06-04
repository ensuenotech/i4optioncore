using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using i4optioncore.DBModelsUser;
using i4optioncore.Models;
using Microsoft.Extensions.Configuration;

using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace i4optioncore.Repositories
{
    public class AzureBL:IAzureBL
    {
        I4optionUserDbContext db;
        public AzureBL(IConfiguration _configuration, I4optionUserDbContext _db)
        {
            db = _db;
        }
        #region Properties

        public string AzureContainerReference()
        {
            //Azurecontainerreference
            //return ExtensionMethods.MyExtensions.Decrypt(FindConfiguration("ClientName", )).Replace(" ", "").ToLower();
            return db.Configurations.FirstOrDefault(x => x.Key == "azure.azurecontainerreference").Value;
            //return "testblob";
        }
        private string AzureStorageConnectionString()
        {
            return db.Configurations.FirstOrDefault(x => x.Key == "azure.azurestorageconnectionstring").Value;


        }
        public string AzureImageUrl()
        {
            return db.Configurations.FirstOrDefault(x => x.Key == "azure.azureimageurl").Value;


        }
        #endregion

        public async Task<CommonModel.FileDetails> UploadBlob(byte[] fileContent, string fileName, bool regenerateName)
        {
            // Retrieve the connection string for use with the application.
            var storageConnectionString = AzureStorageConnectionString();

            try
            {
                // Create the BlobServiceClient that represents the Blob storage endpoint for the storage account.
                var blobServiceClient = new BlobServiceClient(storageConnectionString);

                // Get a reference to a container
                var blobContainerClient = blobServiceClient.GetBlobContainerClient(AzureContainerReference().ToLower());

                // Create the container if it does not exist
                await blobContainerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

                var reference = fileName;
                if (regenerateName)
                    reference = Guid.NewGuid() + "|" + fileName;

                // Get a reference to a blob
                var blobClient = blobContainerClient.GetBlobClient(reference);

                // Check if the blob already exists
                if (await blobClient.ExistsAsync())
                {
                    await blobClient.DeleteAsync();
                }

                var fileInfo = new FileInfo(fileName);
                var contentType = fileInfo.Extension.Replace(".", "").ToLower() switch
                {
                    "jpg" or "jpeg" => "image/jpeg",
                    "gif" => "image/gif",
                    "png" => "image/png",
                    "bmp" => "image/bmp",
                    "pdf" => "application/pdf",
                    "doc" or "docx" => "application/vnd.ms-word",
                    "xls" or "xlsx" => "application/vnd.ms-excel",
                    _ => "application/octet-stream"
                };

                var blobHttpHeaders = new BlobHttpHeaders { ContentType = contentType };

                using var stream = new MemoryStream(fileContent);

                // Upload the blob
                await blobClient.UploadAsync(stream, blobHttpHeaders);

                return new CommonModel.FileDetails
                {
                    Blob = reference,
                    FileName = fileName,
                    Url = $"{AzureImageUrl()}{AzureContainerReference().ToLower()}/{reference}"
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error uploading to blob: {ex.Message}", ex);
            }
        }

        public async Task<string> DownloadBlob(string blobReference)
        {
            var storageConnectionString = AzureStorageConnectionString();

            try
            {
                // Create a BlobServiceClient that represents the Blob storage endpoint for the storage account.
                var blobServiceClient = new BlobServiceClient(storageConnectionString);

                // Get a reference to the container
                var containerClient = blobServiceClient.GetBlobContainerClient(AzureContainerReference().ToLower());

                // Extract the file name from the blobReference
                var fileName = blobReference.Substring(blobReference.IndexOf("|", StringComparison.Ordinal) + 1);
                var destinationFile = Path.Combine(Path.GetTempPath(), fileName);

                // Get a reference to the blob
                var blobClient = containerClient.GetBlobClient(blobReference);

                // Check if the file is locked and, if so, generate a new destination file name
                if (IsFileLocked(destinationFile))
                {
                    destinationFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + fileName);
                }

                // Download the blob to a file
                await blobClient.DownloadToAsync(destinationFile);

                // Ensure the file exists before returning
                while (!System.IO.File.Exists(destinationFile))
                {
                    await Task.Delay(100); // Wait for 100 ms before checking again
                }

                return destinationFile;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error downloading blob: {ex.Message}", ex);
            }
        }
        public async Task<byte[]> DownloadBlobBytes(string blobReference)
        {
            var storageConnectionString = AzureStorageConnectionString();

            try
            {
                // Create a BlobServiceClient that represents the Blob storage endpoint for the storage account.
                var blobServiceClient = new BlobServiceClient(storageConnectionString);

                // Get a reference to a container
                var containerReference = AzureContainerReference().ToLower();
                var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerReference);

                // Get a reference to the blob
                var blobClient = blobContainerClient.GetBlobClient(blobReference);

                using (var ms = new MemoryStream())
                {
                    // Download the blob's contents to a stream
                    await blobClient.DownloadToAsync(ms);
                    return ms.ToArray();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error downloading blob: {ex.Message}", ex);
            }
        }

        public async Task DeleteBlob(string blobReference)
        {
            var storageConnectionString = AzureStorageConnectionString();

            try
            {
                // Create a BlobServiceClient that represents the Blob storage endpoint for the storage account.
                var blobServiceClient = new BlobServiceClient(storageConnectionString);

                // Get a reference to the container
                var containerClient = blobServiceClient.GetBlobContainerClient(AzureContainerReference().ToLower());

                // Get a reference to the blob
                var blobClient = containerClient.GetBlobClient(blobReference);

                // Delete the blob if it exists
                await blobClient.DeleteIfExistsAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting blob: {ex.Message}", ex);
            }
        }


        private bool IsFileLocked(string filename)
        {
            var Locked = false;
            try
            {
                var fs = File.Open(filename, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
                fs.Close();
            }
            catch (IOException ex)
            {
                Locked = true;
            }
            return Locked;
        }
    }
}
