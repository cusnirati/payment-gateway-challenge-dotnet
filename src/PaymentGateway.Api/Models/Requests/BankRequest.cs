using System.Text.Json.Serialization;

namespace PaymentGateway.Api.Models.Requests;

public class BankRequest
{
    [JsonPropertyName("card_number")]
    public string CardNumber { get; set; }

    [JsonPropertyName("expiry_date")]
    public string ExpiryDate { get; set; }

    public string Currency { get; set; }
    public long Amount { get; set; }
    public string Cvv { get; set; }

    //   "card_number": "2222405343248877",
    //   "expiry_date": "04/2025",
    //   "currency": "GBP",
    //   "amount": 100,
    //   "cvv": "123"
}