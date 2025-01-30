using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.Animals;
using H.Core.Providers.Feed;
using H.Core.Services.Initialization;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H.Core.Models.Animals.Beef;

namespace H.Core.Services.Animals
{
    public interface IManagementPeriodService
    {
        /// <summary>
        /// Configures the appropriate steer <see cref="ManagementPeriod"/> for the finishing <see cref="AnimalGroup"/> 
        /// </summary>
        /// <param name="farm">The <see cref="Farm"/> containing the relevant <see cref="AnimalGroup"/> used to derive relevant data</param>
        /// <param name="animalGroup">The <see cref="AnimalGroup"/> containing the <see cref="ManagementPeriod"/>'s that will be configured</param>
        /// <param name="bindingManagementPeriod">The instance of <see cref="ManagementPeriod"/> that is used for binding</param>
        /// <param name="animalGroupOnPropertyChanged">The instance of a <see cref="PropertyChangedEventHandler"/> that monitors animal group for property changed</param>
        void FinishingSteerGroupManagementPeriod(Farm farm, AnimalGroup animalGroup,
            ManagementPeriod bindingManagementPeriod, PropertyChangedEventHandler animalGroupOnPropertyChanged);

        /// <summary>
        /// Configures the appropriate heifer <see cref="ManagementPeriod"/> for the finishing <see cref="AnimalGroup"/> 
        /// </summary>
        /// <param name="farm">The <see cref="Farm"/> containing the relevant <see cref="AnimalGroup"/> used to derive relevant data</param>
        /// <param name="animalGroup">The <see cref="AnimalGroup"/> containing the <see cref="ManagementPeriod"/>'s that will be configured</param>
        /// <param name="bindingManagementPeriod">The instance of <see cref="ManagementPeriod"/> that is used for binding</param>
        /// <param name="animalGroupOnPropertyChanged">The instance of a <see cref="PropertyChangedEventHandler"/> that monitors animal group for property changed</param>
        void FinishingHeiferGroupManagementPeriod(Farm farm, AnimalGroup animalGroup,
            ManagementPeriod bindingManagementPeriod, PropertyChangedEventHandler animalGroupOnPropertyChanged);

        /// <summary>
        /// Configures the appropriate steer <see cref="ManagementPeriod"/> for the backgrounding <see cref="AnimalGroup"/> 
        /// </summary>
        /// <param name="farm">The <see cref="Farm"/> containing the relevant <see cref="AnimalGroup"/> used to derive relevant data</param>
        /// <param name="animalGroup">The <see cref="AnimalGroup"/> containing the <see cref="ManagementPeriod"/>'s that will be configured</param>
        /// <param name="bindingManagementPeriod">The instance of <see cref="ManagementPeriod"/> that is used for binding</param>
        /// <param name="animalGroupOnPropertyChanged">The instance of a <see cref="PropertyChangedEventHandler"/> that monitors animal group for property changed</param>
        void BackgrounderSteerGroupManagementPeriod(Farm farm, AnimalGroup animalGroup,
            ManagementPeriod bindingManagementPeriod, PropertyChangedEventHandler animalGroupOnPropertyChanged);

        /// <summary>
        /// Configures the appropriate heifers <see cref="ManagementPeriod"/> for the backgrounding <see cref="AnimalGroup"/> 
        /// </summary>
        /// <param name="farm">The <see cref="Farm"/> containing the relevant <see cref="AnimalGroup"/> used to derive relevant data</param>
        /// <param name="animalGroup">The <see cref="AnimalGroup"/> containing the <see cref="ManagementPeriod"/>'s that will be configured</param>
        /// <param name="bindingManagementPeriod">The instance of <see cref="ManagementPeriod"/> that is used for binding</param>
        /// <param name="animalGroupOnPropertyChanged">The instance of a <see cref="PropertyChangedEventHandler"/> that monitors animal group for property changed</param>
        void BackgrounderHeifersGroupManagementPeriod(Farm farm, AnimalGroup animalGroup,
            ManagementPeriod bindingManagementPeriod, PropertyChangedEventHandler animalGroupOnPropertyChanged);

        /// <summary>
        /// Configures the appropriate bull <see cref="ManagementPeriod"/> for the cow calf <see cref="AnimalGroup"/> 
        /// </summary>
        /// <param name="farm">The <see cref="Farm"/> containing the relevant <see cref="AnimalGroup"/> used to derive relevant data</param>
        /// <param name="animalGroup">The <see cref="AnimalGroup"/> containing the <see cref="ManagementPeriod"/>'s that will be configured</param>
        /// <param name="bindingManagementPeriod">The instance of <see cref="ManagementPeriod"/> that is used for binding</param>
        /// <param name="animalGroupOnPropertyChanged">The instance of a <see cref="PropertyChangedEventHandler"/> that monitors animal group for property changed</param>
        void CowCalfBullGroupManagementPeriod(Farm farm, AnimalGroup animalGroup,
            ManagementPeriod bindingManagementPeriod, PropertyChangedEventHandler animalGroupOnPropertyChanged);

        /// <summary>
        /// Configures the appropriate replacement heifer <see cref="ManagementPeriod"/> for the cow calf <see cref="AnimalGroup"/> 
        /// </summary>
        /// <param name="farm">The <see cref="Farm"/> containing the relevant <see cref="AnimalGroup"/> used to derive relevant data</param>
        /// <param name="animalGroup">The <see cref="AnimalGroup"/> containing the <see cref="ManagementPeriod"/>'s that will be configured</param>
        /// <param name="bindingManagementPeriod">The instance of <see cref="ManagementPeriod"/> that is used for binding</param>
        /// <param name="animalGroupOnPropertyChanged">The instance of a <see cref="PropertyChangedEventHandler"/> that monitors animal group for property changed</param>
        void CowCalfReplacementHeifersManagementPeriod(Farm farm, AnimalGroup animalGroup,
            ManagementPeriod bindingManagementPeriod, PropertyChangedEventHandler animalGroupOnPropertyChanged);

