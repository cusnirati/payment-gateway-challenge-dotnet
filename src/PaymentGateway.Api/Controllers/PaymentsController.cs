using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

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

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PostPaymentResponse>> GetPaymentAsync(Guid id)
    {
        var payment = paymentsRepo.Get(id);

        return new OkObjectResult(payment);
    }

    public async Task<ActionResult<PaymentRequest>> PostPayment([FromBody] PaymentRequest payment)
    {
        

        return new OkObjectResult(payment);
    }
}