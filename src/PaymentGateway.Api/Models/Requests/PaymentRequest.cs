using System.Text.Json.Serialization;

namespace PaymentGateway.Api.Models.Requests;

public class PaymentRequest
{
    // was this a mistake ?
    // public int CardNumberLastFour { get; set; }

    public string CardNumber {get;set;}
    public int ExpiryMonth { get; set; }
    public int ExpiryYear { get; set; }
    public string Currency { get; set; }

    // decimal?
    public long Amount { get; set; }
    public string Cvv { get; set; }
}