        /// <summary>
        /// Configures the appropriate calf <see cref="ManagementPeriod"/> for the cow calf <see cref="AnimalGroup"/> 
        /// </summary>
        /// <param name="farm">The <see cref="Farm"/> containing the relevant <see cref="AnimalGroup"/> used to derive relevant data</param>
        /// <param name="animalGroup">The <see cref="AnimalGroup"/> containing the <see cref="ManagementPeriod"/>'s that will be configured</param>
        /// <param name="bindingManagementPeriod">The instance of <see cref="ManagementPeriod"/> that is used for binding</param>
        /// <param name="animalGroupOnPropertyChanged">The instance of a <see cref="PropertyChangedEventHandler"/> that monitors animal group for property changed</param>
        void CowCalfCalfGroupManagementPeriod(Farm farm, AnimalGroup animalGroup,
            ManagementPeriod bindingManagementPeriod, PropertyChangedEventHandler animalGroupOnPropertyChanged);

        /// <summary>
        /// Configures the appropriate cow <see cref="ManagementPeriod"/> for the cow calf <see cref="AnimalGroup"/> 
        /// </summary>
        /// <param name="farm">The <see cref="Farm"/> containing the relevant <see cref="AnimalGroup"/> used to derive relevant data</param>
        /// <param name="animalGroup">The <see cref="AnimalGroup"/> containing the <see cref="ManagementPeriod"/>'s that will be configured</param>
        /// <param name="bindingManagementPeriod">The instance of <see cref="ManagementPeriod"/> that is used for binding</param>
        /// <param name="animalGroupOnPropertyChanged">The instance of a <see cref="PropertyChangedEventHandler"/> that monitors animal group for property changed</param>
        void CowCalfCowGroupManagementPeriod(Farm farm, AnimalGroup animalGroup,
            ManagementPeriod bindingManagementPeriod, PropertyChangedEventHandler animalGroupOnPropertyChanged);

        /// <summary>
        /// Configures the appropriate calves <see cref="ManagementPeriod"/> for the dairy <see cref="AnimalGroup"/> 
        /// </summary>
        /// <param name="farm">The <see cref="Farm"/> containing the relevant <see cref="AnimalGroup"/> used to derive relevant data</param>
        /// <param name="animalGroup">The <see cref="AnimalGroup"/> containing the <see cref="ManagementPeriod"/>'s that will be configured</param>
        /// <param name="bindingManagementPeriod">The instance of <see cref="ManagementPeriod"/> that is used for binding</param>
        /// <param name="animalGroupOnPropertyChanged">The instance of a <see cref="PropertyChangedEventHandler"/> that monitors animal group for property changed</param>
        void DairyCalvesGroupManagementPeriod(Farm farm, AnimalGroup animalGroup,
            ManagementPeriod bindingManagementPeriod, PropertyChangedEventHandler animalGroupOnPropertyChanged);

        /// <summary>
        /// Configures the appropriate replacement heifer <see cref="ManagementPeriod"/> for the dairy <see cref="AnimalGroup"/> 
        /// </summary>
        /// <param name="farm">The <see cref="Farm"/> containing the relevant <see cref="AnimalGroup"/> used to derive relevant data</param>
        /// <param name="animalGroup">The <see cref="AnimalGroup"/> containing the <see cref="ManagementPeriod"/>'s that will be configured</param>
        /// <param name="bindingManagementPeriod">The instance of <see cref="ManagementPeriod"/> that is used for binding</param>
        /// <param name="animalGroupOnPropertyChanged">The instance of a <see cref="PropertyChangedEventHandler"/> that monitors animal group for property changed</param>
        void DairyReplacementHeifersManagementPeriod(Farm farm, AnimalGroup animalGroup,
            ManagementPeriod bindingManagementPeriod, PropertyChangedEventHandler animalGroupOnPropertyChanged);

        /// <summary>
        /// Configures the appropriate dry <see cref="ManagementPeriod"/> for the dairy <see cref="AnimalGroup"/> 
        /// </summary>
        /// <param name="farm">The <see cref="Farm"/> containing the relevant <see cref="AnimalGroup"/> used to derive relevant data</param>
        /// <param name="animalGroup">The <see cref="AnimalGroup"/> containing the <see cref="ManagementPeriod"/>'s that will be configured</param>
        /// <param name="bindingManagementPeriod">The instance of <see cref="ManagementPeriod"/> that is used for binding</param>
        /// <param name="animalGroupOnPropertyChanged">The instance of a <see cref="PropertyChangedEventHandler"/> that monitors animal group for property changed</param>
        void DairyDryManagementPeriod(Farm farm, AnimalGroup animalGroup, ManagementPeriod bindingManagementPeriod,
            PropertyChangedEventHandler animalGroupOnPropertyChanged);

        /// <summary>
        /// Configures the appropriate lactating <see cref="ManagementPeriod"/> for the dairy <see cref="AnimalGroup"/> 
        /// </summary>
        /// <param name="farm">The <see cref="Farm"/> containing the relevant <see cref="AnimalGroup"/> used to derive relevant data</param>
        /// <param name="animalGroup">The <see cref="AnimalGroup"/> containing the <see cref="ManagementPeriod"/>'s that will be configured</param>
        /// <param name="bindingManagementPeriod">The instance of <see cref="ManagementPeriod"/> that is used for binding</param>
        /// <param name="animalGroupOnPropertyChanged">The instance of a <see cref="PropertyChangedEventHandler"/> that monitors animal group for property changed</param>
        void DairyLactatingManagementPeriod(Farm farm, AnimalGroup animalGroup,
            ManagementPeriod bindingManagementPeriod, PropertyChangedEventHandler animalGroupOnPropertyChanged);

