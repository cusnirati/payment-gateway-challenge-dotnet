
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

using PaymentGateway.Api.Models.Requests;

namespace PaymentGateway.Api.Services;


public interface IBankHttpClient
{
    Task<HttpResponseMessage> PostBankPayment(PaymentRequest payment);
}

public class BankHttpClient : IBankHttpClient
{
    public async Task<HttpResponseMessage> PostBankPayment(PaymentRequest payment)
    {
        // "04/2025"
        string expiry = $"{payment.ExpiryMonth}/{payment.ExpiryYear}";

        var bankRequest = new BankRequest();
        bankRequest.CardNumber = payment.CardNumber;
        bankRequest.ExpiryDate = expiry;
        bankRequest.Currency = payment.Currency;
        bankRequest.Amount = payment.Amount;
        bankRequest.Cvv = payment.Cvv;

        // create client using a factory in Program.cs
        // read from config
        using HttpClient bankClient = new HttpClient();
        bankClient.BaseAddress = new Uri("http://localhost:8080/");
        HttpResponseMessage response = await bankClient.PostAsJsonAsync($"/payments", bankRequest);

        return response;
    }
}