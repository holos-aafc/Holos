# Example Input Tables For Command Line Interface

This document provides example data for the user created input Excel file(s) that will be processed by the Holos CLI. Please read [here](https://github.com/holos-aafc/Holos/blob/main/H.Content/Documentation/User%20Guide/User%20Guide.md#chapter-10---command-line-interface) for more information on the Holos CLI.

<br>

# Field
## Phase Number

Example value: 0

Type value: 

Is user provided: x 

has default?: 

note: Deprecated. Do not use. Will be removed in future version 

Source (code, csv?, etc.): 

***
## Name

Example value: wheat and hairy vetch 

Type value: 

Is user provided: x 

has default?: Yes 

note: User defined name 

Source (code, csv?, etc.): 

***
## Area

Example value: 18

Type value: Double 

Is user provided: Yes 

has default?: No 

note: 

Source (code, csv?, etc.): 

***
## Current Year

Example value: 2024

Type value: Int 

Is user provided: Yes 

has default?: Current year (i.e. 2024) 

note: 

Source (code, csv?, etc.): 

***
## Crop Year

Example value: 1985

Type value: 

Is user provided: Yes 

has default?: 

note: Each (row) in input file must correspond to a certain year 

Source (code, csv?, etc.): 

***
## Crop Type

Example value: Wheat 

Type value: 

Is user provided: x 

has default?: 

note: 

Source (code, csv?, etc.): 

***
## Tillage Type

Example value: Reduced 

Type value: Enum 

Is user provided: x 

has default?: Wheat 

note: See GUI for supported list of crop types since not all items in the enum are supported in calculations 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Enumerations/CropType.cs 

***
## Year In Perennial Stand

Example value: 0

Type value: Int 

Is user provided: yes 

has default?: no 

note: Each year of a perennial stand must have the year identified in the row of the input file. E.g. a six year perennial stand would have one row with this value set 1 for the first year, 2 for the second year, etc 

Source (code, csv?, etc.): 

***
## Perennial Stand ID

Example value: 00000000-0000-0000-0000-000000000000 

Type value: GUID 

Is user provided: yes 

has default?: no 

note: Used to group all years of a perennial stand together. Each year in a distinct perennial stand must have this value set. All years in the same perennial stand must have this same ID/value. Can be thought of as a 'group' ID 

Source (code, csv?, etc.): 

***
## Perennial Stand Length

Example value: 1

Type value: Int 

Is user provided: ? 

has default?: 1

note: Indicates how long the perennial is grown 

Source (code, csv?, etc.): 

***
## Biomass Coefficient Product

Example value: 0.244 

Type value: Double 

Is user provided: Default assigned but user can override 

has default?: Yes 

note: Rp Product 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Content/Resources/Table_7_Relative_Biomass_Information.csv 

***
## Biomass Coefficient Straw

Example value: 0.518 

Type value: Double 

Is user provided: Default assigned but user can override 

has default?: Yes 

note: Rs Straw 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Content/Resources/Table_7_Relative_Biomass_Information.csv 

***
## Biomass Coefficient Roots

Example value: 0.147 

Type value: Double 

Is user provided: Default assigned but user can override 

has default?: Yes 

note: Rr Root 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Content/Resources/Table_7_Relative_Biomass_Information.csv 

***
## Biomass Coefficient Extraroot

Example value: 0.091 

Type value: Double 

Is user provided: Default assigned but user can override 

has default?: Yes 

note: Re Extra-root 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Content/Resources/Table_7_Relative_Biomass_Information.csv 

***
## Nitrogen Content In Product

Example value: 0.0263 

Type value: Double 

Is user provided: Default assigned but user can override 

has default?: Yes 

note: Np Product 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Content/Resources/Table_7_Relative_Biomass_Information.csv 

***
## Nitrogen Content In Straw

Example value: 0.0082 

Type value: Double 

Is user provided: Default assigned but user can override 

has default?: Yes 

note: Ns Straw 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Content/Resources/Table_7_Relative_Biomass_Information.csv 

***
## Nitrogen Content In Roots

Example value: 0.0104 

Type value: Double 

Is user provided: Default assigned but user can override 

has default?: Yes 

note: Nr Root 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Content/Resources/Table_7_Relative_Biomass_Information.csv 

***
## Nitrogen Content In Extraroot

Example value: 0.0104 

Type value: Double 

Is user provided: Default assigned but user can override 

has default?: Yes 

note: Ne Extra-root 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Content/Resources/Table_7_Relative_Biomass_Information.csv 

***
## Nitrogen Fixation

Example value: 0

Type value: Double 

Is user provided: Default assigned but user can override 

has default?: Yes 

note: Indexed by crop type 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Providers/Nitrogen/NitogenFixationProvider.cs 

***
## Nitrogen Deposit

Example value: 5

Type value: Double 

Is user provided: Default assigned but user can override 

has default?: 5

note: Common value for all crop types. Page 74 in algorithm document 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/raw/refs/heads/main/AAFC_Technical_Report_Holos_V4.0_Algorithm_Document_DRAFTVersion_18Nov2024.docx 

***
## Carbon Concentration

Example value: 0.45 

Type value: Double 

Is user provided: Default assigned but user can override 

has default?: 0.45 

note: Common value for all crop types. Page 37 in algorithm document 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/raw/refs/heads/main/AAFC_Technical_Report_Holos_V4.0_Algorithm_Document_DRAFTVersion_18Nov2024.docx 

***
## Yield

Example value: 2700

Type value: Double 

Is user provided: Default assigned but user can override 

has default?: Yes 

note: Look up value from Small Area Database 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Content/Resources/small_area_yields.csv 

***
## Harvest Method

Example value: CashCrop 

Type value: Enum 

Is user provided: Default assigned but user can override 

has default?: Yes 

note: Depends on crop type. Line 19 in source 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Crops/CropInitializationService.Harvest.cs 

***
## Nitrogen Fertilizer Rate

Example value: 87.7608533333333 

Type value: Double 

Is user provided: Default assigned but user can override 

has default?: Yes 

note: Calculated based on yield. Line 17 in source 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Crops/CropInitializationService.Fertilizer.cs 

***
## Phosphorous Fertilizer Rate

Example value: 0

Type value: 

Is user provided: x 

has default?: 

note: Not used/implemented yet. Future version will utilize 

Source (code, csv?, etc.): 

***
## Is Irrigated

Example value: No 

Type value: Enum 

Is user provided: x 

has default?: No 

note: Not used/implemented yet. Future version will utilize 

Source (code, csv?, etc.): 

***
## Irrigation Type

Example value: RainFed 

Type value: Enum 

Is user provided: x 

has default?: Yes 

note: Used to lookup values in Table 7. Line 1290 in source 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/LandManagement/Fields/CropViewItem.cs 

***
## Amount Of Irrigation

Example value: 200

Type value: Double 

Is user provided: Default assigned but user can override 

has default?: Yes 

note: Line 35 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Crops/CropInitializationService.Water.cs 

***
## Moisture Content Of Crop

Example value: 0.12 

Type value: Double 

Is user provided: Default assigned but user can override 

has default?: Yes 

note: Look up value by crop type and irrigation amount. Additional logic in source file on line 60 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Content/Resources/Table_7_Relative_Biomass_Information.csv, https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Crops/CropInitializationService.Water.cs 

***
## Moisture Content Of Crop Percentage

Example value: 12

Type value: Double 

Is user provided: Default assigned but user can override 

has default?: Yes 

note: Look up value by crop type and irrigation amount. Additional logic in source file on line 61 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Content/Resources/Table_7_Relative_Biomass_Information.csv, https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Crops/CropInitializationService.Water.cs 

***
## PercentageOfStrawReturnedToSoil

Example value: 100

Type value: Double 

Is user provided: Default assigned but user can override 

has default?: Yes 

note: Line 35 in source. Page 36 in algorithm document has references 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Crops/CropInitializationService.Returns.cs, https://github.com/holos-aafc/Holos/raw/refs/heads/main/AAFC_Technical_Report_Holos_V4.0_Algorithm_Document_DRAFTVersion_18Nov2024.docx 

***
## PercentageOfRootsReturnedToSoil

Example value: 100

Type value: Double 

Is user provided: Default assigned but user can override 

has default?: Yes 

note: Line 35 in source. Page 36 in algorithm document has references 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Crops/CropInitializationService.Returns.cs, https://github.com/holos-aafc/Holos/raw/refs/heads/main/AAFC_Technical_Report_Holos_V4.0_Algorithm_Document_DRAFTVersion_18Nov2024.docx 

***
## PercentageOfProductYieldReturnedToSoil

Example value: 2

Type value: Double 

Is user provided: Default assigned but user can override 

has default?: Yes 

note: Line 35 in source. Page 36 in algorithm document has references 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Crops/CropInitializationService.Returns.cs, https://github.com/holos-aafc/Holos/raw/refs/heads/main/AAFC_Technical_Report_Holos_V4.0_Algorithm_Document_DRAFTVersion_18Nov2024.docx 

***
## Is Pesticide Used

Example value: No 

Type value: Bool 

Is user provided: x 

has default?: 

note: Should be set if "Number of Pesticide Passes" > 0 

Source (code, csv?, etc.): 

***
## Number Of Pesticide Passes

Example value: 0

Type value: Int 

Is user provided: Yes 

has default?: No 

note: Any value > 0 

Source (code, csv?, etc.): 

***
## Manure Applied

Example value: False 

Type value: Bool 

Is user provided: Yes 

has default?: Yes 

note: Should be set to true if any manure application/amount has been applied to field 

Source (code, csv?, etc.): 

***
## Amount Of Manure Applied

Example value: 0

Type value: Double 

Is user provided: Yes 

has default?: No 

note: Amount of manure applied to field (kg/ha) 

Source (code, csv?, etc.): 

***
## Manure Application Type

Example value: NotSelected 

Type value: Enum 

Is user provided: Yes 

has default?: No 

note: See page 201 in algorithm document and table 43 line 113 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Enumerations/ManureApplicationTypes.cs, https://github.com/holos-aafc/Holos/blob/main/H.Core/Providers/Animals/Table_43_Beef_Dairy_Default_Emission_Factors_Provider.cs 

***
## Manure Animal Source Type

Example value: NotSelected 

Type value: Enum 

Is user provided: Yes 

has default?: No 

note: Used for various table lookups 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Enumerations/ManureAnimalSourceTypes.cs 

***
## Manure State Type

Example value: NotSelected 

Type value: Enum 

Is user provided: Yes 

has default?: No 

note: Used for various table lookups 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Enumerations/ManureStateType.cs 

***
## Manure Location Source Type

Example value: NotSelected 

Type value: Enum 

Is user provided: Default assigned but user can override 

has default?: "Livestock" 

note: Used to inidcate if manure was source from on farm or imported onto farm 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Enumerations/ManureLocationSourceType.cs 

***
## Under Sown Crops Used

Example value: False 

Type value: Bool 

Is user provided: Default assigned but user can override 

has default?: Yes 

note: See notes in source file on line 449 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/LandManagement/Fields/CropViewItem.cs 

***
## Crop Is Grazed

Example value: False 

Type value: 

Is user provided: x 

has default?: 

note: Not used/implemented yet. Future version will utilize 

Source (code, csv?, etc.): 

***
## Field System Component Guid

Example value: 642a2cb7-0321-4395-9ebb-d5743c27c960 

Type value: GUID 

Is user provided: No 

has default?: No 

note: Unique ID for each field component on the farm 

Source (code, csv?, etc.): 

***
## Time Period Category String

Example value: Current 

Type value: Enum 

Is user provided: No 

has default?: "Current" 

note: Used to indicate time period in field history. Leave as "Current" if not sure 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Enumerations/TimePeriodCategory.cs 

***
## Climate Parameter

Example value: 1.363 

Type value: 

Is user provided: 

has default?: 0/Calculated 

note: Used with the ICBM C model. Will be generated when the user imports GUI farm files (0 otherwise). Can be set when creating new (blank) CLI input file(s) 

Source (code, csv?, etc.): 

***
## Tillage Factor

Example value: 0.8 

Type value: Double 

Is user provided: Default assigned but user can override 

has default?: 0/Calculated 

note: Used with the ICBM C model. Will be generated when the user imports GUI farm files (0 otherwise). Can be set when creating new (blank) CLI input file(s). See section 2.1.1.2 in algorithm document 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/raw/refs/heads/main/AAFC_Technical_Report_Holos_V4.0_Algorithm_Document_DRAFTVersion_18Nov2024.docx 

***
## Management Factor

Example value: 1.09 

Type value: Double 

Is user provided: Default assigned but user can override 

has default?: 0/Calculated 

note: Used with the ICBM C model. Will be generated when the user imports GUI farm files (0 otherwise). Can be set when creating new (blank) CLI input file(s). See section 2.1.1.2 in algorithm document 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/raw/refs/heads/main/AAFC_Technical_Report_Holos_V4.0_Algorithm_Document_DRAFTVersion_18Nov2024.docx 

***
## Plant Carbon In Agricultural Product

Example value: 1211.76 

Type value: 

Is user provided: 

has default?: 

note: Deprecated. Do not use. Will be removed in future version 

Source (code, csv?, etc.): 

***
## Carbon Input From Product

Example value: 24.2352 

Type value: 

Is user provided: 

has default?: 

note: Deprecated. Do not use. Will be removed in future version 

Source (code, csv?, etc.): 

***
## Carbon Input From Straw

Example value: 2572.5068852459 

Type value: 

Is user provided: 

has default?: 

note: Deprecated. Do not use. Will be removed in future version 

Source (code, csv?, etc.): 

***
## Carbon Input From Roots

Example value: 730.035737704918 

Type value: 

Is user provided: 

has default?: 

note: Deprecated. Do not use. Will be removed in future version 

Source (code, csv?, etc.): 

***
## Carbon Input From Extraroots

Example value: 451.926885245902 

Type value: 

Is user provided: 

has default?: 

note: Deprecated. Do not use. Will be removed in future version 

Source (code, csv?, etc.): 

***
## Size Of First Rotation For Field

Example value: 1

Type value: 

Is user provided: x 

has default?: 

note: Deprecated. Do not use. Will be removed in future version 

Source (code, csv?, etc.): 

***
## Above Ground Carbon Input

Example value: 1650.63391153725 

Type value: 

Is user provided: 

has default?: 

note: Deprecated. Do not use. Will be removed in future version 

Source (code, csv?, etc.): 

***
## Below Ground Carbon Input

Example value: 601.948372142029 

Type value: 

Is user provided: 

has default?: 

note: Deprecated. Do not use. Will be removed in future version 

Source (code, csv?, etc.): 

***
## Manure Carbon Inputs Per Hectare

Example value: 0

Type value: 

Is user provided: 

has default?: 

note: Deprecated. Do not use. Will be removed in future version 

Source (code, csv?, etc.): 

***
## Digestate Carbon Inputs Per Hectare

Example value: 0

Type value: 

Is user provided: 

has default?: 

note: Deprecated. Do not use. Will be removed in future version 

Source (code, csv?, etc.): 

***
## Total Carbon Inputs

Example value: 2845.39665493676 

Type value: 

Is user provided: 

has default?: 

note: Deprecated. Do not use. Will be removed in future version 

Source (code, csv?, etc.): 

***
## Sand

Example value: 0.2 

Type value: 

Is user provided: 

has default?: 

note: Deprecated. Do not use. Will be removed in future version 

Source (code, csv?, etc.): 

***
## Lignin

Example value: 0.053 

Type value: 

Is user provided: 

has default?: 

note: Deprecated. Do not use. Will be removed in future version 

Source (code, csv?, etc.): 

***
## WFac

Example value: 0

Type value: 

Is user provided: 

has default?: 

note: Deprecated. Do not use. Will be removed in future version 

Source (code, csv?, etc.): 

***
## TFac

Example value: 0

Type value: 

Is user provided: 

has default?: 

note: Deprecated. Do not use. Will be removed in future version 

Source (code, csv?, etc.): 

***
## Total Nitrogen Inputs For Ipcc Tier 2

Example value: 0

Type value: 

Is user provided: 

has default?: 

note: Deprecated. Do not use. Will be removed in future version 

Source (code, csv?, etc.): 

***
## Nitrogen Content

Example value: 0.007 

Type value: 

Is user provided: 

has default?: 

note: Deprecated. Do not use. Will be removed in future version 

Source (code, csv?, etc.): 

***
## Above Ground Residue Dry Matter

Example value: 3930.08074175535 

Type value: 

Is user provided: 

has default?: 

note: Deprecated. Do not use. Will be removed in future version 

Source (code, csv?, etc.): 

***
## Below Ground Residue Dry Matter

Example value: 25797.7873775155 

Type value: 

Is user provided: 

has default?: 

note: Deprecated. Do not use. Will be removed in future version 

Source (code, csv?, etc.): 

***
## Fuel Energy

Example value: 2.39 

Type value: Double 

Is user provided: yes 

has default?: no 

note: See table 50 and section 6 in algorithm document 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Content/Resources/Table_50_Fuel_Energy_Requirement_Estimates_By_Region.csv, https://github.com/holos-aafc/Holos/raw/refs/heads/main/AAFC_Technical_Report_Holos_V4.0_Algorithm_Document_DRAFTVersion_18Nov2024.docx 

***
## Herbicide Energy

Example value: 0.23 

Type value: 

Is user provided: 

has default?: no 

note: See table 51 and section 6 in algorithm document 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Content/Resources/Table_51_Herbicide_Energy_Requirement_Estimates_By_Region.csv, https://github.com/holos-aafc/Holos/raw/refs/heads/main/AAFC_Technical_Report_Holos_V4.0_Algorithm_Document_DRAFTVersion_18Nov2024.docx 

***
## Fertilizer Blend

Example value: Urea 

Type value: Enum 

Is user provided: yes 

has default?: no 

note: See GUI for supported list of blends 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Enumerations/FertilizerBlends.cs 

***

<br>

# Dairy
## Name

Example value: 

Type value: 

Is user provided: x 

has default ?: 

Source (code, csv?, etc.): 

note: 

***
## Component Type

Example value: 

Type value: 

Is user provided: x 

has default ?: 

Source (code, csv?, etc.): 

note: 

***
## Group Name

Example value: 

Type value: 

Is user provided: x 

has default ?: 

Source (code, csv?, etc.): 

note: 

***
## Group Type

Example value: 

Type value: 

Is user provided: x 

has default ?: 

Source (code, csv?, etc.): 

note: 

***
## Management Period Name

Example value: 

Type value: 

Is user provided: x 

has default ?: 

Source (code, csv?, etc.): 

note: 

***
## Management Period Start Date

Example value: 

Type value: 

Is user provided: x 

has default ?: 

Source (code, csv?, etc.): 

note: 

***
## Management Period Days

Example value: 

Type value: 

Is user provided: x 

has default ?: 

Source (code, csv?, etc.): 

note: 

***
## Number Of Animals

Example value: 

Type value: 

Is user provided: x 

has default ?: 

Source (code, csv?, etc.): 

note: 

***
## Production Stage

Example value: 

Type value: 

Is user provided: x 

has default ?: 

Source (code, csv?, etc.): 

note: 

***
## Number Of Young Animals

Example value: 

Type value: 

Is user provided: x 

has default ?: 

Source (code, csv?, etc.): 

note: 

***
## Group Pairing Number

Example value: 

Type value: 

Is user provided: x 

has default ?: 

Source (code, csv?, etc.): 

note: 

***
## Start Weight(kg)

Example value: 

Type value: 

Is user provided: x 

has default ?: 

Source (code, csv?, etc.): 

note: 

***
## End Weight(kg)

Example value: 

Type value: 

Is user provided: x 

has default ?: 

Source (code, csv?, etc.): 

note: 

***
## Average Daily Gain(kg)

Example value: 

Type value: 

Is user provided: x 

has default ?: 

Source (code, csv?, etc.): 

note: 

***
## Milk Production

Example value: 

Type value: 

Is user provided: x 

has default ?: 

Source (code, csv?, etc.): 

note: 

***
## Milk Fat Content

Example value: 

Type value: 

Is user provided: x 

has default ?: 

Source (code, csv?, etc.): 

note: 

***
## Milk Protein Content As Percentage

Example value: 

Type value: 

Is user provided: x 

has default ?: 

Source (code, csv?, etc.): 

note: 

***
## Diet Additive Type

Example value: 

Type value: 

Is user provided: x 

has default ?: 

Source (code, csv?, etc.): 

note: 

***
## Methane Conversion Factor Of Diet(kg CH4 (kg CH4)^-1)

Example value: 

Type value: 

Is user provided: 

has default ?: 

Source (code, csv?, etc.): 

note: 

***
## Methane Conversion Factor Adjusted(%)

Example value: 

Type value: 

Is user provided: 

has default ?: 

Source (code, csv?, etc.): 

note: 

***
## Feed Intake(kg head^-1 day^-1)

Example value: 

Type value: 

Is user provided: x 

has default ?: 

Source (code, csv?, etc.): 

note: 

***
## Crude Protein(kg kg^-1)

Example value: 

Type value: 

Is user provided: x 

has default ?: 

Source (code, csv?, etc.): 

note: 

***
## Ash Content Of Diet(kg kg^-1)

Example value: 

Type value: 

Is user provided: x 

has default ?: 

Source (code, csv?, etc.): 

note: 

***
## Forage(% DM)

Example value: 

Type value: 

Is user provided: x 

has default ?: 

Source (code, csv?, etc.): 

note: 

***
## TDN(% DM)

Example value: 

Type value: 

Is user provided: x 

has default ?: 

Source (code, csv?, etc.): 

note: 

***
## Starch(% DM)

Example value: 

Type value: 

Is user provided: x 

has default ?: 

Source (code, csv?, etc.): 

note: 

***
## Fat(% DM)

Example value: 

Type value: 

Is user provided: x 

has default ?: 

Source (code, csv?, etc.): 

note: 

***
## ME(MJ kg^-1)

Example value: 

Type value: 

Is user provided: x 

has default ?: 

Source (code, csv?, etc.): 

note: 

***
## NDF(% DM)

Example value: 

Type value: 

Is user provided: x 

has default ?: 

Source (code, csv?, etc.): 

note: 

***
## Volatile Solid Adjusted(kg kg^-1)

Example value: 

Type value: 

Is user provided: 

has default ?: 

Source (code, csv?, etc.): 

note: 

***
## Nitrogen Excretion Adjusted(kg kg^-1)

Example value: 

Type value: 

Is user provided: 

has default ?: 

Source (code, csv?, etc.): 

note: 

***
## Dietary Net Energy Concentration

Example value: 

Type value: 

Is user provided: 

has default ?: 

Source (code, csv?, etc.): 

note: 

***
## Gain Coefficient

Example value: 

Type value: 

Is user provided: 

has default ?: 

Source (code, csv?, etc.): 

note: 

***
## Gain Coefficient A

Example value: 

Type value: 

Is user provided: 

has default ?: 

Source (code, csv?, etc.): 

note: 

***
## Gain Coefficient B

Example value: 

Type value: 

Is user provided: 

has default ?: 

Source (code, csv?, etc.): 

note: 

***
## Housing Type

Example value: 

Type value: 

Is user provided: x 

has default ?: 

Source (code, csv?, etc.): 

note: 

***
## Activity Coefficient Of Feeding Situation(MJ day^-1 kg^-1)

Example value: 

Type value: 

Is user provided: 

has default ?: 

Source (code, csv?, etc.): 

note: 

***
## Maintenance Coefficient(MJ day^-1 kg^-1)

Example value: 

Type value: 

Is user provided: 

has default ?: 

Source (code, csv?, etc.): 

note: 

***
## User Defined Bedding Rate

Example value: 

Type value: 

Is user provided: x 

has default ?: 

Source (code, csv?, etc.): 

note: 

***
## Total Carbon Kilograms Dry Matter For Bedding

Example value: 

Type value: 

Is user provided: 

has default ?: 

Source (code, csv?, etc.): 

note: 

***
## Total Nitrogen Kilograms Dry Matter For Bedding

Example value: 

Type value: 

Is user provided: 

has default ?: 

Source (code, csv?, etc.): 

note: 

***
## Moisture Content Of Bedding Material

Example value: 

Type value: 

Is user provided: x 

has default ?: 

Source (code, csv?, etc.): 

note: 

***
## Methane Conversion Factor Of Manure(kg CH4 (kg CH4)^-1)

Example value: 

Type value: 

Is user provided: 

has default ?: 

Source (code, csv?, etc.): 

note: 

***
## N2O Direct Emission Factor(kg N2O-N (kg N)^-1)

Example value: 

Type value: 

Is user provided: 

has default ?: 

Source (code, csv?, etc.): 

note: 

***
## Emission Factor Volatilization

Example value: 

Type value: 

Is user provided: 

has default ?: 

Source (code, csv?, etc.): 

note: 

***
## Volatilization Fraction

Example value: 

Type value: 

Is user provided: 

has default ?: 

Source (code, csv?, etc.): 

note: 

***
## Emission Factor Leaching

Example value: 

Type value: 

Is user provided: 

has default ?: 

Source (code, csv?, etc.): 

note: 

***
## Fraction Leaching

Example value: 

Type value: 

Is user provided: 

has default ?: 

Source (code, csv?, etc.): 

note: 

***
## Ash Content(%)

Example value: 

Type value: 

Is user provided: 

has default ?: 

Source (code, csv?, etc.): 

note: 

***
## Methane Producing Capacity Of Manure

Example value: 

Type value: 

Is user provided: 

has default ?: 

Source (code, csv?, etc.): 

note: 

***

<br>

# Beef
## Name

Example value: Beef Stockers & Backgrounders 

Type Value: str (from enum) 

Is user provided: x 

has default?: 

note: Should be unique string differentiate from other components/input files 

Source (code, csv?, etc.): 

***
## Component Type

Example value: H.Core.Models.Animals.Beef.BackgroundingComponent 

Type Value: str (from enum) 

Is user provided: x 

has default?: 

note: 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/ComponentType.cs 

***
## Group Name

Example value: Heifers 

Type Value: str (from enum) 

Is user provided: x 

has default?: 

note: Must be unique string differentiate from other animal groups in the same component (e.g. "Bulls group #1") 

Source (code, csv?, etc.): 

***
## Group Type

Example value: BeefBackgrounderHeifer 

Type Value: str (from enum) 

Is user provided: x 

has default?: 

note: See converter class used to convert animal type string names to enumeration values 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Enumerations/AnimalType.cs, https://github.com/holos-aafc/Holos/blob/main/H.Core/Converters/AnimalTypeStringConverter.cs 

***
## Management Period Name

Example value: Management period #1 

Type Value: str 

Is user provided: x 

has default?: 

note: Must be a unique string within the animal group 

Source (code, csv?, etc.): 

***
## Group Pairing Number

Example value: 0

Type Value: int 

Is user provided: x 

has default?: 

note: Used to group a parent and child group of animals. E.g. a group of lactating cows and a group of beef calves must have the same pairing number. Leave as zero when a parent/child grouping does not exist (most cases). See unit test class for example on setting this value 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core.Test/Models/Animals/Beef/CowCalfComponentTest.cs 

***
## Management Period Start Date

Example value: 2023-10-01 00:00:00 

Type Value: strftime 

Is user provided: x 

has default?: 

note: Must be set to indicate the start of management period 

Source (code, csv?, etc.): 

***
## Management Period Days

Example value: 110

Type Value: int 

Is user provided: x 

has default?: 

note: Must be set to indicate how long the management period lasts 

Source (code, csv?, etc.): 

***
## Number Of Animals

Example value: 100

Type Value: int 

Is user provided: x 

has default?: 

note: Number of animals in the animal group 

Source (code, csv?, etc.): 

***
## Production Stage

Example value: Gestating 

Type Value: str (from enum) 

Is user provided: x 

has default?: 

note: Must be set to indicate which stage a group of animals are in the lifecycle of the animal group (i.e. Lactating cows will be at the lactating production stage). This is not used for all animal types 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Enumerations/ProductionStages.cs 

***
## Number Of Young Animals

Example value: 0

Type Value: int 

Is user provided: x 

has default?: 

note: Used to indicate how many young animals (i.e. beef calves) are associated with a parent group. See line 208 of source file on how to use 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Animals/AnimalComponentBase.cs 

***
## Animals Are Milk Fed Only

Example value: False 

Type Value: bool 

Is user provided: x 

has default?: 

note: Use to specify that a group of animals are on a milk diet only. Used when referring to a group of young animals that are suckling/nursing 

Source (code, csv?, etc.): 

***
## Start Weight

Example value: 240

Type Value: float 

Is user provided: x 

has default?: 

note: Start weight of the animals in a particular management period 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Animals/ManagementPeriod.cs 

***
## End Weight

Example value: 361

Type Value: float 

Is user provided: x 

has default?: 

note: End weight of the animals in a particular management period 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Animals/ManagementPeriod.cs 

***
## Average Daily Gain

Example value: 1.1 

Type Value: float 

Is user provided: x 

has default?: 

note: This will be a value that is calculated based on the start and end weight 

Source (code, csv?, etc.): 

***
## Milk Production

Example value: 0

Type Value: float 

Is user provided: x 

has default?: 

note: The amount of milk produced by the group of animals 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Content/Resources/Table_21_Average_Milk_Production_For_Dairy_Cows_By_Province.csv, https://github.com/holos-aafc/Holos/blob/main/H.Core/Providers/Animals/Table_21_Average_Milk_Production_Dairy_Cows_Provider.cs 

***
## Milk Fat Content

Example value: 4

Type Value: float 

Is user provided: x 

has default?: 3.71 

note: Used with dairy components. Old default value was 4but has been changed 

Source (code, csv?, etc.): 

***
## Milk Protein Content As Percentage

Example value: 3.5 

Type Value: float 

Is user provided: x 

has default?: y 

note: Deprecated. Do not use. Will be removed in future version 

Source (code, csv?, etc.): 

***
## Diet Additive Type

Example value: 

Type Value: Enum 

Is user provided: x 

has default?: No 

note: Optional input used to calculate enteric CH4. See GUI for supported types 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Enumerations/DietAdditiveType.cs 

***
## Methane Conversion Factor Of Diet

Example value: 0.063 

Type Value: float 

Is user provided: 

has default?: Yes, based on diet 

note: Also known as Ym of diet. See source file for defaults based on diet type 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Providers/Feed/DietProvider.cs 

***
## Methane Conversion Factor Adjusted

Example value: 0

Type Value: float 

Is user provided: 

has default?: 

note: Deprecated. Do not use. Will be removed in future version 

Source (code, csv?, etc.): 

***
## Feed Intake

Example value: 0

Type Value: float 

Is user provided: x 

has default?: 

note: Used with some animal types (i.e. swine types). See swine diets in source file for defaults 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Providers/Feed/DietProvider.cs 

***
## Crude Protein

Example value: 12.28 

Type Value: float 

Is user provided: x 

has default?: 

note: Crude protein value of diet. See feed ingredient list for values 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Content/Resources/feeds.csv 

***
## Forage

Example value: 65

Type Value: float 

Is user provided: x 

has default?: 

note: Forage value of diet. See feed ingredient list for values 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Content/Resources/feeds.csv 

***
## TDN

Example value: 68.825 

Type Value: float 

Is user provided: x 

has default?: 

note: TDN value of diet. See feed ingredient list tdn values 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Content/Resources/feeds.csv 

***
## Ash Content Of Diet

Example value: 6.57 

Type Value: float 

Is user provided: x 

has default?: 

note: Ash content of diet. See line 434 for more informtation on how to calculate averages 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Providers/Feed/Diet.cs 

***
## Starch

Example value: 25.825 

Type Value: float 

Is user provided: x 

has default?: 

note: Starch content of diet 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Content/Resources/feeds.csv 

***
## Fat

Example value: 3.045 

Type Value: float 

Is user provided: x 

has default?: 

note: Fat content of diet 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Content/Resources/feeds.csv 

***
## ME

Example value: 2.48 

Type Value: float 

Is user provided: x 

has default?: 

note: Metabolizable energy of diet 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Content/Resources/feeds.csv 

***
## NDF

Example value: 42.025 

Type Value: float 

Is user provided: x 

has default?: 

note: Neutral detergent fibre of diet 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Content/Resources/feeds.csv 

***
## Dietary Net Energy Concentration

Example value: 0

Type Value: float 

Is user provided: x 

has default?: 

note: Used only for diet/DMI calculations for beef calves. See line 419 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Providers/Feed/Diet.cs 

***
## Housing Type

Example value: ConfinedNoBarn 

Type Value: str (from enum) 

Is user provided: x 

has default?: 

note: Required field used for many calculations. See GUI for correct types when considering a particular animal type 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Enumerations/HousingType.cs 

***
## Gain Coefficient

Example value: 1

Type Value: float 

Is user provided: 

has default?: 

note: See line 134 for default setting 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Animals/AnimalInitializationService.EmissionFactors.cs 

***
## User Defined Bedding Rate

Example value: 1.5 

Type Value: float 

Is user provided: x 

has default?: 

note: Amount of bedding added. Used in C and N input calculations. See line 52 in source and table 30 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Animals/AnimalInitializationService.Bedding.cs, https://github.com/holos-aafc/Holos/blob/main/H.Core/Providers/Animals/Table_30_Default_Bedding_Material_Composition_Provider.cs 

***
## Total Carbon Kilograms Dry Matter For Bedding

Example value: 0.447 

Type Value: float 

Is user provided: 

has default?: 

note: See HousingDetails.cs line 186 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Animals/HousingDetails.cs 

***
## Total Nitrogen Kilograms Dry Matter For Bedding

Example value: 0.0057 

Type Value: float 

Is user provided: 

has default?: 

note: See HousingDetails.cs line 177 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Animals/HousingDetails.cs 

***
## Moisture Content Of Bedding Material

Example value: 9.57 

Type Value: float 

Is user provided: 

has default?: 

note: See HousingDetails.cs line 219 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Animals/HousingDetails.cs 

***
## Activity Coefficient Of Feeding Situation

Example value: 0

Type Value: float 

Is user provided: 

has default?: 

note: See line 74 for defaults 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Animals/AnimalInitializationService.EmissionFactors.cs 

***
## Maintenance Coefficient

Example value: 0.322 

Type Value: float 

Is user provided: 

has default?: 

note: See line 108 for defaults 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Animals/AnimalInitializationService.EmissionFactors.cs 

***
## Methane Conversion Factor Of Manure

Example value: 0.26 

Type Value: float 

Is user provided: 

has default?: 

note: Methane conversion factor of manure, not to be consufed with Methane conversion factor of diet (Ym). See line 89 for defaults 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Animals/AnimalInitializationService.Methane.cs 

***
## N2O Direct Emission Factor

Example value: 0.01 

Type Value: float 

Is user provided: 

has default?: 

note: See line 34 for defaults 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Animals/AnimalInitializationService.EmissionFactors.cs 

***
## Emission Factor Volatilization

Example value: 0.005 

Type Value: float 

Is user provided: 

has default?: 

note: See line 34 for defaults 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Animals/AnimalInitializationService.EmissionFactors.cs 

***
## Volatilization Fraction

Example value: 0.25 

Type Value: float 

Is user provided: 

has default?: 

note: See line 34 for defaults 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Animals/AnimalInitializationService.EmissionFactors.cs 

***
## Emission Factor Leaching

Example value: 0.011 

Type Value: float 

Is user provided: 

has default?: 

note: See line 34 for defaults 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Animals/AnimalInitializationService.EmissionFactors.cs 

***
## Fraction Leaching

Example value: 0.035 

Type Value: float 

Is user provided: 

has default?: 

note: See line 55 for defaults 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Animals/AnimalInitializationService.Ammonia.cs 

***
## Ash Content

Example value: 8

Type Value: float 

Is user provided: 

has default?: 

note: Deprecated. Do not use. Will be removed in future version 

Source (code, csv?, etc.): 

***
## Methane Producing Capacity Of Manure

Example value: 0.19 

Type Value: float 

Is user provided: 

has default?: 

note: Also known as Bo. See line 89 for defaults 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Animals/AnimalInitializationService.Methane.cs 

***
## Fraction Of Organic Nitrogen Immobilized

Example value: 0

Type Value: float 

Is user provided: 

has default?: 

note: See line 31 for defaults 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Animals/AnimalInitializationService.Ammonia.cs 

***
## Fraction Of Organic Nitrogen Nitrified

Example value: 0.125 

Type Value: float 

Is user provided: 

has default?: 

note: See line 31 for defaults 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Animals/AnimalInitializationService.Ammonia.cs 

***
## Fraction Of Organic Nitrogen Mineralized

Example value: 0.28 

Type Value: float 

Is user provided: 

has default?: 

note: See line 31 for defaults 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Animals/AnimalInitializationService.Ammonia.cs 

***
## Manure State Type

Example value: DeepBedding 

Type Value: str (from enum) 

Is user provided: x 

has default?: 

note: Required. See GUI for valid types for particular animal type 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Enumerations/ManureStateType.cs 

***
## Ammonia Emission Factor For Manure Storage

Example value: 0.35 

Type Value: float 

Is user provided: 

has default?: 

note: For poultry animals only. See line 7 in source 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Providers/Animals/DefaultAmmoniaEmissionFactorsForPoultryManureStorageProvider.cs 

***

<br>

# Sheep
## Name

Example value: 

Value Type: str (from enum) 

Is user provided: x 

has default?: 

Source (code, csv?, etc.): 

note: 

***
## Component Type

Example value: 

Value Type: str (from enum) 

Is user provided: 

has default?: 

Source (code, csv?, etc.): 

note: 

***
## Group Name

Example value: 

Value Type: str (from enum) 

Is user provided: x 

has default?: 

Source (code, csv?, etc.): 

note: 

***
## Group Type

Example value: 

Value Type: str (from enum) 

Is user provided: x 

has default?: 

Source (code, csv?, etc.): 

note: 

***
## Management Period Name

Example value: 

Value Type: str 

Is user provided: x 

has default?: 

Source (code, csv?, etc.): 

note: 

***
## Group Pairing Number

Example value: 

Value Type: int 

Is user provided: x 

has default?: 

Source (code, csv?, etc.): 

note: 

***
## Management Period Start Date

Example value: 

Value Type: strftime 

Is user provided: x 

has default?: 

Source (code, csv?, etc.): 

note: 

***
## Management Period Days

Example value: 

Value Type: int 

Is user provided: x 

has default?: 

Source (code, csv?, etc.): 

note: 

***
## Number Of Animals

Example value: 

Value Type: int 

Is user provided: x 

has default?: 

Source (code, csv?, etc.): 

note: 

***
## Production Stage

Example value: 

Value Type: str (from enum) 

Is user provided: x 

has default?: 

Source (code, csv?, etc.): 

note: 

***
## Number Of Young Animals

Example value: 

Value Type: int 

Is user provided: x 

has default?: 

Source (code, csv?, etc.): 

note: 

***
## Start Weight(kg)

Example value: 

Value Type: float 

Is user provided: x 

has default?: 

Source (code, csv?, etc.): 

note: 

***
## End Weight(kg)

Example value: 

Value Type: float 

Is user provided: x 

has default?: 

Source (code, csv?, etc.): 

note: 

***
## Average Daily Gain(kg)

Example value: 

Value Type: float 

Is user provided: x 

has default?: 

Source (code, csv?, etc.): 

note: 

***
## Energy Required To Produce Wool(MJ kg^-1)

Example value: 

Value Type: float 

Is user provided: 

has default?: 

Source (code, csv?, etc.): 

note: 

***
## Wool Production(kg year^-1)

Example value: 

Value Type: float 

Is user provided: 

has default?: 

Source (code, csv?, etc.): 

note: 

***
## Energy Required To Produce Milk(MJ kg^-1)

Example value: 

Value Type: float 

Is user provided: 

has default?: 

Source (code, csv?, etc.): 

note: 

***
## Diet Additive Type

Example value: 

Value Type: str (from enum) 

Is user provided: x 

has default?: 

Source (code, csv?, etc.): 

note: 

***
## Methane Conversion Factor Of Diet(kg CH4 (kg CH4)^-1)

Example value: 

Value Type: float 

Is user provided: 

has default?: 

Source (code, csv?, etc.): 

note: 

***
## Methane Conversion Factor Adjusted(%)

Example value: 

Value Type: float 

Is user provided: 

has default?: 

Source (code, csv?, etc.): 

note: 

***
## Feed Intake(kg head^-1 day^-1)

Example value: 

Value Type: float 

Is user provided: x 

has default?: 

Source (code, csv?, etc.): 

note: 

***
## Crude Protein(kg kg^-1)

Example value: 

Value Type: float 

Is user provided: x 

has default?: 

Source (code, csv?, etc.): 

note: 

***
## Forage(% DM)

Example value: 

Value Type: float 

Is user provided: x 

has default?: 

Source (code, csv?, etc.): 

note: 

***
## TDN(% DM)

Example value: 

Value Type: float 

Is user provided: x 

has default?: 

Source (code, csv?, etc.): 

note: 

***
## Ash Content Of Diet(% DM)

Example value: 

Value Type: float 

Is user provided: x 

has default?: 

Source (code, csv?, etc.): 

note: 

***
## Starch(% DM)

Example value: 

Value Type: float 

Is user provided: x 

has default?: 

Source (code, csv?, etc.): 

note: 

***
## Fat(% DM)

Example value: 

Value Type: float 

Is user provided: x 

has default?: 

Source (code, csv?, etc.): 

note: 

***
## ME(MJ kg^-1)

Example value: 

Value Type: float 

Is user provided: x 

has default?: 

Source (code, csv?, etc.): 

note: 

***
## NDF(% DM)

Example value: 

Value Type: float 

Is user provided: x 

has default?: 

Source (code, csv?, etc.): 

note: 

***
## Gain Coefficient A

Example value: 

Value Type: float 

Is user provided: 

has default?: 

Source (code, csv?, etc.): 

note: 

***
## Gain Coefficient B

Example value: 

Value Type: float 

Is user provided: 

has default?: 

Source (code, csv?, etc.): 

note: 

***
## Activity Coefficient Of Feeding Situation(MJ day^-1 kg^-1)

Example value: 

Value Type: float 

Is user provided: 

has default?: 

Source (code, csv?, etc.): 

note: 

***
## Maintenance Coefficient(MJ day^-1 kg^-1)

Example value: 

Value Type: float 

Is user provided: 

has default?: 

Source (code, csv?, etc.): 

note: 

***
## User Defined Bedding Rate

Example value: 

Value Type: float 

Is user provided: x 

has default?: 

Source (code, csv?, etc.): 

note: 

***
## Total Carbon Kilograms Dry Matter For Bedding

Example value: 

Value Type: float 

Is user provided: 

has default?: 

Source (code, csv?, etc.): 

note: 

***
## Total Nitrogen Kilograms Dry Matter For Bedding

Example value: 

Value Type: float 

Is user provided: 

has default?: 

Source (code, csv?, etc.): 

note: 

***
## Moisture Content Of Bedding Material

Example value: 

Value Type: float 

Is user provided: 

has default?: 

Source (code, csv?, etc.): 

note: 

***
## Methane Conversion Factor Of Manure(kg CH4 (kg CH4)^-1)

Example value: 

Value Type: float 

Is user provided: 

has default?: 

Source (code, csv?, etc.): 

note: 

***
## N2O Direct Emission Factor(kg N2O-N (kg N)^-1)

Example value: 

Value Type: float 

Is user provided: 

has default?: 

Source (code, csv?, etc.): 

note: 

***
## Emission Factor Volatilization

Example value: 

Value Type: float 

Is user provided: 

has default?: 

Source (code, csv?, etc.): 

note: 

***
## Volatilization Fraction

Example value: 

Value Type: float 

Is user provided: 

has default?: 

Source (code, csv?, etc.): 

note: 

***
## Emission Factor Leaching

Example value: 

Value Type: float 

Is user provided: 

has default?: 

Source (code, csv?, etc.): 

note: 

***
## Fraction Leaching

Example value: 

Value Type: float 

Is user provided: 

has default?: 

Source (code, csv?, etc.): 

note: 

***
## Ash Content(%)

Example value: 

Value Type: float 

Is user provided: 

has default?: 

Source (code, csv?, etc.): 

note: 

***
## Methane Producing Capacity Of Manure

Example value: 

Value Type: float 

Is user provided: 

has default?: 

Source (code, csv?, etc.): 

note: 

***
## Manure Excretion Rate

Example value: 

Value Type: float 

Is user provided: x 

has default?: 

Source (code, csv?, etc.): 

note: 

***
## Fraction Of Carbon In Manure

Example value: 

Value Type: float 

Is user provided: 

has default?: 

Source (code, csv?, etc.): 

note: 

***

<br>

# Farm Setting
## Polygon ID: 

Example value: 851003

Value type: int 

Is user provided: x 

has default ?: N 

note: Required. Use GUI map view to get polygon number if needed 

Source (code, csv?, etc.): 

***
## Yield Assignment Method = 

Example value: SmallAreaData 

Value type: enum 

Is user provided: 

has default ?: SmallAreaData 

note: Used to lookup default yields for a particular year and crop type 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Enumerations/YieldAssignmentMethod.cs 

***
## Polygon Number = 

Example value: 851003

Value type: int 

Is user provided: x 

has default ?: 

note: Required. Use GUI map view to get polygon number if needed 

Source (code, csv?, etc.): 

***
## Latitude = 

Example value: 49.9805772869656 

Value type: float 

Is user provided: x 

has default ?: 

note: Required if using NASA as the source for climate data. Values are determined when user clicks on a location in the GUI 

Source (code, csv?, etc.): 

***
## Longitude = 

Example value: -98.0433082580568 

Value type: float 

Is user provided: x 

has default ?: 

note: Required if using NASA as the source for climate data. Values are determined when user clicks on a location in the GUI 

Source (code, csv?, etc.): 

***
## Carbon Concentration  (kg kg^-1) =

Example value: 0.45 

Value type: float 

Is user provided: 

has default ?: 0.4 

note: See line 90 in source 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/CoreConstants.cs 

***
## Emergence Day = 

Example value: 141

Value type: int 

Is user provided: is used ? 

has default ?: 

note: Used with ICBM carbon model. See line 167 for default 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Defaults.cs 

***
## Ripening Day = 

Example value: 197

Value type: int 

Is user provided: is used ? 

has default ?: 

note: Used with ICBM carbon model. See line 168 for default 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Defaults.cs 

***
## Variance = 

Example value: 300

Value type: float? 

Is user provided: 

has default ?: 

note: Used with ICBM carbon model. See line 169 for default 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Defaults.cs 

***
## Alfa =

Example value: 0.7 

Value type: float 

Is user provided: 

has default ?: 

note: Used with ICBM carbon model. See line 175 for default 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Defaults.cs 

***
## Decomposition Minimum Temperature  (�C) = -3.78

Example value: -3.78 

Value type: float 

Is user provided: 

has default ?: 

note: Used with ICBM carbon model. See line 176 for default 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Defaults.cs 

***
## Decomposition Maximum Temperature  (�C) =

Example value: 30

Value type: float 

Is user provided: 

has default ?: 

note: Used with ICBM carbon model. See line 177 for default 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Defaults.cs 

***
## Moisture Response Function At Saturation = 

Example value: 0.42 

Value type: float 

Is user provided: 

has default ?: 

note: Used with ICBM carbon model. See line 178 for default 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Defaults.cs 

***
## Moisture Response Function At Wilting Point = 

Example value: 0.18 

Value type: float 

Is user provided: 

has default ?: 

note: Used with ICBM carbon model. See line 179 for default 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Defaults.cs 

***
## Percentage Of Product Returned To Soil For Annuals = 2

Example value: 2

Value type: float 

Is user provided: 

has default ?: 

note: Used as a global default. See line 24 on how the default is used. User can override 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Crops/CropInitializationService.Returns.cs 

***
## Percentage Of Straw Returned To Soil For Annuals = 100

Example value: 100

Value type: float 

Is user provided: 

has default ?: 

note: Used as a global default. See line 24 on how the default is used. User can override 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Crops/CropInitializationService.Returns.cs 

***
## Percentage Of Roots Returned To Soil For Annuals = 100

Example value: 100

Value type: float 

Is user provided: 

has default ?: 

note: Used as a global default. See line 24 on how the default is used. User can override 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Crops/CropInitializationService.Returns.cs 

***
## Percentage Of Product Yield Returned To Soil For Silage Crops = 35

Example value: 35

Value type: float 

Is user provided: 

has default ?: 

note: Used as a global default. See line 24 on how the default is used. User can override 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Crops/CropInitializationService.Returns.cs 

***
## Percentage Of Roots Returned To Soil For Silage Crops = 100

Example value: 100

Value type: float 

Is user provided: 

has default ?: 

note: Used as a global default. See line 24 on how the default is used. User can override 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Crops/CropInitializationService.Returns.cs 

***
## Percentage Of Product Yield Returned To Soil For Cover Crops = 100

Example value: 100

Value type: float 

Is user provided: 

has default ?: 

note: Used as a global default. See line 24 on how the default is used. User can override 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Crops/CropInitializationService.Returns.cs 

***
## Percentage Of Product Yield Returned To Soil For Cover Crops Forage = 35

Example value: 35

Value type: float 

Is user provided: 

has default ?: 

note: Used as a global default. See line 24 on how the default is used. User can override 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Crops/CropInitializationService.Returns.cs 

***
## Percentage Of Product Yield Returned To Soil For Cover Crops Produce = 0

Example value: 0

Value type: float 

Is user provided: 

has default ?: 

note: Used as a global default. See line 24 on how the default is used. User can override 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Crops/CropInitializationService.Returns.cs 

***
## Percentage Of Straw Returned To Soil For Cover Crops = 100

Example value: 100

Value type: float 

Is user provided: 

has default ?: 

note: Used as a global default. See line 24 on how the default is used. User can override 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Crops/CropInitializationService.Returns.cs 

***
## Percentage Of Roots Returned To Soil For Cover Crops = 100

Example value: 100

Value type: float 

Is user provided: 

has default ?: 

note: Used as a global default. See line 24 on how the default is used. User can override 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Crops/CropInitializationService.Returns.cs 

***
## Percentage Of Product Returned To Soil For Root Crops = 0

Example value: 0

Value type: float 

Is user provided: 

has default ?: 

note: Used as a global default. See line 24 on how the default is used. User can override 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Crops/CropInitializationService.Returns.cs 

***
## Percentage Of Straw Returned To Soil For Root Crops = 100

Example value: 100

Value type: float 

Is user provided: 

has default ?: 

note: Used as a global default. See line 24 on how the default is used. User can override 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Crops/CropInitializationService.Returns.cs 

***
## Percentage Of Product Returned To Soil For Perennials = 35

Example value: 35

Value type: float 

Is user provided: 

has default ?: 

note: Used as a global default. See line 24 on how the default is used. User can override 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Crops/CropInitializationService.Returns.cs 

***
## Percentage Of Roots Returned To Soil For Perennials = 100

Example value: 100

Value type: float 

Is user provided: 

has default ?: 

note: Used as a global default. See line 24 on how the default is used. User can override 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Crops/CropInitializationService.Returns.cs 

***
## Percentage Of Product Returned To Soil For Rangeland Due To Harvest Loss = 35

Example value: 35

Value type: float 

Is user provided: 

has default ?: 

note: Used as a global default. See line 24 on how the default is used. User can override 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Crops/CropInitializationService.Returns.cs 

***
## Percentage Of Roots Returned To Soil For Rangeland = 100

Example value: 100

Value type: float 

Is user provided: 

has default ?: 

note: Used as a global default. See line 24 on how the default is used. User can override 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Crops/CropInitializationService.Returns.cs 

***
## Percentage Of Product Returned To Soil For Fodder Corn = 35

Example value: 35

Value type: float 

Is user provided: 

has default ?: 

note: Used as a global default. See line 24 on how the default is used. User can override 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Crops/CropInitializationService.Returns.cs 

***
## Percentage Of Roots Returned To Soil For Fodder Corn = 100

Example value: 100

Value type: float 

Is user provided: 

has default ?: 

note: Used as a global default. See line 24 on how the default is used. User can override 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Crops/CropInitializationService.Returns.cs 

***
## Decomposition Rate Constant Young Pool = 0.8

Example value: 0.8 

Value type: float 

Is user provided: 

has default ?: 

note: Used with ICBM carbon model. See line 221 for default 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Defaults.cs 

***
## Decomposition Rate Constant Old Pool =

Example value: 0.00605 

Value type: float 

Is user provided: 

has default ?: 

note: Used with ICBM carbon model. See line 222 for default 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Defaults.cs 

***
## Old Pool Carbon N =

Example value: 0.1 

Value type: float 

Is user provided: 

has default ?: 

note: Used with both ICBM and IPCC Tier 2 carbon models. See line 224 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Defaults.cs 

***
## NO Ratio =

Example value: 0.1 

Value type: float 

Is user provided: 

has default ?: 

note: Used with both ICBM and IPCC Tier 2 carbon models. See line 227 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Defaults.cs 

***
## Emission Factor For Leaching And Runoff  (kg N2O-N (kg N)^-1) = 

Example value: 0.011 

Value type: float 

Is user provided: 

has default ?: 

note: Used for N2O calculations. See line 228  

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Defaults.cs 

***
## Emission Factor For Volatilization  (kg N2O-N (kg N)^-1) = 

Example value: 0.01 

Value type: float 

Is user provided: 

has default ?: 

note: Used for N2O calculations. See line 229 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Defaults.cs 

***
## Fraction Of N Lost By Volatilization = 

Example value: 0.21 

Value type: float 

Is user provided: 

has default ?: 

note: Used for N2O calculations. See line 232 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Defaults.cs 

***
## Microbe Death = 

Example value: 0.2 

Value type: float 

Is user provided: 

has default ?: 

note: Used by both ICBM and IPCC Tier carbon models. See line 236 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Defaults.cs 

***
## Denitrification = 

Example value: 0.5 

Value type: float 

Is user provided: 

has default ?: 

note: Used by both ICBM and IPCC Tier carbon models. See line 237 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Defaults.cs 

***
## Carbon modelling strategy =

Example value:  IPCCTier2 

Value type: enum 

Is user provided: 

has default ?: 

note: Will determine which carbon model will be used (IPCC Tier 2 is the newest C model and is the new default model) 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Enumerations/CarbonModellingStrategies.cs 

***
## Run In Period Years = 

Example value: 15

Value type: int 

Is user provided: x 

has default ?: 

note: Used to indicate how many years will be used when calculating equilibrium soil carbon. Can leave default in most cases. See line 269 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Defaults.cs 

***
## Humification Coefficient Above Ground = 

Example value: 0.125 

Value type: float 

Is user provided: 

has default ?: 

note: Used with ICBM carbon model. See line 217 for default 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Defaults.cs 

***
## Humification Coefficient Below Ground = 

Example value: 0.3 

Value type: float 

Is user provided: 

has default ?: 

note: Used with ICBM carbon model. See line 218 for default 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Defaults.cs 

***
## Humification Coefficient Manure = 

Example value: 0.31 

Value type: float 

Is user provided: 

has default ?: 

note: Used with ICBM carbon model. See line 219 for default 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Defaults.cs 

***
## Climate filename = climate.csv

Example value: climate.csv 

Value type: 

Is user provided: x 

has default ?: 

note: Used when climate acquisition is set to "InputFile" 

Source (code, csv?, etc.): 

***
## Climate Data Acquisition = 

Example value: NASA 

Value type: enum 

Is user provided: 

has default ?: 

note: Specifies how Holos will aquire climate data. See source file for more details 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Providers/Climate/ClimateProvider.cs 

***
## Use climate parameter instead of management factor = 

Example value: True 

Value type: bool 

Is user provided: 

has default ?: 

note: Set to true for most scenarios 

Source (code, csv?, etc.): 

***
## Enable Carbon Modelling = 

Example value: True 

Value type: bool 

Is user provided: 

has default ?: 

note: Set to true for most scenarios 

Source (code, csv?, etc.): 

***
## January Precipitation = 

Example value: 17.6213 

Value type: float 

Is user provided: not used 

has default ?: 

note: Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else. See line 37 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs 

***
## February Precipitation = 

Example value: 12.8316 

Value type: float 

Is user provided: not used 

has default ?: 

note: Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs 

***
## March Precipitation = 

Example value: 22.426 

Value type: float 

Is user provided: not used 

has default ?: 

note: Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs 

***
## April Precipitation = 

Example value: 27.4144 

Value type: float 

Is user provided: not used 

has default ?: 

note: Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs 

***
## May Precipitation = 

Example value: 61.5015 

Value type: float 

Is user provided: not used 

has default ?: 

note: Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs 

***
## June Precipitation = 

Example value: 77.9022 

Value type: float 

Is user provided: not used 

has default ?: 

note: Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs 

***
## July Precipitation = 

Example value: 57.274 

Value type: float 

Is user provided: not used 

has default ?: 

note: Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs 

***
## August Precipitation = 

Example value: 53.0356 

Value type: float 

Is user provided: not used 

has default ?: 

note: Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs 

***
## September Precipitation = 

Example value: 40.4796 

Value type: float 

Is user provided: not used 

has default ?: 

note: Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs 

***
## October Precipitation = 

Example value: 33.7571 

Value type: float 

Is user provided: not used 

has default ?: 

note: Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs 

***
## November Precipitation = 

Example value: 23.0151 

Value type: float 

Is user provided: not used 

has default ?: 

note: Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs 

***
## December Precipitation = 

Example value: 21.4046 

Value type: float 

Is user provided: not used 

has default ?: 

note: Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs 

***
## January Potential Evapotranspiration = 

Example value: 0.0327 

Value type: float 

Is user provided: not used 

has default ?: 

note: Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs 

***
## February Potential Evapotranspiration = 

Example value: 0.0888 

Value type: float 

Is user provided: not used 

has default ?: 

note: Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs 

***
## March Potential Evapotranspiration = 

Example value: 3.5731 

Value type: float 

Is user provided: not used 

has default ?: 

note: Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs 

***
## April Potential Evapotranspiration = 

Example value: 44.1505 

Value type: float 

Is user provided: not used 

has default ?: 

note: Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs 

***
## May Potential Evapotranspiration = 

Example value: 100.0393 

Value type: float 

Is user provided: not used 

has default ?: 

note: Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs 

***
## June Potential Evapotranspiration = 

Example value: 123.5476 

Value type: float 

Is user provided: not used 

has default ?: 

note: Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs 

***
## July Potential Evapotranspiration = 

Example value: 135.7116 

Value type: float 

Is user provided: not used 

has default ?: 

note: Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs 

***
## August Potential Evapotranspiration = 

Example value: 120.4341 

Value type: float 

Is user provided: not used 

has default ?: 

note: Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs 

***
## September Potential Evapotranspiration = 

Example value: 66.0041 

Value type: float 

Is user provided: not used 

has default ?: 

note: Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs 

***
## October Potential Evapotranspiration = 

Example value: 16.8898 

Value type: float 

Is user provided: not used 

has default ?: 

note: Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs 

***
## November Potential Evapotranspiration = 

Example value: 0.7677 

Value type: float 

Is user provided: not used 

has default ?: 

note: Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs 

***
## December Potential Evapotranspiration = 

Example value: 0.0252 

Value type: float 

Is user provided: not used 

has default ?: 

note: Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs 

***
## January Mean Temperature = 

Example value: -14.8531 

Value type: float 

Is user provided: not used 

has default ?: 

note: Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs 

***
## February Mean Temperature = 

Example value: -12.4063 

Value type: float 

Is user provided: not used 

has default ?: 

note: Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs 

***
## March Mean Temperature = 

Example value: -5.3584 

Value type: float 

Is user provided: not used 

has default ?: 

note: Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs 

***
## April Mean Temperature =

Example value: 3.7295 

Value type: float 

Is user provided: not used 

has default ?: 

note: Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs 

***
## May Mean Temperature = 

Example value: 10.7967 

Value type: float 

Is user provided: not used 

has default ?: 

note: Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs 

***
## June Mean Temperature = 

Example value: 16.4886 

Value type: float 

Is user provided: not used 

has default ?: 

note: Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs 

***
## July Mean Temperature = 

Example value: 18.8914 

Value type: float 

Is user provided: not used 

has default ?: 

note: Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs 

***
## August Mean Temperature = 

Example value: 18.2291 

Value type: float 

Is user provided: not used 

has default ?: 

note: Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs 

***
## September Mean Temperature = 

Example value: 13.2652 

Value type: float 

Is user provided: not used 

has default ?: 

note: Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs 

***
## October Mean Temperature = 

Example value: 5.6419 

Value type: float 

Is user provided: not used 

has default ?: 

note: Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs 

***
## November Mean Temperature = 

Example value: -3.5511 

Value type: float 

Is user provided: not used 

has default ?: 

note: Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs 

***
## December Mean Temperature = 

Example value: -11.9174 

Value type: float 

Is user provided: not used 

has default ?: 

note: Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else 

Source (code, csv?, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs 

***
## Province = 

Example value: Manitoba 

Value type: enum 

Is user provided: x 

has default ?: 

note: extracted from slc DB with lat long data 

Source (code, csv?, etc.): 

***
## Year Of Observation =

Example value: 2024

Value type: int 

Is user provided: x 

has default ?: 

note: Defaults to the current year 

Source (code, csv?, etc.): 

***
## Ecodistrict ID = 

Example value: 851

Value type: int 

Is user provided: x 

has default ?: 

note: extracted from slc DB with lat long data 

Source (code, csv?, etc.): 

***
## Soil Great Group = 

Example value: Regosol 

Value type: enum 

Is user provided: x 

has default ?: 

note: extracted from slc DB with lat long data 

Source (code, csv?, etc.): 

***
## Soil functional category = 

Example value: Black 

Value type: enum 

Is user provided: x 

has default ?: 

note: deduced following code in Holos 

Source (code, csv?, etc.): 

***
## Bulk Density = 

Example value: 1.2 

Value type: float 

Is user provided: x 

has default ?: 

note: extracted from slc DB with lat long data 

Source (code, csv?, etc.): 

***
## Soil Texture = 

Example value: Fine 

Value type: enum 

Is user provided: x 

has default ?: 

note: deduced following code in Holos 

Source (code, csv?, etc.): 

***
## Soil Ph = 

Example value: 7.8 

Value type: float 

Is user provided: x 

has default ?: 

note: extracted from slc DB with lat long data 

Source (code, csv?, etc.): 

***
## Top Layer Thickness  (mm) = 

Example value: 200

Value type: float 

Is user provided: x 

has default ?: 

note: extracted from slc DB with lat long data 

Source (code, csv?, etc.): 

***
## Proportion Of Sand In Soil = 

Example value: 0.2 

Value type: float 

Is user provided: x 

has default ?: 

note: extracted from slc DB with lat long data 

Source (code, csv?, etc.): 

***
## Proportion Of Clay In Soil = 

Example value: 0.3 

Value type: float 

Is user provided: x 

has default ?: 

note: extracted from slc DB with lat long data 

Source (code, csv?, etc.): 

***
## Proportion Of Soil Organic Carbon = 

Example value: 3.1 

Value type: float 

Is user provided: x 

has default ?: 

note: extracted from slc DB with lat long data 

Source (code, csv?, etc.): 

***
