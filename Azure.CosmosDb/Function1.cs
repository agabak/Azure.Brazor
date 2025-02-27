using System;
using System.Collections.Generic;
using Azure.CosmosDb.Documents;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Azure.CosmosDb
{
    public class Function1
    {
        private readonly ILogger _logger;

        public Function1(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<Function1>();
        }

        [Function("Function1")]
        public void Run([CosmosDBTrigger(
            databaseName: "%TS:CosmosDbSettings:DatabaseName%",
            containerName: "%TS:CosmosDbSettings:ContainerName%",
            Connection = "%TS:CosmosDbSettings:MobileEldConnectionString%",
            LeaseContainerName = "leases",
            CreateLeaseContainerIfNotExists = true)] IReadOnlyList<ChangeFeedDataDocument> input)
        {
            if (input != null && input.Count > 0)
            {
                _logger.LogInformation("Documents modified: " + input.Count);
                _logger.LogInformation("First document Id: " + input[0].Id);
            }
        }
    } 
}
