# Example Input Tables For Command Line Interface

The purpose of this document is to provide examples of how to format input tables in an Excel file for the Holos Command Line Interface. Please read [here](https://github.com/holos-aafc/Holos/blob/main/H.Content/Documentation/User%20Guide/User%20Guide.md#chapter-10---command-line-interface) for more information on the Holos CLI.

<br>

## Field
| Parameter | Example value | Type value | Is user provided | has default? | note | Source (code, csv?, etc.) |
|---|---|---|---|---|---|---|
| Phase Number | 0|   | x |   | Deprecated. Do not use. Will be removed in future version |   |
| Name | wheat and hairy vetch |   | x | Yes | User defined name |   |
| Area | 18| Double | Yes | No |   |   |
| Current Year | 2024| Int | Yes | Current year (i.e. 2024) |   |   |
| Crop Year | 1985|   | Yes |   | Each (row) in input file must correspond to a certain year |   |
| Crop Type | Wheat |   | x |   |   |   |
| Tillage Type | Reduced | Enum | x | Wheat | See GUI for supported list of crop types since not all items in the enum are supported in calculations | https://github.com/holos-aafc/Holos/blob/main/H.Core/Enumerations/CropType.cs |
| Year In Perennial Stand | 0| Int | yes | no | Each year of a perennial stand must have the year identified in the row of the input file. E.g. a six year perennial stand would have one row with this value set 1 for the first year, 2 for the second year, etc |   |
| Perennial Stand ID | 00000000-0000-0000-0000-000000000000 | GUID | yes | no | Used to group all years of a perennial stand together. Each year in a distinct perennial stand must have this value set. All years in the same perennial stand must have this same ID/value. Can be thought of as a 'group' ID |   |
| Perennial Stand Length | 1| Int | ? | 1| Indicates how long the perennial is grown |   |
| Biomass Coefficient Product | 0.244 | Double | Default assigned but user can override | Yes | Rp Product | https://github.com/holos-aafc/Holos/blob/main/H.Content/Resources/Table_7_Relative_Biomass_Information.csv |
| Biomass Coefficient Straw | 0.518 | Double | Default assigned but user can override | Yes | Rs Straw | https://github.com/holos-aafc/Holos/blob/main/H.Content/Resources/Table_7_Relative_Biomass_Information.csv |
| Biomass Coefficient Roots | 0.147 | Double | Default assigned but user can override | Yes | Rr Root | https://github.com/holos-aafc/Holos/blob/main/H.Content/Resources/Table_7_Relative_Biomass_Information.csv |
| Biomass Coefficient Extraroot | 0.091 | Double | Default assigned but user can override | Yes | Re Extra-root | https://github.com/holos-aafc/Holos/blob/main/H.Content/Resources/Table_7_Relative_Biomass_Information.csv |
| Nitrogen Content In Product | 0.0263 | Double | Default assigned but user can override | Yes | Np Product | https://github.com/holos-aafc/Holos/blob/main/H.Content/Resources/Table_7_Relative_Biomass_Information.csv |
| Nitrogen Content In Straw | 0.0082 | Double | Default assigned but user can override | Yes | Ns Straw | https://github.com/holos-aafc/Holos/blob/main/H.Content/Resources/Table_7_Relative_Biomass_Information.csv |
| Nitrogen Content In Roots | 0.0104 | Double | Default assigned but user can override | Yes | Nr Root | https://github.com/holos-aafc/Holos/blob/main/H.Content/Resources/Table_7_Relative_Biomass_Information.csv |
| Nitrogen Content In Extraroot | 0.0104 | Double | Default assigned but user can override | Yes | Ne Extra-root | https://github.com/holos-aafc/Holos/blob/main/H.Content/Resources/Table_7_Relative_Biomass_Information.csv |
| Nitrogen Fixation | 0| Double | Default assigned but user can override | Yes | Indexed by crop type | https://github.com/holos-aafc/Holos/blob/main/H.Core/Providers/Nitrogen/NitogenFixationProvider.cs |
| Nitrogen Deposit | 5| Double | Default assigned but user can override | 5| Common value for all crop types. Page 74 in algorithm document | https://github.com/holos-aafc/Holos/raw/refs/heads/main/AAFC_Technical_Report_Holos_V4.0_Algorithm_Document_DRAFTVersion_18Nov2024.docx |
| Carbon Concentration | 0.45 | Double | Default assigned but user can override | 0.45 | Common value for all crop types. Page 37 in algorithm document | https://github.com/holos-aafc/Holos/raw/refs/heads/main/AAFC_Technical_Report_Holos_V4.0_Algorithm_Document_DRAFTVersion_18Nov2024.docx |
| Yield | 2700| Double | Default assigned but user can override | Yes | Look up value from Small Area Database | https://github.com/holos-aafc/Holos/blob/main/H.Content/Resources/small_area_yields.csv |
| Harvest Method | CashCrop | Enum | Default assigned but user can override | Yes | Depends on crop type. Line 19 in source | https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Crops/CropInitializationService.Harvest.cs |
| Nitrogen Fertilizer Rate | 87.7608533333333 | Double | Default assigned but user can override | Yes | Calculated based on yield. Line 17 in source | https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Crops/CropInitializationService.Fertilizer.cs |
| Phosphorous Fertilizer Rate | 0|   | x |   | Not used/implemented yet. Future version will utilize |   |
| Is Irrigated | No | Enum | x | No | Not used/implemented yet. Future version will utilize |   |
| Irrigation Type | RainFed | Enum | x | Yes | Used to lookup values in Table 7. Line 1290 in source | https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/LandManagement/Fields/CropViewItem.cs |
| Amount Of Irrigation | 200| Double | Default assigned but user can override | Yes | Line 35 | https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Crops/CropInitializationService.Water.cs |
| Moisture Content Of Crop | 0.12 | Double | Default assigned but user can override | Yes | Look up value by crop type and irrigation amount. Additional logic in source file on line 60 | https://github.com/holos-aafc/Holos/blob/main/H.Content/Resources/Table_7_Relative_Biomass_Information.csv, https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Crops/CropInitializationService.Water.cs |
| Moisture Content Of Crop Percentage | 12| Double | Default assigned but user can override | Yes | Look up value by crop type and irrigation amount. Additional logic in source file on line 61 | https://github.com/holos-aafc/Holos/blob/main/H.Content/Resources/Table_7_Relative_Biomass_Information.csv, https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Crops/CropInitializationService.Water.cs |
| PercentageOfStrawReturnedToSoil | 100| Double | Default assigned but user can override | Yes | Line 35 in source. Page 36 in algorithm document has references | https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Crops/CropInitializationService.Returns.cs, https://github.com/holos-aafc/Holos/raw/refs/heads/main/AAFC_Technical_Report_Holos_V4.0_Algorithm_Document_DRAFTVersion_18Nov2024.docx |
| PercentageOfRootsReturnedToSoil | 100| Double | Default assigned but user can override | Yes | Line 35 in source. Page 36 in algorithm document has references | https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Crops/CropInitializationService.Returns.cs, https://github.com/holos-aafc/Holos/raw/refs/heads/main/AAFC_Technical_Report_Holos_V4.0_Algorithm_Document_DRAFTVersion_18Nov2024.docx |
| PercentageOfProductYieldReturnedToSoil | 2| Double | Default assigned but user can override | Yes | Line 35 in source. Page 36 in algorithm document has references | https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Crops/CropInitializationService.Returns.cs, https://github.com/holos-aafc/Holos/raw/refs/heads/main/AAFC_Technical_Report_Holos_V4.0_Algorithm_Document_DRAFTVersion_18Nov2024.docx |
| Is Pesticide Used | No | Bool | x |   | Should be set if "Number of Pesticide Passes" > 0 |   |
| Number Of Pesticide Passes | 0| Int | Yes | No | Any value > 0 |   |
| Manure Applied | False | Bool | Yes | Yes | Should be set to true if any manure application/amount has been applied to field |   |
| Amount Of Manure Applied | 0| Double | Yes | No | Amount of manure applied to field (kg/ha) |   |
| Manure Application Type | NotSelected | Enum | Yes | No | See page 201 in algorithm document and table 43 line 113 | https://github.com/holos-aafc/Holos/blob/main/H.Core/Enumerations/ManureApplicationTypes.cs, https://github.com/holos-aafc/Holos/blob/main/H.Core/Providers/Animals/Table_43_Beef_Dairy_Default_Emission_Factors_Provider.cs |
| Manure Animal Source Type | NotSelected | Enum | Yes | No | Used for various table lookups | https://github.com/holos-aafc/Holos/blob/main/H.Core/Enumerations/ManureAnimalSourceTypes.cs |
| Manure State Type | NotSelected | Enum | Yes | No | Used for various table lookups | https://github.com/holos-aafc/Holos/blob/main/H.Core/Enumerations/ManureStateType.cs |
| Manure Location Source Type | NotSelected | Enum | Default assigned but user can override | "Livestock" | Used to inidcate if manure was source from on farm or imported onto farm | https://github.com/holos-aafc/Holos/blob/main/H.Core/Enumerations/ManureLocationSourceType.cs |
| Under Sown Crops Used | False | Bool | Default assigned but user can override | Yes | See notes in source file on line 449 | https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/LandManagement/Fields/CropViewItem.cs |
| Crop Is Grazed | False |   | x |   | Not used/implemented yet. Future version will utilize |   |
| Field System Component Guid | 642a2cb7-0321-4395-9ebb-d5743c27c960 | GUID | No | No | Unique ID for each field component on the farm |   |
| Time Period Category String | Current | Enum | No | "Current" | Used to indicate time period in field history. Leave as "Current" if not sure | https://github.com/holos-aafc/Holos/blob/main/H.Core/Enumerations/TimePeriodCategory.cs |
| Climate Parameter | 1.363 |   |   | 0/Calculated | Used with the ICBM C model. Will be generated when the user imports GUI farm files (0 otherwise). Can be set when creating new (blank) CLI input file(s) |   |
| Tillage Factor | 0.8 | Double | Default assigned but user can override | 0/Calculated | Used with the ICBM C model. Will be generated when the user imports GUI farm files (0 otherwise). Can be set when creating new (blank) CLI input file(s). See section 2.1.1.2 in algorithm document | https://github.com/holos-aafc/Holos/raw/refs/heads/main/AAFC_Technical_Report_Holos_V4.0_Algorithm_Document_DRAFTVersion_18Nov2024.docx |
| Management Factor | 1.09 | Double | Default assigned but user can override | 0/Calculated | Used with the ICBM C model. Will be generated when the user imports GUI farm files (0 otherwise). Can be set when creating new (blank) CLI input file(s). See section 2.1.1.2 in algorithm document | https://github.com/holos-aafc/Holos/raw/refs/heads/main/AAFC_Technical_Report_Holos_V4.0_Algorithm_Document_DRAFTVersion_18Nov2024.docx |
| Plant Carbon In Agricultural Product | 1211.76 |   |   |   | Deprecated. Do not use. Will be removed in future version |   |
| Carbon Input From Product | 24.2352 |   |   |   | Deprecated. Do not use. Will be removed in future version |   |
| Carbon Input From Straw | 2572.5068852459 |   |   |   | Deprecated. Do not use. Will be removed in future version |   |
| Carbon Input From Roots | 730.035737704918 |   |   |   | Deprecated. Do not use. Will be removed in future version |   |
| Carbon Input From Extraroots | 451.926885245902 |   |   |   | Deprecated. Do not use. Will be removed in future version |   |
| Size Of First Rotation For Field | 1|   | x |   | Deprecated. Do not use. Will be removed in future version |   |
| Above Ground Carbon Input | 1650.63391153725 |   |   |   | Deprecated. Do not use. Will be removed in future version |   |
| Below Ground Carbon Input | 601.948372142029 |   |   |   | Deprecated. Do not use. Will be removed in future version |   |
| Manure Carbon Inputs Per Hectare | 0|   |   |   | Deprecated. Do not use. Will be removed in future version |   |
| Digestate Carbon Inputs Per Hectare | 0|   |   |   | Deprecated. Do not use. Will be removed in future version |   |
| Total Carbon Inputs | 2845.39665493676 |   |   |   | Deprecated. Do not use. Will be removed in future version |   |
| Sand | 0.2 |   |   |   | Deprecated. Do not use. Will be removed in future version |   |
| Lignin | 0.053 |   |   |   | Deprecated. Do not use. Will be removed in future version |   |
| WFac | 0|   |   |   | Deprecated. Do not use. Will be removed in future version |   |
| TFac | 0|   |   |   | Deprecated. Do not use. Will be removed in future version |   |
| Total Nitrogen Inputs For Ipcc Tier 2 | 0|   |   |   | Deprecated. Do not use. Will be removed in future version |   |
| Nitrogen Content | 0.007 |   |   |   | Deprecated. Do not use. Will be removed in future version |   |
| Above Ground Residue Dry Matter | 3930.08074175535 |   |   |   | Deprecated. Do not use. Will be removed in future version |   |
| Below Ground Residue Dry Matter | 25797.7873775155 |   |   |   | Deprecated. Do not use. Will be removed in future version |   |
| Fuel Energy | 2.39 | Double | yes | no | See table 50 and section 6 in algorithm document | https://github.com/holos-aafc/Holos/blob/main/H.Content/Resources/Table_50_Fuel_Energy_Requirement_Estimates_By_Region.csv, https://github.com/holos-aafc/Holos/raw/refs/heads/main/AAFC_Technical_Report_Holos_V4.0_Algorithm_Document_DRAFTVersion_18Nov2024.docx |
| Herbicide Energy | 0.23 |   |   | no | See table 51 and section 6 in algorithm document | https://github.com/holos-aafc/Holos/blob/main/H.Content/Resources/Table_51_Herbicide_Energy_Requirement_Estimates_By_Region.csv, https://github.com/holos-aafc/Holos/raw/refs/heads/main/AAFC_Technical_Report_Holos_V4.0_Algorithm_Document_DRAFTVersion_18Nov2024.docx |
| Fertilizer Blend | Urea | Enum | yes | no | See GUI for supported list of blends | https://github.com/holos-aafc/Holos/blob/main/H.Core/Enumerations/FertilizerBlends.cs |

## Dairy
| Parameter | Example value | Type value | Is user provided | has default ? | Source (code, csv?, etc.) | note |
|---|---|---|---|---|---|---|
| Name |   |   | x |   |   |   |
| Component Type |   |   | x |   |   |   |
| Group Name |   |   | x |   |   |   |
| Group Type |   |   | x |   |   |   |
| Management Period Name |   |   | x |   |   |   |
| Management Period Start Date |   |   | x |   |   |   |
| Management Period Days |   |   | x |   |   |   |
| Number Of Animals |   |   | x |   |   |   |
| Production Stage |   |   | x |   |   |   |
| Number Of Young Animals |   |   | x |   |   |   |
| Group Pairing Number |   |   | x |   |   |   |
| Start Weight(kg) |   |   | x |   |   |   |
| End Weight(kg) |   |   | x |   |   |   |
| Average Daily Gain(kg) |   |   | x |   |   |   |
| Milk Production |   |   | x |   |   |   |
| Milk Fat Content |   |   | x |   |   |   |
| Milk Protein Content As Percentage |   |   | x |   |   |   |
| Diet Additive Type |   |   | x |   |   |   |
| Methane Conversion Factor Of Diet(kg CH4 (kg CH4)^-1) |   |   |   |   |   |   |
| Methane Conversion Factor Adjusted(%) |   |   |   |   |   |   |
| Feed Intake(kg head^-1 day^-1) |   |   | x |   |   |   |
| Crude Protein(kg kg^-1) |   |   | x |   |   |   |
| Ash Content Of Diet(kg kg^-1) |   |   | x |   |   |   |
| Forage(% DM) |   |   | x |   |   |   |
| TDN(% DM) |   |   | x |   |   |   |
| Starch(% DM) |   |   | x |   |   |   |
| Fat(% DM) |   |   | x |   |   |   |
| ME(MJ kg^-1) |   |   | x |   |   |   |
| NDF(% DM) |   |   | x |   |   |   |
| Volatile Solid Adjusted(kg kg^-1) |   |   |   |   |   |   |
| Nitrogen Excretion Adjusted(kg kg^-1) |   |   |   |   |   |   |
| Dietary Net Energy Concentration |   |   |   |   |   |   |
| Gain Coefficient |   |   |   |   |   |   |
| Gain Coefficient A |   |   |   |   |   |   |
| Gain Coefficient B |   |   |   |   |   |   |
| Housing Type |   |   | x |   |   |   |
| Activity Coefficient Of Feeding Situation(MJ day^-1 kg^-1) |   |   |   |   |   |   |
| Maintenance Coefficient(MJ day^-1 kg^-1) |   |   |   |   |   |   |
| User Defined Bedding Rate |   |   | x |   |   |   |
| Total Carbon Kilograms Dry Matter For Bedding |   |   |   |   |   |   |
| Total Nitrogen Kilograms Dry Matter For Bedding |   |   |   |   |   |   |
| Moisture Content Of Bedding Material |   |   | x |   |   |   |
| Methane Conversion Factor Of Manure(kg CH4 (kg CH4)^-1) |   |   |   |   |   |   |
| N2O Direct Emission Factor(kg N2O-N (kg N)^-1) |   |   |   |   |   |   |
| Emission Factor Volatilization |   |   |   |   |   |   |
| Volatilization Fraction |   |   |   |   |   |   |
| Emission Factor Leaching |   |   |   |   |   |   |
| Fraction Leaching |   |   |   |   |   |   |
| Ash Content(%) |   |   |   |   |   |   |
| Methane Producing Capacity Of Manure |   |   |   |   |   |   |

## Beef
| Parameter | Example value | Type Value | Is user provided | has default? | note | Source (code, csv?, etc.) |
|---|---|---|---|---|---|---|
| Name | Beef Stockers & Backgrounders | str (from enum) | x |   | Should be unique string differentiate from other components/input files |   |
| Component Type | H.Core.Models.Animals.Beef.BackgroundingComponent | str (from enum) | x |   |   | https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/ComponentType.cs |
| Group Name | Heifers | str (from enum) | x |   | Must be unique string differentiate from other animal groups in the same component (e.g. "Bulls group #1") |   |
| Group Type | BeefBackgrounderHeifer | str (from enum) | x |   | See converter class used to convert animal type string names to enumeration values | https://github.com/holos-aafc/Holos/blob/main/H.Core/Enumerations/AnimalType.cs, https://github.com/holos-aafc/Holos/blob/main/H.Core/Converters/AnimalTypeStringConverter.cs |
| Management Period Name | Management period #1 | str | x |   | Must be a unique string within the animal group |   |
| Group Pairing Number | 0| int | x |   | Used to group a parent and child group of animals. E.g. a group of lactating cows and a group of beef calves must have the same pairing number. Leave as zero when a parent/child grouping does not exist (most cases). See unit test class for example on setting this value | https://github.com/holos-aafc/Holos/blob/main/H.Core.Test/Models/Animals/Beef/CowCalfComponentTest.cs |
| Management Period Start Date | 2023-10-01 00:00:00 | strftime | x |   | Must be set to indicate the start of management period |   |
| Management Period Days | 110| int | x |   | Must be set to indicate how long the management period lasts |   |
| Number Of Animals | 100| int | x |   | Number of animals in the animal group |   |
| Production Stage | Gestating | str (from enum) | x |   | Must be set to indicate which stage a group of animals are in the lifecycle of the animal group (i.e. Lactating cows will be at the lactating production stage). This is not used for all animal types | https://github.com/holos-aafc/Holos/blob/main/H.Core/Enumerations/ProductionStages.cs |
| Number Of Young Animals | 0| int | x |   | Used to indicate how many young animals (i.e. beef calves) are associated with a parent group. See line 208 of source file on how to use | https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Animals/AnimalComponentBase.cs |
| Animals Are Milk Fed Only | False | bool | x |   | Use to specify that a group of animals are on a milk diet only. Used when referring to a group of young animals that are suckling/nursing |   |
| Start Weight | 240| float | x |   | Start weight of the animals in a particular management period | https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Animals/ManagementPeriod.cs |
| End Weight | 361| float | x |   | End weight of the animals in a particular management period | https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Animals/ManagementPeriod.cs |
| Average Daily Gain | 1.1 | float | x |   | This will be a value that is calculated based on the start and end weight |   |
| Milk Production | 0| float | x |   | The amount of milk produced by the group of animals | https://github.com/holos-aafc/Holos/blob/main/H.Content/Resources/Table_21_Average_Milk_Production_For_Dairy_Cows_By_Province.csv, https://github.com/holos-aafc/Holos/blob/main/H.Core/Providers/Animals/Table_21_Average_Milk_Production_Dairy_Cows_Provider.cs |
| Milk Fat Content | 4| float | x | 3.71 | Used with dairy components. Old default value was 4but has been changed |   |
| Milk Protein Content As Percentage | 3.5 | float | x | y | Deprecated. Do not use. Will be removed in future version |   |
| Diet Additive Type |   | Enum | x | No | Optional input used to calculate enteric CH4. See GUI for supported types | https://github.com/holos-aafc/Holos/blob/main/H.Core/Enumerations/DietAdditiveType.cs |
| Methane Conversion Factor Of Diet | 0.063 | float |   | Yes, based on diet | Also known as Ym of diet. See source file for defaults based on diet type | https://github.com/holos-aafc/Holos/blob/main/H.Core/Providers/Feed/DietProvider.cs |
| Methane Conversion Factor Adjusted | 0| float |   |   | Deprecated. Do not use. Will be removed in future version |   |
| Feed Intake | 0| float | x |   | Used with some animal types (i.e. swine types). See swine diets in source file for defaults | https://github.com/holos-aafc/Holos/blob/main/H.Core/Providers/Feed/DietProvider.cs |
| Crude Protein | 12.28 | float | x |   | Crude protein value of diet. See feed ingredient list for values | https://github.com/holos-aafc/Holos/blob/main/H.Content/Resources/feeds.csv |
| Forage | 65| float | x |   | Forage value of diet. See feed ingredient list for values | https://github.com/holos-aafc/Holos/blob/main/H.Content/Resources/feeds.csv |
| TDN | 68.825 | float | x |   | TDN value of diet. See feed ingredient list tdn values | https://github.com/holos-aafc/Holos/blob/main/H.Content/Resources/feeds.csv |
| Ash Content Of Diet | 6.57 | float | x |   | Ash content of diet. See line 434 for more informtation on how to calculate averages | https://github.com/holos-aafc/Holos/blob/main/H.Core/Providers/Feed/Diet.cs |
| Starch | 25.825 | float | x |   | Starch content of diet | https://github.com/holos-aafc/Holos/blob/main/H.Content/Resources/feeds.csv |
| Fat | 3.045 | float | x |   | Fat content of diet | https://github.com/holos-aafc/Holos/blob/main/H.Content/Resources/feeds.csv |
| ME | 2.48 | float | x |   | Metabolizable energy of diet | https://github.com/holos-aafc/Holos/blob/main/H.Content/Resources/feeds.csv |
| NDF | 42.025 | float | x |   | Neutral detergent fibre of diet | https://github.com/holos-aafc/Holos/blob/main/H.Content/Resources/feeds.csv |
| Dietary Net Energy Concentration | 0| float | x |   | Used only for diet/DMI calculations for beef calves. See line 419 | https://github.com/holos-aafc/Holos/blob/main/H.Core/Providers/Feed/Diet.cs |
| Housing Type | ConfinedNoBarn | str (from enum) | x |   | Required field used for many calculations. See GUI for correct types when considering a particular animal type | https://github.com/holos-aafc/Holos/blob/main/H.Core/Enumerations/HousingType.cs |
| Gain Coefficient | 1| float |   |   | See line 134 for default setting | https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Animals/AnimalInitializationService.EmissionFactors.cs |
| User Defined Bedding Rate | 1.5 | float | x |   | Amount of bedding added. Used in C and N input calculations. See line 52 in source and table 30 | https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Animals/AnimalInitializationService.Bedding.cs, https://github.com/holos-aafc/Holos/blob/main/H.Core/Providers/Animals/Table_30_Default_Bedding_Material_Composition_Provider.cs |
| Total Carbon Kilograms Dry Matter For Bedding | 0.447 | float |   |   | See HousingDetails.cs line 186 | https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Animals/HousingDetails.cs |
| Total Nitrogen Kilograms Dry Matter For Bedding | 0.0057 | float |   |   | See HousingDetails.cs line 177 | https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Animals/HousingDetails.cs |
| Moisture Content Of Bedding Material | 9.57 | float |   |   | See HousingDetails.cs line 219 | https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Animals/HousingDetails.cs |
| Activity Coefficient Of Feeding Situation | 0| float |   |   | See line 74 for defaults | https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Animals/AnimalInitializationService.EmissionFactors.cs |
| Maintenance Coefficient | 0.322 | float |   |   | See line 108 for defaults | https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Animals/AnimalInitializationService.EmissionFactors.cs |
| Methane Conversion Factor Of Manure | 0.26 | float |   |   | Methane conversion factor of manure, not to be consufed with Methane conversion factor of diet (Ym). See line 89 for defaults | https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Animals/AnimalInitializationService.Methane.cs |
| N2O Direct Emission Factor | 0.01 | float |   |   | See line 34 for defaults | https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Animals/AnimalInitializationService.EmissionFactors.cs |
| Emission Factor Volatilization | 0.005 | float |   |   | See line 34 for defaults | https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Animals/AnimalInitializationService.EmissionFactors.cs |
| Volatilization Fraction | 0.25 | float |   |   | See line 34 for defaults | https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Animals/AnimalInitializationService.EmissionFactors.cs |
| Emission Factor Leaching | 0.011 | float |   |   | See line 34 for defaults | https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Animals/AnimalInitializationService.EmissionFactors.cs |
| Fraction Leaching | 0.035 | float |   |   | See line 55 for defaults | https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Animals/AnimalInitializationService.Ammonia.cs |
| Ash Content | 8| float |   |   | Deprecated. Do not use. Will be removed in future version |   |
| Methane Producing Capacity Of Manure | 0.19 | float |   |   | Also known as Bo. See line 89 for defaults | https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Animals/AnimalInitializationService.Methane.cs |
| Fraction Of Organic Nitrogen Immobilized | 0| float |   |   | See line 31 for defaults | https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Animals/AnimalInitializationService.Ammonia.cs |
| Fraction Of Organic Nitrogen Nitrified | 0.125 | float |   |   | See line 31 for defaults | https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Animals/AnimalInitializationService.Ammonia.cs |
| Fraction Of Organic Nitrogen Mineralized | 0.28 | float |   |   | See line 31 for defaults | https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Animals/AnimalInitializationService.Ammonia.cs |
| Manure State Type | DeepBedding | str (from enum) | x |   | Required. See GUI for valid types for particular animal type | https://github.com/holos-aafc/Holos/blob/main/H.Core/Enumerations/ManureStateType.cs |
| Ammonia Emission Factor For Manure Storage | 0.35 | float |   |   | For poultry animals only. See line 7 in source | https://github.com/holos-aafc/Holos/blob/main/H.Core/Providers/Animals/DefaultAmmoniaEmissionFactorsForPoultryManureStorageProvider.cs |

## Sheep
| Parameter | Example value | Value Type | Is user provided | has default? | Source (code, csv?, etc.) | note |
|---|---|---|---|---|---|---|
| Name |   | str (from enum) | x |   |   |   |
| Component Type |   | str (from enum) |   |   |   |   |
| Group Name |   | str (from enum) | x |   |   |   |
| Group Type |   | str (from enum) | x |   |   |   |
| Management Period Name |   | str | x |   |   |   |
| Group Pairing Number |   | int | x |   |   |   |
| Management Period Start Date |   | strftime | x |   |   |   |
| Management Period Days |   | int | x |   |   |   |
| Number Of Animals |   | int | x |   |   |   |
| Production Stage |   | str (from enum) | x |   |   |   |
| Number Of Young Animals |   | int | x |   |   |   |
| Start Weight(kg) |   | float | x |   |   |   |
| End Weight(kg) |   | float | x |   |   |   |
| Average Daily Gain(kg) |   | float | x |   |   |   |
| Energy Required To Produce Wool(MJ kg^-1) |   | float |   |   |   |   |
| Wool Production(kg year^-1) |   | float |   |   |   |   |
| Energy Required To Produce Milk(MJ kg^-1) |   | float |   |   |   |   |
| Diet Additive Type |   | str (from enum) | x |   |   |   |
| Methane Conversion Factor Of Diet(kg CH4 (kg CH4)^-1) |   | float |   |   |   |   |
| Methane Conversion Factor Adjusted(%) |   | float |   |   |   |   |
| Feed Intake(kg head^-1 day^-1) |   | float | x |   |   |   |
| Crude Protein(kg kg^-1) |   | float | x |   |   |   |
| Forage(% DM) |   | float | x |   |   |   |
| TDN(% DM) |   | float | x |   |   |   |
| Ash Content Of Diet(% DM) |   | float | x |   |   |   |
| Starch(% DM) |   | float | x |   |   |   |
| Fat(% DM) |   | float | x |   |   |   |
| ME(MJ kg^-1) |   | float | x |   |   |   |
| NDF(% DM) |   | float | x |   |   |   |
| Gain Coefficient A |   | float |   |   |   |   |
| Gain Coefficient B |   | float |   |   |   |   |
| Activity Coefficient Of Feeding Situation(MJ day^-1 kg^-1) |   | float |   |   |   |   |
| Maintenance Coefficient(MJ day^-1 kg^-1) |   | float |   |   |   |   |
| User Defined Bedding Rate |   | float | x |   |   |   |
| Total Carbon Kilograms Dry Matter For Bedding |   | float |   |   |   |   |
| Total Nitrogen Kilograms Dry Matter For Bedding |   | float |   |   |   |   |
| Moisture Content Of Bedding Material |   | float |   |   |   |   |
| Methane Conversion Factor Of Manure(kg CH4 (kg CH4)^-1) |   | float |   |   |   |   |
| N2O Direct Emission Factor(kg N2O-N (kg N)^-1) |   | float |   |   |   |   |
| Emission Factor Volatilization |   | float |   |   |   |   |
| Volatilization Fraction |   | float |   |   |   |   |
| Emission Factor Leaching |   | float |   |   |   |   |
| Fraction Leaching |   | float |   |   |   |   |
| Ash Content(%) |   | float |   |   |   |   |
| Methane Producing Capacity Of Manure |   | float |   |   |   |   |
| Manure Excretion Rate |   | float | x |   |   |   |
| Fraction Of Carbon In Manure |   | float |   |   |   |   |

## Farm Setting
| Parameter | Example value | Value type | Is user provided | has default ? | note | Source (code, csv?, etc.) |
|---|---|---|---|---|---|---|
| Polygon ID:  | 851003| int | x | N | Required. Use GUI map view to get polygon number if needed |   |
| Yield Assignment Method =  | SmallAreaData | enum |   | SmallAreaData | Used to lookup default yields for a particular year and crop type | https://github.com/holos-aafc/Holos/blob/main/H.Core/Enumerations/YieldAssignmentMethod.cs |
| Polygon Number =  | 851003| int | x |   | Required. Use GUI map view to get polygon number if needed |   |
| Latitude =  | 49.9805772869656 | float | x |   | Required if using NASA as the source for climate data. Values are determined when user clicks on a location in the GUI |   |
| Longitude =  | -98.0433082580568 | float | x |   | Required if using NASA as the source for climate data. Values are determined when user clicks on a location in the GUI |   |
| Carbon Concentration  (kg kg^-1) = | 0.45 | float |   | 0.4 | See line 90 in source | https://github.com/holos-aafc/Holos/blob/main/H.Core/CoreConstants.cs |
| Emergence Day =  | 141| int | is used ? |   | Used with ICBM carbon model. See line 167 for default | https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Defaults.cs |
| Ripening Day =  | 197| int | is used ? |   | Used with ICBM carbon model. See line 168 for default | https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Defaults.cs |
| Variance =  | 300| float? |   |   | Used with ICBM carbon model. See line 169 for default | https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Defaults.cs |
| Alfa = | 0.7 | float |   |   | Used with ICBM carbon model. See line 175 for default | https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Defaults.cs |
| Decomposition Minimum Temperature  (�C) = -3.78 | -3.78 | float |   |   | Used with ICBM carbon model. See line 176 for default | https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Defaults.cs |
| Decomposition Maximum Temperature  (�C) = | 30| float |   |   | Used with ICBM carbon model. See line 177 for default | https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Defaults.cs |
| Moisture Response Function At Saturation =  | 0.42 | float |   |   | Used with ICBM carbon model. See line 178 for default | https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Defaults.cs |
| Moisture Response Function At Wilting Point =  | 0.18 | float |   |   | Used with ICBM carbon model. See line 179 for default | https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Defaults.cs |
| Percentage Of Product Returned To Soil For Annuals = 2 | 2| float |   |   | Used as a global default. See line 24 on how the default is used. User can override | https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Crops/CropInitializationService.Returns.cs |
| Percentage Of Straw Returned To Soil For Annuals = 100 | 100| float |   |   | Used as a global default. See line 24 on how the default is used. User can override | https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Crops/CropInitializationService.Returns.cs |
| Percentage Of Roots Returned To Soil For Annuals = 100 | 100| float |   |   | Used as a global default. See line 24 on how the default is used. User can override | https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Crops/CropInitializationService.Returns.cs |
| Percentage Of Product Yield Returned To Soil For Silage Crops = 35 | 35| float |   |   | Used as a global default. See line 24 on how the default is used. User can override | https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Crops/CropInitializationService.Returns.cs |
| Percentage Of Roots Returned To Soil For Silage Crops = 100 | 100| float |   |   | Used as a global default. See line 24 on how the default is used. User can override | https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Crops/CropInitializationService.Returns.cs |
| Percentage Of Product Yield Returned To Soil For Cover Crops = 100 | 100| float |   |   | Used as a global default. See line 24 on how the default is used. User can override | https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Crops/CropInitializationService.Returns.cs |
| Percentage Of Product Yield Returned To Soil For Cover Crops Forage = 35 | 35| float |   |   | Used as a global default. See line 24 on how the default is used. User can override | https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Crops/CropInitializationService.Returns.cs |
| Percentage Of Product Yield Returned To Soil For Cover Crops Produce = 0 | 0| float |   |   | Used as a global default. See line 24 on how the default is used. User can override | https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Crops/CropInitializationService.Returns.cs |
| Percentage Of Straw Returned To Soil For Cover Crops = 100 | 100| float |   |   | Used as a global default. See line 24 on how the default is used. User can override | https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Crops/CropInitializationService.Returns.cs |
| Percentage Of Roots Returned To Soil For Cover Crops = 100 | 100| float |   |   | Used as a global default. See line 24 on how the default is used. User can override | https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Crops/CropInitializationService.Returns.cs |
| Percentage Of Product Returned To Soil For Root Crops = 0 | 0| float |   |   | Used as a global default. See line 24 on how the default is used. User can override | https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Crops/CropInitializationService.Returns.cs |
| Percentage Of Straw Returned To Soil For Root Crops = 100 | 100| float |   |   | Used as a global default. See line 24 on how the default is used. User can override | https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Crops/CropInitializationService.Returns.cs |
| Percentage Of Product Returned To Soil For Perennials = 35 | 35| float |   |   | Used as a global default. See line 24 on how the default is used. User can override | https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Crops/CropInitializationService.Returns.cs |
| Percentage Of Roots Returned To Soil For Perennials = 100 | 100| float |   |   | Used as a global default. See line 24 on how the default is used. User can override | https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Crops/CropInitializationService.Returns.cs |
| Percentage Of Product Returned To Soil For Rangeland Due To Harvest Loss = 35 | 35| float |   |   | Used as a global default. See line 24 on how the default is used. User can override | https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Crops/CropInitializationService.Returns.cs |
| Percentage Of Roots Returned To Soil For Rangeland = 100 | 100| float |   |   | Used as a global default. See line 24 on how the default is used. User can override | https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Crops/CropInitializationService.Returns.cs |
| Percentage Of Product Returned To Soil For Fodder Corn = 35 | 35| float |   |   | Used as a global default. See line 24 on how the default is used. User can override | https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Crops/CropInitializationService.Returns.cs |
| Percentage Of Roots Returned To Soil For Fodder Corn = 100 | 100| float |   |   | Used as a global default. See line 24 on how the default is used. User can override | https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Crops/CropInitializationService.Returns.cs |
| Decomposition Rate Constant Young Pool = 0.8 | 0.8 | float |   |   | Used with ICBM carbon model. See line 221 for default | https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Defaults.cs |
| Decomposition Rate Constant Old Pool = | 0.00605 | float |   |   | Used with ICBM carbon model. See line 222 for default | https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Defaults.cs |
| Old Pool Carbon N = | 0.1 | float |   |   | Used with both ICBM and IPCC Tier 2 carbon models. See line 224 | https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Defaults.cs |
| NO Ratio = | 0.1 | float |   |   | Used with both ICBM and IPCC Tier 2 carbon models. See line 227 | https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Defaults.cs |
| Emission Factor For Leaching And Runoff  (kg N2O-N (kg N)^-1) =  | 0.011 | float |   |   | Used for N2O calculations. See line 228  | https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Defaults.cs |
| Emission Factor For Volatilization  (kg N2O-N (kg N)^-1) =  | 0.01 | float |   |   | Used for N2O calculations. See line 229 | https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Defaults.cs |
| Fraction Of N Lost By Volatilization =  | 0.21 | float |   |   | Used for N2O calculations. See line 232 | https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Defaults.cs |
| Microbe Death =  | 0.2 | float |   |   | Used by both ICBM and IPCC Tier carbon models. See line 236 | https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Defaults.cs |
| Denitrification =  | 0.5 | float |   |   | Used by both ICBM and IPCC Tier carbon models. See line 237 | https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Defaults.cs |
| Carbon modelling strategy = |  IPCCTier2 | enum |   |   | Will determine which carbon model will be used (IPCC Tier 2 is the newest C model and is the new default model) | https://github.com/holos-aafc/Holos/blob/main/H.Core/Enumerations/CarbonModellingStrategies.cs |
| Run In Period Years =  | 15| int | x |   | Used to indicate how many years will be used when calculating equilibrium soil carbon. Can leave default in most cases. See line 269 | https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Defaults.cs |
| Humification Coefficient Above Ground =  | 0.125 | float |   |   | Used with ICBM carbon model. See line 217 for default | https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Defaults.cs |
| Humification Coefficient Below Ground =  | 0.3 | float |   |   | Used with ICBM carbon model. See line 218 for default | https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Defaults.cs |
| Humification Coefficient Manure =  | 0.31 | float |   |   | Used with ICBM carbon model. See line 219 for default | https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Defaults.cs |
| Climate filename = climate.csv | climate.csv |   | x |   | Used when climate acquisition is set to "InputFile" |   |
| Climate Data Acquisition =  | NASA | enum |   |   | Specifies how Holos will aquire climate data. See source file for more details | https://github.com/holos-aafc/Holos/blob/main/H.Core/Providers/Climate/ClimateProvider.cs |
| Use climate parameter instead of management factor =  | True | bool |   |   | Set to true for most scenarios |   |
| Enable Carbon Modelling =  | True | bool |   |   | Set to true for most scenarios |   |
| January Precipitation =  | 17.6213 | float | not used |   | Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else. See line 37 | https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs |
| February Precipitation =  | 12.8316 | float | not used |   | Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else | https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs |
| March Precipitation =  | 22.426 | float | not used |   | Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else | https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs |
| April Precipitation =  | 27.4144 | float | not used |   | Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else | https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs |
| May Precipitation =  | 61.5015 | float | not used |   | Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else | https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs |
| June Precipitation =  | 77.9022 | float | not used |   | Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else | https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs |
| July Precipitation =  | 57.274 | float | not used |   | Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else | https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs |
| August Precipitation =  | 53.0356 | float | not used |   | Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else | https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs |
| September Precipitation =  | 40.4796 | float | not used |   | Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else | https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs |
| October Precipitation =  | 33.7571 | float | not used |   | Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else | https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs |
| November Precipitation =  | 23.0151 | float | not used |   | Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else | https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs |
| December Precipitation =  | 21.4046 | float | not used |   | Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else | https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs |
| January Potential Evapotranspiration =  | 0.0327 | float | not used |   | Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else | https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs |
| February Potential Evapotranspiration =  | 0.0888 | float | not used |   | Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else | https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs |
| March Potential Evapotranspiration =  | 3.5731 | float | not used |   | Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else | https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs |
| April Potential Evapotranspiration =  | 44.1505 | float | not used |   | Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else | https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs |
| May Potential Evapotranspiration =  | 100.0393 | float | not used |   | Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else | https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs |
| June Potential Evapotranspiration =  | 123.5476 | float | not used |   | Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else | https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs |
| July Potential Evapotranspiration =  | 135.7116 | float | not used |   | Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else | https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs |
| August Potential Evapotranspiration =  | 120.4341 | float | not used |   | Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else | https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs |
| September Potential Evapotranspiration =  | 66.0041 | float | not used |   | Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else | https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs |
| October Potential Evapotranspiration =  | 16.8898 | float | not used |   | Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else | https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs |
| November Potential Evapotranspiration =  | 0.7677 | float | not used |   | Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else | https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs |
| December Potential Evapotranspiration =  | 0.0252 | float | not used |   | Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else | https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs |
| January Mean Temperature =  | -14.8531 | float | not used |   | Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else | https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs |
| February Mean Temperature =  | -12.4063 | float | not used |   | Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else | https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs |
| March Mean Temperature =  | -5.3584 | float | not used |   | Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else | https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs |
| April Mean Temperature = | 3.7295 | float | not used |   | Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else | https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs |
| May Mean Temperature =  | 10.7967 | float | not used |   | Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else | https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs |
| June Mean Temperature =  | 16.4886 | float | not used |   | Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else | https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs |
| July Mean Temperature =  | 18.8914 | float | not used |   | Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else | https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs |
| August Mean Temperature =  | 18.2291 | float | not used |   | Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else | https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs |
| September Mean Temperature =  | 13.2652 | float | not used |   | Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else | https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs |
| October Mean Temperature =  | 5.6419 | float | not used |   | Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else | https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs |
| November Mean Temperature =  | -3.5511 | float | not used |   | Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else | https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs |
| December Mean Temperature =  | -11.9174 | float | not used |   | Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else | https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs |
| Province =  | Manitoba | enum | x |   | extracted from slc DB with lat long data |   |
| Year Of Observation = | 2024| int | x |   | Defaults to the current year |   |
| Ecodistrict ID =  | 851| int | x |   | extracted from slc DB with lat long data |   |
| Soil Great Group =  | Regosol | enum | x |   | extracted from slc DB with lat long data |   |
| Soil functional category =  | Black | enum | x |   | deduced following code in Holos |   |
| Bulk Density =  | 1.2 | float | x |   | extracted from slc DB with lat long data |   |
| Soil Texture =  | Fine | enum | x |   | deduced following code in Holos |   |
| Soil Ph =  | 7.8 | float | x |   | extracted from slc DB with lat long data |   |
| Top Layer Thickness  (mm) =  | 200| float | x |   | extracted from slc DB with lat long data |   |
| Proportion Of Sand In Soil =  | 0.2 | float | x |   | extracted from slc DB with lat long data |   |
| Proportion Of Clay In Soil =  | 0.3 | float | x |   | extracted from slc DB with lat long data |   |
| Proportion Of Soil Organic Carbon =  | 3.1 | float | x |   | extracted from slc DB with lat long data |   |