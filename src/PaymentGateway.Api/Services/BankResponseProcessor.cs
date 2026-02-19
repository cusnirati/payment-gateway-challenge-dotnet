using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

using PaymentGateway.Api.Models;

using PaymentGateway.Api.Models.Requests;

using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Services;


public interface IBankResponseProcessor
{
    Task<PostPaymentResponse> ParseBankResponse(PaymentRequest payment, HttpResponseMessage response);
}

public class BankResponseProcessor : IBankResponseProcessor
{
    public async Task<PostPaymentResponse> ParseBankResponse(PaymentRequest payment, HttpResponseMessage response)
    {
        BankResponse bankResponse = await response.Content.ReadFromJsonAsync<BankResponse>();

        var ok = new PostPaymentResponse();

        if (bankResponse.Authorized)
        {
            ok.Status = PaymentStatus.Authorized;
        }
        else
        {
            ok.Status = PaymentStatus.Declined;
        }

        ok.Id = Guid.NewGuid(); // or use the bank one? but could be empty
        ok.CardNumberLastFour = payment.CardNumber.Substring(payment.CardNumber.Length - 4);
        ok.ExpiryMonth = payment.ExpiryMonth;
        ok.ExpiryYear = payment.ExpiryYear;
        ok.Currency = payment.Currency;
        ok.Amount = payment.Amount;

        return ok;
    }
}