        /// <summary>
        /// Configures the appropriate boars <see cref="ManagementPeriod"/> for the farrow to finish <see cref="AnimalGroup"/> 
        /// </summary>
        /// <param name="farm">The <see cref="Farm"/> containing the relevant <see cref="AnimalGroup"/> used to derive relevant data</param>
        /// <param name="animalGroup">The <see cref="AnimalGroup"/> containing the <see cref="ManagementPeriod"/>'s that will be configured</param>
        /// <param name="bindingManagementPeriod">The instance of <see cref="ManagementPeriod"/> that is used for binding</param>
        /// <param name="animalGroupOnPropertyChanged">The instance of a <see cref="PropertyChangedEventHandler"/> that monitors animal group for property changed</param>
        /// <param name="managementPeriodOnPropertyChanged">The instance of a <see cref="PropertyChangedEventHandler"/> that monitors management period on property changed</param>
        void FarrowToFinishBoarsManagementPeriod(Farm farm, AnimalGroup animalGroup,
            ManagementPeriod bindingManagementPeriod, PropertyChangedEventHandler animalGroupOnPropertyChanged,
            PropertyChangedEventHandler managementPeriodOnPropertyChanged);

        /// <summary>
        /// Configures the appropriate gilts <see cref="ManagementPeriod"/> for the farrow to finish <see cref="AnimalGroup"/> 
        /// </summary>
        /// <param name="farm">The <see cref="Farm"/> containing the relevant <see cref="AnimalGroup"/> used to derive relevant data</param>
        /// <param name="animalGroup">The <see cref="AnimalGroup"/> containing the <see cref="ManagementPeriod"/>'s that will be configured</param>
        /// <param name="bindingManagementPeriod">The instance of <see cref="ManagementPeriod"/> that is used for binding</param>
        /// <param name="animalGroupOnPropertyChanged">The instance of a <see cref="PropertyChangedEventHandler"/> that monitors animal group for property changed</param>
        /// <param name="managementPeriodOnPropertyChanged">The instance of a <see cref="PropertyChangedEventHandler"/> that monitors management period on property changed</param>
        void FarrowToFinishGiltsManagementPeriod(Farm farm, AnimalGroup animalGroup,
            ManagementPeriod bindingManagementPeriod, PropertyChangedEventHandler animalGroupOnPropertyChanged,
            PropertyChangedEventHandler managementPeriodOnPropertyChanged);

        /// <summary>
        /// Configures the appropriate sows <see cref="ManagementPeriod"/> for the farrow to finish <see cref="AnimalGroup"/> 
        /// </summary>
        /// <param name="farm">The <see cref="Farm"/> containing the relevant <see cref="AnimalGroup"/> used to derive relevant data</param>
        /// <param name="animalGroup">The <see cref="AnimalGroup"/> containing the <see cref="ManagementPeriod"/>'s that will be configured</param>
        /// <param name="bindingManagementPeriod">The instance of <see cref="ManagementPeriod"/> that is used for binding</param>
        /// <param name="animalGroupOnPropertyChanged">The instance of a <see cref="PropertyChangedEventHandler"/> that monitors animal group for property changed</param>
        /// <param name="managementPeriodOnPropertyChanged">The instance of a <see cref="PropertyChangedEventHandler"/> that monitors management period on property changed</param>
        void FarrowToFinishSowsManagementPeriod(Farm farm, AnimalGroup animalGroup,
            ManagementPeriod bindingManagementPeriod, PropertyChangedEventHandler animalGroupOnPropertyChanged,
            PropertyChangedEventHandler managementPeriodOnPropertyChanged);

        /// <summary>
        /// Configures the appropriate hogs <see cref="ManagementPeriod"/> for the farrow to finish <see cref="AnimalGroup"/> 
        /// </summary>
        /// <param name="farm">The <see cref="Farm"/> containing the relevant <see cref="AnimalGroup"/> used to derive relevant data</param>
        /// <param name="animalGroup">The <see cref="AnimalGroup"/> containing the <see cref="ManagementPeriod"/>'s that will be configured</param>
        /// <param name="bindingManagementPeriod">The instance of <see cref="ManagementPeriod"/> that is used for binding</param>
        /// <param name="animalGroupOnPropertyChanged">The instance of a <see cref="PropertyChangedEventHandler"/> that monitors animal group for property changed</param>
        /// <param name="managementPeriodOnPropertyChanged">The instance of a <see cref="PropertyChangedEventHandler"/> that monitors management period on property changed</param>
        void FarrowToFinishHogsManagementPeriod(Farm farm, AnimalGroup animalGroup,
            ManagementPeriod bindingManagementPeriod, PropertyChangedEventHandler animalGroupOnPropertyChanged,
            PropertyChangedEventHandler managementPeriodOnPropertyChanged);

        /// <summary>
        /// Configures the appropriate piglets <see cref="ManagementPeriod"/> for the farrow to finish <see cref="AnimalGroup"/> 
        /// </summary>
        /// <param name="farm">The <see cref="Farm"/> containing the relevant <see cref="AnimalGroup"/> used to derive relevant data</param>
        /// <param name="animalGroup">The <see cref="AnimalGroup"/> containing the <see cref="ManagementPeriod"/>'s that will be configured</param>
        /// <param name="bindingManagementPeriod">The instance of <see cref="ManagementPeriod"/> that is used for binding</param>
        /// <param name="animalGroupOnPropertyChanged">The instance of a <see cref="PropertyChangedEventHandler"/> that monitors animal group for property changed</param>
        /// <param name="managementPeriodOnPropertyChanged">The instance of a <see cref="PropertyChangedEventHandler"/> that monitors management period on property changed</param>
        void FarrowToFinishPigletsManagementPeriod(Farm farm, AnimalGroup animalGroup,
            ManagementPeriod bindingManagementPeriod, PropertyChangedEventHandler animalGroupOnPropertyChanged,
            PropertyChangedEventHandler managementPeriodOnPropertyChanged);

