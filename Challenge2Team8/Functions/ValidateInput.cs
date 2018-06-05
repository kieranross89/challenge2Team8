using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Challenge2Team8.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace Challenge2Team8.Functions
{
    public static class ValidateInput
    {
        [FunctionName("validateinput")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            var data = await req.Content.ReadAsAsync<RatingObject>();

            if (data.UserId == null || data.ProductId == null)
            {
                return req.CreateResponse(HttpStatusCode.BadRequest, "Please provide userId and productId");
            }

            using (var client = new HttpClient())
            {
                var userValidationResult =
                    await client.GetAsync($"https://serverlessohlondonuser.azurewebsites.net/api/GetUser?userId={data.UserId}");

                var productValidationResult = 
                    await client.GetAsync($"https://serverlessohlondonproduct.azurewebsites.net/api/GetProduct?productId={data.ProductId}");

                if (!userValidationResult.IsSuccessStatusCode || !productValidationResult.IsSuccessStatusCode)
                {
                    return req.CreateResponse(HttpStatusCode.BadRequest, "Validation Failure");
                }
            }

            return req.CreateResponse(HttpStatusCode.OK, "Validation Success");
        }
    }
}
