using FicheroNacionalPip.Common;
using FicheroNacionalPip.Business.Models;

namespace FicheroNacionalPip.Business.Interfaces;

public class PasswordPolicyChangedEventArgs : EventArgs
{
    public PasswordPolicyDto NewPolicy { get; }
    public PasswordPolicyChangedEventArgs(PasswordPolicyDto newPolicy)
    {
        NewPolicy = newPolicy;
    }
}

public interface IPasswordPolicyService {
    event EventHandler<PasswordPolicyChangedEventArgs>? PolicyChanged;
    
    Task<Result<PasswordPolicyDto, string>> GetActivePasswordPolicyAsync();
    Task<Result<bool, string>> UpdatePasswordPolicyAsync(PasswordPolicyDto policy);
    Task<Result<bool, string>> ValidatePasswordAsync(string password);
    Task<Result<bool, string>> IsPasswordExpiredAsync(string username);
    Task<Result<IEnumerable<string>, string>> GetPasswordValidationErrorsAsync(string password);
    
    void NotifyPolicyChanged(PasswordPolicyDto newPolicy);
}