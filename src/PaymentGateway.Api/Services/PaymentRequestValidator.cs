using System;
using System.Text.RegularExpressions;

using PaymentGateway.Api.Models.Requests;

// todo add return message what exactly is invalid
public class PaymentRequestValidator
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