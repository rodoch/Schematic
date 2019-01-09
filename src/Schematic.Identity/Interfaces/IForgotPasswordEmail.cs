namespace Schematic.Identity
{
    public interface IForgotPasswordEmail<TUser>
    {
        string Subject(string applicationName = "");

        string Body(TUser user, string domain, string subject, string token);
    }
}