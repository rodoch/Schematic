namespace Schematic.Identity
{
    public interface IUserInvitationEmail<TUser>
    {
        string Subject(string applicationName = "");

        string Body(TUser user, string domain, string subject, string token);
    }
}