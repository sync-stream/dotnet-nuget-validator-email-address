using Microsoft.Extensions.DependencyInjection;
using SyncStream.Validator.Hostname;

// Define our namespace
namespace SyncStream.Validator.EmailAddress;

/// <summary>
/// This class maintains the IServiceCollection extensions for the package
/// </summary>
public static class EmailAddressValidatorServiceCollectionExtensions
{
    /// <summary>
    /// This method registers any services or providers to the IServiceCollection
    /// </summary>
    /// <param name="instance">The IServiceCollection with which to register everything</param>
    /// <returns><paramref name="instance" /> for a fluid interface</returns>
    public static IServiceCollection UseSyncStreamEmailAddressValidator(this IServiceCollection instance) =>
        instance.UseSyncStreamHostnameValidator();
}
