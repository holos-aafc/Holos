using H.CLI.Interfaces;
using System.IO;

namespace H.CLI
{
    public class DriveInfoWrapper : IDriveInfoWrapper
    {
        private DriveInfo _driveInfo;

        public DriveInfoWrapper(DriveInfo driveInfo)
        {
            _driveInfo = driveInfo;
        }
        public void SetDriveInfo(DriveInfo driveInfo)
        {
            _driveInfo = driveInfo;
        }

        public DriveType DriveType => _driveInfo.DriveType;
        public string Name => _driveInfo.Name;
    }
}