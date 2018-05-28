// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGridExtensionConfig?functionName={functionname}

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using System;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace AnalyseImage
{
    public static class AnalyseImage
    {
        private static readonly IComputerVisionAPI computerVisionApi = new ComputerVisionAPI(new ApiKeyServiceClientCredentials(Environment.GetEnvironmentVariable("computerVisionApiKey"))) { AzureRegion = AzureRegions.Northeurope };
        private static readonly CloudBlobClient cloudBlobClient = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("storageAccountCS")).CreateCloudBlobClient();

        [FunctionName("AnalyseImage")]
        public static async Task Run([EventGridTrigger] EventGridEvent eventGridEvent, TraceWriter log)
        {
            log.Info($"Function trigger with data: {eventGridEvent.ToString()}");

            var eventType = eventGridEvent.EventType;
            var dataObject = eventGridEvent.Data as JObject;
            var eventData = dataObject.ToObject<StorageBlobCreatedEventData>();
            var imageUrl = eventData.Url;

            log.Info($"Triggering with event {eventType} and url {imageUrl}");

            var visualFeatureTypes = new List<VisualFeatureTypes>
            {
                VisualFeatureTypes.Adult,
                VisualFeatureTypes.Categories,
                VisualFeatureTypes.Color,
                VisualFeatureTypes.Description
                //VisualFeatureTypes.Faces,
                //VisualFeatureTypes.ImageType,
                //VisualFeatureTypes.Tags
            };

            var analyzeImage = await computerVisionApi.AnalyzeImageAsync(imageUrl, visualFeatureTypes);

            var blobReference = await cloudBlobClient.GetBlobReferenceFromServerAsync(new Uri(imageUrl));
            
            try
            {
                blobReference.Metadata.Add("IsAdultContent", analyzeImage.Adult.IsAdultContent.ToString());
                blobReference.Metadata.Add("AdultScore", analyzeImage.Adult.AdultScore.ToString("P0").Replace(" ", ""));
                blobReference.Metadata.Add("IsRacyContent", analyzeImage.Adult.IsRacyContent.ToString());
                blobReference.Metadata.Add("RacyScore", analyzeImage.Adult.RacyScore.ToString("P0").Replace(" ", ""));

                blobReference.Metadata.Add("Tags", string.Join(", ", analyzeImage.Description.Tags));
                blobReference.Metadata.Add("Category", analyzeImage.Categories[0].Name);
                blobReference.Metadata.Add("Description", analyzeImage.Description.Captions[0].Text);
                blobReference.Metadata.Add("IsBWImg", analyzeImage.Color.IsBWImg.ToString());

                await blobReference.SetMetadataAsync();
            }
            catch (Exception e)
            {
                log.Info($"Exception caught at: {DateTime.UtcNow}, Message: {e.Message}");
            }
        }
    }
}
