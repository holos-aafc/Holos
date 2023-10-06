using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H.CLI.Interfaces
{
    public interface IDriveInfoWrapper
    {
        DriveType DriveType { get; }
    }
}
