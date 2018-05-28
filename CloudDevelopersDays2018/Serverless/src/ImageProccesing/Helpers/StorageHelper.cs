using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.IO;

namespace BlobImageProccesing.Helpers
{
    public static class StorageHelper
    {
        private static readonly CloudBlobClient cloudBlobClient = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("storageAccountCS")).CreateCloudBlobClient();
        private static readonly string imageContainerName = Environment.GetEnvironmentVariable("imageContainerName");
        private static readonly string thumbContainerName = Environment.GetEnvironmentVariable("thumbContainerName");

        public static CloudBlob GetBlobReference(Uri blobUri)
        {
            return (CloudBlob)cloudBlobClient.GetBlobReferenceFromServer(blobUri);
        }

        public static void RequestInputBlob(CloudBlob cloudBlob, Stream inputStream)
        {
            cloudBlob.DownloadToStream(inputStream);
        }

        public static CloudBlockBlob GetImageBlobReference(string blobName)
        {
            var container = cloudBlobClient.GetContainerReference(imageContainerName);
            return container.GetBlockBlobReference(blobName);
        }

        public static CloudBlockBlob GetThumbnailBlobReference(string blobName)
        {
            var container = cloudBlobClient.GetContainerReference(thumbContainerName);
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
            return imageContainerName;
        }

        public static string GetThumbContainerName()
        {
            return thumbContainerName;
        }
    }
}
