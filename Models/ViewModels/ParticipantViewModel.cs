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
        public string Email { get; set; }

        public string Area { get; set; }

        public string Address { get; set; }

        public string Institute { get; set; }
        public DateTime DoB { get; set; } = new DateTime(2009, 1, 1);
        public string Choice { get; set; }
        public IFormFile Image { get; set; }
    }
}
