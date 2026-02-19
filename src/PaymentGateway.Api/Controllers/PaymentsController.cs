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
    private readonly IPostPaymentRepository paymentRepo;
    private readonly IPaymentRequestValidator validator;
    private readonly IBankHttpClient bankHttpClient;
    private readonly IBankResponseProcessor responseProcessor;

    public PaymentsController(
        IPostPaymentRepository paymentRepo, IPaymentRequestValidator validator, IBankHttpClient bankHttpClient, IBankResponseProcessor responseProcessor)
    {
        this.paymentRepo = paymentRepo;
        this.validator = validator;
        this.bankHttpClient = bankHttpClient;
        this.responseProcessor = responseProcessor;
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
        bool isValid =
            this.validator.IsCardNumberValid(payment) &&
            this.validator.IsExpiryValid(payment) &&
            this.validator.IsAmountValid(payment) &&
            this.validator.IsCurrencyCodeValid(payment) &&
            this.validator.ValidateCvv(payment);

        if (!isValid)
        {
            return new OkObjectResult(PaymentStatus.Rejected);
        }

        PostPaymentResponse paymentResponse = new();

        HttpResponseMessage bankResponse = await this.bankHttpClient.PostBankPayment(payment);
        if (bankResponse.IsSuccessStatusCode)
        {
            paymentResponse = await this.responseProcessor.ParseBankResponse(payment, bankResponse);
            this.paymentRepo.Add(paymentResponse);

            return new OkObjectResult(paymentResponse.Status);
        }
        else // http 503
        {
            return new OkObjectResult(PaymentStatus.Rejected);
        }
    }
}
