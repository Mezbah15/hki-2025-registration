using System.Diagnostics;
using System.Text.Json;
using System.Text.RegularExpressions;
using Aspose.Pdf;
using Aspose.Pdf.Text;
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
        private readonly EmailService _emailService;
        private readonly PdfService _pdfService;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, 
            BkashService bkashService, EmailService emailService,
            PdfService pdfService)
        {
            _logger = logger;
            _context = context;
            _bkashService = bkashService;
            _emailService = emailService;
            this._pdfService = pdfService;
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

        public async Task GeneratePdf()
        {
            Participant participant = new Participant
            {
                Id = 15,
                Name = "??? ??????? ?????",
                FatherName = "??? ??????? ?????",
                Contact = "01710870812",
                Email = "marufbdonline@gmail.com",
                Area = "Haragach",
                Address = "???????",
                Institute = "??? ??????? ?????? ??????? ????? ???????",
                DoB = DateOnly.Parse("1994-09-15"),
                Choice = "hamdth",
                Image = "2394a4dc-18e2-4300-a266-4ecb25b9cc6e_0c2d8058-5051-442f-b112-9c3b05417417(1)(1).jpeg",
                InvoiceNumber = "INV-20250214012246-ec0187c0",
                PaymentId = "TR0011i4S05Pu1739474569176",
                CreatePaymentResponse = "{\"paymentID\":\"TR0011i4S05Pu1739474569176\",\"bkashURL\":\"https://payment.bkash.com/?paymentId=TR0011i4S05Pu1739474569176&hash=dvDk8JCrGUqT1YI*YBZek-7l8AHWRi9fpgNwkbl!!)9-X0sDSM3U*fLJ-FPI5xF2Df3PJ)c!QvAB0pCPOczAMebDR_XVrR37nbTb1739474569176&mode=0011&apiVersion=v1.2.0-beta/\",\"callbackURL\":\"https://localhost:7280/Home/Callback\",\"successCallbackURL\":\"https://localhost:7280/Home/Callback?paymentID=TR0011i4S05Pu1739474569176&status=success&signature=v8eBia1EZy\",\"failureCallbackURL\":\"https://localhost:7280/Home/Callback?paymentID=TR0011i4S05Pu1739474569176&status=failure&signature=v8eBia1EZy\",\"cancelledCallbackURL\":\"https://localhost:7280/Home/Callback?paymentID=TR0011i4S05Pu1739474569176&status=cancel&signature=v8eBia1EZy\",\"amount\":\"1\",\"intent\":\"sale\",\"currency\":\"BDT\",\"paymentCreateTime\":\"2025-02-14T01:22:49:176 GMT+0600\",\"transactionStatus\":\"Initiated\",\"merchantInvoiceNumber\":\"INV-20250214012246-ec0187c0\",\"statusCode\":\"0000\",\"statusMessage\":\"Successful\"}",
                PaymentStatus = "Failure while execute {\"statusCode\":null,\"statusMessage\":null,\"paymentID\":null,\"payerReference\":null,\"customerMsisdn\":null,\"trxID\":null,\"amount\":null,\"transactionStatus\":null,\"paymentExecuteTime\":null,\"currency\":null,\"intent\":null,\"merchantInvoiceNumber\":null}",
                ExecutePaymentResponse = "{\"statusCode\":\"0000\",\"statusMessage\":\"Successful\",\"paymentID\":\"TR0011i4S05Pu1739474569176\",\"payerReference\":\"01710870812\",\"customerMsisdn\":\"01710870812\",\"trxID\":\"CBE7SHKV7T\",\"amount\":\"1\",\"transactionStatus\":\"Completed\",\"paymentExecuteTime\":\"2025-02-14T01:24:43:252 GMT+0600\",\"currency\":\"BDT\",\"intent\":\"sale\",\"merchantInvoiceNumber\":\"INV-20250214012246-ec0187c0\"}"
            };

            string pdfFilePath = "ParticipantHallTicket.pdf";
            PdfService.GenerateHallTicketPdf(pdfFilePath, participant);

            Console.WriteLine($"PDF Hall Ticket generated successfully at: {pdfFilePath}");
        }

        [HttpGet]
        public async Task<IActionResult> Callback(string? apiVersion, string? product, string paymentID, string status, string? signature)
        {
            try
            {
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                _logger.LogInformation("Callback received from IP: {IpAddress}, apiVersion: {ApiVersion}, product: {Product}, paymentID: {PaymentID}, status: {Status}, signature: {Signature}", ipAddress, apiVersion, product, paymentID, status, signature);

                var participant = await _context.Participants.FirstOrDefaultAsync(p => p.PaymentId == paymentID);
                var data = JsonConvert.SerializeObject(participant);
                if (participant == null)
                {
                    _logger.LogError("Participant not found for payment ID {PaymentId}", paymentID);
                    throw new Exception("Participant not found. Might be Malicious callback.");
                }
                else
                {
                    if (status == "success")
                    {
                        var response = new BkashExecutePaymentResponse(); //await _bkashService.ExecutePaymentAsync(paymentID);
                        response.statusMessage = "Successful";
                        response.paymentID = participant.PaymentId;
                        if (response.statusMessage == "Successful" && response.paymentID == participant.PaymentId)
                        {
                            participant.PaymentStatus = "Success";
                            participant.ExecutePaymentResponse = JsonConvert.SerializeObject(response);

                            // Generate PDF invoice
                            var invoiceBytes = GenerateInvoice(participant);

                            // Send email with PDF attachment
                            var emailBody = $"Dear {participant.Name},\n\nThank you for your registration. Please find your invoice attached.";
                            await _emailService.SendEmailAsync(participant.Email, "Your Registration Invoice", emailBody, invoiceBytes, "Invoice.pdf");
                            _logger.LogInformation("Payment successful for participant {ParticipantId} with payment ID {PaymentId}", participant.Id, paymentID);

                            // Return success view with invoice
                            return View("PaymentSuccess", new PaymentResultViewModel
                            {
                                Participant = participant,
                                InvoiceBytes = invoiceBytes
                            });
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

            // Return failure view
            return View("PaymentFailure");
        }

        private byte[] GenerateInvoice(Participant participant)
        {
            // Create a new PDF document
            var pdfDocument = new Document();
            var page = pdfDocument.Pages.Add();

            // Add title to the PDF
            var title = new TextFragment("Invoice");
            title.TextState.FontSize = 20;
            title.TextState.FontStyle = FontStyles.Bold;
            title.HorizontalAlignment = HorizontalAlignment.Center;
            page.Paragraphs.Add(title);

            // Add participant image
            if (!string.IsNullOrEmpty(participant.Image))
            {
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", participant.Image);
                var imageStream = new FileStream(imagePath, FileMode.Open, FileAccess.Read);
                var image = new Aspose.Pdf.Image { ImageStream = imageStream };
                page.Paragraphs.Add(image);
            }

            // Add participant information
            var participantInfo = new TextFragment($"Name: {participant.Name}\nContact: {participant.Contact}\nEmail: {participant.Email}\nPayment ID: {participant.PaymentId}\nPayment Status: {participant.PaymentStatus}");
            participantInfo.Margin.Top = 20;
            page.Paragraphs.Add(participantInfo);

            // Add a table for better layout
            var table = new Table
            {
                ColumnWidths = "100 400",
                Border = new BorderInfo(BorderSide.All, 0.5f, Color.Black),
                DefaultCellBorder = new BorderInfo(BorderSide.All, 0.5f, Color.Black)
            };

            // Add table header
            var headerRow = table.Rows.Add();
            headerRow.Cells.Add("Field").BackgroundColor = Color.Gray;
            headerRow.Cells.Add("Value").BackgroundColor = Color.Gray;

            // Add participant details to the table
            AddTableRow(table, "Name", participant.Name);
            AddTableRow(table, "Contact", participant.Contact);
            AddTableRow(table, "Email", participant.Email);
            AddTableRow(table, "Payment ID", participant.PaymentId);
            AddTableRow(table, "Payment Status", participant.PaymentStatus);

            page.Paragraphs.Add(table);

            // Save the PDF to a memory stream
            using (var stream = new MemoryStream())
            {
                pdfDocument.Save(stream);
                return stream.ToArray();
            }
        }

        private void AddTableRow(Table table, string fieldName, string fieldValue)
        {
            var row = table.Rows.Add();
            row.Cells.Add(fieldName);
            row.Cells.Add(fieldValue);
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
