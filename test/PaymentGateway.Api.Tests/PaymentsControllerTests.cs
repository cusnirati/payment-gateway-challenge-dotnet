using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

using Moq;

using NUnit.Framework;

using PaymentGateway.Api.Models.Requests;

using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Services;

namespace PaymentGateway.Api.Controllers;


public class PaymentsControllerTests
{
    private HttpClient controller;
    private Random random = new();

    private Mock<IPostPaymentRepository> paymentRepo;
    // private readonly IPaymentRequestValidator validator;
    // private readonly IBankHttpClient bankHttpClient;
    // private readonly IBankResponseProcessor responseProcessor;

    private Mock<IPaymentRequestValidator> validator;


    [SetUp]
    public async Task SetUp()
    {
        var payment = new PostPaymentResponse
        {
            Id = Guid.Parse("123e4567-e89b-12d3-a456-426614174000"),
            ExpiryYear = random.Next(2023, 2030),
            ExpiryMonth = random.Next(1, 12),
            Amount = random.Next(1, 10000),
            CardNumberLastFour = random.Next(1111, 9999).ToString(),
            Currency = "GBP"
        };

        this.paymentRepo = new Mock<IPostPaymentRepository>();
        this.paymentRepo.Setup(x=>x.Get(It.IsAny<Guid>())).Returns(payment);


        this.validator = new Mock<IPaymentRequestValidator>();
        this.validator.Setup(x=>x.IsCardNumberValid(It.IsAny<PaymentRequest>())).Returns(true);
        this.validator.Setup(x=>x.IsExpiryValid(It.IsAny<PaymentRequest>())).Returns(true);
        this.validator.Setup(x=>x.IsAmountValid(It.IsAny<PaymentRequest>())).Returns(true);
        this.validator.Setup(x=>x.IsCurrencyCodeValid(It.IsAny<PaymentRequest>())).Returns(true);
        this.validator.Setup(x=>x.ValidateCvv(It.IsAny<PaymentRequest>())).Returns(true);


        var webApplicationFactory = new WebApplicationFactory<PaymentsController>();
        this.controller = webApplicationFactory.WithWebHostBuilder(builder =>
        builder.ConfigureServices(services =>
        {
            services.AddSingleton(this.validator.Object);
            services.AddSingleton(this.paymentRepo.Object);
        })).CreateClient();
    }

    [TearDown]
    public void TearDown()
    {
        this.controller.Dispose();
    }

    [Test]
    public async Task RetrievesAPaymentSuccessfully()
    {
        var response = await this.controller.GetAsync($"/api/Payments/123e4567-e89b-12d3-a456-426614174000");
        var paymentResponse = await response.Content.ReadFromJsonAsync<PostPaymentResponse>();
        
        Assert.Equals(HttpStatusCode.OK, response.StatusCode);
        // Assert.IsNotNull(paymentResponse);
        // Assert.IsNotNull(paymentResponse);
    }

    // [Test]
    // public async Task Returns404IfPaymentNotFound()
    // {
    //     // Arrange
    //     var webApplicationFactory = new WebApplicationFactory<PaymentsController>();
    //     var client = webApplicationFactory.CreateClient();
        
    //     // Act
    //     var response = await client.GetAsync($"/api/Payments/{Guid.NewGuid()}");
        
    //     // Assert
    //     Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    // }
}