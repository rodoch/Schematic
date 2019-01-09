namespace Schematic.Core.Mvc
{
    public static class UserErrorMessages
    {
        public const string InvalidEmail = "The e-mail address supplied was not a valid e-mail address";
        public const string DuplicateUser = "A user with the same e-mail address already exists";
        public const string TwoPasswordsRequired = "Please enter your new password below twice to confirm";
        public const string PasswordsDoNotMatch = "The passwords provided do not match";
    }
}