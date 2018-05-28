// This is the default URL for triggering event grid function in the local environment.
// http://localhost:7071/admin/extensions/EventGridExtensionConfig?functionName={functionname} 

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using System;
using Microsoft.WindowsAzure.Storage.Blob;
using BlobImageProccesing.Helpers;
using System.IO;
using ImageResizer;
using Newtonsoft.Json.Linq;

namespace BlobImageProccesing
{
    public static class ImageProcessing
    {
        [FunctionName("ImageProcessing")]
        public static void Run([EventGridTrigger] JObject eventGridEvent, TraceWriter log)
        {
            var eventGridInfo = eventGridEvent.ToObject<dynamic>();

            log.Info($"Function trigger with data: {eventGridInfo}");

            var eventType = eventGridInfo.eventType.ToString();
            var imageUrl = new Uri(eventGridInfo.data.url.ToString());

            log.Info($"Triggering with event {eventType} and url {imageUrl.ToString()}");

            switch (eventType)
            {
                case "Microsoft.Storage.BlobCreated":
                    CreateAction(imageUrl, log);
                    break;
                case "Microsoft.Storage.BlobDeleted":
                    DeleteAction(imageUrl, log);
                    break;
                default:
                    throw new ArgumentException($"Unsupported event type: {eventType}");
            }
        }

        private static void CreateAction(Uri imageUrl, TraceWriter log)
        {
            var cloudBlob = StorageHelper.GetBlobReference(imageUrl);
            string blobName = StorageHelper.GetBlobName(cloudBlob);

            var instructions = new Instructions
            {
                Width = Int32.Parse(GetEnvVariable("thumbWidth", "400")),
                Height = Int32.Parse(GetEnvVariable("thumbHeight", "300")),
                Mode = FitMode.Max,
                Scale = ScaleMode.DownscaleOnly
            };

            var outputBlob = StorageHelper.GetThumbnailBlobReference(blobName);

            using (MemoryStream inStream = new MemoryStream())
            {
                StorageHelper.RequestInputBlob(cloudBlob, inStream);
                inStream.Position = 0;

                using (MemoryStream outStream = new MemoryStream())
                {
                    try
                    {
                        ImageBuilder.Current.Build(new ImageJob(inStream, outStream, instructions));
                        outStream.Position = 0;
                        outputBlob.UploadFromStream(outStream);
                    }
                    catch (Exception e)
                    {
                        log.Info($"Exception caught at: {DateTime.UtcNow}, Message: {e.Message}");
                    }
                }
            }
        }

        private static void DeleteAction(Uri imageUrl, TraceWriter log)
        {
            var deletedImage = new CloudBlob(imageUrl);

            try
            {
                StorageHelper.GetThumbnailBlobReference(deletedImage.Name).DeleteIfExists();
            }
            catch (Exception e)
            {
                log.Info($"Exception caught at: {DateTime.UtcNow}, Message: {e.Message}");
            }

        }

        private static string GetEnvVariable(string key, string defaultValue)
        {
            var envVar = Environment.GetEnvironmentVariable(key);

            if (envVar == "" || envVar == null)
            {
                envVar = defaultValue;
            }

            return envVar;
        }
    }
}
