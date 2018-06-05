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

            var ratingToAdd = await req.Content.ReadAsAsync<RatingObject>();

            if (ratingToAdd.UserId == null || ratingToAdd.ProductId == null)
            {
                return req.CreateResponse(HttpStatusCode.BadRequest, "Please provide userId and productId");
            }

            using (var client = new HttpClient())
            {
                var userValidationResult =
                    await client.GetAsync($"https://serverlessohlondonuser.azurewebsites.net/api/GetUser?userId={ratingToAdd.UserId}");

                var productValidationResult =
                    await client.GetAsync($"https://serverlessohlondonproduct.azurewebsites.net/api/GetProduct?productId={ratingToAdd.ProductId}");

                if (!userValidationResult.IsSuccessStatusCode )
                {
                    return req.CreateResponse(HttpStatusCode.NotFound, "UserId not Found");
                }

                if (!productValidationResult.IsSuccessStatusCode)
                {
                    return req.CreateResponse(HttpStatusCode.NotFound, "ProductId not Found");
                }

                if(ratingToAdd.Rating>5 || ratingToAdd.Rating <0) return req.CreateResponse(HttpStatusCode.BadRequest, "Rating should be within 1-5");


            }

            ratingToAdd.id = Guid.NewGuid().ToString();
            ratingToAdd.Timestamp = DateTime.Now.ToString();

            try
            {
                await document.AddAsync(ratingToAdd);
                return req.CreateResponse(HttpStatusCode.OK, ratingToAdd);
            }
            catch (Exception e)
            {
                return req.CreateResponse(HttpStatusCode.BadRequest, $"Adding record failed. Error:{e.ToString()}");
            }



            //return name == null
            //    ? req.CreateResponse(HttpStatusCode.BadRequest, "Please pass a name on the query string or in the request body")
            //    : req.CreateResponse(HttpStatusCode.OK, ratingToAdd);

            
        }
    }
}
