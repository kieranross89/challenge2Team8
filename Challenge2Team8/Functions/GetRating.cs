using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System.Net;
using System.Net.Http;

namespace CosmosDBSamplesV1
{
    public static class GetRating
    {
        [FunctionName("GetRating")]
        public static HttpResponseMessage Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req,
            [DocumentDB(
                databaseName: "icecream",
                collectionName: "icecreamcoll",
                ConnectionStringSetting = "dbConn",
                Id = "{Query.id}")] Challenge2Team8.Models.RatingObject ratingObject,
            TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");
            if (ratingObject == null)
            {
                log.Info($"Rating Object not found");
            }
            else
            {
                log.Info($"Found Rating Object, Description={ratingObject.Id}");
            }
            return req.CreateResponse(HttpStatusCode.OK,ratingObject);
        }
    }
}