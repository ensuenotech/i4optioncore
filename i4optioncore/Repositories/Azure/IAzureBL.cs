using i4optioncore.Models;
using System.Threading.Tasks;

namespace i4optioncore.Repositories
{
    public interface IAzureBL
    {
        string AzureImageUrl();
        string AzureContainerReference();
        Task<CommonModel.FileDetails> UploadBlob(byte[] fileContent, string fileName, bool regenerateName);
        Task<string> DownloadBlob(string blobReference);
        Task<byte[]> DownloadBlobBytes(string blobReference);
        Task DeleteBlob(string blobReference);
    }
}
