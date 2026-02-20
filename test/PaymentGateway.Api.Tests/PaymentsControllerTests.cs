using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

using Moq;

using PaymentGateway.Api.Models.Requests;

using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Services;

using Xunit;

namespace PaymentGateway.Api.Controllers;

public class PaymentsControllerTests
{
    private HttpClient controller;
    private Random random = new();

    private Mock<IPostPaymentRepository> paymentRepo;
    private Mock<IPaymentRequestValidator> validator;
    private Mock<IBankHttpClient> bankHttpClient;
    private Mock<IBankResponseProcessor> bankResponseProcessor;

    public PaymentsControllerTests()
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
        this.paymentRepo.Setup(x => x.Get(It.IsAny<Guid>())).Returns(payment);

        this.validator = new Mock<IPaymentRequestValidator>();
        this.validator.Setup(x => x.IsCardNumberValid(It.IsAny<PaymentRequest>())).Returns(true);
        this.validator.Setup(x => x.IsExpiryValid(It.IsAny<PaymentRequest>())).Returns(true);
        this.validator.Setup(x => x.IsAmountValid(It.IsAny<PaymentRequest>())).Returns(true);
        this.validator.Setup(x => x.IsCurrencyCodeValid(It.IsAny<PaymentRequest>())).Returns(true);
        this.validator.Setup(x => x.ValidateCvv(It.IsAny<PaymentRequest>())).Returns(true);

        this.bankHttpClient = new Mock<IBankHttpClient>();
        this.bankHttpClient.Setup(x => x.PostBankPayment(It.IsAny<PaymentRequest>())).ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

        this.bankResponseProcessor = new Mock<IBankResponseProcessor>();
        this.bankResponseProcessor
            .Setup(x => x.ParseBankResponse(It.IsAny<PaymentRequest>(), It.IsAny<HttpResponseMessage>())).ReturnsAsync(new PostPaymentResponse());


        var webApplicationFactory = new WebApplicationFactory<PaymentsController>();
        this.controller = webApplicationFactory.WithWebHostBuilder(builder =>
        builder.ConfigureServices(services =>
        {
            services.AddSingleton(this.paymentRepo.Object);
            services.AddSingleton(this.bankHttpClient.Object);
            services.AddSingleton(this.bankResponseProcessor.Object);

            services.AddSingleton(this.validator.Object);

        })).CreateClient();
    }

    public void Dispose()
    {
        this.controller.Dispose();
    }

    [Fact]
    public async Task RetrievesAPaymentSuccessfully()
    {
        var response = await this.controller.GetAsync($"/api/Payments/123e4567-e89b-12d3-a456-426614174000");
        var paymentResponse = await response.Content.ReadFromJsonAsync<PostPaymentResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(paymentResponse);
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