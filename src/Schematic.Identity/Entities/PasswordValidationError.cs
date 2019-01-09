namespace Schematic.Identity
{
    public enum PasswordValidationError
    {
        DoesNotMeetLengthRequirements = 0,
        DoesNotHaveUpperCaseLetter = 1,
        DoesNotHaveLowerCaseLetter = 2,
        DoesNotHaveDecimalDigit = 3
    }
}