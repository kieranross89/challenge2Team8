using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Challenge2Team8.Models;

namespace CosmosDBSamplesV1
{
    public static class GetRatings
    {
        [FunctionName("GetRatings")]
        public static HttpResponseMessage Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "GetRatings/{userId}")]HttpRequestMessage req,
          [DocumentDB( databaseName: "icecream",
                collectionName: "icecreamcoll",
                ConnectionStringSetting = "CosmosDB",
                SqlQuery = "SELECT * FROM c where c.UserId={userId}")] IEnumerable<RatingObject> ratingObjects, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");
            if (ratingObjects.ToString() == "[]")
            {
                log.Info($"Rating Object not found");
                return req.CreateResponse(HttpStatusCode.NotFound, "No rating object found.");
            }
            else
            {
                log.Info($"Found Rating Object(s)");
                return req.CreateResponse(HttpStatusCode.OK, ratingObjects);
            }
            
        }
    }
}