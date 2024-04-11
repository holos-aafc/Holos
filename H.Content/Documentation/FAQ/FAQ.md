<p align="center">
<img src="../../Images/logo.png" alt="Holos Logo" width="550" />
<br>
</p>

# Frequently Asked Questions

The purpose of this document is to allow for users to find answers to their questions in one place. 

<br>

# General FAQ

### 1. I do not know how to use the Holos program.
A training guide is available. Following this step by step guide will help you to learn how to use the program. The links are below:

<a href="https://github.com/holos-aafc/Holos/blob/main/H.Content/Documentation/Training/Holos_4_Training_Guide.md">Training Guide [ENG]</a>

<a href="https://github.com/holos-aafc/Holos/blob/main/H.Content/Documentation/Training/Holos_4_Training_Guide-fr.md">Training Guide [FR]</a>


### 2. Are there any videos I can watch?
Yes, Holos has a Youtube channel that offers tutorials to new users. Click the link:&nbsp;&nbsp;<a href="https://www.youtube.com/channel/UCHDORmZ73VICHzqm_yVpM_Q">Tutorial Videos</a>


### 3. How can I participate in the Holos discussion forum?
Holos has a discussion board that users can leave feedback and ask questions. To begin using the discussion forum, a GitHub account needs to be created. After signing up for the GitHub account, you can start to chat in the forum. There are step by step guides for how to sign up for a GitHub account and how to create a simple post in the discussion forum:        

<a href="https://github.com/holos-aafc/Holos/discussions">Holos Discussion Forum</a>

<a href="https://github.com/holos-aafc/Holos/blob/main/H.Content/Documentation/GitHub%20Guide/GitHub%20Guide.md#creating-an-account">How to sign up for a Github account</a>

<a href="https://github.com/holos-aafc/Holos/blob/main/H.Content/Documentation/GitHub%20Guide/GitHub%20Guide.md#how-to-write-a-post-in-the-discussion-forum">How to create a simple post in the discussion forum</a>


### 4. How do I add/edit this FAQ?
Users can add new entries to the FAQ and edit existing entries. Note that this page uses markdown. To add and edit the FAQ, two steps are required: 1. Sign up for a GitHub account 2. Create a pull request to Holos repository. Once the Holos repository administrator appoves your pull request, the changes you made will be applied to the FAQ page.

<a href="https://github.com/holos-aafc/Holos/blob/main/H.Content/Documentation/GitHub%20Guide/GitHub%20Guide.md#creating-an-account">How to sign up for a Github account</a>

<a href="https://github.com/holos-aafc/Holos/blob/main/H.Content/Documentation/GitHub%20Guide/GitHub%20Guide.md#contributing-changes-to-the-original-repository">How to make a pull request to Holos repository</a>

---

# Model Setup FAQ

### 1. Why is there (currently) no imperial option for the units of measurement?
Holos was developed to offer both the metric and imperial measurement system for inputs. However, when deploying updates to the model, the imperial option may be temporarily disabled. The team is working hard to ensure that the imperial option becomes available again as soon as possible.


### 2. Does it matter where I locate my farm within the SLC (Soil Landscapes of Canada) polygon?
In relation to the default soil types that will appear on the right-hand side of the screen when you locate your farm within an SLC polygon, the precise location of the farm within the polygon does not make a difference (i.e., the same options for soils data will appear regardless of where the farm is located within the polygon). The precise location of the farm within a polygon also does not affect the default hardiness zones. However, the default daily climate data used within the model are uploaded automatically from NASA – these data are available in a 10 km grid, and so can vary throughout the SLC polygon, depending on the precise location of the farm. Therefore, if possible, the user should choose the location of their farm as precisely as possible. Doing so can be aided by using different views (e.g., the Aerial view), which can be selected via the eye icon at the bottom of the map on the Farm Location screen.


### 3. When I right-click to select my farm location on the map, I get an error message saying that Holos cannot download the daily climate data from NASA. What happens now?
The download of the daily climate data from the NASA website requires a stable internet connection. Occasionally, if there is an issue with the model user’s internet connection or with the NASA website itself, the data will not download. In these instances, the model will default to using an in-built 30-year climate normals dataset (1981-2010 normals), available at the SLC polygon scale.