        /// <summary>
        /// Configures the appropriate weaned piglets <see cref="ManagementPeriod"/>'s for the farrow to finish <see cref="AnimalGroup"/> 
        /// </summary>
        /// <param name="farm">The <see cref="Farm"/> containing the relevant <see cref="AnimalGroup"/> used to derive relevant data</param>
        /// <param name="animalGroup">The <see cref="AnimalGroup"/> containing the <see cref="ManagementPeriod"/>'s that will be configured</param>
        /// <param name="bindingManagementPeriod">The instance of <see cref="ManagementPeriod"/> that is used for binding</param>
        /// <param name="animalGroupOnPropertyChanged">The instance of a <see cref="PropertyChangedEventHandler"/> that monitors animal group for property changed</param>
        /// <param name="managementPeriodOnPropertyChanged">The instance of a <see cref="PropertyChangedEventHandler"/> that monitors management period on property changed</param>
        void FarrowToFinishWeanedPigletsManagementPeriod(Farm farm, AnimalGroup animalGroup,
            ManagementPeriod bindingManagementPeriod, PropertyChangedEventHandler animalGroupOnPropertyChanged,
            PropertyChangedEventHandler managementPeriodOnPropertyChanged);

        /// <summary>
        /// Configures the appropriate piglets <see cref="ManagementPeriod"/>'s for the iso wean <see cref="AnimalGroup"/> 
        /// </summary>
        /// <param name="farm">The <see cref="Farm"/> containing the relevant <see cref="AnimalGroup"/> used to derive relevant data</param>
        /// <param name="animalGroup">The <see cref="AnimalGroup"/> containing the <see cref="ManagementPeriod"/>'s that will be configured</param>
        /// <param name="bindingManagementPeriod">The instance of <see cref="ManagementPeriod"/> that is used for binding</param>
        /// <param name="animalGroupOnPropertyChanged">The instance of a <see cref="PropertyChangedEventHandler"/> that monitors animal group for property changed</param>
        /// <param name="managementPeriodOnPropertyChanged">The instance of a <see cref="PropertyChangedEventHandler"/> that monitors management period on property changed</param>
        void IsoWeanPigletsManagementPeriod(Farm farm, AnimalGroup animalGroup,
            ManagementPeriod bindingManagementPeriod, PropertyChangedEventHandler animalGroupOnPropertyChanged,
            PropertyChangedEventHandler managementPeriodOnPropertyChanged);

        /// <summary>
        /// Configures the appropriate piglets <see cref="ManagementPeriod"/>'s for the farrow to wean <see cref="AnimalGroup"/> 
        /// </summary>
        /// <param name="farm">The <see cref="Farm"/> containing the relevant <see cref="AnimalGroup"/> used to derive relevant data</param>
        /// <param name="animalGroup">The <see cref="AnimalGroup"/> containing the <see cref="ManagementPeriod"/>'s that will be configured</param>
        /// <param name="bindingManagementPeriod">The instance of <see cref="ManagementPeriod"/> that is used for binding</param>
        /// <param name="animalGroupOnPropertyChanged">The instance of a <see cref="PropertyChangedEventHandler"/> that monitors animal group for property changed</param>
        /// <param name="managementPeriodOnPropertyChanged">The instance of a <see cref="PropertyChangedEventHandler"/> that monitors management period on property changed</param>
        void FarrowToWeanPigletsManagementPeriod(Farm farm, AnimalGroup animalGroup,
            ManagementPeriod bindingManagementPeriod, PropertyChangedEventHandler animalGroupOnPropertyChanged,
            PropertyChangedEventHandler managementPeriodOnPropertyChanged);

        /// <summary>
        /// Configures the appropriate gilts <see cref="ManagementPeriod"/>'s for the farrow to wean <see cref="AnimalGroup"/> 
        /// </summary>
        /// <param name="farm">The <see cref="Farm"/> containing the relevant <see cref="AnimalGroup"/> used to derive relevant data</param>
        /// <param name="animalGroup">The <see cref="AnimalGroup"/> containing the <see cref="ManagementPeriod"/>'s that will be configured</param>
        /// <param name="bindingManagementPeriod">The instance of <see cref="ManagementPeriod"/> that is used for binding</param>
        /// <param name="animalGroupOnPropertyChanged">The instance of a <see cref="PropertyChangedEventHandler"/> that monitors animal group for property changed</param>
        /// <param name="managementPeriodOnPropertyChanged">The instance of a <see cref="PropertyChangedEventHandler"/> that monitors management period on property changed</param>
        void FarrowToWeanGiltsManagementPeriod(Farm farm, AnimalGroup animalGroup,
            ManagementPeriod bindingManagementPeriod, PropertyChangedEventHandler animalGroupOnPropertyChanged,
            PropertyChangedEventHandler managementPeriodOnPropertyChanged);

        /// <summary>
        /// Configures the appropriate sows <see cref="ManagementPeriod"/>'s for the farrow to wean <see cref="AnimalGroup"/> 
        /// </summary>
        /// <param name="farm">The <see cref="Farm"/> containing the relevant <see cref="AnimalGroup"/> used to derive relevant data</param>
        /// <param name="animalGroup">The <see cref="AnimalGroup"/> containing the <see cref="ManagementPeriod"/>'s that will be configured</param>
        /// <param name="bindingManagementPeriod">The instance of <see cref="ManagementPeriod"/> that is used for binding</param>
        /// <param name="animalGroupOnPropertyChanged">The instance of a <see cref="PropertyChangedEventHandler"/> that monitors animal group for property changed</param>
        /// <param name="managementPeriodOnPropertyChanged">The instance of a <see cref="PropertyChangedEventHandler"/> that monitors management period on property changed</param>
        void FarrowToWeanSowsManagementPeriod(Farm farm, AnimalGroup animalGroup,
            ManagementPeriod bindingManagementPeriod, PropertyChangedEventHandler animalGroupOnPropertyChanged,
            PropertyChangedEventHandler managementPeriodOnPropertyChanged);

