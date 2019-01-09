namespace Schematic.Identity
{
    public interface IEmailValidatorService
    {
        bool IsValidEmail(string email);
    }
}