### 4. How can I upload my own climate data?
In the map selection screen, once a location is chosen, an information panel appears on the right-hand side of the screen that displays the available soil types for the selected Soil Landscape of Canada (SLC) polygon. This panel has two other tabs, including one for Climate Data. As a default, when the model user selects heir farm location, Holos automatically downloads climate data from NASA (10 km grid daily weather data). However, on the Climate Data tab, the user has the option to override the default climate data by uploading their own custom data (this is typically measured local data). 
>*Please note: a complete climate dataset for the entire simulation period is required – (i.e., no gaps or missing data as Holos does not check for completeness and will attempt to work with the data as it is provided); the data must also be in the prescribed format [(see here)](https://github.com/holos-aafc/Holos/blob/main/H.Content/Documentation/User%20Guide/User%20Guide.md#formatting-the-custom-climate-data-file).*


### 5. The default options for soils data for my chosen farm location do not exactly match the conditions on my operation. How can I rectify this?
After you have chosen your farm location, select the default soil type that most closely matches the soil type/soil characteristics on your farm. Click ‘Next’. On the Component Selection screen, go to Settings > Farm Defaults > Soil. On this tab, you can modify some of the soil type characteristics to more closely resemble the soil conditions on your operation. 
>*Please note: you cannot change the soil functional category or great group. Once you are finished, click ‘Close’ and your changes will be saved automatically.*


### 6. Can I select a different soil type for the different fields on my farm?
Currently, it is not possible to select a different soil type for individual fields on the simulated farm. Once the soil type is selected on the Component Selection screen, the soils data is applied to all fields added to the farm. If you have more than one soil type on your farm, you could account for this be setting up multiple farms (one for each soil type), including in each farm the relevant fields. The model outputs for the different farms could then be summed to obtain an overall emissions estimate for the entire operation. 


### 7. What do I do if my farm extends over multiple SLC (Soil Landscapes of Canada) polygons?
Currently, the model user must locate their farm within a single SLC polygon. If the user wishes to model separately those parts of the farm that fall within different SLC polygons, they can follow the approach outlined in the response to Q4 above.


### 8. If I am at the 'Details' or 'Results' screens, can I go back to previous screens to make additional changes to my farm?
Yes, if you have already set up your farm and progressed to the 'Details' or the 'Results' screen, you can go back to previous screens to make any additional changes required. However, if you have a multi-year setup, after making any such changes you must then go forward to the 'Details' screen to click the 'Reload Data from Previous Screen' button, in the top left-hand corner of the screen. This will lead Holos to overwrite the previous set of input data. Please note that after doing this, you will need to redo any previous changes made to the data on the 'Details' screen (e.g., custom yield data, etc.).

---

# Crop FAQ

### 1. Where is crop X, I cannot find it on the list?
If a crop is not on the list, this means that we could not find (published Canadian) data for it. Three options are available to rectify the issue (if you do have the necessary data):
- Single instance: Choose an existing crop from the list and adjust the Carbon (C) coefficient values (which describe the proportion of total plant biomass or carbon contained in the different plant parts) and their nitrogen (N) concentrations and lignin contents – this can be done using the Residue tab for the field or crop rotation in question;
- Continuous setting: On the component selection screen, go to Settings > Crop Defaults, where you can overwrite the default settings for individual crops. Once you do this, these new settings/default values will then  appear whenever the crop is chosen;
- Permanent addition: Add a new crop to the Holos lookup table, with the associated required data (i.e., C coefficients, N concentrations, lignin contents, moisture content) [(Table 9)](https://github.com/holos-aafc/Holos/blob/main/H.Content/Resources/Table_9_Default_Values_For_Nitrogen_Lignin_In_Crops.csv). 
>*Please note: for permanent crop additions, peer-reviewed publications need to be provided as a reference and those publications should refer to Canadian studies.*


### 2. What is the difference between the field component and the crop rotation component?
In short, the field component sets up a crop rotation/sequence for a field, while the crop rotation component sets up a number of fields for a crop rotation (# of fields = the number of crops in the rotation). Other than that, both components have the same functionality, but for the crop rotation component all specific inputs are copied to all fields of the rotation. If there is a field that has distinctly different management than the others, the single field component will have to be used instead.


### 3. How can the model user deal with different field configurations?
At the moment, there is no way to deal with this automatically in Holos, but the model user can address this by creating different field components for different time periods, with the appropriate management practices for each field/timeframe.


### 4. My fields change in size over time, how can I reflect that in Holos?
As for different field configurations over time, changes in field size over time cannot be dealt with automatically in Holos. If field size changes over the timeframe of the simulation, the user can address this by sub-dividing the field(s) into different field components that remain consistent over the simulation period, each of which has their own individual field history. This is because, in Holos, the carbon models estimate carbon change on a per land area basis, and this area is assumed to remain stable over time.


### 5. How do I know the area of land required to grow enough feed for the animals on my farm - does Holos calculate this automatically?
Holos does not automatically calculate the area of each crop required to feed the animals on the simulated farm. The user can either calculate this area outside of Holos, using information on the amount of each feed type required, crop yields and harvest/feeding losses, etc., or they can enter area values for actual fields used to grow feed for the animals included on the farm (e.g., if the user is simulating an actual agricultural operation).



### 6. I am applying a fertilizer that is not on the drop-down list, what can I do?
The fertilizer tab of the field or crop rotation component provides the option to add custom synthetic and organic fertilizers. The choice of fertilizer will influence the fraction of nitrous oxide (N2O) that will be emitted. When choosing what type of custom fertilizer to add, we recommend that fertilizers containing a lot of reactive N be input as custom synthetic fertilizer, while for fertilizers where N is bound up in the biomass and released through decomposition (i.e., compost) the custom organic option is appropriate. Slow-release fertilizers are still deemed synthetic as they release reactive N over time. Measurements of N/C/P and moisture content of the custom fertilizer are required.


### 7. I am importing manure / organic fertilizers onto the farm, don’t the emissions Holos reports belong to the source/origin of these fertilizers?
When importing manure/organic fertilizers from other farms, the farmer gains the benefit of adding carbon to the soil. However, when applying these organic materials, emissions will take place. As it is the farmer’s choice to apply the materials, and the emissions take place on the farm to which they were imported/applied, Holos does report the related emissions (for the purpose of the whole-farm GHG budget). If the purpose of the simulation is a life-cycle analysis approach (to calculate the GHG efficiency/intensity of a product), emission allocation may have to be employed.


### 8. How do I add a field where I grow cover crop mixes?
At this time, Holos does not offer an option to input cover crop mixes. The future development and addition of such functionality is planned. At this current time, a single cover crop can be selected from the list of available options, and the carbon coefficients and N concentrations can be adjusted to better reflect the desired mix, e.g., by using average values for the mix).


### 9. How do I add a field with inter-cropping?
At this time, Holos does not offer an option to input inter-cropping systems. The future development and addition of such functionality is planned. At this time, separate field components for each crop can be used.


### 10. How can I explore the effects of 4R nutrient management in Holos?
Following the National GHG Inventory methodology, we do not yet know how to account for 4R nutrient management effects. However, we do have preliminary factors for some practices – you can see these by selecting ‘Yes’ for the ‘Show Additional Information’ option in the Fertilizer tab and choose the desired ‘Additive’). A ‘Custom‘ additive option has been added to the model to permit the testing of the preliminary results of other practice applications.


### 11. When I add a fall/winter crop in Holos, what crop should I specify for that field in spring of the following year when that crop will be harvested?
Currently, in Holos V4, when the user ‘grows’ a crop such as fall rye or winter wheat (via the winter/cover crop section), residues from the winter/cover crop are added to the main crop in the planting year (year t) – this is the default approach. If there is no harvest of the winter crop in year t, the model user must enter 0 for the winter crop yield for this year, and then add a non-zero yield in year t+1. The model user can also omit the winter/cover crop in year t, and specify it in year t+1 with a non-zero yield and the effect on the Holos estimates will be the same in both scenarios. 

---

# Carbon FAQ

### 1. What are the inputs *Start* year and *End* year doing?
Start and End year determine the length of the soil carbon model simulation. This will largely depend on the historic farm activity data that are available, but we recommend that the user starts the simulation as close to the year 1985 as possible. This is because carbon change is a long-term process, with soil scientists frequently stating that soil carbon changes related to the implementation of a particular management practice (or bundle of practices) can be measured at the earliest after 10 years after the implementation of the management change. When defining the management history for a field or crop rotation component, Holos primarily needs the cropping sequence and approximate fertilizer/ manure / residue management data. Crop yield data are also important – as a default, Holos uploads annual crop-specific yield data at the Small Area Data Region scale from a database, but the user can override these default yields under the General tab on the Component selection screen and/or on the Details screen.


### 2. I have measured soil carbon data, how can I input these into Holos?
As a default, Holos estimates a starting soil organic carbon value (kg SOC ha-1) for each field. However, the user can override this value by using measured soil carbon data to initialize the soil carbon model. To do this, on the Component selection screen go to Settings > User settings. There, enable the ’Use custom equilibrium carbon value’ field and enter the value in the ‘Equilibrium carbon’ box, the unit is in kg C ha-1. Once you are finished, click ‘OK’ and your changes will be saved automatically. Currently, this custom starting value applies to the whole farm (i.e., all simulated fields/crop rotations). 


### 3. Holos outputs multiple results on the ‘Multiyear Carbon Modelling” tab, which should I use?
Carbon models with annual time steps (such as those utilized in Holos) provide an estimate of the total soil carbon stock for every year of the simulation (kg C ha-1). As these annual estimates represent the sum of the previous year’s SOC stock plus C inputs for the current year from crop residues and manure, minus losses due to decomposition, SOC stock levels can vary from year to year as well as over longer time spans (e.g., due to a change of cropping system), resulting in net positive or net negative carbon change estimates depending on the time window being assessed. As the model developers, we cannot foresee what changes occur on the farm and when, the model provides estimates of SOC stock change over several different time spans. However, the model user can calculate changes in SOC stocks over alternative timespans, using the data reported in the ‘Grid’ format on the Multiyear Carbon Modelling results screen, to represent changes and trends over the desired time period.

---

# Livestock FAQ

### 1. I cannot find options to input my grazing systems, how do I represent grazing system X in Holos?
Currently Holos does not offer the option to simulate different grazing systems, as there still is a lack of scientific clarity on what the exact effects of such systems are. Furthermore, there is also some confusion related to terminology. Our team is involved in several projects that attempt to provide more clarity and future updates to the model are intended to provide appropriate options. In the meantime, using Holos, the model user can place animal groups on specific pasture fields. In this way, multiple fields could be created to represent different paddocks for a rotational grazing system, with the management history for each field detailed. However, for grazed fields/paddocks, Holos estimates the aboveground biomass productivity based on the animal vegetation biomass consumption in combination with estimates of biomass utilization (grazing efficiency), thus more efficient grazing systems could be represented using a single pasture, rather than subdividing it into parcels. 
>*Please note: more intensive grazing systems have been shown to improve the feed quality, but this must be specified for each animal group grazing on a specific pasture and for each relevant management period, using the Diet tab for that animal group/management period, e.g., by creating a custom pasture diet using the ‘Custom Diet Creator’ tool.*


### 2. How does Holos estimate pasture yields when animals are grazing on a field?
When the model user 'places' grazing animals on a perennial field (i.e., rangeland (native), seeded grassland, tame grass, tame legume, tame mixed (grass/legume)) for a specified period of time, Holos estimates the total annual aboveground biomass yield for each year based on the number of animals grazing on the field, the number of days spent grazing, the total daily dry matter intake and the pasture utilization rate (default values: 40% for grassland (native), 50% for seeded grassland, 60% for tame grass, tame legume and tame mixed (grass/legume) pasture). When there are two or more grazing periods in a calendar year, we assume that the leftover (i.e., unconsumed) forage/forage carbon (C) remaining at the end of the first grazing period is not returned to the soil but rather forms the basis for later vegetation growth that is consumed by grazing livestock in subsequent grazing periods in the same year. Therefore, leftover forage/C is added to the soil only at the end of the final grazing period in a given year, with the Cptosoil value for all earlier grazing periods set to zero. Further details of the methodology used to estimate aboveground biomass yield and C inputs to soil from forage residue on grazed fields can be found in Appendix 11 in the Holos algorithm document. Note: the user can adjust the default Utilization rates for each grazing animal group and field on the 'Grazing' tab for the relevant field in the user interface. The user can also override the Holos-estimated forage biomass yield estimates by entering custom yield estimates on the Details screen for the relevant field and years - these new yield estimates will then be used in all subsequent equations to estimate forage C and N inputs to the soil.


### 3. How can I graze animals on harvested annual croplands (stubble grazing) in Holos?
To graze animals on crop stubble, you should create the field as normal, specifying the field history and management practices under Steps 1-3 on the Component Selection screen, making sure to select 'Cash crop' as the Harvest method under Step 3. Grazing animals should be 'placed' on this field in the appropriate livestock components. Note: typically for annual croplands, the 'Straw returned to soil' value on the Residue tab is set to 100%, but when stubble grazing takes place, this value needs to be adjusted to reflect the proportion of this straw/residue that is removed by grazing animals, e.g., if the animals consume 60% of the stubble, then the 'Straw returned to soil' should be set to 40%. Holos accounts for manure C and N deposited on the field by the animals during the grazing period. Note: as these fields are harvested in a conventional manner using the 'Cash crop' Harvest method, the Yield (kg product/grain per ha) is estimated based on either user input or default Small Area Data yields, rather than based on animal forage intake (as is the case with grazed perennial or stubble/swath grazed fields, where livestock feed on the total aboveground biomass (i.e., product/grain + straw).


### 4. How can I swath graze a field or graze animals on standing vegetation in an annual cropfield in Holos?
To graze animals on swathed or standing vegetation, you should create the field as normal, specifying the field history and management practices under Steps 1-3 on the Component Selection screen, making sure to select 'Swathing' as the Harvest method under Step 3 (even if the vegetation is not actually swathed, it still remains in the field). Note: when you select 'Swathing' as the Harvest method, the Yield value should represent the total aboveground biomass (i.e., product + straw/residue).  'Product returned to soil' and 'Straw returned to soil' values on the Residue tab should be altered as needed, to reflect the amount of aboveground biomass that remains in the field after the animals have finished grazing, i.e., if the model user estimates that the grazing animals would consume 60% of the swathed forage, then the 'Product returned to soil' and 'Straw returned to soil' values can be set to 40% - the C in this biomass will be added to the soil C pool. Swath grazing animals should be 'placed' on this field in the appropriate livestock components. Holos accounts for manure C and N deposited on the field by the animals during the grazing period.


### 5. I want to compare livestock management options, how can I do that in Holos?
There are three options to do this:
- Set up two different farms
- Set up two livestock components within a single farm
- Set up two livestock groups within a single livestock component on a single farm
Each of these allows the model user to compare model outputs for the different management options. 
>*Please note: if the model user sets up two (or more) different farms, they can compare the model outputs for these farms by selecting ‘Yes’ for ‘Compare Multiple Farms’ on the Results screen and selecting the farms they wish to compare from the list available.*


### 6. I want to know what the carbon footprint of my livestock system is, what do I need to do?
The Holos model is set up to calculate a farm’s greenhouse gas (GHG) budget, meaning it accounts for all farm-based sources of GHG that we can estimate based on available information and data . To calculate the carbon footprint of a product, we need to account for all emissions generated as a result of the production of this product. For a livestock system, that means accounting for the feed production, whether that feed is grown on the actual farm or not. Before adding feed-producing fields to the simulated farm, the user must first calculate the area of each pasture or crop field required to sustain the animals on the farm – Holos will generate a warning message if not enough feed is being “grown” to satisfy animal requirements, as an internal check. Emissions generated as a result of inputs to the feed production system (e.g., fertilizer and pesticide production) are also accounted for. In Holos, upstream emissions for these farm inputs are also reported, i.e., CO2 generated from the upstream production of synthetic For the livestock system itself, emissions related to the breeding stock must be included in the calculations, as well as those relating to their progeny. Holos then outputs all of the emissions for this system up until the farm gate – any emissions related to transport, processing, etc. will need to be estimated by the user outside of Holos and added to the Holos outputs, if so desired.
>*Please note: it is up to the user to allocate the emissions according to the product, e.g., in a beef production system the outputs could easily be broken down to CO2eq per animal carcass, but for a CO2eq per kg meat it needs to be decided whether all emissions are assigned to the meat part of the carcass, or whether a portion of the emissions are allocated to the different parts of the carcass (Consulting an LCA expert is advised.)*


### 7. I want to add an ingredient to my custom diet that is not in the ingredient list, how can I do this?
Using the Custom Diet Creator, the model user can create new feed ingredients, that can then be added to a custom diet. Open the Custom Diet Creator on the Diet tab and under Step 2, click on ‘Create Custom Ingredient’. A new row should appear at the top of the ingredient list – you can click on the ingredient name to change it. To define this ingredient, you will need to enter the relevant data in the rest of the row. 
>*Please note: not all data columns in this table are necessary for the Holos calculations and the data required vary depending on the animal group under consideration.* 

At a minimum, the following data are required for different animal groups :

 - for ***beef and dairy cattle***: DM (Dry matter content of ingredient (as fed), % AF), Forage (% DM – this value will be either 0 (if the custom ingredient is not a forage ingredient) or 100 (if it is a forage ingredient)), CP (Crude protein content, % of DM), TDN (Total digestible nutrient, % of DM), Starch (Starch concentration in the ingredient, % of DM), Ash (Ash content of feed, % of DM), NEma (Net energy for maintenance, Mcal kg-1), NEga (Net energy for growth, Mcal kg-1) – these last two parameters are needed only for the estimation of methane emissions for calves not fed on a milk diet;- for ***sheep, swine, poultry and other livestock***: DM (% AF), Forage (% DM), CP (% DM), TDN (% DM), Ash (% DM).

Once you are finished, click ‘OK’ and your changes will be saved automatically.


### 8. Where can I see the full details of the default diets built into Holos?
Some of the data for the selected diet is visible when you select ‘Show Additional Information’ on the Diet tab, however you can see the full details for this diet if you open the Custom Diet Creator. Once open, select ‘Yes’ for ‘Show Default Diets’ under Step 1 – you will now see data related to the nutritional content of each default diet available for the relevant livestock type in this section, as well as data related to the percentage of the total dietary DM that is composed of the different diet ingredients (under Step 3). 


### 9. How can I modify one of the default diets in Holos or create a new diet based on a default diet?
To modify one of the default diets in Holos for a specific animal group and management period, make a copy of the default diet and then alter the data inputs for the copy. To make a copy of a default diet, open the Custom Diet Creator and under Step 1, click ‘Yes’ for ‘Show Default Diets’. In the list of default diets below, right-click on the diet of interest and click ‘Create a Copy’. You can change the name of the copy diet, if needed. Under Step 3, you can alter the % of the different ingredients in the diet or delete specific ingredients, and under Step 2 you can add ingredients. Once you have finished modifying the diet, click ‘OK’ to return to the main Diet tab and select the copied diet from the drop-down Diet menu. 
>*Please note: if the user selects the same diet (either the original default diet or a copy) for more than one management period, any changes made to the diet in one management period will automatically be replicated for any other management periods using the same diet. Therefore, if the user needs to make different modifications to an existing diet for multiple animal groups/management periods, it is recommended that a copy be created for each management period, and then modified accordingly. Alternatively, the user can create an entirely new diet for each management period using the Custom Diet Creator.*


---

To download Holos, for more information, or to access a recent list of Holos related publications, visit:
https://agriculture.canada.ca/en/agricultural-science-and-innovation/agricultural-research-results/holos-software-program

To contact us, email:
aafc.holos.acc@canada.ca
