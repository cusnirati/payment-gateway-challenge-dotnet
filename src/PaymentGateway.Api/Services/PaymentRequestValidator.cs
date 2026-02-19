using System;
using System.Text.RegularExpressions;

using PaymentGateway.Api.Models.Requests;

namespace PaymentGateway.Api.Services;


public interface IPaymentRequestValidator
{
    bool IsCardNumberValid(PaymentRequest payment);
    bool IsExpiryValid(PaymentRequest payment);
    bool IsAmountValid(PaymentRequest payment);
    bool IsCurrencyCodeValid(PaymentRequest payment);
    bool ValidateCvv(PaymentRequest payment);
}

public class PaymentRequestValidator : IPaymentRequestValidator
{
    public bool IsCardNumberValid(PaymentRequest payment)
    {
        if (!Regex.IsMatch(payment.CardNumber, @"^\d{14,19}$"))
        {
            return false;
        }

        return true;
    }

    public bool IsExpiryValid(PaymentRequest payment)
    {
        if (payment.ExpiryMonth < 1 || payment.ExpiryMonth > 12)
        {
            return false;
        }

        if (payment.ExpiryYear < DateTime.Now.Year)
        {
            return false;
        }

        var expiry = new DateTime(payment.ExpiryYear, payment.ExpiryMonth, 1);
        var current = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

        if (expiry < current)
        {
            return false;
        }

        return true;
    }

    public bool IsAmountValid(PaymentRequest payment)
    {
        if (payment.Amount < 1)
        {
            return false;
        }

        return true;
    }

    public bool IsCurrencyCodeValid(PaymentRequest payment)
    {
        // ?? Ensure your submission validates against no more than 3 currency codes

        if (payment.Currency.Length != 3)
        {
            return false;
        }

        return true;
    }

    public bool ValidateCvv(PaymentRequest payment)
    {
        if (!Regex.IsMatch(payment.Cvv, @"^\d{3,4}$"))
        {
            return false;
        }

        return true;
    }
}