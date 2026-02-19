using System.Text.Json.Serialization;

namespace PaymentGateway.Api.Models.Requests;

public class PaymentRequest
{
    // was this a mistake ?
    // public int CardNumberLastFour { get; set; }

    public int CardNumber {get;set;}
    public int ExpiryMonth { get; set; }
    public int ExpiryYear { get; set; }
    public string Currency { get; set; }
    public int Amount { get; set; }
    public int Cvv { get; set; }
}
