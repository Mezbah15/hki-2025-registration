using System.ComponentModel.DataAnnotations;

namespace hki_2025_registration.Models
{
    public class ShopKeeper
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [RegularExpression(@"^\d{11}$", ErrorMessage = "Contact number must be exactly 11 digits.")]
        public string Contact { get; set; }
        public string Address { get; set; }
        public string? Description { get; set; }
    }
}
