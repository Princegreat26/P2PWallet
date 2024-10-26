using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace P2PWallet.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PayStackController : ControllerBase
    {
        private const string PaystackUrl = "https://api.paystack.co/transaction/initialize";
        private const string SecretKey = "";

        [HttpPost("initialize")]
        public async Task<IActionResult> InitializeTransaction(string email, decimal amount)
        {
            using var httpClient = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, PaystackUrl);
            request.Headers.Add("Authorization", $"Bearer {SecretKey}");
            request.Headers.Add("Content-Type", "application/json");

            var data = new
            {
                email = email,
                amount = (int)(amount * 100) // amount in kobo
            };
            var jsonContent = JsonConvert.SerializeObject(data);
            request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await httpClient.SendAsync(request);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var responseData = JsonConvert.DeserializeObject<PaystackResponse>(responseBody);
                return Ok(new
                {
                    status = true,
                    message = "Authorization URL created",
                    data = new
                    {
                        authorization_url = responseData.Data.AuthorizationUrl,
                        access_code = responseData.Data.AccessCode,
                        reference = responseData.Data.Reference
                    }
                });
            }
            else
            {
                return BadRequest(responseBody);
            }
        }

        private class PaystackResponse
        {
            public bool Status { get; set; }

            public string Message { get; set; }

            public PaystackData Data { get; set; }
        }

        private class PaystackData
        {
            [JsonProperty("authorization_url")]

            public string AuthorizationUrl { get; set; }

            public string AccessCode { get; set; }

            public string Reference { get; set; }
        }
    }
}
