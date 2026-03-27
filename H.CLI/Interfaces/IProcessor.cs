using System.Collections.Generic;
using H.Core.Models;

namespace H.CLI.Interfaces
{
    public interface IProcessor
    {
        void ProcessComponent(Farm farm, List<ComponentBase> component, ApplicationData applicationData);
    }
}
