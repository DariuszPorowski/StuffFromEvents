using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using EventGrid.Models;
using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Extensions.Configuration;

namespace EventGrid
{
    class Program
    {
        public static IConfiguration Configuration { get; set; }
 
        public static async Task Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                        .Build();

            Configuration = builder;
            await SendToEventGridWithSdk();
        }

        private static async Task SendToEventGridWithSdk()
        {
            try
            {
                var eventGridClient = new EventGridClient(new TopicCredentials(Configuration["eventGridTopicKey"]));

                IList<EventGridEvent> eventGridEvents = new List<EventGridEvent>
                {
                    new EventGridEvent
                    {
                        EventTime = DateTime.UtcNow,
                        Id = Guid.NewGuid().ToString(),
                        Subject = "custom/ServerlessDEMO",
                        DataVersion = "1.0",
                        EventType = "DemoStarted",
                        Data = new EventGridData{FirstName = "Dariusz", LastName = "Porowski"}
                    }
                };

                await eventGridClient.PublishEventsAsync(Configuration["eventGridTopicHostname"], eventGridEvents);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
