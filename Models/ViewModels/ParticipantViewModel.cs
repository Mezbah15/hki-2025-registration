using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
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

        public string? Institute { get; set; }

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
                Email = Email ?? "",
                Area = Area,
                Address = Address,
                Institute = Institute ?? "",
                DoB = DoB,
                Choice = Choice,
                CreatePaymentResponse = "test",
                InvoiceNumber = GenerateInvoiceNumber(),
                Image = await SaveImageToS3Async(Image),
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

        private async Task<string> SaveImageToS3Async(IFormFile image)
        {
            try
            {
                var accessKey = "0b34a74083a35c11e4590d9ee09757ad";
                var secretKey = "a3118d0c2526d3cdddcf1b5b77b1c5f03fa0e6b213cabedf4a39ffd58dc34ad0";
                var bucketName = "ski-images";
                var credentials = new BasicAWSCredentials(accessKey, secretKey);

                var imageFileName = $"{Guid.NewGuid()}_{image.FileName}";

                var config = new AmazonS3Config
                {
                    ServiceURL = "https://366cf6bcdccea547e047ba7c26b080c1.r2.cloudflarestorage.com/ski-images",
                };

                using (var s3Client = new AmazonS3Client(credentials, config))
                {
                    using (var newMemoryStream = new MemoryStream())
                    {
                        image.CopyTo(newMemoryStream);

                        var request = new PutObjectRequest
                        {
                            Key = imageFileName,
                            BucketName = bucketName,
                            InputStream = newMemoryStream,
                            DisablePayloadSigning = true
                        };

                        var response = await s3Client.PutObjectAsync(request);
                    }
                }

                return imageFileName;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
