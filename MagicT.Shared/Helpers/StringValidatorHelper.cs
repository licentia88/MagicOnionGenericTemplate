using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace MagicT.Shared.Helpers;

/// <summary>
/// Provides various string validation methods.
/// </summary>
public static class ValidationHelper
{
    /// <summary>
    /// Validates if a string is a valid DateTime.
    /// </summary>
    /// <param name="dateTimeString">The string to validate.</param>
    /// <returns>\c true if the string is a valid DateTime; otherwise, \c false.</returns>
    public static bool IsValidDateTime([StringSyntax(StringSyntaxAttribute.DateTimeFormat)] string dateTimeString)
    {
        return DateTime.TryParse(dateTimeString, out _);
    }

    /// <summary>
    /// Validates if a string is a valid URL.
    /// </summary>
    /// <param name="uriString">The string to validate.</param>
    /// <returns>\c true if the string is a valid URL; otherwise, \c false.</returns>
    public static bool IsValidUri([StringSyntax(StringSyntaxAttribute.Uri)] string uriString)
    {
        return Uri.IsWellFormedUriString(uriString, UriKind.Absolute);
    }

    /// <summary>
    /// Validates if a string is a valid Email.
    /// </summary>
    /// <param name="email">The string to validate.</param>
    /// <returns>\c true if the string is a valid Email; otherwise, \c false.</returns>
    public static bool IsValidEmail([StringSyntax(StringSyntaxAttribute.Regex)] string email)
    {
        string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$"; // Basic email regex
        return Regex.IsMatch(email, emailPattern);
    }

    /// <summary>
    /// Validates a string against a specific regex pattern.
    /// </summary>
    /// <param name="password">The string to validate.</param>
    /// <returns>\c true if the string matches the regex pattern; otherwise, \c false.</returns>
    public static bool IsValidPassword([StringSyntax(StringSyntaxAttribute.Regex)] string password)
    {
        string passwordPattern = @"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d@$!%*?&]{8,}$"; // Password with at least 8 characters, a letter, and a digit
        return Regex.IsMatch(password, passwordPattern);
    }

    /// <summary>
    /// Checks if a string contains valid JSON.
    /// </summary>
    /// <param name="jsonString">The string to validate.</param>
    /// <returns>\c true if the string is valid JSON; otherwise, \c false.</returns>
    public static bool IsValidJson([StringSyntax(StringSyntaxAttribute.Json)] string jsonString)
    {
        try
        {
            var jsonObject = System.Text.Json.JsonSerializer.Deserialize<object>(jsonString);
            return true; // Valid JSON string
        }
        catch (Exception)
        {
            return false; // Invalid JSON format
        }
    }

    /// <summary>
    /// Ensures that a string is a valid phone number.
    /// </summary>
    /// <param name="phoneNumber">The string to validate.</param>
    /// <returns>\c true if the string is a valid phone number; otherwise, \c false.</returns>
    public static bool IsValidPhoneNumber([StringSyntax(StringSyntaxAttribute.Regex)] string phoneNumber)
    {
        string phonePattern = @"^\+?[1-9]\d{1,14}$"; // International phone number format
        return Regex.IsMatch(phoneNumber, phonePattern);
    }

    /// <summary>
    /// Ensures the string is a valid credit card number.
    /// </summary>
    /// <param name="creditCard">The string to validate.</param>
    /// <returns>\c true if the string is a valid credit card number; otherwise, \c false.</returns>
    public static bool IsValidCreditCard([StringSyntax(StringSyntaxAttribute.Regex)] string creditCard)
    {
        string cardPattern = @"^4[0-9]{12}(?:[0-9]{3})?$"; // Simple Visa card number regex (you can extend for other card types)
        return Regex.IsMatch(creditCard, cardPattern);
    }

    /// <summary>
    /// Ensures the string is a valid ZIP code.
    /// </summary>
    /// <param name="zipCode">The string to validate.</param>
    /// <returns>\c true if the string is a valid ZIP code; otherwise, \c false.</returns>
    public static bool IsValidZipCode([StringSyntax(StringSyntaxAttribute.Regex)] string zipCode)
    {
        string zipPattern = @"^\d{5}(-\d{4})?$"; // Standard US ZIP code pattern
        return Regex.IsMatch(zipCode, zipPattern);
    }
}