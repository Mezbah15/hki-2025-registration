using System.ComponentModel.DataAnnotations;

namespace hki_2025_registration.Models
{
    public class Participant
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string FatherName { get; set; }
        public string Contact { get; set; }
        public string Email { get; set; }

        public string Area { get; set; } 

        public string Address { get; set; }

        public string Institute { get; set; }
        public DateOnly DoB { get; set; }

        public string Choice { get; set; } 
        public string Image { get; set; }
        public string InvoiceNumber { get; set; }
        public string PaymentId { get; set; }
        public string CreatePaymentResponse { get; set; }
        public string PaymentStatus { get; internal set; }
        public string ExecutePaymentResponse { get; internal set; }
    }
}
