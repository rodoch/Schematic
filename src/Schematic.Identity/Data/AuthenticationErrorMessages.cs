namespace Schematic.Identity
{
    public static class AuthenticationErrorMessages
    {
        public const string InvalidData = "We were unable to sign you in. Please check that your details are correct.";

        public const string EmailRequired = "You need to provide an e-mail address";
        
        public const string InvalidEmail = "The e-mail address supplied was not a valid e-mail address";

        public const string PasswordRequired = "You need to provide a password";

        public const string PasswordConfirmationRequired = "You need to confirm your password";

        public const string PasswordsDoNotMatch = "The passwords provided do not match";

        public const string UserDoesNotExist = "We have no record of this user. Please check that the e-mail address you provided is correct.";

        public const string PasswordReminderFailed = "An error occurred and we were unable to send you a password reset e-mail. Please contact the site administrators.";

        public const string NoTokenProvided = "This is not a valid request. No user token was provided.";

        public const string InvalidToken = "The request to set your password is invalid. Have you made a request to set a new password?";

        public const string ExpiredToken = "The request to set your password has expired. Please make a new request.";

        public const string PasswordSetFailed = "An error occurred and we were unable to set your new password. Please contact the site administratiors.";
    }
}