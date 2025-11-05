# Example Input Tables For Command Line Interface

This document provides example data for the user created input Excel file(s) that will be processed by the Holos CLI. Please read [here](https://github.com/holos-aafc/Holos/blob/main/H.Content/Documentation/User%20Guide/User%20Guide.md#chapter-10---command-line-interface) for more information on the Holos CLI.

<br>

# Field
## PhaseNumber

Example value: 0

Type value: 

Does user have to provide value: Yes

Holos has a default value: No

note: Deprecated. Do not use. Will be removed in future version 

Source (source code file, table, algorithm document, etc.): 

***
## Name

Example value: Wheat and hairy vetch 

Type value: String (Text)

Does user have to provide value: Yes

Holos has a default value: Yes 

note: User defined name 

Source (source code file, table, algorithm document, etc.): 

***
## Area

Example value: 18

Type value: Double (Decimal/Numeric)

Does user have to provide value: Yes 

Holos has a default value: No

Valid range of values: (x ≥ 0)

note: 

Source (source code file, table, algorithm document, etc.): 

***
## CurrentYear

Example value: 2024

Type value: Int (Integer/Numeric)

Does user have to provide value: Yes 

Holos has a default value: Current year (i.e. 2024) 

note: 

Source (source code file, table, algorithm document, etc.): 

***
## CropYear

Example value: 1985

Type value: Int (Integer/Numeric)

Does user have to provide value: Yes 

Holos has a default value: No

Possible values: 1985-(current year)

note: Each (row) in input file must correspond to a certain year 

Source (source code file, table, algorithm document, etc.): 

***
## CropType

Example value: Wheat 

Type value: Text

Does user have to provide value: Yes

Holos has a default value: No

Possible values (see table below):
<br>
| Oilseeds  | Other field crops  |  Pulse crops | Root crop   | Silage  | Small grain cereals   | Fallow  | Perenial  |
|---|---|---|---|---|---|---|---|
| Carnelina  | Berries & Grapes  | Beans (dry field) | Potatoes  | Barley silage   | Barley  | Fallow  | Forage for seed  |
| Canola  | Other Field Crops  | Chickpeas  | Sugar beets  | Grass silage  | Buckwheat  |   | Rangeland (Native)  |
| Flax  | Safflower  | Dry/Field Peas  |   | Oat silage  | Canary seed  |   | Seeded grassland  |
| Mustard  | Sunflower seed  | Lentils  |   | Silage corn  | Fall rye  |   | Tame grass  |
| Oilseeds  | Tobacco  | Pulse Crops  |   | Triticale silage  | Grain corn  |   | Tame legume  |
|  Soybeans | Vegetables  |   |   | Wheat silage  | Mixed grains  |   | tame mixed (grass/legume)  |
|   |   |   |   |   | Oats  |   |   |
|   |   |   |   |   | Small grain cereals  |   |   |
|   |   |   |   |   | Sorghurn |   |   |
|   |   |   |   |   | Triticale |   |   |
|   |   |   |   |   | Undersown barley |   |   |
|   |   |   |   |   |  Wheat |   |   |


note: 

Source (source code file, table, algorithm document, etc.): 

***
## TillageType

Example value: Reduced 

Type value: Text 

Does user have to provide value: Yes

Holos has a default value: Wheat 

Possible values: No tillage, Reduced tillage, Intensive tillage

note: See GUI for supported list of crop types since not all items in the enum are supported in calculations 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Enumerations/CropType.cs 

***
## YearInPerennialStand

Example value: 2018

Type value: Int (Integer/Numeric)

Does user have to provide value: Yes 

Holos has a default value: No 

Possible values: 1 up to length of perennial stand

note: Each year of a perennial stand must have the year identified in the row of the input file. E.g. a six year perennial stand would have one row with this value set 1 for the first year, 2 for the second year, etc 

Source (source code file, table, algorithm document, etc.): 

***
## PerennialStandID

Example value: 00000000-0000-0000-0000-000000000000 

Type value: GUID 

Does user have to provide value: Yes 

Holos has a default value: No 

Valid values:

note: Used to group all years of a perennial stand together. Each year in a distinct perennial stand must have this value set. All years in the same perennial stand must have this same ID/value. Can be thought of as a 'group' ID 

Source (source code file, table, algorithm document, etc.): 

***
## PerennialStandLength

Example value: 1

Type value: Int (Integer/Numeric)

Does user have to provide value: No

Holos has a default value: 1

Valid range of values: (x ≥ 0)

note: Indicates how long the perennial is grown 

Source (source code file, table, algorithm document, etc.): 

***
## BiomassCoefficientProduct

Example value: 0.244 

Type value: Double (Decimal/Numeric) 

Does user have to provide value: Default assigned but user can override 

Holos has a default value: Yes 

Valid range of values: (x ≥ 0)

note: Rp Product 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Content/Resources/Table_7_Relative_Biomass_Information.csv 

***
## BiomassCoefficientStraw

Example value: 0.518 

Type value: Double (Decimal/Numeric) 

Does user have to provide value: Default assigned but user can override 

Holos has a default value: Yes 

Valid range of values: (x ≥ 0)

note: Rs Straw 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Content/Resources/Table_7_Relative_Biomass_Information.csv 

***
## BiomassCoefficientRoots

Example value: 0.147 

Type value: Double (Decimal/Numeric) 

Does user have to provide value: Default assigned but user can override 

Holos has a default value: Yes 

Valid range of values: (x ≥ 0)

note: Rr Root 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Content/Resources/Table_7_Relative_Biomass_Information.csv 

***
## BiomassCoefficientExtraroot

Example value: 0.091 

Type value: Double (Decimal/Numeric) 

Does user have to provide value: Default assigned but user can override 

Holos has a default value: Yes 

Valid range of values: (x ≥ 0)

note: Re Extra-root 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Content/Resources/Table_7_Relative_Biomass_Information.csv 

***
## NitrogenContentInProduct

Example value: 0.0263 

Type value: Double (Decimal/Numeric) 

Does user have to provide value: Default assigned but user can override 

Holos has a default value: Yes 

Valid range of values: (x ≥ 0)

note: Np Product 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Content/Resources/Table_7_Relative_Biomass_Information.csv 

***
## NitrogenContentInStraw

Example value: 0.0082 

Type value: Double (Decimal/Numeric) 

Does user have to provide value: Default assigned but user can override 

Holos has a default value: Yes 

Valid range of values: (x ≥ 0)

note: Ns Straw 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Content/Resources/Table_7_Relative_Biomass_Information.csv 

***
## NitrogenContentInRoots

Example value: 0.0104 

Type value: Double (Decimal/Numeric) 

Does user have to provide value: Default assigned but user can override 

Holos has a default value: Yes 

Valid range of values: (x ≥ 0)

note: Nr Root 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Content/Resources/Table_7_Relative_Biomass_Information.csv 

***
## NitrogenContentInExtraroot

Example value: 0.0104 

Type value: Double (Decimal/Numeric)

Does user have to provide value: Default assigned but user can override 

Holos has a default value: Yes 

Valid range of values: (x ≥ 0)

note: Ne Extra-root 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Content/Resources/Table_7_Relative_Biomass_Information.csv 

***
## NitrogenFixation

Example value: 0

Type value: Double (Decimal/Numeric)

Does user have to provide value: Default assigned but user can override 

Holos has a default value: Yes 

Valid range of values: (0 ≤ x ≤ 100)

note: Indexed by crop type 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Providers/Nitrogen/NitogenFixationProvider.cs 

***
## NitrogenDeposit

Example value: 5

Type value: Double (Decimal/Numeric)

Does user have to provide value: Default assigned but user can override 

Holos has a default value: 5

Valid range of values: (x ≥ 0)

note: Common value for all crop types. Page 74 in algorithm document 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/raw/refs/heads/main/Pogue%20et%20al%202025_Printversion_Holos_V4.0_Algorithm_Document.docx

***
## CarbonConcentration

Example value: 0.45 

Type value: Double (Decimal/Numeric)

Does user have to provide value: Default assigned but user can override 

Holos has a default value: 0.45 

Valid range of values: (x ≥ 0)

note: Common value for all crop types. Page 37 in algorithm document 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/raw/refs/heads/main/Pogue%20et%20al%202025_Printversion_Holos_V4.0_Algorithm_Document.docx

***
## Yield

Example value: 2700

Type value: Double (Decimal/Numeric) 

Does user have to provide value: Default assigned but user can override 

Holos has a default value: Yes

Valid range of values: (x ≥ 0)

note: Look up value from Small Area Database 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Content/Resources/small_area_yields.csv 

***
## HarvestMethod

Example value: CashCrop 

Type value: Enum (Text)

Does user have to provide value: Default assigned but user can override 

Holos has a default value: Yes

Valid values: CashCrop, GreenManure, None, Silage, StubbleGrazing, Swathing

note: Depends on crop type. Line 19 in source 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Crops/CropInitializationService.Harvest.cs 

***
## NitrogenFertilizerRate

Example value: 87.7608533333333 

Type value: Double (Decimal/Numeric)  

Does user have to provide value: Default assigned but user can override 

Holos has a default value: Yes

Valid range of values: (x ≥ 0)

note: Calculated based on yield. Line 17 in source 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Crops/CropInitializationService.Fertilizer.cs 

***
## PhosphorousFertilizerRate

Example value: 0

Type value: Double (Decimal/Numeric)  

Does user have to provide value: Yes

Holos has a default value:

Valid range of values: (x ≥ 0)

note: Not used/implemented yet. Future version will utilize 

Source (source code file, table, algorithm document, etc.): 

***
## IsIrrigated

Example value: No 

Type value: Enum (Yes/No)

Does user have to provide value: Yes

Holos has a default value: No 

Valid values: Yes, No

note: Not used/implemented yet. Future version will utilize 

Source (source code file, table, algorithm document, etc.): 

***
## IrrigationType

Example value: RainFed 

Type value: Enum (Text)

Does user have to provide value: Yes

Holos has a default value: Yes 

Valid values: Rainfed, Irrigated

note: Used to lookup values in Table 7. Line 1290 in source 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/LandManagement/Fields/CropViewItem.cs 

***
## AmountOfIrrigation

Example value: 200

Type value: Double (Decimal/Numeric) 

Does user have to provide value: Default assigned but user can override 

Holos has a default value: Yes

Valid range of values: (x ≥ 0)

note: Line 35 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Crops/CropInitializationService.Water.cs 

***
## MoistureContentOfCrop

Example value: 0.12 

Type value: Double (Decimal/Numeric)  

Does user have to provide value: Default assigned but user can override 

Holos has a default value: Yes

Valid range of values: (0 ≤ x ≤ 1)

note: Look up value by crop type and irrigation amount. Additional logic in source file on line 60 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Content/Resources/Table_7_Relative_Biomass_Information.csv, https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Crops/CropInitializationService.Water.cs 

***
## MoistureContentOfCropPercentage

Example value: 12

Type value: Double (Decimal/Numeric)  

Does user have to provide value: Default assigned but user can override 

Holos has a default value: Yes 

Valid range of values: (0 ≤ x ≤ 100)

note: Look up value by crop type and irrigation amount. Additional logic in source file on line 61 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Content/Resources/Table_7_Relative_Biomass_Information.csv, https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Crops/CropInitializationService.Water.cs 

***
## PercentageOfStrawReturnedToSoil

Example value: 100

Type value: Double (Decimal/Numeric) 

Does user have to provide value: Default assigned but user can override 

Holos has a default value: Yes

Valid range of values: (0 ≤ x ≤ 100)

note: Line 35 in source. Page 36 in algorithm document has references 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Crops/CropInitializationService.Returns.cs, https://github.com/holos-aafc/Holos/raw/refs/heads/main/Pogue%20et%20al%202025_Printversion_Holos_V4.0_Algorithm_Document.docx

***
## PercentageOfRootsReturnedToSoil

Example value: 100

Type value: Double (Decimal/Numeric)  

Does user have to provide value: Default assigned but user can override 

Holos has a default value: Yes

Valid range of values: (0 ≤ x ≤ 100)

note: Line 35 in source. Page 36 in algorithm document has references 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Crops/CropInitializationService.Returns.cs, https://github.com/holos-aafc/Holos/raw/refs/heads/main/Pogue%20et%20al%202025_Printversion_Holos_V4.0_Algorithm_Document.docx

***
## PercentageOfProductYieldReturnedToSoil

Example value: 2

Type value: Double (Decimal/Numeric) 

Does user have to provide value: Default assigned but user can override 

Holos has a default value: Yes

Valid range of values: (0 ≤ x ≤ 100)

note: Line 35 in source. Page 36 in algorithm document has references 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Crops/CropInitializationService.Returns.cs, https://github.com/holos-aafc/Holos/raw/refs/heads/main/Pogue%20et%20al%202025_Printversion_Holos_V4.0_Algorithm_Document.docx

***
## IsPesticideUsed

Example value: No 

Type value: Bool (Yes/No)

Does user have to provide value: Yes

Holos has a default value:

Valid values: Yes/No

note: Should be set if "Number of Pesticide Passes" > 0 

Source (source code file, table, algorithm document, etc.): 

***
## NumberOfPesticidePasses

Example value: 0

Type value: Int (Integer/Numeric)

Does user have to provide value: Yes 

Holos has a default value: No

Valid range of values: (x ≥ 0)

note: Any value > 0 

Source (source code file, table, algorithm document, etc.): 

***
## ManureApplied

Example value: False 

Type value: Bool (True/False)

Does user have to provide value: Yes 

Holos has a default value: Yes

Valid values: True, False

note: Should be set to true if any manure application/amount has been applied to field 

Source (source code file, table, algorithm document, etc.): 

***
## AmountOfManureApplied

Example value: 0

Type value: Double (Decimal/Numeric) 

Does user have to provide value: Yes 

Holos has a default value: No 

Valid values: True/False

note: Amount of manure applied to field (kg/ha) 

Source (source code file, table, algorithm document, etc.): 

***
## ManureApplicationType

Example value: NotSelected 

Type value: Enum (Text)

Does user have to provide value: Yes 

Holos has a default value: No 

Valid values: NotSelected, TilledLandSolidSpread, UntilledLandSolidSpread, Slurrybroadcasting, DropHoseBanding, ShallowInjection, DeepInjection

note: See page 201 in algorithm document and table 43 line 113 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Enumerations/ManureApplicationTypes.cs, https://github.com/holos-aafc/Holos/blob/main/H.Core/Providers/Animals/Table_43_Beef_Dairy_Default_Emission_Factors_Provider.cs, https://github.com/holos-aafc/Holos/raw/refs/heads/main/Pogue%20et%20al%202025_Printversion_Holos_V4.0_Algorithm_Document.docx

***
## ManureAnimalSourceType

Example value: NotSelected 

Type value: Enum (Text)

Does user have to provide value: Yes 

Holos has a default value: No 

Valid values: NotSelected, BeefManure, DairyManure, SwineManure, PoultryManure, SheepManure, OtherLivestockManure

note: Used for various table lookups 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Enumerations/ManureAnimalSourceTypes.cs 

***
## ManureStateType

Example value: NotSelected 

Type value: Enum (Text)

Does user have to provide value: Yes 

Holos has a default value: No 

Valid values: NotSelected, AnaerobicDigster, Composted, CompostIntensive, CompostPassive, DailySpread, DeepBedding, DeepPit, Liquid, LiquidCrust, LiquidSeperated, LiquidNoCrust, Pasture, Range, Paddock, Solid, Slurry, SlurryWithNaturalCrust, SlurryWithoutNaturalCrust, SolidStorage, Custom, PitLagoonNoCover, LiquidWithNaturalCrust, LiquidWithSolidCover, CompostedInVessel, SolidStorageWithOrWithoutLitter

note: Used for various table lookups 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Enumerations/ManureStateType.cs 

***
## ManureLocationSourceType

Example value: NotSelected 

Type value: Enum (Text)

Does user have to provide value: Default assigned but user can override 

Holos has a default value: "Livestock" 

Valid values: NotSelected, Livestock, Imported

note: Used to inidcate if manure was source from on farm or imported onto farm 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Enumerations/ManureLocationSourceType.cs 

***
## UnderSownCropsUsed

Example value: False 

Type value: Bool (True/False)

Does user have to provide value: Default assigned but user can override 

Holos has a default value: Yes 

Valid values: True, False

note: See notes in source file on line 449 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/LandManagement/Fields/CropViewItem.cs 

***
## CropIsGrazed

Example value: False 

Type value: Bool (True/False)

Does user have to provide value: No

Holos has a default value:

Valid values: True, False

note: Not used/implemented yet. Future version will utilize 

Source (source code file, table, algorithm document, etc.): 

***
## FieldSystemComponentGuid

Example value: 642a2cb7-0321-4395-9ebb-d5743c27c960 

Type value: GUID 

Does user have to provide value: No 

Holos has a default value: No 

Valid values:

note: Unique ID for each field component on the farm 

Source (source code file, table, algorithm document, etc.): 

***
## TimePeriodCategoryString

Example value: Current 

Type value: Enum (Text)

Does user have to provide value: No 

Holos has a default value: "Current" 

Valid values: Past, Current, Future

note: Used to indicate time period in field history. Leave as "Current" if not sure 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Enumerations/TimePeriodCategory.cs 

***
## ClimateParameter

Example value: 1.363 

Type value: Double (Decimal/Numeric)

Does user have to provide value:

Holos has a default value: 0/Calculated 

Valid range of values: (x ≥ 0)

note: Used with the ICBM C model. Will be generated when the user imports GUI farm files (0 otherwise). Can be set when creating new (blank) CLI input file(s) 

Source (source code file, table, algorithm document, etc.): 

***
## TillageFactor

Example value: 0.8 

Type value: Double (Decimal/Numeric)

Does user have to provide value: Default assigned but user can override 

Holos has a default value: 0/Calculated 

Valid range of values: (x ≥ 0)

note: Used with the ICBM C model. Will be generated when the user imports GUI farm files (0 otherwise). Can be set when creating new (blank) CLI input file(s). See section 2.1.1.2 in algorithm document 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/raw/refs/heads/main/Pogue%20et%20al%202025_Printversion_Holos_V4.0_Algorithm_Document.docx

***
## ManagementFactor

Example value: 1.09 

Type value: Double (Decimal/Numeric) 

Does user have to provide value: Default assigned but user can override 

Holos has a default value: 0/Calculated 

Valid range of values: (x ≥ 0)

note: Used with the ICBM C model. Will be generated when the user imports GUI farm files (0 otherwise). Can be set when creating new (blank) CLI input file(s). See section 2.1.1.2 in algorithm document 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/raw/refs/heads/main/Pogue%20et%20al%202025_Printversion_Holos_V4.0_Algorithm_Document.docx

***
## PlantCarbonInAgriculturalProduct

Example value: 1211.76 

Type value: Double (Decimal/Numeric)

Does user have to provide value:

Holos has a default value:

Valid range of values: (x ≥ 0)

note: Deprecated. Do not use. Will be removed in future version 

Source (source code file, table, algorithm document, etc.): 

***
## CarbonInputFromProduct

Example value: 24.2352 

Type value: Double (Decimal/Numeric)

Does user have to provide value:

Holos has a default value:

Valid range of values: (x ≥ 0)

note: Deprecated. Do not use. Will be removed in future version 

Source (source code file, table, algorithm document, etc.): 

***
## CarbonInputFromStraw

Example value: 2572.5068852459 

Type value: Double (Decimal/Numeric)

Does user have to provide value:

Holos has a default value:

Valid range of values: (x ≥ 0)

note: Deprecated. Do not use. Will be removed in future version 

Source (source code file, table, algorithm document, etc.): 

***
## CarbonInputFromRoots

Example value: 730.035737704918 

Type value: Double (Decimal/Numeric)

Does user have to provide value:

Holos has a default value:

Valid range of values: (x ≥ 0)

note: Deprecated. Do not use. Will be removed in future version 

Source (source code file, table, algorithm document, etc.): 

***
## CarbonInputFromExtraroots

Example value: 451.926885245902 

Type value: Double (Decimal/Numeric)

Does user have to provide value:

Holos has a default value:

Valid range of values: (x ≥ 0)

note: Deprecated. Do not use. Will be removed in future version 

Source (source code file, table, algorithm document, etc.): 

***
## SizeOfFirstRotationForField

Example value: 1

Type value: Double (Decimal/Numeric)

Does user have to provide value: Yes

Holos has a default value:

Valid range of values: (x ≥ 0)

note: Deprecated. Do not use. Will be removed in future version 

Source (source code file, table, algorithm document, etc.): 

***
## AboveGroundCarbonInput

Example value: 1650.63391153725 

Type value: Double (Decimal/Numeric)

Does user have to provide value:

Holos has a default value:

Valid range of values: (x ≥ 0)

note: Deprecated. Do not use. Will be removed in future version 

Source (source code file, table, algorithm document, etc.): 

***
## BelowGroundCarbonInput

Example value: 601.948372142029 

Type value: Double (Decimal/Numeric)

Does user have to provide value:

Holos has a default value:

Valid range of values: (x ≥ 0)

note: Deprecated. Do not use. Will be removed in future version 

Source (source code file, table, algorithm document, etc.): 

***
## ManureCarbonInputsPerHectare

Example value: 0

Type value: Double (Decimal/Numeric)

Does user have to provide value:

Holos has a default value:

Valid range of values: (x ≥ 0)

note: Deprecated. Do not use. Will be removed in future version 

Source (source code file, table, algorithm document, etc.): 

***
## DigestateCarbonInputsPerHectare

Example value: 0

Type value: Double (Decimal/Numeric)

Does user have to provide value:

Holos has a default value:

Valid range of values: (x ≥ 0)

note: Deprecated. Do not use. Will be removed in future version 

Source (source code file, table, algorithm document, etc.): 

***
## TotalCarbonInputs

Example value: 2845.39665493676 

Type value: Double (Decimal/Numeric)

Does user have to provide value:

Holos has a default value:

Valid range of values: (x ≥ 0)

note: Deprecated. Do not use. Will be removed in future version 

Source (source code file, table, algorithm document, etc.): 

***
## Sand

Example value: 0.2 

Type value: Double (Decimal/Numeric)

Does user have to provide value:

Holos has a default value:

Valid range of values: (x ≥ 0)

note: Deprecated. Do not use. Will be removed in future version 

Source (source code file, table, algorithm document, etc.): 

***
## Lignin

Example value: 0.053 

Type value: Double (Decimal/Numeric)

Does user have to provide value:

Holos has a default value:

Valid range of values: (x ≥ 0)

note: Deprecated. Do not use. Will be removed in future version 

Source (source code file, table, algorithm document, etc.): 

***
## WFac

Example value: 0

Type value: Double (Decimal/Numeric)

Does user have to provide value:

Holos has a default value:

Valid range of values: (x ≥ 0)

note: Deprecated. Do not use. Will be removed in future version 

Source (source code file, table, algorithm document, etc.): 

***
## TFac

Example value: 0

Type value: Double (Decimal/Numeric)

Does user have to provide value:

Holos has a default value:

Valid range of values: (x ≥ 0)

note: Deprecated. Do not use. Will be removed in future version 

Source (source code file, table, algorithm document, etc.): 

***
## TotalNitrogenInputsForIpccTier2

Example value: 0

Type value: Double (Decimal/Numeric)

Does user have to provide value:

Holos has a default value:

Valid range of values: (x ≥ 0)

note: Deprecated. Do not use. Will be removed in future version     

Source (source code file, table, algorithm document, etc.): 

***
## NitrogenContent

Example value: 0.007 

Type value: Double (Decimal/Numeric)

Does user have to provide value:

Holos has a default value:

Valid range of values: (x ≥ 0)

note: Deprecated. Do not use. Will be removed in future version 

Source (source code file, table, algorithm document, etc.): 

***
## AboveGroundResidueDryMatter

Example value: 3930.08074175535 

Type value: Double (Decimal/Numeric)

Does user have to provide value:

Holos has a default value:

Valid range of values: (x ≥ 0)

note: Deprecated. Do not use. Will be removed in future version 

Source (source code file, table, algorithm document, etc.): 

***
## BelowGroundResidueDryMatter

Example value: 25797.7873775155 

Type value: Double (Decimal/Numeric)

Does user have to provide value:

Holos has a default value:

Valid range of values: (x ≥ 0)

note: Deprecated. Do not use. Will be removed in future version 

Source (source code file, table, algorithm document, etc.): 

***
## FuelEnergy

Example value: 2.39 

Type value: Double (Decimal/Numeric)

Does user have to provide value: Yes 

Holos has a default value: No 

Valid range of values: (x ≥ 0)

note: See table 50 and section 6 in algorithm document 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Content/Resources/Table_50_Fuel_Energy_Requirement_Estimates_By_Region.csv, https://github.com/holos-aafc/Holos/raw/refs/heads/main/Pogue%20et%20al%202025_Printversion_Holos_V4.0_Algorithm_Document.docx

***
## HerbicideEnergy

Example value: 0.23 

Type value: Double (Decimal/Numeric)

Does user have to provide value:

Holos has a default value: No 

Valid range of values: (x ≥ 0)

note: See table 51 and section 6 in algorithm document 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Content/Resources/Table_51_Herbicide_Energy_Requirement_Estimates_By_Region.csv, https://github.com/holos-aafc/Holos/raw/refs/heads/main/Pogue%20et%20al%202025_Printversion_Holos_V4.0_Algorithm_Document.docx

***
## FertilizerBlend

Example value: Urea 

Type value: Enum (Text) 

Does user have to provide value: Yes 

Holos has a default value: No 

Valid values: Urea, Ammonia, UreaAmmoniumNitrate, AmmoniumNitrate, CalciumAmmoniumNitrate, AmmoniumSulphate, MesS15, MonoAmmoniumPhosphate, DiAmmoniumPhosphate, TripleSuperPhosphate, Potash, Npk, CalciumNitrate, AmmoniumNitroSulphate, Custom, Lime, CustomOrganic, AmmoniumNitratePrilled, AmmoniumNitrateGranulated, SuperPhosphate, NpkMixedAcid, NpkNitrophosphate, PotassiumSulphate

note: See GUI for supported list of blends 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Enumerations/FertilizerBlends.cs 

***

<br>

# Dairy
## Name

Example value: Dairy heifers

Type value: String (Text)

Does user have to provide value: Yes

Holos has a default value:

Source (source code file, table, algorithm document, etc.): 

note: Should be unique string differentiate from other components/input files

***
## ComponentType

Example value: H.Core.Models.Animals.Dairy.DairyComponent 

Type value: Enum (Text)

Does user have to provide value: Yes

Holos has a default value:

Valid values: H.Core.Models.Animals.Dairy.DairyBullComponent, H.Core.Models.Animals.Dairy.DairyCalfComponent, H.Core.Models.Animals.Dairy.DairyComponent, H.Core.Models.Animals.Dairy.DairyDryComponent, H.Core.Models.Animals.Dairy.DairyHeiferComponent, H.Core.Models.Animals.Dairy.DairyLactatingComponent     

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/ComponentType.cs


note: 

***
## GroupName

Example value: Heifers

Type value: Enum (Text)

Does user have to provide value: Yes

Holos has a default value:

Valid values: Heifers, Lactating, Calves, Dry

Source (source code file, table, algorithm document, etc.): 

note: Must be unique string differentiate from other animal groups in the same component (e.g. "Heifers group #1")

***
## GroupType

Example value: DairyHeifers

Type value: Enum (Text)

Does user have to provide value: Yes

Holos has a default value:

Valid values: Dairy, DairyBulls, DairyDryCow, DairyCalves, DairyHeifers, DairyLactatingCow

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Enumerations/AnimalType.cs, https://github.com/holos-aafc/Holos/blob/main/H.Core/Converters/AnimalTypeStringConverter.cs

note: See converter class used to convert animal type string names to enumeration values


***
## ManagementPeriodName

Example value: Management period #1 

Type value: String (Text)

Does user have to provide value: Yes

Holos has a default value:

Valid values: Must be a unique string within the animal group 

Source (source code file, table, algorithm document, etc.): 

note: Must be a unique string within the animal group

***
## ManagementPeriodStartDate

Example value: 2023-10-01 00:00:00 

Type value: strftime (Text)

Does user have to provide value: Yes

Holos has a default value:

Source (source code file, table, algorithm document, etc.): 

note: Must be set to indicate the start of management period 

***
## ManagementPeriodDays

Example value: 109

Type value: Int (Integer/Numeric)

Does user have to provide value: Yes

Holos has a default value:

Source (source code file, table, algorithm document, etc.): 

note: Must be set to indicate how long the management period lasts

***
## NumberOfAnimals

Example value: 50

Type value: Int (Integer/Numeric)

Does user have to provide value: Yes

Holos has a default value:

Source (source code file, table, algorithm document, etc.): 

note: Number of animals in the animal group

***
## ProductionStage

Example value: Lactating

Type value: Enum (Text)

Does user have to provide value: Yes

Holos has a default value:

Valid values: Gestating, Lactating, Open, Weaning, GrowingAndFinishing, BreedingStock, Weaned

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Enumerations/ProductionStages.cs 

note: Must be set to indicate which stage a group of animals are in the lifecycle of the animal group (i.e. Lactating cows will be at the lactating production stage). This is not used for all animal types

***
## NumberOfYoungAnimals

Example value: 12

Type value: Int (Integer/Numeric)

Does user have to provide value: Yes 

Holos has a default value:

Valid range of values: (x ≥ 0)

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Animals/AnimalComponentBase.cs 

note: Used to indicate how many young animals (i.e. beef calves) are associated with a parent group. See line 208 of source file on how to use 

***
## GroupPairingNumber

Example value: 0 

Type value: Int (Integer/Numeric)

Does user have to provide value: Yes

Holos has a default value:

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core.Test/Models/Animals/Beef/CowCalfComponentTest.cs

note: Used to group a parent and child group of animals. E.g. a group of lactating cows and a group of beef calves must have the same pairing number. Leave as zero when a parent/child grouping does not exist (most cases). See unit test class for example on setting this value 

***
## StartWeight(kg)

Example value: 240

Type value: Double (Decimal/Numeric)

Does user have to provide value: Yes

Holos has a default value:

Valid range of values: (x ≥ 0)

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Animals/ManagementPeriod.cs 

note: Start weight of the animals in a particular management period

***
## EndWeight(kg)

Example value: 360

Type value: Double (Decimal/Numeric)

Does user have to provide value: Yes

Holos has a default value:

Valid range of values: (x ≥ 0)

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Animals/ManagementPeriod.cs 

note: End weight of the animals in a particular management period 

***
## AverageDailyGain(kg)

Example value: 1.1

Type value: Double (Decimal/Numeric)

Does user have to provide value: Yes

Holos has a default value:

Valid range of values: (x ≥ 0)

Source (source code file, table, algorithm document, etc.): 

note: This will be a value that is calculated based on the start and end weight

***
## MilkProduction

Example value: 34.7

Type value: Double (Decimal/Numeric)

Does user have to provide value: Yes

Holos has a default value:

Valid range of values: (x ≥ 0)

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Content/Resources/Table_21_Average_Milk_Production_For_Dairy_Cows_By_Province.csv, https://github.com/holos-aafc/Holos/blob/main/H.Core/Providers/Animals/Table_21_Average_Milk_Production_Dairy_Cows_Provider.cs

note: The amount of milk produced by the group of animals

***
## MilkFatContent

Example value: 3.71

Type value: Double (Decimal/Numeric)

Does user have to provide value: Yes

Holos has a default value: 3.71

Valid range of values: (x ≥ 0)

Source (source code file, table, algorithm document, etc.): 

note: Used with dairy components. Old default value was 4.0 but has been changed

***
## MilkProteinContentAsPercentage

Example value: 3.5

Type value: Double (Decimal/Numeric)

Does user have to provide value: Yes

Holos has a default value: Yes

Valid range of values: (0 ≤ x ≤ 100)

Source (source code file, table, algorithm document, etc.): 

note: Deprecated. Do not use. Will be removed in future version

***
## DietAdditiveType

Example value: None

Type value: Enum (Text)

Does user have to provide value: Yes

Holos has a default value: No

Valid values: None, Ionophore, FivePercentFat, IonophorePlusFicePercentFat

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Enumerations/DietAdditiveType.cs

note: Optional input used to calculate enteric CH4. See GUI for supported types

***
## MethaneConversionFactorOfDiet(kgCH4(kgCH4)^-1)

Example value: 0.056

Type value: Double (Decimal/Numeric)

Does user have to provide value: Yes

Holos has a default value: Yes, based on diet 

Valid range of values: (x ≥ 0)

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Providers/Feed/DietProvider.cs 

note: Also known as Ym of diet. See source file for defaults based on diet type 

***
## MethaneConversionFactorAdjusted(%)

Example value: 0

Type value: Double (Decimal/Numeric)

Does user have to provide value: 

Holos has a default value:

Source (source code file, table, algorithm document, etc.): 

Valid range of values: (0 ≤ x ≤ 100)

note: Deprecated. Do not use. Will be removed in future version 

***
## FeedIntake(kghead^-1day^-1)

Example value: 0

Type value: Double (Decimal/Numeric)

Does user have to provide value: Yes

Holos has a default value:

Valid range of values: (x ≥ 0)

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Providers/Feed/DietProvider.cs 

note: Used with some animal types (i.e. swine types). See swine diets in source file for defaults 

***
## CrudeProtein(kgkg^-1)

Example value: 12.28

Type value: Double (Decimal/Numeric)

Does user have to provide value: Yes

Holos has a default value:

Valid range of values: (x ≥ 0)

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Content/Resources/feeds.csv 

note: Crude protein value of diet. See feed ingredient list for values 

***
## AshContentOfDiet(kgkg^-1)

Example value: 6.57 

Type value: Double (Decimal/Numeric)

Does user have to provide value: Yes

Holos has a default value:

Valid range of values: (x ≥ 0)

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Providers/Feed/Diet.cs 

note: Ash content of diet. See line 434 for more information on how to calculate averages 

***
## Forage(%DM)

Example value: 97

Type value: Double (Decimal/Numeric)

Does user have to provide value: Yes

Holos has a default value:

Valid range of values: (0 ≤ x ≤ 100)

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Content/Resources/feeds.csv

note: Forage value of diet. See feed ingredient list for values

***
## TDN(%DM)

Example value: 54.6

Type value: Double (Decimal/Numeric)

Does user have to provide value: Yes

Holos has a default value: 

Valid range of values: (0 ≤ x ≤ 100)

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Content/Resources/feeds.csv

note: TDN value of diet. See feed ingredient list tdn values

***
## Starch(%DM)

Example value: 7.1

Type value: Double (Decimal/Numeric)

Does user have to provide value: Yes

Holos has a default value: 

Valid range of values: (0 ≤ x ≤ 100)

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Content/Resources/feeds.csv

note: Starch content of diet

***
## Fat(%DM)

Example value: 1.7

Type value: Double (Decimal/Numeric)

Does user have to provide value: Yes

Holos has a default value: 

Valid range of values: (0 ≤ x ≤ 100)

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Content/Resources/feeds.csv

note: Fat content of diet

***
## ME(MJkg^-1)

Example value: 2

Type value: Double (Decimal/Numeric)

Does user have to provide value: Yes

Holos has a default value:

Valid range of values: (x ≥ 0)

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Content/Resources/feeds.csv

note: Metabolizable energy of diet

***
## NDF(%DM)

Example value: 53.5

Type value: Double (Decimal/Numeric)

Does user have to provide value: Yes

Holos has a default value:

Valid range of values: (x ≥ 0)

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Content/Resources/feeds.csv

note: Neutral detergent fibre of diet

***
## VolatileSolidAdjusted(kgkg^-1)

Example value: 

Type value: 

Does user have to provide value: 

Holos has a default value: (x ≥ 0)

Source (source code file, table, algorithm document, etc.): 4.1.2-2 in [algorithm document](https://github.com/holos-aafc/Holos/blob/main/Pogue%20et%20al%202025_Printversion_Holos_V4.0_Algorithm_Document.docx)

note: Deprecated. Do not use. Will be removed in future version

***
## NitrogenExcretionAdjusted(kgkg^-1)

Example value: 0.95

Type value: Double (Decimal/Numeric)

Does user have to provide value: 

Holos has a default value: 

Valid range of values: (0 ≤ x ≤ 1)

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Providers/Animals/Table_32_Swine_VS_Nitrogen_Excretion_Factors_Provider.cs

note: 

***
## DietaryNetEnergyConcentration

Example value: 0

Type value: Double (Decimal/Numeric)

Does user have to provide value: Yes

Holos has a default value: 

Valid range of values: (x ≥ 0)

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Providers/Feed/Diet.cs 

note: Used only for diet/DMI calculations for beef calves. See line 419 

***
## GainCoefficient

Example value: 1

Type value: Double (Decimal/Numeric)

Does user have to provide value: Yes

Holos has a default value: 

Valid range of values: (x ≥ 0)

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Animals/AnimalInitializationService.EmissionFactors.cs 

note: See line 134 for default setting 

***
## GainCoefficientA

Example value: 2.10

Type value: Double (Decimal/Numeric)

Does user have to provide value: 

Holos has a default value: 

Valid range of values: (x ≥ 0)

Source (source code file, table, algorithm document, etc.): 

note: 

***
## GainCoefficientB

Example value: 0.45

Type value: Double (Decimal/Numeric)

Does user have to provide value: 

Holos has a default value: 

Valid range of values: (x ≥ 0)

Source (source code file, table, algorithm document, etc.): 

note: 

***
## HousingType

Example value: FreeStallBarnSolidLitter

Type value: Enum (Text)

Does user have to provide value: Yes

Holos has a default value:

Valid values: FreeStallBarnSolidLitter, FreeStallBarnSlurryScraping, FreeStallBarnFlushing, FreeStallBarnMilkParlourSlurryFlushing, TieStallSolidLitter, TieStallSlurry, DryLot, Pasture

Source (source code file, table, algorithm document, etc.): 

note: 

***
## ActivityCoefficientOfFeedingSituation(MJday^-1kg^-1)

Example value: 0.1700

Type value: Double (Decimal/Numeric)

Does user have to provide value: Yes

Holos has a default value:

Valid range of values: (0 ≤ x ≤ 1)

Source (source code file, table, algorithm document, etc.): 

note: 

***
## MaintenanceCoefficient(MJday^-1kg^-1)

Example value: 0.322

Type value: Double (Decimal/Numeric)

Does user have to provide value:

Holos has a default value:

Valid range of values: (0 ≤ x ≤ 1)

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Animals/AnimalInitializationService.EmissionFactors.cs

note: See line 108 for defaults

***
## UserDefinedBeddingRate

Example value: 1.5

Type value: Double (Decimal/Numeric)

Does user have to provide value: Yes 

Holos has a default value:

Valid range of values: (x ≥ 0)

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Animals/AnimalInitializationService.Bedding.cs, https://github.com/holos-aafc/Holos/blob/main/H.Core/Providers/Animals/Table_30_Default_Bedding_Material_Composition_Provider.cs 

note: Amount of bedding added. Used in C and N input calculations. See line 52 in source and table 30 

***
## TotalCarbonKilogramsDryMatterForBedding

Example value: 0.447

Type value: Double (Decimal/Numeric)

Does user have to provide value:

Holos has a default value:

Valid range of values: (x ≥ 0)

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Animals/HousingDetails.cs 

note: See HousingDetails.cs line 186 

***
## TotalNitrogenKilogramsDryMatterForBedding

Example value: 0.0057 

Type value: Double (Decimal/Numeric)

Does user have to provide value:

Holos has a default value:

Valid range of values: (x ≥ 0)

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Animals/HousingDetails.cs

note: See HousingDetails.cs line 177

***
## MoistureContentOfBeddingMaterial

Example value: 9.57 

Type value: Double (Decimal/Numeric)

Does user have to provide value: Yes 

Holos has a default value:

Valid range of values: (x ≥ 0)

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Animals/HousingDetails.cs

note: See HousingDetails.cs line 219

***
## MethaneConversionFactorOfManure(kgCH4(kgCH4)^-1)

Example value: 0.26 

Type value: Double (Decimal/Numeric)

Does user have to provide value:

Holos has a default value:

Valid range of values: (x ≥ 0)

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Animals/AnimalInitializationService.Methane.cs 

note: Methane conversion factor of manure, not to be confused with Methane conversion factor of diet (Ym). See line 89 for defaults 

***
## N2ODirectEmissionFactor(kgN2O-N(kgN)^-1)

Example value: 0.01 

Type value: Double (Decimal/Numeric)

Does user have to provide value:

Holos has a default value:

Valid range of values: (x ≥ 0)

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Animals/AnimalInitializationService.EmissionFactors.cs 

note: See line 34 for defaults 

***
## EmissionFactorVolatilization

Example value: 0.005 

Type value: Double (Decimal/Numeric)

Does user have to provide value:

Holos has a default value:

Valid range of values: (x ≥ 0)

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Animals/AnimalInitializationService.EmissionFactors.cs 

note: See line 34 for defaults 

***
## VolatilizationFraction

Example value: 0.25

Type value: Double (Decimal/Numeric)

Does user have to provide value:

Holos has a default value:

Valid range of values: (x ≥ 0)

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Animals/AnimalInitializationService.EmissionFactors.cs 

note: See line 34 for defaults 

***
## EmissionFactorLeaching

Example value: 0.011 

Type value: Double (Decimal/Numeric)

Does user have to provide value:

Holos has a default value:

Valid range of values: (x ≥ 0)

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Animals/AnimalInitializationService.EmissionFactors.cs 

note: See line 34 for defaults 

***
## FractionLeaching

Example value: 0.035 

Type value: Double (Decimal/Numeric)

Does user have to provide value:

Holos has a default value:

Valid range of values: (x ≥ 0)

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Animals/AnimalInitializationService.Ammonia.cs 

note: See line 55 for defaults 

***
## AshContent(%)

Example value: 8

Type value: Double (Decimal/Numeric)

Does user have to provide value:

Holos has a default value:

Valid range of values: (0 ≤ x ≤ 100)

Source (source code file, table, algorithm document, etc.): 

note: Deprecated. Do not use. Will be removed in future version 

***
## MethaneProducingCapacityOfManure

Example value: 0.19

Type value: Double (Decimal/Numeric)

Does user have to provide value:

Holos has a default value:

Valid range of values: (x ≥ 0)

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Animals/AnimalInitializationService.Methane.cs 

note: Also known as Bo. See line 89 for defaults 

***

<br>

# Beef
## Name

Example value: Beef Stockers & Backgrounders 

Type Value: String (Text)

Does user have to provide value: Yes

Holos has a default value:

note: Should be unique string differentiate from other components/input files 

Source (source code file, table, algorithm document, etc.): 

***
## ComponentType

Example value: H.Core.Models.Animals.Beef.BackgroundingComponent 

Type Value: Enum (Text) 

Does user have to provide value: Yes

Holos has a default value:

Valid values: H.Core.Models.Animals.Beef.BackgroundingComponent, H.Core.Models.Animals.Beef.CowCalfComponent, H.Core.Models.Animals.Beef.FinishingComponent

note: 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/ComponentType.cs 

***
## GroupName

Example value: Heifers 

Type Value: Enum (Text) 

Does user have to provide value: Yes

Holos has a default value:

Valid values: Bulls, Heifers, Cows, Calves, Steers

note: Must be unique string differentiate from other animal groups in the same component (e.g. "Bulls group #1") 

Source (source code file, table, algorithm document, etc.): 

***
## GroupType

Example value: BeefBackgrounderHeifer 

Type Value: Enum (Text) 

Does user have to provide value: Yes 

Holos has a default value:

Valid values: BeefBackgrounder, BeefBackgrounderSteer, BeefBackgrounderHeifer, BeefFinishingSteer, BeefFinishingHeifer, Beef, BeefBulls, BeefCalf, BeefCowLactating, BeefCowDry, BeefFinisher, BeefCow

note: See converter class used to convert animal type string names to enumeration values 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Enumerations/AnimalType.cs, https://github.com/holos-aafc/Holos/blob/main/H.Core/Converters/AnimalTypeStringConverter.cs 

***
## ManagementPeriodName

Example value: Management period #1 

Type Value: String (Text)

Does user have to provide value: Yes

Holos has a default value:

Valid values:

note: Must be a unique string within the animal group 

Source (source code file, table, algorithm document, etc.): 

***
## GroupPairingNumber

Example value: 0

Type Value: Int (Integer/Numeric)

Does user have to provide value: Yes

Holos has a default value:

Valid range of values:

note: Used to group a parent and child group of animals. E.g. a group of lactating cows and a group of beef calves must have the same pairing number. Leave as zero when a parent/child grouping does not exist (most cases). See unit test class for example on setting this value 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core.Test/Models/Animals/Beef/CowCalfComponentTest.cs 

***
## ManagementPeriodStartDate

Example value: 2023-10-01 00:00:00 

Type Value: strftime (Text)

Does user have to provide value: Yes

Holos has a default value:

Valid values:

note: Must be set to indicate the start of management period 

Source (source code file, table, algorithm document, etc.): 

***
## ManagementPeriodDays

Example value: 110

Type Value: Int (Integer/Numeric)

Does user have to provide value: Yes

Holos has a default value:

Valid range of values: (x ≥ 0)

note: Must be set to indicate how long the management period lasts 

Source (source code file, table, algorithm document, etc.): 

***
## NumberOfAnimals

Example value: 100

Type Value: Int (Integer/Numeric) 

Does user have to provide value: Yes

Holos has a default value:

note: Number of animals in the animal group 

Valid range of values: (x ≥ 0)

Source (source code file, table, algorithm document, etc.): 

***
## ProductionStage

Example value:  

Type Value: Enum (Text)

Does user have to provide value:

Holos has a default value:

Valid values: 

note: Must be set to indicate which stage a group of animals are in the lifecycle of the animal group (i.e. Lactating cows will be at the lactating production stage). This is not used for all animal types 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Enumerations/ProductionStages.cs 

***
## NumberOfYoungAnimals

Example value: 0

Type Value: Int (Integer/Numeric)

Does user have to provide value: Yes

Holos has a default value: 

Valid range of values: (x ≥ 0)

note: Used to indicate how many young animals (i.e. beef calves) are associated with a parent group. See line 208 of source file on how to use 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Animals/AnimalComponentBase.cs 

***
## AnimalsAreMilkFedOnly

Example value: False 

Type value: Bool (True/False)

Does user have to provide value: Yes

Holos has a default value: 

Valid values: True/False

note: Use to specify that a group of animals are on a milk diet only. Used when referring to a group of young animals that are suckling/nursing 

Source (source code file, table, algorithm document, etc.): 

***
## StartWeight

Example value: 240

Type Value: Double (Decimal/Numeric) 

Does user have to provide value: Yes

Holos has a default value: 

Valid range of values: (x ≥ 0)

note: Start weight of the animals in a particular management period 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Animals/ManagementPeriod.cs 

***
## EndWeight

Example value: 361

Type Value: Double (Decimal/Numeric) 

Does user have to provide value: Yes

Holos has a default value: 

Valid range of values: (x ≥ 0)

note: End weight of the animals in a particular management period 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Animals/ManagementPeriod.cs 

***
## AverageDailyGain

Example value: 1.1 

Type Value: Double (Decimal/Numeric) 

Does user have to provide value: Yes

Holos has a default value: 

Valid range of values: (x ≥ 0)

note: This will be a value that is calculated based on the start and end weight 

Source (source code file, table, algorithm document, etc.): 

***
## MilkProduction

Example value: 0

Type Value: Double (Decimal/Numeric) 

Does user have to provide value: Yes

Holos has a default value: 

Valid range of values: (x ≥ 0)

note: The amount of milk produced by the group of animals 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Content/Resources/Table_21_Average_Milk_Production_For_Dairy_Cows_By_Province.csv, https://github.com/holos-aafc/Holos/blob/main/H.Core/Providers/Animals/Table_21_Average_Milk_Production_Dairy_Cows_Provider.cs 

***
## MilkFatContent

Example value: 4

Type Value: Double (Decimal/Numeric)  

Does user have to provide value: Yes

Holos has a default value:  3.71 

Valid range of values: (x ≥ 0)

note: Used with dairy components. Old default value was 4 but has been changed 

Source (source code file, table, algorithm document, etc.): 

***
## MilkProteinContentAsPercentage

Example value: 3.5 

Type Value: Double (Decimal/Numeric)  

Does user have to provide value: Yes

Holos has a default value: Yes

Valid range of values: (x ≥ 0)

note: Deprecated. Do not use. Will be removed in future version 

Source (source code file, table, algorithm document, etc.): 

***
## DietAdditiveType

Example value: None

Type Value: Enum (Text) 

Does user have to provide value: Yes

Holos has a default value: No 

Valid values: None, TwoPercentFat, FourPercentFat, Inonophore, InonophorePlusTwoPercentFat, InonophorePlusFourPercentFat, Custon, FivePercentFat, InonophorePlusFivePercentFat

note: Optional input used to calculate enteric CH4. See GUI for supported types 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Enumerations/DietAdditiveType.cs 

***
## MethaneConversionFactorOfDiet

Example value: 0.063 

Type Value: Double (Decimal/Numeric)   

Does user have to provide value:

Holos has a default value: Yes, based on diet 

Valid range of values: (x ≥ 0)

note: Also known as Ym of diet. See source file for defaults based on diet type 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Providers/Feed/DietProvider.cs 

***
## MethaneConversionFactorAdjusted

Example value: 0

Type Value: Double (Decimal/Numeric)  

Does user have to provide value:

Holos has a default value:

Valid range of values: (x ≥ 0)

note: Deprecated. Do not use. Will be removed in future version 

Source (source code file, table, algorithm document, etc.): 

***
## FeedIntake

Example value: 0

Type Value: Double (Decimal/Numeric) 

Does user have to provide value: Yes

Holos has a default value:

Valid range of values: (x ≥ 0)

note: Used with some animal types (i.e. swine types). See swine diets in source file for defaults 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Providers/Feed/DietProvider.cs 

***
## CrudeProtein

Example value: 12.28 

Type Value: Double (Decimal/Numeric)

Does user have to provide value: Yes

Holos has a default value:

Valid range of values: (x ≥ 0)

note: Crude protein value of diet. See feed ingredient list for values 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Content/Resources/feeds.csv 

***
## Forage

Example value: 65

Type Value: Double (Decimal/Numeric) 

Does user have to provide value: Yes

Holos has a default value:

Valid range of values: (x ≥ 0)

note: Forage value of diet. See feed ingredient list for values 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Content/Resources/feeds.csv 

***
## TDN

Example value: 68.825 

Type Value: Double (Decimal/Numeric)  

Does user have to provide value: Yes

Holos has a default value:

Valid range of values: (x ≥ 0)

note: TDN value of diet. See feed ingredient list tdn values 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Content/Resources/feeds.csv 

***
## AshContentOfDiet

Example value: 6.57 

Type Value: Double (Decimal/Numeric)  

Does user have to provide value: Yes

Holos has a default value:

Valid range of values: (x ≥ 0)

note: Ash content of diet. See line 434 for more informtation on how to calculate averages 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Providers/Feed/Diet.cs 

***
## Starch

Example value: 25.825 

Type Value: Double (Decimal/Numeric)  

Does user have to provide value: Yes

Holos has a default value:

Valid range of values: (x ≥ 0)

note: Starch content of diet 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Content/Resources/feeds.csv 

***
## Fat

Example value: 3.045 

Type Value: Double (Decimal/Numeric)  

Does user have to provide value: Yes

Holos has a default value:

Valid range of values: (x ≥ 0)

note: Fat content of diet 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Content/Resources/feeds.csv 

***
## ME

Example value: 2.48 

Type Value: Double (Decimal/Numeric)   

Does user have to provide value: Yes 

Holos has a default value:

Valid range of values: (x ≥ 0)

note: Metabolizable energy of diet 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Content/Resources/feeds.csv 

***
## NDF

Example value: 42.025 

Type Value: Double (Decimal/Numeric)    

Does user have to provide value: Yes 

Holos has a default value:

Valid range of values: (x ≥ 0)

note: Neutral detergent fibre of diet 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Content/Resources/feeds.csv 

***
## DietaryNetEnergyConcentration

Example value: 0

Type Value: Double (Decimal/Numeric)

Does user have to provide value: Yes 

Holos has a default value:

Valid range of values: (x ≥ 0)

note: Used only for diet/DMI calculations for beef calves. See line 419 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Providers/Feed/Diet.cs 

***
## HousingType

Example value: ConfinedNoBarn 

Type Value: String (Text)

Does user have to provide value: Yes 

Holos has a default value:

Valid values: ConfinedNoBarn, HousedInBarnSolid, Pasture

note: Required field used for many calculations. See GUI for correct types when considering a particular animal type 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Enumerations/HousingType.cs 

***
## GainCoefficient

Example value: 1

Type Value: Double (Decimal/Numeric) 

Does user have to provide value: Yes 

Holos has a default value:

Valid range of values: (0 ≤ x ≤ 1)

note: See line 134 for default setting 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Animals/AnimalInitializationService.EmissionFactors.cs 

***
## UserDefinedBeddingRate

Example value: 1.5 

Type Value: Double (Decimal/Numeric) 

Does user have to provide value: Yes 

Holos has a default value: 

Valid range of values: (x ≥ 0)

note: Amount of bedding added. Used in C and N input calculations. See line 52 in source and table 30 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Animals/AnimalInitializationService.Bedding.cs, https://github.com/holos-aafc/Holos/blob/main/H.Core/Providers/Animals/Table_30_Default_Bedding_Material_Composition_Provider.cs 

***
## TotalCarbonKilogramsDryMatterForBedding

Example value: 0.447 

Type Value: Double (Decimal/Numeric) 

Does user have to provide value:

Holos has a default value:

Valid range of values: (x ≥ 0)

note: See HousingDetails.cs line 186 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Animals/HousingDetails.cs 

***
## TotalNitrogenKilogramsDryMatterForBedding

Example value: 0.0057 

Type Value: Double (Decimal/Numeric)  

Does user have to provide value:

Holos has a default value:

Valid range of values: (x ≥ 0)

note: See HousingDetails.cs line 177 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Animals/HousingDetails.cs 

***
## MoistureContentOfBeddingMaterial

Example value: 9.57 

Type Value: Double (Decimal/Numeric)   

Does user have to provide value:

Holos has a default value:

Valid range of values: (x ≥ 0)

note: See HousingDetails.cs line 219 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Animals/HousingDetails.cs 

***
## ActivityCoefficientOfFeedingSituation

Example value: 0

Type Value: Double (Decimal/Numeric)   

Does user have to provide value:

Holos has a default value:

Valid range of values: (0 ≤ x ≤ 1)

note: See line 74 for defaults 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Animals/AnimalInitializationService.EmissionFactors.cs 

***
## MaintenanceCoefficient

Example value: 0.322 

Type Value: Double (Decimal/Numeric)    

Does user have to provide value:

Holos has a default value:

Valid range of values: (0 ≤ x ≤ 1)

note: See line 108 for defaults 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Animals/AnimalInitializationService.EmissionFactors.cs 

***
## MethaneConversionFactorOfManure

Example value: 0.26 

Type Value: Double (Decimal/Numeric)     

Does user have to provide value:

Holos has a default value:

Valid range of values: (x ≥ 0)

note: Methane conversion factor of manure, not to be consufed with Methane conversion factor of diet (Ym). See line 89 for defaults 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Animals/AnimalInitializationService.Methane.cs 

***
## N2ODirectEmissionFactor

Example value: 0.01 

Type Value: Double (Decimal/Numeric)   

Does user have to provide value:

Holos has a default value:

Valid range of values: (x ≥ 0)

note: See line 34 for defaults 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Animals/AnimalInitializationService.EmissionFactors.cs 

***
## EmissionFactorVolatilization

Example value: 0.005 

Type Value: Double (Decimal/Numeric)   

Does user have to provide value:

Holos has a default value:

Valid range of values: (x ≥ 0)

note: See line 34 for defaults 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Animals/AnimalInitializationService.EmissionFactors.cs 

***
## VolatilizationFraction

Example value: 0.25 

Type Value: Double (Decimal/Numeric)    

Does user have to provide value:

Holos has a default value:

Valid range of values: (x ≥ 0)

note: See line 34 for defaults 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Animals/AnimalInitializationService.EmissionFactors.cs 

***
## EmissionFactorLeaching

Example value: 0.011 

Type Value: Double (Decimal/Numeric)    

Does user have to provide value:

Holos has a default value:

Valid range of values: (x ≥ 0)

note: See line 34 for defaults 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Animals/AnimalInitializationService.EmissionFactors.cs 

***
## FractionLeaching

Example value: 0.035 

Type Value: Double (Decimal/Numeric)    

Does user have to provide value:

Holos has a default value:

Valid range of values: (x ≥ 0)

note: See line 55 for defaults 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Animals/AnimalInitializationService.Ammonia.cs 

***
## AshContent

Example value: 8

Type Value: Double (Decimal/Numeric)    

Does user have to provide value:

Holos has a default value:

Valid range of values: (x ≥ 0)

note: Deprecated. Do not use. Will be removed in future version 

Source (source code file, table, algorithm document, etc.): 

***
## MethaneProducingCapacityOfManure

Example value: 0.19 

Type Value: Double (Decimal/Numeric)    

Does user have to provide value:

Holos has a default value:

Valid range of values: (x ≥ 0)

note: Also known as Bo. See line 89 for defaults 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Animals/AnimalInitializationService.Methane.cs 

***
## FractionOfOrganicNitrogenImmobilized

Example value: 0

Type Value: Double (Decimal/Numeric)  

Does user have to provide value:

Holos has a default value: 0

Valid range of values: (0 ≤ x ≤ 1)

note: See line 31 for defaults 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Animals/AnimalInitializationService.Ammonia.cs 

***
## FractionOfOrganicNitrogenNitrified

Example value: 0.125 

Type Value: Double (Decimal/Numeric)  

Does user have to provide value:

Holos has a default value:

Valid range of values:

note: See line 31 for defaults 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Animals/AnimalInitializationService.Ammonia.cs 

***
## FractionOfOrganicNitrogenMineralized

Example value: 0.28 

Type Value: Double (Decimal/Numeric)   

Does user have to provide value:

Holos has a default value:

Valid range of values:

note: See line 31 for defaults 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Animals/AnimalInitializationService.Ammonia.cs 

***
## ManureStateType

Example value: DeepBedding 

Type Value: Enum (Text)

Does user have to provide value: Yes

Holos has a default value:

Valid values: DeepBedding, SolidStorage, Pasture, CompostPassive, CompostIntensive

note: Required. See GUI for valid types for particular animal type 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Enumerations/ManureStateType.cs 

***
## AmmoniaEmissionFactorForManureStorage

Example value: 0.35 

Type Value: Double (Decimal/Numeric)  

Does user have to provide value:

Holos has a default value:

Valid range of values:

note: For poultry animals only. See line 7 in source 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Providers/Animals/DefaultAmmoniaEmissionFactorsForPoultryManureStorageProvider.cs 

***

<br>

# Sheep
## Name

Example value: 

Value Type: String (Text) 

Does user have to provide value: Yes 

Holos has a default value: Yes

Source (source code file, table, algorithm document, etc.): 

note: 

***
## ComponentType

Example value: H.Core.Models.Animals.Sheep.SheepComponent

Value Type: Enum (String)

Does user have to provide value: Yes

Holos has a default value: 

Valid values: H.Core.Models.Animals.Sheep.SheepComponent, H.Core.Models.Animals.Sheep.RamsComponent, H.Core.Models.Animals.Sheep.SheepFeedlotComponent, H.Core.Models.Animals.Sheep.EwesAndLambsComponent

Source (source code file, table, algorithm document, etc.): 

note: 

***
## GroupName

Example value: Sheep

Value Type: Enum (Text)

Does user have to provide value: Yes 

Holos has a default value: 

Valid values: Sheep, Sheep Feedlot, Ram, Lambs, Ewes

Source (source code file, table, algorithm document, etc.): 

note: Must be unique string differentiate from other animal groups in the same component (e.g. "Sheep group #1")


***
## GroupType

Example value: SheepFeedlot

Value Type: Enum (Text) 

Does user have to provide value: Yes

Holos has a default value: 

Valid values: Sheep, SheepFeedlot, Ram, Ewes, Lambs

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Enumerations/AnimalType.cs, https://github.com/holos-aafc/Holos/blob/main/H.Core/Converters/AnimalTypeStringConverter.cs

note: See converter class used to convert animal type string names to enumeration values

***
## ManagementPeriodName

Example value: Management period #1

Value Type: String (Text) 

Does user have to provide value: Yes 

Holos has a default value: 

Valid values: Must be a unique string within the animal group

Source (source code file, table, algorithm document, etc.): 

note: Must be a unique string within the animal group

***
## GroupPairingNumber

Example value: 0

Value Type: Int (Integer/Numeric)

Does user have to provide value: Yes 

Holos has a default value: 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core.Test/Models/Animals/Beef/CowCalfComponentTest.cs

note: Used to group a parent and child group of animals. E.g. a group of lactating cows and a group of beef calves must have the same pairing number. Leave as zero when a parent/child grouping does not exist (most cases). See unit test class for example on setting this value



***
## ManagementPeriodStartDate

Example value: 2023-10-01 00:00:00

Value Type: strftime 

Does user have to provide value: Yes 

Holos has a default value: 

Source (source code file, table, algorithm document, etc.): 

note: Must be set to indicate the start of management period

***
## ManagementPeriodDays

Example value: 109

Value Type: Int (Integer/Numeric)

Does user have to provide value: Yes

Holos has a default value: 

Source (source code file, table, algorithm document, etc.): 

note: Must be set to indicate how long the management period lasts

***
## NumberOfAnimals

Example value: 50

Value Type: Int (Integer/Numeric)

Does user have to provide value: Yes 

Holos has a default value: 

Valid range of values: (x ≥ 0)

Source (source code file, table, algorithm document, etc.): 

note: Number of animals in the animal group

***
## ProductionStage

Example value: Lactating

Value Type: Enum (Text) 

Does user have to provide value: Yes

Holos has a default value: 

Valid values: Gestating, Lactating

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Enumerations/ProductionStages.cs

note: Must be set to indicate which stage a group of animals are in the lifecycle of the animal group (i.e. Lactating cows will be at the lactating production stage). This is not used for all animal types

***
## NumberOfYoungAnimals

Example value: 12

Value Type: Int (Integer/Numeric)

Does user have to provide value: Yes

Holos has a default value: 

Valid range of values: (x ≥ 0)

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Animals/AnimalComponentBase.cs

note: Used to indicate how many young animals (i.e. beef calves) are associated with a parent group. See line 208 of source file on how to use

***
## StartWeight(kg)

Example value: 240

Value Type: Double (Decimal/Numeric) 

Does user have to provide value: Yes

Holos has a default value: 

Valid range of values: (x ≥ 0)

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Animals/ManagementPeriod.cs

note: Start weight of the animals in a particular management period

***
## EndWeight(kg)

Example value: 360

Value Type: Double (Decimal/Numeric) 

Does user have to provide value: Yes 

Holos has a default value: 

Valid range of values: (x ≥ 0)

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Animals/ManagementPeriod.cs

note: End weight of the animals in a particular management period

***
## AverageDailyGain(kg)

Example value: 1.1

Value Type: Double (Decimal/Numeric) 

Does user have to provide value: Yes 

Holos has a default value: 

Valid range of values: (x ≥ 0)

Source (source code file, table, algorithm document, etc.): 

note: This will be a value that is calculated based on the start and end weight

***
## EnergyRequiredToProduceWool(MJkg^-1)

Example value: 24

Value Type: Double (Decimal/Numeric)  

Does user have to provide value: 

Holos has a default value: 

Valid range of values: (x ≥ 0)

Source (source code file, table, algorithm document, etc.): 

note: 

***
## WoolProduction(kgyear^-1)

Example value: 4

Value Type: Double (Decimal/Numeric) 

Does user have to provide value: 

Holos has a default value: 

Valid range of values: (x ≥ 0)

Source (source code file, table, algorithm document, etc.): 

note: 

***
## EnergyRequiredToProduceMilk(MJkg^-1)

Example value: 24

Value Type: Double (Decimal/Numeric) 

Does user have to provide value: 

Holos has a default value: 

Valid range of values: (x ≥ 0)

Source (source code file, table, algorithm document, etc.): 

note: 

***
## DietAdditiveType

Example value: None

Value Type: Enum (Text) 

Does user have to provide value: Yes 

Holos has a default value: No

Valid values: None, Ionophore, FivePercentFat, IonophorePlusFicePercentFat

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Enumerations/DietAdditiveType.cs

note: Optional input used to calculate enteric CH4. See GUI for supported types

***
## MethaneConversionFactorOfDiet(kgCH4(kgCH4)^-1)

Example value: 0.056

Value Type: Double (Decimal/Numeric) 

Does user have to provide value: Yes

Holos has a default value: Yes, based on diet

Valid range of values: (x ≥ 0)

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Providers/Feed/DietProvider.cs

note: Also known as Ym of diet. See source file for defaults based on diet type

***
## MethaneConversionFactorAdjusted(%)

Example value: 0

Value Type: Double (Decimal/Numeric) 

Does user have to provide value: 

Holos has a default value: 

Valid range of values: (x ≥ 0)

Source (source code file, table, algorithm document, etc.): 

note: Deprecated. Do not use. Will be removed in future version

***
## FeedIntake(kghead^-1day^-1)

Example value: 0

Value Type: Double (Decimal/Numeric) 

Does user have to provide value: Yes

Holos has a default value: 

Valid range of values: (x ≥ 0)

Source (source code file, table, algorithm document, etc.):  https://github.com/holos-aafc/Holos/blob/main/H.Core/Providers/Feed/DietProvider.cs

note: Used with some animal types (i.e. swine types). See swine diets in source file for defaults

***
## CrudeProtein(kgkg^-1)

Example value: 12.28

Value Type: Double (Decimal/Numeric) 

Does user have to provide value: Yes 

Holos has a default value: 

Valid range of values: (x ≥ 0)

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Content/Resources/feeds.csv

note: Crude protein value of diet. See feed ingredient list for values

***
## Forage(%DM)

Example value: 97

Value Type: Double (Decimal/Numeric) 

Does user have to provide value: Yes 

Holos has a default value: 

Valid range of values: (0 ≤ x ≤ 100)

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Content/Resources/feeds.csv

note: Forage value of diet. See feed ingredient list for values

***
## TDN(%DM)

Example value: 54.6

Value Type: Double (Decimal/Numeric) 

Does user have to provide value: Yes

Holos has a default value: 

Valid range of values: (0 ≤ x ≤ 100)

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Content/Resources/feeds.csv

note: TDN value of diet. See feed ingredient list tdn values

***
## AshContentOfDiet(%DM)

Example value: 6.57

Value Type: Double (Decimal/Numeric) 

Does user have to provide value: Yes 

Holos has a default value: 

Valid range of values: (0 ≤ x ≤ 100)

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Providers/Feed/Diet.cs

note: Ash content of diet. See line 434 for more information on how to calculate averages

***
## Starch(%DM)

Example value: 7.1

Value Type: Double (Decimal/Numeric) 

Does user have to provide value: Yes

Holos has a default value: 

Valid range of values: (0 ≤ x ≤ 100)

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Content/Resources/feeds.csv

note: Starch content of diet

***
## Fat(%DM)

Example value: 1.7

Value Type: Double (Decimal/Numeric) 

Does user have to provide value: Yes 

Holos has a default value: 

Valid range of values: (0 ≤ x ≤ 100)

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Content/Resources/feeds.csv

note: Fat content of diet

***
## ME(MJkg^-1)

Example value: 2

Value Type: Double (Decimal/Numeric) 

Does user have to provide value: Yes

Holos has a default value: 

Valid range of values: (x ≥ 0)

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Content/Resources/feeds.csv

note: Metabolizable energy of diet

***
## NDF(%DM)

Example value: 53.5 

Value Type: Double (Decimal/Numeric) 

Does user have to provide value: Yes 

Holos has a default value: 

Valid range of values: (0 ≤ x ≤ 100)

Source (source code file, table, algorithm document, etc.): 

note: 

***
## GainCoefficientA

Example value: 2.10

Value Type: Double (Decimal/Numeric)  

Does user have to provide value: 

Holos has a default value: 

Valid range of values: (x ≥ 0)

Source (source code file, table, algorithm document, etc.): 

note: 

***
## GainCoefficientB

Example value: 0.45

Value Type: Double (Decimal/Numeric) 

Does user have to provide value: 

Holos has a default value: 

Valid range of values: (x ≥ 0)

Source (source code file, table, algorithm document, etc.): 

note: 

***
## ActivityCoefficientOfFeedingSituation(MJday^-1kg^-1)

Example value: 0.1700

Value Type: Double (Decimal/Numeric) 

Does user have to provide value: Yes

Holos has a default value: 

Valid range of values: (0 ≤ x ≤ 1)

Source (source code file, table, algorithm document, etc.): 

note: 

***
## MaintenanceCoefficient(MJday^-1kg^-1)

Example value: 0.322

Value Type: flDouble (Decimal/Numeric)oat 

Does user have to provide value: 

Holos has a default value: 

Valid range of values: (0 ≤ x ≤ 1)

Source (source code file, table, algorithm document, etc.): 

note: 

***
## UserDefinedBeddingRate

Example value: 0

Value Type: Double (Decimal/Numeric) 

Does user have to provide value: Yes

Holos has a default value: 

Valid range of values: (0 ≤ x ≤ 1)

Source (source code file, table, algorithm document, etc.): 

note: 

***
## TotalCarbonKilogramsDryMatterForBedding

Example value: 0.447

Value Type: Double (Decimal/Numeric) 

Does user have to provide value: 

Holos has a default value: 

Valid range of values: (x ≥ 0)

Source (source code file, table, algorithm document, etc.): 

note: 

***
## TotalNitrogenKilogramsDryMatterForBedding

Example value: 0.0057

Value Type: Double (Decimal/Numeric) 

Does user have to provide value: 

Holos has a default value: 

Valid range of values: (x ≥ 0)

Source (source code file, table, algorithm document, etc.): 

note: 

***
## MoistureContentOfBeddingMaterial

Example value: 9.57

Value Type: Double (Decimal/Numeric) 

Does user have to provide value: 

Holos has a default value: 

Valid range of values: (x ≥ 0)

Source (source code file, table, algorithm document, etc.): 

note: 

***
## MethaneConversionFactorOfManure(kgCH4(kgCH4)^-1)

Example value: 0.26

Value Type: Double (Decimal/Numeric) 

Does user have to provide value: 

Holos has a default value: 

Valid range of values: (x ≥ 0)

Source (source code file, table, algorithm document, etc.): 

note: 

***
## N2ODirectEmissionFactor(kgN2O-N(kgN)^-1)

Example value: 0.01

Value Type: Double (Decimal/Numeric) 

Does user have to provide value: 

Holos has a default value: 

Valid range of values: (x ≥ 0)

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Animals/AnimalInitializationService.EmissionFactors.cs

note: See line 34 for defaults

***
## EmissionFactorVolatilization

Example value: 0.005

Value Type: Double (Decimal/Numeric) 

Does user have to provide value: 

Holos has a default value: 

Valid range of values: (x ≥ 0)

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Animals/AnimalInitializationService.EmissionFactors.cs

note: See line 34 for defaults

***
## VolatilizationFraction

Example value: 0.25

Value Type: Double (Decimal/Numeric) 

Does user have to provide value: 

Holos has a default value: 

Valid range of values: (x ≥ 0)

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Animals/AnimalInitializationService.EmissionFactors.cs

note: See line 34 for defaults

***
## EmissionFactorLeaching

Example value: 0.011

Value Type: Double (Decimal/Numeric) 

Does user have to provide value: 

Holos has a default value: 

Valid range of values: (x ≥ 0)

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Animals/AnimalInitializationService.EmissionFactors.cs

note: See line 34 for defaults

***
## FractionLeaching

Example value: 0.035

Value Type: Double (Decimal/Numeric) 

Does user have to provide value: 

Holos has a default value: 

Valid range of values: (x ≥ 0)

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Animals/AnimalInitializationService.Ammonia.cs

note: 

***
## AshContent(%)

Example value: 8.00

Value Type: Double (Decimal/Numeric) 

Does user have to provide value: 

Holos has a default value: 

Valid range of values: (x ≥ 0)

Source (source code file, table, algorithm document, etc.): 

note: 

***
## MethaneProducingCapacityOfManure

Example value: 0.19

Value Type: Double (Decimal/Numeric) 

Does user have to provide value: Yes

Holos has a default value: Yes

Valid range of values: (x ≥ 0)

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Animals/ManureDetails.cs

note: 

***
## ManureExcretionRate

Example value: 1.23

Value Type: double (Decimal/Numeric) 

Does user have to provide value: Yes

Holos has a default value: Yes

Possible Values: (0 < x)

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Animals/ManureDetails.cs

note: 

***
## FractionOfCarbonInManure

Example value: 6.182

Value Type: double (Decimal/Numeric)

Does user have to provide value: Yes

Holos has a default value: Yes

Possible Values: (0 < x < 100)

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Animals/ManureDetails.cs

note: 

***

<br>

# Farm Setting
## PolygonID:

Example value: 851003

Value type: Int (Integer/Numeric) 

Does user have to provide value: Yes

Holos has a default value: No

Possible Values:

note: Required. Use GUI map view to get polygon number if needed 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Providers/Soil/SoilData.cs

***
## YieldAssignmentMethod

Example value: SmallAreaData 

Value type: enum 

Does user have to provide value: Yes

Holos has a default value: Yes

Possible Values: Average, Custom, CARValue, InputFile, InputFileThenAverage, SmallAreaData

note: Used to lookup default yields for a particular year and crop type 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Enumerations/YieldAssignmentMethod.cs 

***
## PolygonNumber

Example value: 851003

Value type: Int (Integer/Numeric) 

Does user have to provide value: Yes

Holos has a default value: No

Possible Values: 

note: Required. Use GUI map view to get polygon number if needed 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Providers/Soil/SmallAreaYieldData.cs

***
## Latitude

Example value: 49.9805772869656 

Value type: double (Decimal/Numeric) 

Does user have to provide value: Yes

Holos has a default value: Yes

Possible Values: Any unrestricted value

note: Required if using NASA as the source for climate data. Values are determined when user clicks on a location in the GUI 

Source (source code file, table, algorithm document, etc.): 

***
## Longitude

Example value: -98.0433082580568 

Value type: double (Decimal/Numeric) 

Does user have to provide value: Yes

Holos has a default value: Yes

Possible Values: Any unrestricted value

note: Required if using NASA as the source for climate data. Values are determined when user clicks on a location in the GUI 

Source (source code file, table, algorithm document, etc.): 

***
## CarbonConcentration(kgkg^-1)

Example value: 0.45 

Value type: float 

Does user have to provide value: Yes

Holos has a default value: Yes

Possible Values: 0 < x 

note: See line 90 in source 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/CoreConstants.cs 

***
## EmergenceDay

Example value: 141

Value type: Int (Integer/Numeric)

Does user have to provide value: Yes

Holos has a default value: Yes

Possible Values: 0 < x 

note: Used with ICBM carbon model. See line 167 for default 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Defaults.cs 

***
## RipeningDay

Example value: 197

Value type: Int (Integer/Numeric)

Does user have to provide value: Yes

Holos has a default value: Yes

Possible Values: 0 < x 

note: Used with ICBM carbon model. See line 168 for default 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Defaults.cs 

***
## Variance

Example value: 300

Value type: double (Decimal/Numeric) 

Does user have to provide value: Yes

Holos has a default value: Yes

Possible Values: 0 < x 

note: Used with ICBM carbon model. See line 169 for default 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Defaults.cs 

***
## Alfa

Example value: 0.7 

Value type: double (Decimal/Numeric) 

Does user have to provide value: Yes

Holos has a default value: Yes

Possible Values: 0 < x 

note: Used with ICBM carbon model. See line 175 for default 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Defaults.cs 

***
## DecompositionMinimumTemperature( C)=-3.78

Example value: -3.78 

Value type: double (Decimal/Numeric) 

Does user have to provide value: Yes

Holos has a default value: Yes

Possible Values: Any Unrestricted value

note: Used with ICBM carbon model. See line 176 for default 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Defaults.cs 

***
## DecompositionMaximumTemperature( C)=

Example value: 30

Value type: double (Decimal/Numeric) 

Does user have to provide value: Yes

Holos has a default value: Yes

Possible Values: Any unrestricted value

note: Used with ICBM carbon model. See line 177 for default 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Defaults.cs 

***
## MoistureResponseFunctionAtSaturation

Example value: 0.42 

Value type: double (Decimal/Numeric) 

Does user have to provide value: Yes

Holos has a default value: Yes

Possible Values: 0 < x 

note: Used with ICBM carbon model. See line 178 for default 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Defaults.cs 

***
## MoistureResponseFunctionAtWiltingPoint

Example value: 0.18 

Value type: double (Decimal/Numeric) 

Does user have to provide value: Yes

Holos has a default value: Yes

Possible Values: 0 < x

note: Used with ICBM carbon model. See line 179 for default 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Defaults.cs 

***
## PercentageOfProductReturnedToSoilForAnnuals2

Example value: 2

Value type: double (Decimal/Numeric) 

Does user have to provide value: Yes

Holos has a default value: Yes

Possible Values: 0 < x < 100

note: Used as a global default. See line 24 on how the default is used. User can override 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Crops/CropInitializationService.Returns.cs 

***
## PercentageOfStrawReturnedToSoilForAnnuals100

Example value: 100

Value type: double (Decimal/Numeric) 

Does user have to provide value: Yes

Holos has a default value: Yes

Possible Values: 0 < x < 100

note: Used as a global default. See line 24 on how the default is used. User can override 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Crops/CropInitializationService.Returns.cs 

***
## PercentageOfRootsReturnedToSoilForAnnuals100

Example value: 100

Value type: double (Decimal/Numeric) 

Does user have to provide value: Yes

Holos has a default value: Yes

Possible Values: 0 < x < 100

note: Used as a global default. See line 24 on how the default is used. User can override 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Crops/CropInitializationService.Returns.cs 

***
## PercentageOfProductYieldReturnedToSoilForSilageCrops35

Example value: 35

Value type: double (Decimal/Numeric) 

Does user have to provide value: Yes

Holos has a default value: Yes

Possible Values: 0 < x < 100

note: Used as a global default. See line 24 on how the default is used. User can override 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Crops/CropInitializationService.Returns.cs 

***
## PercentageOfRootsReturnedToSoilForSilageCrops

Example value: 100

Value type: double (Decimal/Numeric) 

Does user have to provide value: Yes

Holos has a default value: Yes

Possible Values: 0 < x < 100

note: Used as a global default. See line 24 on how the default is used. User can override 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Crops/CropInitializationService.Returns.cs 

***
## PercentageOfProductYieldReturnedToSoilForCoverCrops

Example value: 100

Value type: double (Decimal/Numeric) 

Does user have to provide value: Yes

Holos has a default value: Yes

Possible Values: 0 < x < 100

note: Used as a global default. See line 24 on how the default is used. User can override 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Crops/CropInitializationService.Returns.cs 

***
## PercentageOfProductYieldReturnedToSoilForCoverCropsForage

Example value: 35

Value type: double (Decimal/Numeric) 

Does user have to provide value: Yes

Holos has a default value: Yes

Possible Values: 0 < x < 100

note: Used as a global default. See line 24 on how the default is used. User can override 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Crops/CropInitializationService.Returns.cs 

***
## PercentageOfProductYieldReturnedToSoilForCoverCropsProduce

Example value: 0

Value type: double (Decimal/Numeric) 

Does user have to provide value: Yes

Holos has a default value: Yes

Possible Values: 0 < x < 100

note: Used as a global default. See line 24 on how the default is used. User can override 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Crops/CropInitializationService.Returns.cs 

***
## PercentageOfStrawReturnedToSoilForCoverCrops

Example value: 100

Value type: double (Decimal/Numeric) 

Does user have to provide value: Yes

Holos has a default value: Yes

Possible Values: 0 < x < 100

note: Used as a global default. See line 24 on how the default is used. User can override 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Crops/CropInitializationService.Returns.cs 

***
## PercentageOfRootsReturnedToSoilForCoverCrops

Example value: 100

Value type: double (Decimal/Numeric) 

Does user have to provide value: Yes

Holos has a default value: Yes

Possible Values: 0 < x < 100

note: Used as a global default. See line 24 on how the default is used. User can override 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Crops/CropInitializationService.Returns.cs 

***
## PercentageOfProductReturnedToSoilForRootCrops

Example value: 0

Value type: double (Decimal/Numeric) 

Does user have to provide value: Yes

Holos has a default value: Yes

Possible Values: 0 < x < 100

note: Used as a global default. See line 24 on how the default is used. User can override 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Crops/CropInitializationService.Returns.cs 

***
## PercentageOfStrawReturnedToSoilForRootCrops

Example value: 100

Value type: double (Decimal/Numeric) 

Does user have to provide value: Yes

Holos has a default value: Yes

Possible Values: 0 < x < 100

note: Used as a global default. See line 24 on how the default is used. User can override 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Crops/CropInitializationService.Returns.cs 

***
## PercentageOfProductReturnedToSoilForPerennials

Example value: 35

Value type: double (Decimal/Numeric) 

Does user have to provide value: Yes

Holos has a default value: Yes

Possible Values: 0 < x < 100

note: Used as a global default. See line 24 on how the default is used. User can override 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Crops/CropInitializationService.Returns.cs 

***
## PercentageOfRootsReturnedToSoilForPerennials

Example value: 100

Value type: double (Decimal/Numeric) 

Does user have to provide value: Yes

Holos has a default value: Yes

Possible Values:

note: Used as a global default. See line 24 on how the default is used. User can override 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Crops/CropInitializationService.Returns.cs 

***
## PercentageOfProductReturnedToSoilForRangelandDueToHarvestLoss

Example value: 35

Value type: double (Decimal/Numeric) 

Does user have to provide value: Yes

Holos has a default value: Yes

Possible Values:

note: Used as a global default. See line 24 on how the default is used. User can override 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Crops/CropInitializationService.Returns.cs 

***
## PercentageOfRootsReturnedToSoilForRangeland

Example value: 100

Value type: double (Decimal/Numeric) 

Does user have to provide value: Yes

Holos has a default value: Yes

Possible Values:

note: Used as a global default. See line 24 on how the default is used. User can override 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Crops/CropInitializationService.Returns.cs 

***
## PercentageOfProductReturnedToSoilForFodderCorn

Example value: 35

Value type: double (Decimal/Numeric) 

Does user have to provide value: Yes

Holos has a default value: Yes

Possible Values: 

note: Used as a global default. See line 24 on how the default is used. User can override 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Crops/CropInitializationService.Returns.cs 

***
## PercentageOfRootsReturnedToSoilForFodderCorn

Example value: 100

Value type: double (Decimal/Numeric) 

Does user have to provide value: Yes

Holos has a default value: Yes

Possible Values: 

note: Used as a global default. See line 24 on how the default is used. User can override 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Services/Initialization/Crops/CropInitializationService.Returns.cs 

***
## DecompositionRateConstantYoungPool

Example value: 0.8 

Value type: double (Decimal/Numeric) 

Does user have to provide value: Yes

Holos has a default value: Yes

Possible Values: 

note: Used with ICBM carbon model. See line 221 for default 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Defaults.cs 

***
## DecompositionRateConstantOldPool

Example value: 0.00605 

Value type: double (Decimal/Numeric) 

Does user have to provide value: Yes

Holos has a default value: Yes

Possible Values:

note: Used with ICBM carbon model. See line 222 for default 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Defaults.cs 

***
## OldPoolCarbonN

Example value: 0.1 

Value type: double (Decimal/Numeric) 

Does user have to provide value: Yes

Holos has a default value: Yes

Possible Values: 0 < x

note: Used with both ICBM and IPCC Tier 2 carbon models. See line 224 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Defaults.cs 

***
## NORatio

Example value: 0.1 

Value type: double (Decimal/Numeric) 

Does user have to provide value: Yes

Holos has a default value: Yes

Possible Values:

note: Used with both ICBM and IPCC Tier 2 carbon models. See line 227 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Defaults.cs 

***
## EmissionFactorForLeachingAndRunoff(kgN2O-N(kgN)^-1)

Example value: 0.011 

Value type: double (Decimal/Numeric) 

Does user have to provide value: Yes

Holos has a default value: Yes

Possible Values: 

note: Used for N2O calculations. See line 228  

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Defaults.cs 

***
## EmissionFactorForVolatilization(kgN2O-N(kgN)^-1)

Example value: 0.01 

Value type: double (Decimal/Numeric) 

Does user have to provide value: Yes

Holos has a default value: Yes

Possible Values:

note: Used for N2O calculations. See line 229 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Defaults.cs 

***
## FractionOfNLostByVolatilization

Example value: 0.21 

Value type: double (Decimal/Numeric) 

Does user have to provide value: Yes

Holos has a default value: Yes

Possible Values:

note: Used for N2O calculations. See line 232 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Defaults.cs 

***
## MicrobeDeath

Example value: 0.2 

Value type: double (Decimal/Numeric) 

Does user have to provide value: Yes

Holos has a default value: Yes

Possible Values: 0 < x  

note: Used by both ICBM and IPCC Tier carbon models. See line 236 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Defaults.cs 

***
## Denitrification

Example value: 0.5 

Value type: double (Decimal/Numeric) 

Does user have to provide value: Yes

Holos has a default value: Yes

Possible Values: 0 < x 

note: Used by both ICBM and IPCC Tier carbon models. See line 237 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Defaults.cs 

***
## Carbonmodellingstrategy

Example value:  IPCCTier2 

Value type: enum 

Does user have to provide value: Yes

Holos has a default value: Yes

Possible values: IPCCTier2, ICBM

note: Will determine which carbon model will be used (IPCC Tier 2 is the newest C model and is the new default model) 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Enumerations/CarbonModellingStrategies.cs 

***
## RunInPeriodYears

Example value: 15

Value type: Int (Integer/Numeric)

Does user have to provide value: Yes

Holos has a default value: Yes

Possible Values: 0 < x 

note: Used to indicate how many years will be used when calculating equilibrium soil carbon. Can leave default in most cases. See line 269 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Defaults.cs 

***
## HumificationCoefficientAboveGround

Example value: 0.125 

Value type: double (Decimal/Numeric) 

Does user have to provide value: Yes

Holos has a default value: Yes

Possible Values: 0 < x 

note: Used with ICBM carbon model. See line 217 for default 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Defaults.cs 

***
## HumificationCoefficientBelowGround

Example value: 0.3 

Value type: double (Decimal/Numeric) 

Does user have to provide value: Yes

Holos has a default value: Yes

Possible Values: 0 < x 

note: Used with ICBM carbon model. See line 218 for default 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Defaults.cs 

***
## HumificationCoefficientManure

Example value: 0.31 

Value type: double (Decimal/Numeric) 

Does user have to provide value: Yes

Holos has a default value: Yes

Possible values: 0 < x

note: Used with ICBM carbon model. See line 219 for default 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Defaults.cs 

***
## Climatefilenameclimate.csv

Example value: climate.csv 

Value type: 

Does user have to provide value: Yes

Holos has a default value: Yes

Possible Values: "Any_Valid_Text".csv

note: Used when climate acquisition is set to "InputFile" 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs

***
## ClimateDataAcquisition

Example value: NASA 

Value type: enum 

Does user have to provide value: Yes

Holos has a default value: Yes

Possible Values:  SLC, Custom, NASA, InputFile

note: Specifies how Holos will aquire climate data. See source file for more details 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Providers/Climate/ClimateProvider.cs | https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs

***
## Useclimateparameterinsteadofmanagementfactor

Example value: True 

Value type: bool 

Does user have to provide value: 

Holos has a default value: Yes

Possible values: True, False

note: Set to true for most scenarios 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Defaults.cs

***
## EnableCarbonModelling

Example value: True 

Value type: bool 

Does user have to provide value: 

Holos has a default value: Yes

Possible values: True, False

note: Set to true for most scenarios 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs

***
## JanuaryPrecipitation

Example value: 17.6213 

Value type: double (Decimal/Numeric) 

Does user have to provide value: not used 

Holos has a default value: Yes

Possible Values: Any unrestricted value 

note: Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else. See line 37 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs 

***
## FebruaryPrecipitation

Example value: 12.8316 

Value type: double (Decimal/Numeric) 

Does user have to provide value: not used 

Holos has a default value: Yes

Possible Values: Any unrestricted value 

note: Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs 

***
## MarchPrecipitation

Example value: 22.426 

Value type: double (Decimal/Numeric) 

Does user have to provide value: not used 

Holos has a default value: Yes

Possible Values: Any unrestricted value 

note: Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs 

***
## AprilPrecipitation

Example value: 27.4144 

Value type: double (Decimal/Numeric) 

Does user have to provide value: not used 

Holos has a default value: Yes

Possible Values: Any unrestricted value 

note: Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs 

***
## MayPrecipitation

Example value: 61.5015 

Value type: double (Decimal/Numeric) 

Does user have to provide value: not used 

Holos has a default value: Yes

Possible Values: Any unrestricted value 

note: Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs 

***
## JunePrecipitation

Example value: 77.9022 

Value type: double (Decimal/Numeric) 

Does user have to provide value: not used 

Holos has a default value: Yes

Possible Values: Any unrestricted value 

note: Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs 

***
## JulyPrecipitation

Example value: 57.274 

Value type: double (Decimal/Numeric) 

Does user have to provide value: not used 

Holos has a default value: Yes

Possible Values: Any unrestricted value 

note: Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs 

***
## AugustPrecipitation

Example value: 53.0356 

Value type: double (Decimal/Numeric) 

Does user have to provide value: not used 

Holos has a default value: Yes

Possible Values: Any unrestricted value 

note: Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs 

***
## SeptemberPrecipitation

Example value: 40.4796 

Value type: double (Decimal/Numeric) 

Does user have to provide value: not used 

Holos has a default value: Yes

Possible Values: Any unrestricted value 

note: Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs 

***
## OctoberPrecipitation

Example value: 33.7571 

Value type: double (Decimal/Numeric) 

Does user have to provide value: not used 

Holos has a default value: Yes

Possible Values: Any unrestricted value 

note: Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs 

***
## NovemberPrecipitation

Example value: 23.0151 

Value type: double (Decimal/Numeric) 

Does user have to provide value: not used 

Holos has a default value: Yes

Possible Values: Any unrestricted value 

note: Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs 

***
## DecemberPrecipitation

Example value: 21.4046 

Value type: double (Decimal/Numeric) 

Does user have to provide value: not used 

Holos has a default value: Yes

Possible Values: Any unrestricted value 

note: Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs 

***
## JanuaryPotentialEvapotranspiration

Example value: 0.0327 

Value type: double (Decimal/Numeric) 

Does user have to provide value: not used 

Holos has a default value: Yes

Possible Values: Any unrestricted value 

note: Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs 

***
## FebruaryPotentialEvapotranspiration

Example value: 0.0888 

Value type: double (Decimal/Numeric) 

Does user have to provide value: not used 

Holos has a default value: Yes

Possible Values: Any unrestricted value

note: Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs 

***
## MarchPotentialEvapotranspiration

Example value: 3.5731 

Value type: double (Decimal/Numeric) 

Does user have to provide value: not used 

Holos has a default value: Yes

Possible Values: Any unrestricted value 

note: Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs 

***
## AprilPotentialEvapotranspiration

Example value: 44.1505 

Value type: double (Decimal/Numeric) 

Does user have to provide value: not used 

Holos has a default value: Yes

Possible Values: Any unrestricted value 

note: Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs 

***
## MayPotentialEvapotranspiration

Example value: 100.0393 

Value type: double (Decimal/Numeric) 

Does user have to provide value: not used 

Holos has a default value: Yes

Possible Values: Any unrestricted value 

note: Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs 

***
## JunePotentialEvapotranspiration

Example value: 123.5476 

Value type: double (Decimal/Numeric) 

Does user have to provide value: not used 

Holos has a default value: Yes

Possible Values: Any unrestricted value 

note: Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs 

***
## JulyPotentialEvapotranspiration

Example value: 135.7116 

Value type: double (Decimal/Numeric) 

Does user have to provide value: not used 

Holos has a default value: Yes

Possible Values: Any unrestricted value 

note: Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs 

***
## AugustPotentialEvapotranspiration

Example value: 120.4341 

Value type: double (Decimal/Numeric) 

Does user have to provide value: not used 

Holos has a default value: Yes

Possible Values: Any unrestricted value

note: Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs 

***
## SeptemberPotentialEvapotranspiration

Example value: 66.0041 

Value type: double (Decimal/Numeric) 

Does user have to provide value: not used 

Holos has a default value: Yes

Possible Values: Any unrestricted value 

note: Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs 

***
## OctoberPotentialEvapotranspiration

Example value: 16.8898 

Value type: double (Decimal/Numeric) 

Does user have to provide value: not used 

Holos has a default value: Yes

Possible Values: Any unrestricted value

note: Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs 

***
## NovemberPotentialEvapotranspiration

Example value: 0.7677 

Value type: double (Decimal/Numeric) 

Does user have to provide value: not used 

Holos has a default value: Yes

Possible Values: Any unrestricted value 

note: Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs 

***
## DecemberPotentialEvapotranspiration

Example value: 0.0252 

Value type: double (Decimal/Numeric) 

Does user have to provide value: not used 

Holos has a default value: Yes

Possible Values: Any unrestricted value

note: Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs 

***
## JanuaryMeanTemperature

Example value: -14.8531 

Value type: double (Decimal/Numeric) 

Does user have to provide value: not used 

Holos has a default value: Yes

Possible values: any unrestricted value

note: Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs 

***
## FebruaryMeanTemperature

Example value: -12.4063 

Value type: double (Decimal/Numeric) 

Does user have to provide value: not used 

Holos has a default value: Yes

Possible values: any unrestricted value

note: Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs 

***
## MarchMeanTemperature

Example value: -5.3584 

Value type: double (Decimal/Numeric) 

Does user have to provide value: not used 

Holos has a default value: Yes

Possible values: any unrestricted value

note: Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs 

***
## AprilMeanTemperature

Example value: 3.7295 

Value type: double (Decimal/Numeric) 

Does user have to provide value: not used 

Holos has a default value: Yes

Possible values: any unrestricted value

note: Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs 

***
## MayMeanTemperature

Example value: 10.7967 

Value type: double (Decimal/Numeric) 

Does user have to provide value: not used 

Holos has a default value: Yes

Possible values: any unrestricted value

note: Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs 

***
## JuneMeanTemperature

Example value: 16.4886 

Value type: double (Decimal/Numeric) 

Does user have to provide value: not used 

Holos has a default value: Yes

Possible values: any unrestricted value

note: Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs 

***
## JulyMeanTemperature

Example value: 18.8914 

Value type: double (Decimal/Numeric) 

Does user have to provide value: not used 

Holos has a default value: Yes

Possible values: any unrestricted value

note: Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs 

***
## AugustMeanTemperature

Example value: 18.2291 

Value type: double (Decimal/Numeric) 

Does user have to provide value: not used 

Holos has a default value: Yes

Possible values: any unrestricted value

note: Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs 

***
## SeptemberMeanTemperature

Example value: 13.2652 

Value type: double (Decimal/Numeric) 

Does user have to provide value: not used 

Holos has a default value: Yes

Possible values: any unrestricted value

note: Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs 

***
## OctoberMeanTemperature

Example value: 5.6419 

Value type: double (Decimal/Numeric) 

Does user have to provide value: not used 

Holos has a default value: Yes

Possible values: any unrestricted value

note: Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs 

***
## NovemberMeanTemperature

Example value: -3.5511 

Value type: double (Decimal/Numeric) 

Does user have to provide value: not used 

Holos has a default value: Yes

Possible values: any unrestricted value

note: Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs 

***
## DecemberMeanTemperature

Example value: -11.9174 

Value type: double (Decimal/Numeric) 

Does user have to provide value: not used 

Holos has a default value: 

Possible values: any unrestricted value

note: Can be set and used if climate data acquisition is set to custom. Value will be ignored if climate data acquisition is set to something else 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Models/Farm.cs 

***
## Province

Example value: Manitoba 

Value type: enum 

Does user have to provide value: Yes

Holos has a default value: 

Possible values: Alberta, BritishColumbia, Manitoba, NewBrunswick, Newfoundland, NorthwestTerritories, NovaScotia, Ontario, Nunavut, PrinceEdwardIsland, Quebec, Saskatchewan, Yukon

note: extracted from slc DB with lat long data 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Enumerations/Province.cs

***
## YearOfObservation

Example value: 2024

Value type: Int (Integer/Numeric)

Does user have to provide value: Yes

Holos has a default value: Yes

Possible values: 1 < x

note: Defaults to the current year 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Infrastructure/ModelBase.cs

***
## EcodistrictID

Example value: 851

Value type: Int (Integer/Numeric)

Does user have to provide value: Yes

Holos has a default value: Yes

Possible Values: 0 < x

note: extracted from slc DB with lat long data 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Providers/Soil/SoilData.cs

***
## SoilGreatGroup

Example value: Regosol 

Value type: enum 

Does user have to provide value: Yes

Holos has a default value: Yes

Possible values: NotApplicable, Unknown, MelanicBrunisol, EutricBrunisol, SombricBrunisol, DystricBrunisol, BrownChernozem, DarkBrownChernozem, BlackChernozem, DarkGrayChernozem, TurbicCryosol, StaticCryosol, OrganicCryosol, HumicGleysol, Gleysol, LuvicGleysol, GrayBrownLuvisol, GrayLuvisol, Fibrisol, Mesisol, Humisol, Folisol, HumicPodzol, FerroHumicPodzol, HumoFerricPodzol, Regosol, HumicRegosol, Solonetz, SolodizedSolonetz, Solod, VerticSolonetz, Vertisol, HumicVertisol

note: extracted from slc DB with lat long data 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Enumerations/SoilGreatGroupType.cs

***
## Soilfunctionalcategory

Example value: Black 

Value type: enum 

Does user have to provide value: Yes

Holos has a default value: Yes

Possible Values: NotApplicable, Brown, BrownChernozem, DarkBrown, DarkBrownChernozem, Black, BlackGrayChernozem, Organic, EasternCanada, EnumSoilFunctionalAll, 

note: deduced following code in Holos 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Enumerations/SoilFunctionalCategory.cs

***
## BulkDensity

Example value: 1.2 

Value type: double (Decimal/Numeric) 

Does user have to provide value: Yes

Holos has a default value: Yes

Possible values: 0 < x

note: extracted from slc DB with lat long data 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Providers/Soil/SoilData.cs

***
## SoilTexture

Example value: Fine 

Value type: enum 

Does user have to provide value: Yes

Holos has a default value: Yes

Possible Values: Fine, Medium, Coarse, Unknown

note: deduced following code in Holos 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Enumerations/SoilTexture.cs

***
## SoilPh

Example value: 7.8 

Value type: double (Decimal/Numeric) 

Does user have to provide value: Yes

Holos has a default value: Yes

Possible Values: 0 < x

note: extracted from slc DB with lat long data 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Providers/Soil/SoilData.cs

***
## TopLayerThickness(mm)

Example value: 200

Value type: double (Decimal/Numeric) 

Does user have to provide value: Yes

Holos has a default value: Yes

Possible Values: 0 < x

note: extracted from slc DB with lat long data 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Providers/Soil/SoilData.cs

***
## ProportionOfSandInSoil

Example value: 0.2 

Value type: double (Decimal/Numeric) 

Does user have to provide value: Yes

Holos has a default value: Yes

Possible Values: 0 < x < 100

note: extracted from slc DB with lat long data 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Providers/Soil/SoilData.cs

***
## ProportionOfClayInSoil

Example value: 0.3 

Value type: double (Decimal/Numeric) 

Does user have to provide value: Yes

Holos has a default value: Yes

Possible Values: 0 < x < 100

note: extracted from slc DB with lat long data 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Providers/Soil/SoilData.cs

***
## ProportionOfSoilOrganicCarbon

Example value: 3.1 

Value type: double (Decimal/Numeric) 

Does user have to provide value: Yes

Holos has a default value: Yes

Possible Values: 0 < x < 100

note: extracted from slc DB with lat long data 

Source (source code file, table, algorithm document, etc.): https://github.com/holos-aafc/Holos/blob/main/H.Core/Providers/Soil/SoilData.cs

***
