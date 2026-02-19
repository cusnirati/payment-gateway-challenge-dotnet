using System.Text.Json.Serialization;

namespace PaymentGateway.Api.Models.Responses;

public class BankResponse
{
    public bool Authorized { get; set; }

    [JsonPropertyName("authorization_code")]
    public string AuthorizationCode { get; set; }

    //   "authorized": true,
    //   "authorization_code": "0bb07405-6d44-4b50-a14f-7ae0beff13ad"
}