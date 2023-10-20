using H.Core.Enumerations;

namespace H.Avalonia.Core.Services.Ireland;

public class IrishDisclaimerService : IDisclaimerService
{
    public string GetDisclaimer(Languages language)
    {
        return Properties.Resources.Disclaimer_Ireland;
    }
}