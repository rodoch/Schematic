using System.Collections.Generic;

namespace Schematic.Identity
{
    public interface IPasswordValidatorService
    {
        List<PasswordValidationError> ValidatePassword(string password);

        // Get error messages via interface as the error message to be returned is infrastructure/implementation-specific
        string GetPasswordValidationErrorMessage();

        string GetPasswordValidationErrorMessage(List<PasswordValidationError> errors);
    }
}