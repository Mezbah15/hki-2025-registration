namespace hki_2025_registration.Models
{
    public class BkashTokenResponse
    {
            public string id_token { get; set; }   // Corresponds to "id_token" in JSON
            public string statusMessage { get; set; }  // Corresponds to "token_type" in JSON
            public int statusCode { get; set; }
    }
}
