using MagicOnion;
using MagicT.Shared.Models.ViewModels;

namespace MagicT.Client.Services.Base;

/// <summary>
/// Defines methods for secure authentication services with encrypted data.
/// </summary>
public interface ISecureAuthenticationService
{
    /// <summary>
    /// Logs in a player using encrypted data and returns the decrypted authentication response.
    /// </summary>
    /// <param name="request">The authentication request containing the player's credentials.</param>
    /// <returns>A <see cref="UnaryResult{T}"/> containing the decrypted authentication response.</returns>
    UnaryResult<AuthenticationResponse> LoginWithEmailEncryptedAsync(AuthenticationRequest request);
        
    /// <summary>
    /// Logs in a player using their username and encrypted data, returning the decrypted authentication response.
    /// </summary>
    /// <param name="request">The authentication request containing the player's credentials.</param>
    /// <returns>A <see cref="UnaryResult{T}"/> containing the decrypted authentication response.</returns>
    UnaryResult<AuthenticationResponse> LoginWithUsernameEncryptedAsync(AuthenticationRequest request);
        
    /// <summary>
    /// Logs in a player using their phone number and encrypted data, returning the decrypted authentication response.
    /// </summary>
    /// <param name="request">The authentication request containing the player's credentials.</param>
    /// <returns>A <see cref="UnaryResult{T}"/> containing the decrypted authentication response.</returns>
    UnaryResult<AuthenticationResponse> LoginWithPhoneEncryptedAsync(AuthenticationRequest request);
        
    /// <summary>
    /// Registers a player using encrypted data and returns the decrypted authentication response.
    /// </summary>
    /// <param name="request">The registration request containing the player's details.</param>
    /// <returns>A <see cref="UnaryResult{T}"/> containing the decrypted authentication response.</returns>
    UnaryResult<AuthenticationResponse> RegisterEncryptedAsync(RegistrationRequest request);
}