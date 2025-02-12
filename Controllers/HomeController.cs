using System.Diagnostics;
using hki_2025_registration.Models;
using hki_2025_registration.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.CodeModifier.CodeChange;
using Newtonsoft.Json;
using RestSharp;

namespace hki_2025_registration.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly string baseUrl = "https://api.bkash.com"; // Replace with actual base URL
        private readonly string username = "your-username"; // Replace with actual username
        private readonly string password = "your-password"; // Replace with actual password
        private readonly string appKey = "test_app_key"; // Replace with actual app key
        private readonly string appSecret = "test_app_secret"; // Replace with actual app secret

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        public IActionResult Index()
        {
            var model = new ParticipantViewModel();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Index(ParticipantViewModel model)
        {
            var client = new RestClient($"https://tokenized.sandbox.bka.sh/v1.2.0-beta/tokenized/checkout/token/grant/");
            var request = new RestRequest();
            request.Method = RestSharp.Method.Post;
            // Set Headers
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Accept", "application/json");
            request.AddHeader("username", "01770618567");
            request.AddHeader("password", "D7DaC<*E*eG");

            // Request Body
            var requestBody = new
            {
                app_key = "0vWQuCRGiUX7EPVjQDr0EUAYtc",
                app_secret = "jcUNPBgbcqEDedNKdvE4G1cAK7D3hCjmJccNPZZBq96QIxxwAMEx"
            };

            request.AddJsonBody(JsonConvert.SerializeObject(requestBody));

            // Execute Request
            var response = await client.ExecuteAsync(request);

            BkashTokenResponse tokenResponse;
            if (response.IsSuccessful)
            {
                tokenResponse = JsonConvert.DeserializeObject<BkashTokenResponse>(response.Content);
            }
            else
            {
                return View();
            }

            var client1 = new RestClient($"https://tokenized.sandbox.bka.sh/v1.2.0-beta/tokenized/checkout/create");
            var request1 = new RestRequest();
            request1.Method = RestSharp.Method.Post;

            // Set headers
            request1.AddHeader("Content-Type", "application/json");
            request1.AddHeader("Accept", "application/json");
            request1.AddHeader("authorization", tokenResponse.id_token);
            request1.AddHeader("x-app-key", "0vWQuCRGiUX7EPVjQDr0EUAYtc");

            // Request body
                var   requestBody1 = new
            {
                mode = "0011",
                payerReference = "01723888888",
                callbackURL = "https://hki-2025.com/",
                merchantAssociationInfo = "MI05MID54RF09123456One",
                amount = "500",
                currency = "BDT",
                intent = "sale",
                merchantInvoiceNumber = "Inv0124"
                };

            request1.AddJsonBody(JsonConvert.SerializeObject(requestBody1));

            // Execute request
            var response1 = await client1.ExecuteAsync(request1);

            if (response1.IsSuccessful)
            {
                var responseData = response1.Content;
                return Content(responseData, "application/json");
            }
            else
            {
                return Content($"Error: {response1.StatusCode} - {response1.Content}", "text/plain");
            }
        }

        public IActionResult Contact()
        {
            return View();
        }
        public IActionResult About()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
