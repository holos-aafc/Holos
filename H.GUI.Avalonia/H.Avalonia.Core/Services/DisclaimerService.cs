using H.Core.Enumerations;

namespace H.Avalonia.Core.Services;

public class DisclaimerService : IDisclaimerService
{
    public string GetDisclaimer(UserRegion userRegion, Languages language)
    {
        switch (userRegion)
        {
            case UserRegion.Canada when language == Languages.French:
                return Properties.FileResources.Disclaimer_French;
            case UserRegion.Ireland:
                return Properties.Resources.Disclaimer_Ireland;
            default:
                return Properties.FileResources.Disclaimer_English;
        }
    }
}