        /// <summary>
        /// Configures the appropriate boars <see cref="ManagementPeriod"/>'s for the farrow to wean <see cref="AnimalGroup"/> 
        /// </summary>
        /// <param name="farm">The <see cref="Farm"/> containing the relevant <see cref="AnimalGroup"/> used to derive relevant data</param>
        /// <param name="animalGroup">The <see cref="AnimalGroup"/> containing the <see cref="ManagementPeriod"/>'s that will be configured</param>
        /// <param name="bindingManagementPeriod">The instance of <see cref="ManagementPeriod"/> that is used for binding</param>
        /// <param name="animalGroupOnPropertyChanged">The instance of a <see cref="PropertyChangedEventHandler"/> that monitors animal group for property changed</param>
        /// <param name="managementPeriodOnPropertyChanged">The instance of a <see cref="PropertyChangedEventHandler"/> that monitors management period on property changed</param>
        void FarrowToWeanBoarsManagementPeriod(Farm farm, AnimalGroup animalGroup,
            ManagementPeriod bindingManagementPeriod, PropertyChangedEventHandler animalGroupOnPropertyChanged,
            PropertyChangedEventHandler managementPeriodOnPropertyChanged);

        /// <summary>
        /// Configures the appropriate hogs <see cref="ManagementPeriod"/>'s for the grower to finish <see cref="AnimalGroup"/> 
        /// </summary>
        /// <param name="farm">The <see cref="Farm"/> containing the relevant <see cref="AnimalGroup"/> used to derive relevant data</param>
        /// <param name="animalGroup">The <see cref="AnimalGroup"/> containing the <see cref="ManagementPeriod"/>'s that will be configured</param>
        /// <param name="bindingManagementPeriod">The instance of <see cref="ManagementPeriod"/> that is used for binding</param>
        /// <param name="animalGroupOnPropertyChanged">The instance of a <see cref="PropertyChangedEventHandler"/> that monitors animal group for property changed</param>
        /// <param name="managementPeriodOnPropertyChanged">The instance of a <see cref="PropertyChangedEventHandler"/> that monitors management period on property changed</param>
        void GrowerToFinishHogsManagementPeriod(Farm farm, AnimalGroup animalGroup,
            ManagementPeriod bindingManagementPeriod, PropertyChangedEventHandler animalGroupOnPropertyChanged,
            PropertyChangedEventHandler managementPeriodOnPropertyChanged);

        /// <summary>
        /// Configures the appropriate ewes <see cref="ManagementPeriod"/>'s for the lambs and ewes <see cref="AnimalGroup"/> 
        /// </summary>
        /// <param name="farm">The <see cref="Farm"/> containing the relevant <see cref="AnimalGroup"/> used to derive relevant data</param>
        /// <param name="animalGroup">The <see cref="AnimalGroup"/> containing the <see cref="ManagementPeriod"/>'s that will be configured</param>
        /// <param name="bindingManagementPeriod">The instance of <see cref="ManagementPeriod"/> that is used for binding</param>
        /// <param name="animalGroupOnPropertyChanged">The instance of a <see cref="PropertyChangedEventHandler"/> that monitors animal group for property changed</param>
        void LambsAndEwesEwesManagementPeriod(Farm farm, AnimalGroup animalGroup,
            ManagementPeriod bindingManagementPeriod, PropertyChangedEventHandler animalGroupOnPropertyChanged);

        /// <summary>
        /// Configures the appropriate lambs <see cref="ManagementPeriod"/>'s for the lamb and ewes <see cref="AnimalGroup"/> 
        /// </summary>
        /// <param name="farm">The <see cref="Farm"/> containing the relevant <see cref="AnimalGroup"/> used to derive relevant data</param>
        /// <param name="animalGroup">The <see cref="AnimalGroup"/> containing the <see cref="ManagementPeriod"/>'s that will be configured</param>
        /// <param name="bindingManagementPeriod">The instance of <see cref="ManagementPeriod"/> that is used for binding</param>
        /// <param name="animalGroupOnPropertyChanged">The instance of a <see cref="PropertyChangedEventHandler"/> that monitors animal group for property changed</param>
        void LambsAndEwesLambsManagementPeriod(Farm farm, AnimalGroup animalGroup,
            ManagementPeriod bindingManagementPeriod, PropertyChangedEventHandler animalGroupOnPropertyChanged);

        /// <summary>
        /// Configures the appropriate rams <see cref="ManagementPeriod"/>'s for the rams <see cref="AnimalGroup"/> 
        /// </summary>
        /// <param name="farm">The <see cref="Farm"/> containing the relevant <see cref="AnimalGroup"/> used to derive relevant data</param>
        /// <param name="animalGroup">The <see cref="AnimalGroup"/> containing the <see cref="ManagementPeriod"/>'s that will be configured</param>
        /// <param name="bindingManagementPeriod">The instance of <see cref="ManagementPeriod"/> that is used for binding</param>
        /// <param name="animalGroupOnPropertyChanged">The instance of a <see cref="PropertyChangedEventHandler"/> that monitors animal group for property changed</param>
        void RamsManagementPeriod(Farm farm, AnimalGroup animalGroup, ManagementPeriod bindingManagementPeriod,
            PropertyChangedEventHandler animalGroupOnPropertyChanged);

        /// <summary>
        /// Configures the appropriate sheep feedlot <see cref="ManagementPeriod"/>'s for the sheep feedlot<see cref="AnimalGroup"/> 
        /// </summary>
        /// <param name="farm">The <see cref="Farm"/> containing the relevant <see cref="AnimalGroup"/> used to derive relevant data</param>
        /// <param name="animalGroup">The <see cref="AnimalGroup"/> containing the <see cref="ManagementPeriod"/>'s that will be configured</param>
        /// <param name="bindingManagementPeriod">The instance of <see cref="ManagementPeriod"/> that is used for binding</param>
        /// <param name="animalGroupOnPropertyChanged">The instance of a <see cref="PropertyChangedEventHandler"/> that monitors animal group for property changed</param>
        void SheepFeedlotManagementPeriod(Farm farm, AnimalGroup animalGroup, ManagementPeriod bindingManagementPeriod,
            PropertyChangedEventHandler animalGroupOnPropertyChanged);

