using System.Text.Json;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using AzureEventHubDemo.Core;
using AzureEventHubDemo.Core.Constants;
using AzureEventHubDemo.Core.Models;

await StartEventGenerating();

async Task StartEventGenerating()
{
  var connectionString = "Endpoint=sb://eventhubcourse250.servicebus.windows.net/;SharedAccessKeyName=sensorpublisher;SharedAccessKey=IGDaFI2Nmd9fsm9KoQzq3z3+9PbYQXmbi+AEhFpzf0Q=";
  var eventHubName = "sensordata";

  await using (var producer = new EventHubProducerClient(connectionString, eventHubName))
  {
    var events = EventGenerator.GetSensorEvents(Sensors.DoorSensor, 900);
    System.Console.WriteLine(events.Count());
    var eventBatch = await producer.CreateBatchAsync();

    foreach (var sensorEvent in events)
    {
      var eventAddedSuccessully = eventBatch.TryAdd(new EventData(JsonSerializer.Serialize(sensorEvent)));

      if (!eventAddedSuccessully)
      {
        if (eventBatch.Count > 0)
        {
          await producer.SendAsync(eventBatch);
          Console.WriteLine($"Batch data sent for {eventBatch.Count} events out of {events.Count()} events because size limit reached");
          eventBatch = await producer.CreateBatchAsync();
          // add unsuccessfully added sensorEvent from last interation
          eventBatch.TryAdd(new EventData(JsonSerializer.Serialize(sensorEvent)));
        }
        else
        {
          // event size is too big and cannot be added. Event needs to be skipped
          //log the error
        }
      }
    }

    if (eventBatch.Count > 0)
    {
      await producer.SendAsync(eventBatch);
      Console.WriteLine($"Batch data sent rest of events - {eventBatch.Count}");
    }

    eventBatch.Dispose();

    Console.WriteLine("Complete publisher work");
  }
}