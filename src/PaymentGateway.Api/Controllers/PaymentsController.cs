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

        // call bank
        // validate bank response
        // add to payments repo




        return new OkObjectResult(PaymentStatus.Authorized);
    }
}

