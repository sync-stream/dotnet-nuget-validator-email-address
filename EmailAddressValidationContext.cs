using System.Text.Json.Serialization;
using System.Xml.Serialization;
using SyncStream.Validator.Hostname;

// Define our namespace
namespace SyncStream.Validator.EmailAddress;

/// <summary>
/// This class maintains the structure for a validated email address
/// </summary>
public class EmailAddressValidationContext : HostnameValidatorService
{
    /// <summary>
    /// This property contains the fully qualified domain name
    /// </summary>
    [JsonPropertyName("fqdn")]
    [XmlAttribute("fqdn")]
    public string FullyQualifiedDomain => ToFullyQualifiedDomainName();

    /// <summary>
    /// This property contains the normalized and validated email address
    /// </summary>
    [JsonPropertyName("resultEmail")]
    [XmlElement("resultEmail")]
    public string NormalizedEmailAddress { get; set; }

    /// <summary>
    /// This property contains the valid flag of the response
    /// </summary>
    [JsonPropertyName("validEmail")]
    [XmlAttribute("validEmail")]
    public bool Valid { get; set; }

    /// <summary>
    /// This property contains the source email address that was validated
    /// </summary>
    [JsonPropertyName("sourceEmail")]
    [XmlElement("sourceEmail")]
    public string SourceEmailAddress { get; set; }

    /// <summary>
    /// This property contains the user part of the email address
    /// </summary>
    [JsonPropertyName("userPart")]
    [XmlElement("userPart")]
    public string UserPart { get; set; }

    /// <summary>
    /// This method populates the DTO from an existing hostname instance
    /// </summary>
    /// <param name="hostname">The existing hostname instance</param>
    /// <returns>The email validation context from the <paramref name="hostname"/> instance</returns>
    public EmailAddressValidationContext FromHostname(HostnameValidatorService hostname)
    {
        // Set the domain into the instance
        Domain = hostname.Domain;

        // Set the host into the instance
        Host = hostname.Host;

        // Set the valid flag into the instance
        IsValid = hostname.IsValid;

        // Set the port into the instance
        Port = hostname.Port;

        // Set the protocol into the instance
        Protocol = hostname.Protocol;

        // Set the source domain into the instance
        Source = hostname.Source;

        // Set the TLD into the instance
        TopLevelDomain = hostname.TopLevelDomain;

        // We're done, return the instance
        return this;
    }

    /// <summary>
    /// This method provides a fluid interface for validating email addresses inside of conditionals
    /// </summary>
    /// <param name="emailAddress">The email address to validate</param>
    /// <param name="context">The output validation context for <paramref name="emailAddress" /></param>
    /// <returns>A boolean denoting validity</returns>
    public static bool TryValidate(string emailAddress, out EmailAddressValidationContext context)
    {
        // Check for a valid string
        if (string.IsNullOrEmpty(emailAddress) || string.IsNullOrWhiteSpace(emailAddress) ||
            !emailAddress.Contains("@"))
        {
            // Reset the context
            context = null;

            // We're done, can't validate
            return false;
        }

        // Validate the email address
        context = Validate(emailAddress);

        // We're done, return the valid flag
        return context.Valid;
    }

    /// <summary>
    /// This method provides a fluid interface for validating email address
    /// </summary>
    /// <param name="emailAddress">The email address to validate</param>
    /// <returns>A validation context for <paramref name="emailAddress" /></returns>
    public static EmailAddressValidationContext Validate(string emailAddress)
    {
        // Define our response
        EmailAddressValidationContext response = new()
        {
            // Set the request email address into the response
            SourceEmailAddress = emailAddress,

            // Set the validated email address into the response
            NormalizedEmailAddress = emailAddress?.Trim().ToLower()
        };

        // Ensure we have an email address
        if (string.IsNullOrEmpty(emailAddress) || string.IsNullOrWhiteSpace(emailAddress))
        {
            // Reset the valid flag on the response
            response.Valid = false;

            // We're done, send the response
            return response;
        }

        // Split the email address into its parts
        List<string> parts = emailAddress.Trim().ToLower().Split('@').ToList();

        // Make sure we have the valid number of parts and set the response valid flag
        if (parts.Count == 2)
        {
            // Set the user part of the email address into the response
            response.UserPart = parts.ElementAt(0);

            // Localize the domain
            string domain = parts.ElementAt(1).Trim().ToLower();

            // Process the hostname
            HostnameValidatorService hostname = Parse(domain);

            // Set the domain into the response
            response.FromHostname(hostname);

            // Check the domain name and set the response valid flag
            if (!hostname.IsValid || !hostname.ToFullyQualifiedDomainName().Equals(domain))
            {
                // Set the valid flag into the response
                response.Valid = false;
            }

            // Otherwise, we have a valid response
            else response.Valid = true;
        }
        else response.Valid = false;

        // We're done, send the response
        return response;
    }
}
