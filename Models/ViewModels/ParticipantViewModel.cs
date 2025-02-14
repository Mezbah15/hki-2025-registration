using System.ComponentModel.DataAnnotations;

namespace hki_2025_registration.Models.ViewModels
{
    public class ParticipantViewModel
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string FatherName { get; set; }

        [RegularExpression(@"^\d{11}$", ErrorMessage = "Contact number must be exactly 11 digits.")]
        public string Contact { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string? Email { get; set; }

        public string Area { get; set; }

        public string Address { get; set; }

        public string Institute { get; set; }

        [Required(ErrorMessage = "Date of Birth is Required")]
        public DateOnly DoB { get; set; } = new DateOnly(2009, 1, 1);

        public string Choice { get; set; }
        public IFormFile Image { get; set; }

        internal async Task<Participant> ToDomainAsync()
        {
            var participant = new Participant
            {
                Name = Name,
                FatherName = FatherName,
                Contact = Contact,
                Email = Email ?? "Not Given",
                Area = Area,
                Address = Address,
                Institute = Institute,
                DoB = DoB,
                Choice = Choice,
                CreatePaymentResponse = "test",
                InvoiceNumber = GenerateInvoiceNumber(),
                Image = await SaveImageAsync(Image),
            };

            return participant;
        }

        private string GenerateInvoiceNumber()
        {
            return $"INV-{DateTime.Now:yyyyMMddHHmmss}-{Guid.NewGuid().ToString().Substring(0, 8)}";
        }

        private async Task<string> SaveImageAsync(IFormFile image)
        {
            var imagesDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
            if (!Directory.Exists(imagesDirectory))
            {
                Directory.CreateDirectory(imagesDirectory);
            }

            var imageFileName = $"{Guid.NewGuid()}_{image.FileName}";
            var imagePath = Path.Combine(imagesDirectory, imageFileName);
            using (var stream = new FileStream(imagePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            return imageFileName;
        }
    }
}
