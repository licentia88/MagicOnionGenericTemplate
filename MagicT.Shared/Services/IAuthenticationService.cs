using MagicOnion;
using MagicT.Shared.Models.ViewModels;

namespace MagicT.Shared.Services;

public interface IAuthenticationService : IService<IAuthenticationService>
{
    public UnaryResult<AuthenticationResponse> LoginWithPhoneAsync(AuthenticationRequest authenticationRequest);

    public UnaryResult<AuthenticationResponse> LoginWithEmailAsync(AuthenticationRequest authenticationRequest);
    
    public UnaryResult<AuthenticationResponse> LoginWithUsername(AuthenticationRequest authenticationRequest);

 
    public UnaryResult<AuthenticationResponse> RegisterAsync(RegistrationRequest registrationRequest);
}