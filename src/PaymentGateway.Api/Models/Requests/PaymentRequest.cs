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


