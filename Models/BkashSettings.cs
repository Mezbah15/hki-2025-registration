namespace hki_2025_registration.Models
{
    public class BkashSettings
    {
        public string BaseUrl { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string AppKey { get; set; }
        public string AppSecret { get; set; }
        public string CallbackUrl { get; set; }
        public int Amount { get; internal set; }
    }
}