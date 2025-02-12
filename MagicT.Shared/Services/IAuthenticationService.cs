using MagicOnion;
using MagicT.Shared.Models.ServiceModels;
using MagicT.Shared.Models.ViewModels;

namespace MagicT.Shared.Services;

public interface IAuthenticationService : IService<IAuthenticationService>
{
    public UnaryResult<EncryptedData<AuthenticationResponse>> LoginWithPhoneAsync(EncryptedData<AuthenticationRequest> authenticationRequest);

    public UnaryResult<EncryptedData<AuthenticationResponse>> LoginWithEmailAsync(EncryptedData<AuthenticationRequest> authenticationRequest);
    
    public UnaryResult<EncryptedData<AuthenticationResponse>> LoginWithUsername(EncryptedData<AuthenticationRequest> authenticationRequest);
    
    public UnaryResult<EncryptedData<AuthenticationResponse>> RegisterAsync(EncryptedData<RegistrationRequest> registrationRequest);
}