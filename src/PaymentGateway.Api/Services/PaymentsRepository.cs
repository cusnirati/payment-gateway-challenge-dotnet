using System;
using System.Collections.Generic;
using System.Linq;

using PaymentGateway.Api.Models.Requests;

using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Services;

public interface IPostPaymentRepository
{
    void Add(PostPaymentResponse payment);
    PostPaymentResponse Get(Guid id);
}

public class PostPaymentRepository : IPostPaymentRepository
{
    public List<PostPaymentResponse> Payments = new();

    public void Add(PostPaymentResponse payment)
    {
        Payments.Add(payment);
    }

    public PostPaymentResponse Get(Guid id)
    {
        return Payments.FirstOrDefault(p => p.Id == id);
    }
}