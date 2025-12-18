using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Events;

namespace H.Core.Events
{
    public class FarmResultsCalculatedEvent : PubSubEvent<FarmResultsCalculatedEventArgs>
    {
    }
}
