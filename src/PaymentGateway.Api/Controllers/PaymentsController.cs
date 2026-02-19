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

    // [HttpGet("{id:guid}")]
    // public async Task<ActionResult<PostPaymentResponse>> GetPaymentAsync(Guid id)
    // {
    //     var payment = paymentsRepo.Get(id);

    //     return new OkObjectResult(payment);
    // }

    [HttpGet("")]
    public async Task<ActionResult<string>> Get()
    {
        return new OkObjectResult("okkkkkkkkk");
    }

    [HttpPost("")]
    public async Task<ActionResult<PaymentStatus>> PostPayment([FromBody] PaymentRequest payment)
    {
        // add interface
        // var validator = new PaymentRequestValidator();
        // var result =
        //     validator.IsCardNumberValid(payment) &&
        //     validator.IsExpiryValid(payment) &&
        //     validator.IsAmountValid(payment) &&
        //     validator.IsCurrencyCodeValid(payment) &&
        //     validator.ValidateCvv(payment);

        // if (!result)
        // {
        //     return new OkObjectResult(PaymentStatus.Rejected);
        // }

        // call bank
        // validate bank response
        // add to payments repo


        //   "card_number": "2222405343248877",
        //   "expiry_date": "04/2025",
        //   "currency": "GBP",
        //   "amount": 100,
        //   "cvv": "123"

        var bankRequest = new BankRequest();
        bankRequest.CardNumber = "2222405343248877";
        bankRequest.ExpiryDate = "04/2025";
        bankRequest.Currency = "GBP";
        bankRequest.Amount = 100;
        bankRequest.Cvv = "123";


        using HttpClient bankClient = new HttpClient();
        bankClient.BaseAddress = new Uri("http://localhost:8080/");
        var response = await bankClient.PostAsJsonAsync($"/payments", bankRequest);


        // todo
        //      200, auth true
        //      200, auth false, no code
        //      503

        Console.WriteLine(response.StatusCode);
        if (response.IsSuccessStatusCode)
        {
            BankResponse bankResponse = await response.Content.ReadFromJsonAsync<BankResponse>();

            Console.WriteLine(bankResponse.Authorized);
            Console.WriteLine(bankResponse.AuthorizationCode);

            var ok = new PostPaymentResponse();

            ok.Id = Guid.NewGuid(); // or use the bank one, but could be empty
            if (bankResponse.Authorized)
            {
                ok.Status = PaymentStatus.Authorized;
            }
            else
            {
                ok.Status = PaymentStatus.Declined;
            }

            ok.CardNumberLastFour = payment.CardNumber.Substring(payment.CardNumber.Length - 4);
            ok.ExpiryMonth = payment.ExpiryMonth;
            ok.ExpiryYear = payment.ExpiryYear;
            ok.Currency = payment.Currency;
            ok.Amount = payment.Amount;



            this.paymentRepo.Add(ok);
        }




        return new OkObjectResult(PaymentStatus.Authorized);
    }
}
