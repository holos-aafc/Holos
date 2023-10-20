using H.Core.Enumerations;

namespace H.Avalonia.Core.Services;

public class DisclaimerService : IDisclaimerService
{
    public string GetDisclaimer(Languages language)
    {
        switch (language)
        {
            case Languages.French:
                return Properties.FileResources.Disclaimer_French;
            default:
                return Properties.FileResources.Disclaimer_English;
        }
    }
}