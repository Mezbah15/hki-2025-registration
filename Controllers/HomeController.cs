using System.Diagnostics;
using System.Text.Json;
using System.Text.RegularExpressions;
using hki_2025_registration.Models;
using hki_2025_registration.Models.ViewModels;
using hki_2025_registration.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace hki_2025_registration.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly BkashService _bkashService;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, BkashService bkashService)
        {
            _logger = logger;
            _context = context;
            _bkashService = bkashService;
        }

        public IActionResult Index()
        {
            var model = new ParticipantViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Index(ParticipantViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                if (!Regex.IsMatch(model.Contact, @"^(?:\+88|88)?(01[3-9]\d{8})$"))
                {
                    ModelState.AddModelError("Contact", "Invalid contact number format.");
                    return View(model);
                }

                var participant = await model.ToDomainAsync();
                var paymentResponse = await _bkashService.CreatePaymentAsync(participant.InvoiceNumber, 1, participant.Contact);
                participant.CreatePaymentResponse = JsonConvert.SerializeObject(paymentResponse);
                participant.PaymentId = paymentResponse.paymentID;
                participant.PaymentStatus = paymentResponse.transactionStatus;
                participant.ExecutePaymentResponse = "NotExecuted";

                _context.Participants.Add(participant);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Participant {ParticipantId} created successfully with payment ID {PaymentId}", participant.Id, paymentResponse.paymentID);

                return Redirect(paymentResponse.bkashURL);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing payment for participant {ParticipantId}", model.Id);
                ModelState.AddModelError(string.Empty, "Something went wrong, Contact Administrator.");
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Callback(string? apiVersion, string? product, string paymentID, string status, string? signature)
        {
            try
            {
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                _logger.LogInformation("Callback received from IP: {IpAddress}, apiVersion: {ApiVersion}, product: {Product}, paymentID: {PaymentID}, status: {Status}, signature: {Signature}", ipAddress, apiVersion, product, paymentID, status, signature);

                var participant = await _context.Participants.FirstOrDefaultAsync(p => p.PaymentId == paymentID);
                if (participant == null)
                {
                    _logger.LogError("Participant not found for payment ID {PaymentId}", paymentID);
                    throw new Exception("Participant not found. Might be Malicious callback.");
                }
                else
                {
                    if (status == "success")
                    {
                        var response = await _bkashService.ExecutePaymentAsync(paymentID);
                        if (response.statusMessage == "Successful" && response.paymentID == participant.PaymentId)
                        {
                            participant.PaymentStatus = "Success";
                            participant.ExecutePaymentResponse = JsonConvert.SerializeObject(response);
                            _logger.LogInformation("Payment successful for participant {ParticipantId} with payment ID {PaymentId}", participant.Id, paymentID);
                        }
                        else
                        {
                            participant.PaymentStatus = $"Failure while execute {JsonConvert.SerializeObject(response)}";
                            _logger.LogInformation("Failure while execute for participant {ParticipantId} with payment ID {PaymentId}. {Response}", participant.Id, paymentID, JsonConvert.SerializeObject(response));
                        }
                    }
                    else if (status == "failure")
                    {
                        participant.PaymentStatus = "Failure";
                        _logger.LogInformation("Payment failed for participant {ParticipantId} with payment ID {PaymentId}", participant.Id, paymentID);
                    }
                    else if (status == "cancel")
                    {
                        participant.PaymentStatus = "Cancelled";
                        _logger.LogInformation("Payment cancelled for participant {ParticipantId} with payment ID {PaymentId}", participant.Id, paymentID);
                    }

                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing callback for payment ID {PaymentId}", paymentID);
            }

            return RedirectToAction("Index");
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
