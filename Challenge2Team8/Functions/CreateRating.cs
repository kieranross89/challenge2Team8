using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Challenge2Team8.Models;
using System;

namespace Challenge2Team8.Functions
{
    public static class CreateRating
    {
        [FunctionName("CreateRating")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req, [DocumentDB("icecream", "icecreamcoll", Id = "id", ConnectionStringSetting = "CosmosDB")]IAsyncCollector<RatingObject> document,  TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            // parse query parameter
            string name = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "name", true) == 0)
                .Value;

            RatingObject ratingToAdd = new RatingObject { ProductId = "4c25613a-a3c2-4ef3-8e02-9c335eb23204", LocationName = "Sample ice cream shop", Rating = 5, UserId = "cc20a6fb-a91f-4192-874d-132493685376", UserNotes = "I love the subtle notes of orange in this ice cream!" };
            ratingToAdd.id = Guid.NewGuid().ToString();

            await document.AddAsync(ratingToAdd);
            

            if (name == null)
            {
                // Get request body
                dynamic data = await req.Content.ReadAsAsync<object>();
                name = data?.name;
            }

            //return name == null
            //    ? req.CreateResponse(HttpStatusCode.BadRequest, "Please pass a name on the query string or in the request body")
            //    : req.CreateResponse(HttpStatusCode.OK, ratingToAdd);

            return req.CreateResponse(HttpStatusCode.OK, ratingToAdd);
        }
    }
}