        /// <summary>
        /// Configures the appropriate hens <see cref="ManagementPeriod"/>'s for the chicken egg production <see cref="AnimalGroup"/> 
        /// </summary>
        /// <param name="farm">The <see cref="Farm"/> containing the relevant <see cref="AnimalGroup"/> used to derive relevant data</param>
        /// <param name="animalGroup">The <see cref="AnimalGroup"/> containing the <see cref="ManagementPeriod"/>'s that will be configured</param>
        /// <param name="bindingManagementPeriod">The instance of <see cref="ManagementPeriod"/> that is used for binding</param>
        /// <param name="animalGroupOnPropertyChanged">The instance of a <see cref="PropertyChangedEventHandler"/> that monitors animal group for property changed</param>
        void ChickenEggProductionHensManagementPeriod(Farm farm, AnimalGroup animalGroup,
            ManagementPeriod bindingManagementPeriod, PropertyChangedEventHandler animalGroupOnPropertyChanged);

        /// <summary>
        /// Configures the appropriate pullets <see cref="ManagementPeriod"/>'s for the pullet farm <see cref="AnimalGroup"/> 
        /// </summary>
        /// <param name="farm">The <see cref="Farm"/> containing the relevant <see cref="AnimalGroup"/> used to derive relevant data</param>
        /// <param name="animalGroup">The <see cref="AnimalGroup"/> containing the <see cref="ManagementPeriod"/>'s that will be configured</param>
        /// <param name="bindingManagementPeriod">The instance of <see cref="ManagementPeriod"/> that is used for binding</param>
        /// <param name="animalGroupOnPropertyChanged">The instance of a <see cref="PropertyChangedEventHandler"/> that monitors animal group for property changed</param>
        void PulletFarmPulletsManagementPeriod(Farm farm, AnimalGroup animalGroup,
            ManagementPeriod bindingManagementPeriod, PropertyChangedEventHandler animalGroupOnPropertyChanged);

        /// <summary>
        /// Configures the appropriate layer <see cref="ManagementPeriod"/>'s for the chicken multiplier breeder <see cref="AnimalGroup"/> 
        /// </summary>
        /// <param name="farm">The <see cref="Farm"/> containing the relevant <see cref="AnimalGroup"/> used to derive relevant data</param>
        /// <param name="animalGroup">The <see cref="AnimalGroup"/> containing the <see cref="ManagementPeriod"/>'s that will be configured</param>
        /// <param name="bindingManagementPeriod">The instance of <see cref="ManagementPeriod"/> that is used for binding</param>
        /// <param name="animalGroupOnPropertyChanged">The instance of a <see cref="PropertyChangedEventHandler"/> that monitors animal group for property changed</param>
        void ChickenMultiplierBreederLayersManagementPeriod(Farm farm, AnimalGroup animalGroup,
            ManagementPeriod bindingManagementPeriod, PropertyChangedEventHandler animalGroupOnPropertyChanged);

        /// <summary>
        /// Configures the appropriate broiler <see cref="ManagementPeriod"/>'s for the chicken multiplier breeder <see cref="AnimalGroup"/> 
        /// </summary>
        /// <param name="farm">The <see cref="Farm"/> containing the relevant <see cref="AnimalGroup"/> used to derive relevant data</param>
        /// <param name="animalGroup">The <see cref="AnimalGroup"/> containing the <see cref="ManagementPeriod"/>'s that will be configured</param>
        /// <param name="bindingManagementPeriod">The instance of <see cref="ManagementPeriod"/> that is used for binding</param>
        /// <param name="animalGroupOnPropertyChanged">The instance of a <see cref="PropertyChangedEventHandler"/> that monitors animal group for property changed</param>
        void ChickenMultiplierBreederBroilersManagementPeriod(Farm farm, AnimalGroup animalGroup,
            ManagementPeriod bindingManagementPeriod, PropertyChangedEventHandler animalGroupOnPropertyChanged);

        /// <summary>
        /// Configures the appropriate broiler <see cref="ManagementPeriod"/>'s for the chicken meat production <see cref="AnimalGroup"/> 
        /// </summary>
        /// <param name="farm">The <see cref="Farm"/> containing the relevant <see cref="AnimalGroup"/> used to derive relevant data</param>
        /// <param name="animalGroup">The <see cref="AnimalGroup"/> containing the <see cref="ManagementPeriod"/>'s that will be configured</param>
        /// <param name="bindingManagementPeriod">The instance of <see cref="ManagementPeriod"/> that is used for binding</param>
        /// <param name="animalGroupOnPropertyChanged">The instance of a <see cref="PropertyChangedEventHandler"/> that monitors animal group for property changed</param>
        void ChickenMeatProductionBroilersManagementPeriod(Farm farm, AnimalGroup animalGroup,
            ManagementPeriod bindingManagementPeriod, PropertyChangedEventHandler animalGroupOnPropertyChanged);

        /// <summary>
        /// Configures the appropriate young toms <see cref="ManagementPeriod"/>'s for the turkey multiplier breeder <see cref="AnimalGroup"/> 
        /// </summary>
        /// <param name="farm">The <see cref="Farm"/> containing the relevant <see cref="AnimalGroup"/> used to derive relevant data</param>
        /// <param name="animalGroup">The <see cref="AnimalGroup"/> containing the <see cref="ManagementPeriod"/>'s that will be configured</param>
        /// <param name="bindingManagementPeriod">The instance of <see cref="ManagementPeriod"/> that is used for binding</param>
        /// <param name="animalGroupOnPropertyChanged">The instance of a <see cref="PropertyChangedEventHandler"/> that monitors animal group for property changed</param>
        void TurkeyMultiplierBreederYoungTomsManagementPeriod(Farm farm, AnimalGroup animalGroup,
            ManagementPeriod bindingManagementPeriod, PropertyChangedEventHandler animalGroupOnPropertyChanged);

        /// <summary>
        /// Configures the appropriate toms <see cref="ManagementPeriod"/>'s for the turkey multiplier breeder <see cref="AnimalGroup"/> 
        /// </summary>
        /// <param name="farm">The <see cref="Farm"/> containing the relevant <see cref="AnimalGroup"/> used to derive relevant data</param>
        /// <param name="animalGroup">The <see cref="AnimalGroup"/> containing the <see cref="ManagementPeriod"/>'s that will be configured</param>
        /// <param name="bindingManagementPeriod">The instance of <see cref="ManagementPeriod"/> that is used for binding</param>
        /// <param name="animalGroupOnPropertyChanged">The instance of a <see cref="PropertyChangedEventHandler"/> that monitors animal group for property changed</param>
        void TurkeyMultiplierBreederTomsManagementPeriod(Farm farm, AnimalGroup animalGroup,
            ManagementPeriod bindingManagementPeriod, PropertyChangedEventHandler animalGroupOnPropertyChanged);

