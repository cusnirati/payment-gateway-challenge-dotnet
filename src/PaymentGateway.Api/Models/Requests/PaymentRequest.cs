using System.Text.Json.Serialization;

namespace PaymentGateway.Api.Models.Requests;

public class PaymentRequest
{
    // was this a mistake ?
    // public int CardNumberLastFour { get; set; }

    public string CardNumber { get; set; }
    public int ExpiryMonth { get; set; }
    public int ExpiryYear { get; set; }
    public string Currency { get; set; }

    public long Amount { get; set; } // or decimal ?
    public string Cvv { get; set; }
}

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

public class BankResponse
{
    public bool Authorized { get; set; }

    [JsonPropertyName("authorization_code")]
    public string AuthorizationCode { get; set; }

    //   "authorized": true,
    //   "authorization_code": "0bb07405-6d44-4b50-a14f-7ae0beff13ad"
}