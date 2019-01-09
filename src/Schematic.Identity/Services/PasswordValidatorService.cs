using System;
using System.Collections.Generic;

namespace Schematic.Identity
{
    public class PasswordValidatorService : IPasswordValidatorService
    {
        private const int _minLength = 7;
        private const int _maxLength = 15;

        public List<PasswordValidationError> ValidatePassword(string password)
        {
            if (password is null)
            { 
                throw new ArgumentNullException();
            }

            bool meetsLengthRequirements = password.Length >= _minLength && password.Length <= _maxLength;
            bool hasUpperCaseLetter = false;
            bool hasLowerCaseLetter = false;
            bool hasDecimalDigit = false;

            foreach (char character in password)
            {
                if (char.IsUpper(character)) 
                {
                    hasUpperCaseLetter = true;
                }
                else if (char.IsLower(character)) 
                {
                    hasLowerCaseLetter = true;
                }
                else if (char.IsDigit(character)) 
                {
                    hasDecimalDigit= true;
                }
            }
            
            var passwordValidationErrors = new List<PasswordValidationError>();

            if (!meetsLengthRequirements)
            {
                passwordValidationErrors.Add(PasswordValidationError.DoesNotMeetLengthRequirements);
            }

            if (!hasUpperCaseLetter)
            {
                passwordValidationErrors.Add(PasswordValidationError.DoesNotHaveUpperCaseLetter);
            }

            if (!hasLowerCaseLetter)
            {
                passwordValidationErrors.Add(PasswordValidationError.DoesNotHaveLowerCaseLetter);
            }

            if (!hasDecimalDigit)
            {
                passwordValidationErrors.Add(PasswordValidationError.DoesNotHaveDecimalDigit);
            }

            return passwordValidationErrors;
        }

        public string GetPasswordValidationErrorMessage()
        {
            return String.Format(@"Your password must be between {0} and {1} characters in length and contain at least one uppercase letter, at least one lowercase letter, and at least one number.", 
                _minLength, _maxLength);
        }

        public string GetPasswordValidationErrorMessage(List<PasswordValidationError> errors)
        {
            // Generate string based on specific errors, e.g. !meetsLengthRequirements = "at least 7 characters"
            throw new NotImplementedException();
        }
    }
}