        /// <summary>
        /// Configures the appropriate young turkey hens <see cref="ManagementPeriod"/>'s for the turkey multiplier breeder <see cref="AnimalGroup"/> 
        /// </summary>
        /// <param name="farm">The <see cref="Farm"/> containing the relevant <see cref="AnimalGroup"/> used to derive relevant data</param>
        /// <param name="animalGroup">The <see cref="AnimalGroup"/> containing the <see cref="ManagementPeriod"/>'s that will be configured</param>
        /// <param name="bindingManagementPeriod">The instance of <see cref="ManagementPeriod"/> that is used for binding</param>
        /// <param name="animalGroupOnPropertyChanged">The instance of a <see cref="PropertyChangedEventHandler"/> that monitors animal group for property changed</param>.
        void TurkeyMultiplierBreederYoungTurkeyHensManagementPeriod(Farm farm, AnimalGroup animalGroup,
            ManagementPeriod bindingManagementPeriod, PropertyChangedEventHandler animalGroupOnPropertyChanged);

        /// <summary>
        /// Configures the appropriate turkey hens <see cref="ManagementPeriod"/>'s for the turkey multiplier breeder <see cref="AnimalGroup"/> 
        /// </summary>
        /// <param name="farm">The <see cref="Farm"/> containing the relevant <see cref="AnimalGroup"/> used to derive relevant data</param>
        /// <param name="animalGroup">The <see cref="AnimalGroup"/> containing the <see cref="ManagementPeriod"/>'s that will be configured</param>
        /// <param name="bindingManagementPeriod">The instance of <see cref="ManagementPeriod"/> that is used for binding</param>
        /// <param name="animalGroupOnPropertyChanged">The instance of a <see cref="PropertyChangedEventHandler"/> that monitors animal group for property changed</param>
        void TurkeyMultiplierBreederTurkeyHensManagementPeriod(Farm farm, AnimalGroup animalGroup,
            ManagementPeriod bindingManagementPeriod, PropertyChangedEventHandler animalGroupOnPropertyChanged);

        /// <summary>
        /// Configures the appropriate young toms <see cref="ManagementPeriod"/>'s for the turkey meat production <see cref="AnimalGroup"/> 
        /// </summary>
        /// <param name="farm">The <see cref="Farm"/> containing the relevant <see cref="AnimalGroup"/> used to derive relevant data</param>
        /// <param name="animalGroup">The <see cref="AnimalGroup"/> containing the <see cref="ManagementPeriod"/>'s that will be configured</param>
        /// <param name="bindingManagementPeriod">The instance of <see cref="ManagementPeriod"/> that is used for binding</param>
        /// <param name="animalGroupOnPropertyChanged">The instance of a <see cref="PropertyChangedEventHandler"/> that monitors animal group for property changed</param>
        void TurkeyMeatProductionYoungTomsManagementPeriod(Farm farm, AnimalGroup animalGroup,
            ManagementPeriod bindingManagementPeriod, PropertyChangedEventHandler animalGroupOnPropertyChanged);

        /// <summary>
        /// Configures the appropriate young turkey hens <see cref="ManagementPeriod"/>'s for the turkey meat production <see cref="AnimalGroup"/> 
        /// </summary>
        /// <param name="farm">The <see cref="Farm"/> containing the relevant <see cref="AnimalGroup"/> used to derive relevant data</param>
        /// <param name="animalGroup">The <see cref="AnimalGroup"/> containing the <see cref="ManagementPeriod"/>'s that will be configured</param>
        /// <param name="bindingManagementPeriod">The instance of <see cref="ManagementPeriod"/> that is used for binding</param>
        /// <param name="animalGroupOnPropertyChanged">The instance of a <see cref="PropertyChangedEventHandler"/> that monitors animal group for property changed</param>
        void TurkeyMeatProductionYoungTurkeyHensManagementPeriod(Farm farm, AnimalGroup animalGroup,
            ManagementPeriod bindingManagementPeriod, PropertyChangedEventHandler animalGroupOnPropertyChanged);

        /// <summary>
        /// Configures the appropriate chicks <see cref="ManagementPeriod"/>'s for the chicken multiplier hatchery <see cref="AnimalGroup"/> 
        /// </summary>
        /// <param name="farm">The <see cref="Farm"/> containing the relevant <see cref="AnimalGroup"/> used to derive relevant data</param>
        /// <param name="animalGroup">The <see cref="AnimalGroup"/> containing the <see cref="ManagementPeriod"/>'s that will be configured</param>
        /// <param name="bindingManagementPeriod">The instance of <see cref="ManagementPeriod"/> that is used for binding</param>
        /// <param name="animalGroupOnPropertyChanged">The instance of a <see cref="PropertyChangedEventHandler"/> that monitors animal group for property changed</param>
        void ChickenMultiplierHatcheryChicksManagementPeriod(Farm farm, AnimalGroup animalGroup,
            ManagementPeriod bindingManagementPeriod, PropertyChangedEventHandler animalGroupOnPropertyChanged);

