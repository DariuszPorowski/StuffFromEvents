using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.IO;

namespace BlobImageProccesing.Helpers
{
    public static class StorageHelper
    {
        private static CloudBlobClient _blobClient = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("storageAccountCS")).CreateCloudBlobClient();
        private static string _imageContainerName = Environment.GetEnvironmentVariable("imageContainerName");
        private static string _thumbContainerName = Environment.GetEnvironmentVariable("thumbContainerName");

        public static CloudBlob GetBlobReference(Uri blobUri)
        {
            return (CloudBlob)_blobClient.GetBlobReferenceFromServer(blobUri);
        }

        public static void RequestInputBlob(CloudBlob cloudBlob, Stream inputStream)
        {
            cloudBlob.DownloadToStream(inputStream);
        }

        public static CloudBlockBlob GetImageBlobReference(string blobName)
        {
            var container = _blobClient.GetContainerReference(_imageContainerName);
            return container.GetBlockBlobReference(blobName);
        }

        public static CloudBlockBlob GetThumbnailBlobReference(string blobName)
        {
            var container = _blobClient.GetContainerReference(_thumbContainerName);
            return container.GetBlockBlobReference(blobName);
        }

        public static string GetBlobName(CloudBlob cloudBlob)
        {
            return cloudBlob.Name;
        }

        public static string GetContainerName(CloudBlob cloudBlob)
        {
            return cloudBlob.Container.Name;
        }

        public static string GetImageContainerName()
        {
            return _imageContainerName;
        }

        public static string GetThumbContainerName()
        {
            return _thumbContainerName;
        }
    }
}
