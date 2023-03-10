Create resource group eventhubcourse

Add event hub namespace eventhubcourse250

Add event hub sensordata and set its name to 

Add SAS policy in Settings => Shared access policies
Name = sensorloggenerator and claims = Send
(you can add same SAS policy to event hub sensordata instead of namespace adding namespace SAS policy)

copy Connection string–primary key and add as parameter to EventHubProducerClient (producer)
copy event hub name (sensordata) nd add as parameter to EventHubProducerClient (producer)

Create EventDataBatch obj using producer.CreateBatchAsync()

Serialize and add events using eventBatch.TryAdd()

Send event to event hub using producer.SendAsync(eventBatch)

Remember to dispose EventDataBatch obj