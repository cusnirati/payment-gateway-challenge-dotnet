using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http.HttpResults;

using Microsoft.AspNetCore.Mvc;

using PaymentGateway.Api.Models;

using PaymentGateway.Api.Models.Requests;

using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Services;

namespace PaymentGateway.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentsController : Controller
{
    private readonly PostPaymentRepository paymentRepo;

    public PaymentsController(PostPaymentRepository paymentRepo)
    {
        this.paymentRepo = paymentRepo;
    }

    // curl -k https://localhost:7092/api/payments
    // health check endpoint
    [HttpGet()]
    public async Task<ActionResult<string>> Get()
    {
        return new OkObjectResult("ok");
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PostPaymentResponse>> GetPaymentAsync(Guid id)
    {
        var payment = this.paymentRepo.Get(id);

        return new OkObjectResult(payment);
    }

    [HttpPost()]
    public async Task<ActionResult<PaymentStatus>> PostPayment([FromBody] PaymentRequest payment)
    {
        // validate
        // call bank
        // validate bank response
        // add to payments repo


        // add interface
        var validator = new PaymentRequestValidator();
        var result =
            validator.IsCardNumberValid(payment) &&
            validator.IsExpiryValid(payment) &&
            validator.IsAmountValid(payment) &&
            validator.IsCurrencyCodeValid(payment) &&
            validator.ValidateCvv(payment);

        if (!result)
        {
            return new OkObjectResult(PaymentStatus.Rejected);
        }

        PostPaymentResponse paymentResponse = new();

        var bankClient = new BankHttpClient();

        HttpResponseMessage httpResponse = await bankClient.PostBankPayment(payment);
        if (httpResponse.IsSuccessStatusCode)
        {
            BankResponseProcessor processor = new();
            paymentResponse = await processor.ParseBankResponse(payment, httpResponse);
            this.paymentRepo.Add(paymentResponse);

            return new OkObjectResult(paymentResponse.Status);
        }
        else // http 503
        {
            return new OkObjectResult(PaymentStatus.Rejected);
        }
    }
}

public class BankResponseProcessor
{
    public async Task<PostPaymentResponse> ParseBankResponse(PaymentRequest payment, HttpResponseMessage response)
    {
        BankResponse bankResponse = await response.Content.ReadFromJsonAsync<BankResponse>();

        Console.WriteLine(bankResponse.Authorized);
        Console.WriteLine(bankResponse.AuthorizationCode);

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

public class BankHttpClient
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