        /// <summary>
        /// Configures the appropriate poults <see cref="ManagementPeriod"/>'s for the chicken multiplier hatchery <see cref="AnimalGroup"/> 
        /// </summary>
        /// <param name="farm">The <see cref="Farm"/> containing the relevant <see cref="AnimalGroup"/> used to derive relevant data</param>
        /// <param name="animalGroup">The <see cref="AnimalGroup"/> containing the <see cref="ManagementPeriod"/>'s that will be configured</param>
        /// <param name="bindingManagementPeriod">The instance of <see cref="ManagementPeriod"/> that is used for binding</param>
        /// <param name="animalGroupOnPropertyChanged">The instance of a <see cref="PropertyChangedEventHandler"/> that monitors animal group for property changed</param>
        void ChickenMultiplierHatcheryPoultsManagementPeriod(Farm farm, AnimalGroup animalGroup,
            ManagementPeriod bindingManagementPeriod, PropertyChangedEventHandler animalGroupOnPropertyChanged);
        /// <summary>
        /// Configures the appropriate goats <see cref="ManagementPeriod"/>'s for the goats <see cref="AnimalGroup"/> 
        /// </summary>
        /// <param name="farm">The <see cref="Farm"/> containing the relevant <see cref="AnimalGroup"/> used to derive relevant data</param>
        /// <param name="animalGroup">The <see cref="AnimalGroup"/> containing the <see cref="ManagementPeriod"/>'s that will be configured</param>
        /// <param name="bindingManagementPeriod">The instance of <see cref="ManagementPeriod"/> that is used for binding</param>
        /// <param name="animalGroupOnPropertyChanged">The instance of a <see cref="PropertyChangedEventHandler"/> that monitors animal group for property changed</param>
        void GoatsManagementPeriod(Farm farm, AnimalGroup animalGroup, ManagementPeriod bindingManagementPeriod,
            PropertyChangedEventHandler animalGroupOnPropertyChanged);
        /// <summary>
        /// Configures the appropriate bison <see cref="ManagementPeriod"/>'s for the bison <see cref="AnimalGroup"/> 
        /// </summary>
        /// <param name="farm">The <see cref="Farm"/> containing the relevant <see cref="AnimalGroup"/> used to derive relevant data</param>
        /// <param name="animalGroup">The <see cref="AnimalGroup"/> containing the <see cref="ManagementPeriod"/>'s that will be configured</param>
        /// <param name="bindingManagementPeriod">The instance of <see cref="ManagementPeriod"/> that is used for binding</param>
        /// <param name="animalGroupOnPropertyChanged">The instance of a <see cref="PropertyChangedEventHandler"/> that monitors animal group for property changed</param>
        void BisonManagementPeriod(Farm farm, AnimalGroup animalGroup, ManagementPeriod bindingManagementPeriod,
            PropertyChangedEventHandler animalGroupOnPropertyChanged);
        /// <summary>
        /// Configures the appropriate mules <see cref="ManagementPeriod"/>'s for the mules <see cref="AnimalGroup"/> 
        /// </summary>
        /// <param name="farm">The <see cref="Farm"/> containing the relevant <see cref="AnimalGroup"/> used to derive relevant data</param>
        /// <param name="animalGroup">The <see cref="AnimalGroup"/> containing the <see cref="ManagementPeriod"/>'s that will be configured</param>
        /// <param name="bindingManagementPeriod">The instance of <see cref="ManagementPeriod"/> that is used for binding</param>
        /// <param name="animalGroupOnPropertyChanged">The instance of a <see cref="PropertyChangedEventHandler"/> that monitors animal group for property changed</param>
        void MulesManagementPeriod(Farm farm, AnimalGroup animalGroup, ManagementPeriod bindingManagementPeriod,
            PropertyChangedEventHandler animalGroupOnPropertyChanged);
        /// <summary>
        /// Configures the appropriate horses <see cref="ManagementPeriod"/>'s for the horses <see cref="AnimalGroup"/> 
        /// </summary>
        /// <param name="farm">The <see cref="Farm"/> containing the relevant <see cref="AnimalGroup"/> used to derive relevant data</param>
        /// <param name="animalGroup">The <see cref="AnimalGroup"/> containing the <see cref="ManagementPeriod"/>'s that will be configured</param>
        /// <param name="bindingManagementPeriod">The instance of <see cref="ManagementPeriod"/> that is used for binding</param>
        /// <param name="animalGroupOnPropertyChanged">The instance of a <see cref="PropertyChangedEventHandler"/> that monitors animal group for property changed</param>
        void HorsesManagementPeriod(Farm farm, AnimalGroup animalGroup, ManagementPeriod bindingManagementPeriod,
            PropertyChangedEventHandler animalGroupOnPropertyChanged);
        /// <summary>
        /// Configures the appropriate deer <see cref="ManagementPeriod"/>'s for the deer <see cref="AnimalGroup"/> 
        /// </summary>
        /// <param name="farm">The <see cref="Farm"/> containing the relevant <see cref="AnimalGroup"/> used to derive relevant data</param>
        /// <param name="animalGroup">The <see cref="AnimalGroup"/> containing the <see cref="ManagementPeriod"/>'s that will be configured</param>
        /// <param name="bindingManagementPeriod">The instance of <see cref="ManagementPeriod"/> that is used for binding</param>
        /// <param name="animalGroupOnPropertyChanged">The instance of a <see cref="PropertyChangedEventHandler"/> that monitors animal group for property changed</param>
        void DeerManagementPeriod(Farm farm, AnimalGroup animalGroup, ManagementPeriod bindingManagementPeriod,
            PropertyChangedEventHandler animalGroupOnPropertyChanged);
        /// <summary>
        /// Configures the appropriate llama <see cref="ManagementPeriod"/>'s for the llama <see cref="AnimalGroup"/> 
        /// </summary>
        /// <param name="farm">The <see cref="Farm"/> containing the relevant <see cref="AnimalGroup"/> used to derive relevant data</param>
        /// <param name="animalGroup">The <see cref="AnimalGroup"/> containing the <see cref="ManagementPeriod"/>'s that will be configured</param>
        /// <param name="bindingManagementPeriod">The instance of <see cref="ManagementPeriod"/> that is used for binding</param>
        /// <param name="animalGroupOnPropertyChanged">The instance of a <see cref="PropertyChangedEventHandler"/> that monitors animal group for property changed</param>
        void LlamaManagementPeriod(Farm farm, AnimalGroup animalGroup, ManagementPeriod bindingManagementPeriod,
            PropertyChangedEventHandler animalGroupOnPropertyChanged);
    }
}