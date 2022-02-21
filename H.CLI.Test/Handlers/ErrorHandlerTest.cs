using H.CLI.TemporaryComponentStorage;
using H.Core.Enumerations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H.CLI.Handlers;
using H.CLI.Interfaces;

namespace H.CLI.Test.Handlers
{
    [TestClass]
    public class ErrorHandlerTest
    {
        [TestMethod]
        public void TestCheckIfComponentsHaveTheSameName_TwoComponentFilesHaveTheSameName_ThrowsException()
        {
            var listOfComponentsAndTheirPaths = new Dictionary<string, List<IComponentTemporaryInput>>();

            var tempComponent1 = new List<IComponentTemporaryInput>()
            {
                new ShelterBeltTemporaryInput()
                {
                  Name = "Shelterbelt1",
                    
                },

            };

            var tempComponent2 = new List<IComponentTemporaryInput>()
            {
                new ShelterBeltTemporaryInput()
                {
                    Name = "Shelterbelt1"
                },

            };

            var tempComponent3 = new List<IComponentTemporaryInput>()
            {
                new ShelterBeltTemporaryInput()
                {
                    Name = "Shelterbelt2"
                },

            };

            var tempComponent4 = new List<IComponentTemporaryInput>()
            {
                new ShelterBeltTemporaryInput()
                {
                    Name = "Shelterbelt2"
                },

            };

            listOfComponentsAndTheirPaths.Add("PathExample1", tempComponent1);
            listOfComponentsAndTheirPaths.Add("PathExample2", tempComponent2);
            listOfComponentsAndTheirPaths.Add("PathExample3", tempComponent3);
            listOfComponentsAndTheirPaths.Add("PathExample4", tempComponent4);
            var errorHandler = new ErrorHandler();
            Assert.ThrowsException<FormatException>(() => errorHandler.CheckIfComponentsHaveTheSameName(listOfComponentsAndTheirPaths));

        }
        [TestMethod]
        public void TestCheckIfComponentsHaveTheSameName_NoComponentsHaveTheSameName_DoesNotThrowException()
        {
            var listOfComponentsAndTheirPaths = new Dictionary<string, List<IComponentTemporaryInput>>();

            var tempComponent1 = new List<IComponentTemporaryInput>()
            {
                new ShelterBeltTemporaryInput()
                {
                    Name = "Shelterbelt1"
                },

            };

            var tempComponent2 = new List<IComponentTemporaryInput>()
            {
                new ShelterBeltTemporaryInput()
                {
                    Name = "Shelterbelt2"
                },

            };

            var tempComponent3 = new List<IComponentTemporaryInput>()
            {
                new ShelterBeltTemporaryInput()
                {
                    Name = "Shelterbelt3"
                },

            };

            listOfComponentsAndTheirPaths.Add("PathExample1", tempComponent1);
            listOfComponentsAndTheirPaths.Add("PathExample2", tempComponent2);
            listOfComponentsAndTheirPaths.Add("PathExample3", tempComponent3);
            var errorHandler = new ErrorHandler();
        }

        [TestMethod]
        public void CheckIfAnimalGroupsOnlyReferToOneAnimalType_DifferentGroupTypes_ExpectExceptionToBeThrown()
        {
            var errorHandler = new ErrorHandler();
            var file = "TestFilePath";
            var parsedFile = new List<IComponentTemporaryInput>()
            {
                new PoultryTemporaryInput()
                {
                    GroupName = "PoultryGroup1",
                    GroupType = AnimalType.LayersWetPoultry
                },

                new PoultryTemporaryInput()
                {
                    GroupName = "PoultryGroup1",
                    GroupType = AnimalType.LayersDryPoultry
                },
            };

            var fileToComponentPair = new KeyValuePair<string, List<IComponentTemporaryInput>>(file, parsedFile);
            Assert.ThrowsException<Exception>(() => errorHandler.CheckIfEachGroupRefersToOneAnimalGroupType(fileToComponentPair));

        }

        [TestMethod]
        public void CheckIfAnimalGroupsOnlyReferToOneAnimalType_SameGroupType_ExpectExceptionNotThrown()
        {
            var errorHandler = new ErrorHandler();
            var file = "TestFilePath";
            var parsedFile = new List<IComponentTemporaryInput>()
            {
                new PoultryTemporaryInput()
                {
                    GroupName = "PoultryGroup1",
                    GroupType = AnimalType.LayersDryPoultry
                },

                new PoultryTemporaryInput()
                {
                    GroupName = "PoultryGroup1",
                    GroupType = AnimalType.LayersDryPoultry
                },
            };

            var fileToComponentPair = new KeyValuePair<string, List<IComponentTemporaryInput>>(file, parsedFile);
            errorHandler.CheckIfEachGroupRefersToOneAnimalGroupType(fileToComponentPair);
        }
    }
}
