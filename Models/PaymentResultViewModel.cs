namespace hki_2025_registration.Models
{
    public class PaymentResultViewModel
    {
        public Participant Participant { get; set; }
        public byte[] InvoiceBytes { get; set; }
        public string Base64Image { get; set; }
    }
}
