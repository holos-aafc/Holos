using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H.Core.Models;

namespace H.CLI.Interfaces
{
    public interface IProcessor
    {
        void ProcessComponent(Farm farm, List<ComponentBase> component, ApplicationData applicationData);
    }
}
