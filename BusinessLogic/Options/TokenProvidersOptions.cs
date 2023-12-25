namespace BusinessLogic.Options;
public class TokenProvidersOptions
{
    public int PasswordResetLifetime { get; set; }
    public int EmailConfirmationLifetime { get; set; }
    public int RefreshTokenLifetimeInDays { get; set; }
}
