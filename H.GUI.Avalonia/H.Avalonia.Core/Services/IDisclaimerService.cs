using H.Core.Enumerations;

namespace H.Avalonia.Core.Services;

public interface IDisclaimerService
{
    string GetDisclaimer(Languages language);
}