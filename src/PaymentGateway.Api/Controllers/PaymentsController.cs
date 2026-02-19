using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http.HttpResults;

using Microsoft.AspNetCore.Mvc;

using PaymentGateway.Api.Models;

using PaymentGateway.Api.Models.Requests;

using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Services;

namespace PaymentGateway.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : Controller
{
    private readonly PaymentsRepository paymentsRepo;

    public PaymentsController(PaymentsRepository paymentsRepo)
    {
        this.paymentsRepo = paymentsRepo;
    }

    // [HttpGet("{id:guid}")]
    // public async Task<ActionResult<PostPaymentResponse>> GetPaymentAsync(Guid id)
    // {
    //     var payment = paymentsRepo.Get(id);

    //     return new OkObjectResult(payment);
    // }

    [HttpPost]
    public async Task<ActionResult<PaymentStatus>> PostPayment([FromBody] PaymentRequest payment)
    {
        // validate
        // call bank
        // validate bank response
        // add to payments repo

        if (!Regex.IsMatch(payment.CardNumber, @"^\d{14,19}$"))
        {
            // invalid card number
        }

        var result = payment.ExpiryMonth < 1 || payment.ExpiryMonth > 12;
        if (result)
        {
            // invalid month
        }

        var result22 = payment.ExpiryYear < DateTime.Now.Year;
        if (result22)
        {
            // invalid month
        }

        var expiry = new DateTime(payment.ExpiryYear, payment.ExpiryMonth, 1);
        var now = DateTime.Now;
        var dt = new DateTime(now.Year, now.Month, 1);

        if (expiry < dt)
        {
            // invalid month + year
        }

        if (payment.Currency.Length != 3)
        {
            // invalid currency
            // ?? Ensure your submission validates against no more than 3 currency codes
        }

        if (payment.Amount < 1)
        {
            // invalid amount
        }

        
        if (!Regex.IsMatch(payment.Cvv, @"^\d{3,4}$"))
        {
            // invalid cvv
        }

        // all of these are rejected









        return new OkObjectResult(PaymentStatus.Authorized);
    }
}

