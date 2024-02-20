using H.CLI.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.IO;

namespace H.CLI.Test
{
    [TestClass]
    public class InfrastructureConstantsTest
    {

        [TestMethod]
        public void TestEmptyGivenPath()
        {
            string givenPath = "";
            string farmsFoldersPath = @"C:\User";
            bool check = InfrastructureConstants.CheckOutputDirectoryPath(givenPath, null, farmsFoldersPath);

            Assert.AreEqual(InfrastructureConstants.BaseOutputDirectoryPath, farmsFoldersPath);
            Assert.IsFalse(check);
        }

        [TestMethod]
        public void TestValidGivenPath()
        {
            string givenPath = @"Z:\networkdrive";
            string farmsFoldersPath = @"C:\User";

            var mockDriveInfo = new Mock<IDriveInfoWrapper>();
            mockDriveInfo.Setup(d => d.DriveType).Returns(DriveType.Fixed);
            IDriveInfoWrapper driveInfo = mockDriveInfo.Object;
            DriveType driveType = driveInfo.DriveType;

            bool check = InfrastructureConstants.CheckOutputDirectoryPath(givenPath, driveInfo, farmsFoldersPath);

            Assert.AreEqual(InfrastructureConstants.BaseOutputDirectoryPath, givenPath);
            Assert.IsTrue(check);
        }

        [TestMethod]
        public void TestAttemptedNetworkDrivePath()
        {
            string givenPath = @"Z:\networkdrive";
            string farmsFoldersPath = @"C:\User";

            var mockDriveInfo = new Mock<IDriveInfoWrapper>();
            mockDriveInfo.Setup(d => d.DriveType).Returns(DriveType.Network);
            IDriveInfoWrapper driveInfo = mockDriveInfo.Object;
            DriveType driveType = driveInfo.DriveType;

            bool check = InfrastructureConstants.CheckOutputDirectoryPath(givenPath, driveInfo, farmsFoldersPath);

            Assert.AreEqual(InfrastructureConstants.BaseOutputDirectoryPath, farmsFoldersPath);
            Assert.IsFalse(check);
        }
    }
}
