<p align="center">
 <img src="../../Images/Training/AnaerobicDigestion/Holos Logo.png" alt="Holos Logo" width="650"/>
    <br>
</p>
The purpose of this document is to provide an introduction on how to use the Holos model (version 4) and the required vs. optional inputs.

For the purpose of this training, we are going to create a farm that has a beef production system, a crop production system, and an anaerobic digestor on site. The farm is located in Manitoba near Portage La Prairie. 

<br>

# Launch Holos

Please note that Holos 4 can be installed on a Microsoft Windows PC only. Mac OS will be supported in the next version.

Launch Holos by double-clicking on the Holos desktop icon. Holos will ask the user to open an existing farm, create a new farm, or import a saved farm file (Figure 1). If there is already a saved farm in the system, the user can click **Open**. If there are no saved farms in the system Holos will ask the user if they want to create a **New** farm or **Import** a saved farm file (i.e., a .json file). If the user creates a new farm, they are asked for the farm name and an optional comment (Figure 2).  

Enter **"Holos 2025"** as the Name and **"Training Version"** as the Comments.  Click **OK** to proceed to the next screen.

Ensure **"Metric"** is selected as the unit of measurement type and then click the **Next** button at the bottom of the screen (Figure 3). 

<br>

<p align="center">
    <img src="../../Images/Training/AnaerobicDigestion/figure1.png" alt="Figure 1" width="650"/>
    <br>
    <em>Figure 1: If a farm has been previously saved, Holos will prompt to re-open that farm.</em>
</p>

<br>

<p align="center">
    <img src="../../Images/Training/AnaerobicDigestion/figure2.png" alt="Figure 2" width="750"/>
    <br>
    <em>Figure 2: Entering a name for the new farm. </em>
</p>

<br>

<p align="center">
    <img src="../../Images/Training/AnaerobicDigestion/figure3.png" alt="Figure 3" width="550"/>
    <br>
    <em>Figure 3: Select metric as the unit of measurement.</em>
</p>

<br>

# Creating and Locating the New Beef Farm

The beef farm that we will create for this exercise is located in the province of Manitoba. Select **"Manitoba"** on the **Select a province** screen, and then click the **Next** button.

<br>

<p align="center">
    <img src="../../Images/Training/AnaerobicDigestion/figure4.png" alt="Figure 4" width="550"/>
    <br>
    <em>Figure 4: Select Manitoba as the province.</em>
</p>

<br>

Holos uses **Soil Landscapes of Canada** (SLC), which are a series of GIS coverages that show the major characteristics of soils and land for all of Canada (compiled at a scale of 1:1 million). SLC polygons may contain one or more distinct soil landscape components.

The **"Farm Location"** screen brings up a map of Canada with the province of Manitoba centered on the screen (Figure 5). 

The map contains red colored polygons that can be selected by moving the cursor over the region that contains the location of your farm. You can zoom in or out of the map by using the mouse wheel or by hovering the cursor over the zoom icon at the bottom of the screen.

The beef farm for this example is located between Winnipeg and Portage la Prairie (Portage) with SLC polygon number **851003**. 


Find and right-click on this polygon to select it on the map (Figure 6). Note that at this point daily climate data will be downloaded from [NASA](https://power.larc.nasa.gov/data-access-viewer/). 

<br>

> *Note: Climate data is central to most calculations performed by Holos. For the most accurate estimation of farm emissions, measured climate data should be provided by the user which will override the default data obtained from the NASA weather API. If the user chooses to use the default NASA climate data, these data are available in a 10 km grid, and so can vary throughout the SLC polygon, depending on the precise location of the farm. Therefore, if possible, the user should choose the location of their farm as precisely as possible. Doing so can be aided by using different views (e.g., the Aerial view), which can be selected via the eye icon at the bottom of the map on the Farm Location screen.*
> 

> *Holos will use daily precipitation, temperature, and potential evapotranspiration values to model soil carbon change (climate parameter), nitrous oxide emissions, as well as ammonia volatilization.*

<br>

<p align="center">
    <img src="../../Images/Training/AnaerobicDigestion/figure5.png" alt="Figure 5" width="950"/>
    <br>
    <em>Figure 5: Map of the Manitoba province showing the different select-able polygons.</em>
</p>

<br>

Once the farm location is selected, soil information (texture, sand, and clay proportions) for the types of soils found in this polygon are displayed on the right side of the screen. It is possible that more than one soil type per polygon will be found and the user is expected to select their soil type from this list or use the default selection (Figure 6). The default soil type selected represents the dominant soil type for the chosen polygon.

For this tutorial, keep the default **Soil Zone** as 'Black' soil, and the default **"Hardiness Zone"** as '3b'. 

<br>

<p align="center">
    <img src="../../Images/Training/AnaerobicDigestion/figure6.png" alt="Figure 6" width="950"/>
    <br>
    <em>Figure 6: Multiple soil types might be available for a given region.</em>
</p>  

<br>

**Note: Soil data obtained from the user’s selected location will be used in the calculation of location-specific N<sub>2</sub>O emission factors. Properties such as soil texture, top layer thickness, and soil pH are required for these calculations, and can be overwritten on the Component Selection screen, under Settings > Farm Defaults > Soil.*

<br>

Click the **Next** button to proceed to the next step.

<div style="page-break-after: always"></div>

# Selecting Farm Components

Now that the farm location has been selected, we can move on to the 'Component Selection' screen. This is where the user can select different components for their farm. Holos will display all available components on the left side of the screen under the “All Available Components” column (Figure 7). These components are grouped into various categories including Land Management, Beef Production and Dairy Cattle.

If we click on the drop-down button next to a category's name, we can then see the available components in that category.  For this portion of the  training section, we will be working with the “Land management” and “Beef production” categories. 

<br>

<p align="center">
    <img src="../../Images/Training/AnaerobicDigestion/figure7.png" alt="Figure 7" width="950"/>
    <br>
    <em>Figure 7: The available components screen. Specific components can be chosen here to include in the farm.</em>
</p> 

<br>

The Holos model is designed so that the land management components are defined before the livestock components. This is because the model allows for the placement of livestock onto a specific field(s) (i.e., pasture(s)) for grazing. It is easier to do this if the pasture field has already been defined. However, the user can first set up their livestock components and then their field components, but will then need to return to their livestock components to specify and ‘place’ them on pasture.

<div style="page-break-after: always"></div>

## Crop Production

Now we can add our first component to the farm. Drag a **Field** component from the left side of the screen and drop it on the **My Farm** on the right side (Figure 8). The screen will now update to reflect this new component that you have added to your farm. Holos will  label the field as **“Field #1”**. At this point, we can enter production information related to the crop being grown on this field.

<br>

<p align="center">
    <img src="../../Images/Training/AnaerobicDigestion/figure8.gif" alt="Figure 8" width="950"/>
    <br>
    <em>Figure 8: Adding a component to the farm.</em>
</p> 

<br>

### Wheat with Cover Crop

Our first field on the farm will grow continuous wheat with a cover crop of hairy vetch. Change the following elements in the **'Field #1'** component.

1. Rename the field to **"Wheat & Hairy Vetch"** in the **Step 1** section of the screen. Change the area of the field to **"18 ha"**.

2. Select **"Wheat"** as the Crop from the **Small grain cereals** subcategory  and **"Hairy Vetch"** as the **Winter/Cover/Undersown Crop** in **Step 2**.

3. Under the **'General'** tab:
    * Enter a yield of **"3,000 kg ha<sup>-1</sup>"** (wet weight). The dry weight value will be calculated automatically based on the moisture content of crop value..
    * Select "**Reduced Tillage**" as the tillage type.
    * Enter "**200**" as the amount of irrigation.
    * Select **"0"** as the number of pesticide passes.
    * Leave 'Harvest method' as the default selection.

<br>

<p align="center">
    <img src="../../Images/Training/AnaerobicDigestion/figure9.png" alt="Figure 9" width="950"/>
    <br>
    <em>Figure 9: Field Component of the farm.</em>
</p> 

<br> 

4. Select the **'Fertilizer'** tab and click the **Add Fertilizer Application** button. Holos has now added a new fertilizer application for this field and will suggest Urea as the fertilizer blend. A default application rate is calculated based on the yield value entered for this field. Details of this fertilizer application can be changed by clicking the **Show Additional Information** button (e.g., season of application, different fertilizer blend, etc.).

<br>

<p align="center">
    <img src="../../Images/Training/AnaerobicDigestion/figure10.gif" alt="Figure 10" width="950"/>
    <br>
    <em>Figure 10: Adding fertilizer to a field.</em>
</p> 

<br>

> *Note: At a minimum, Holos requires the area of the field, type of crop grown, and a field-specific fertilizer application rate to calculate direct and indirect nitrous oxide emissions.*
 
> *Residue management of each crop (and cover crop) can be adjusted in Holos (see ‘Residue’ tab). Holos provides default values depending on the type of crop being grown and will set a value for percentage of product returned to soil, percentage of straw returned to soil, etc. These residue input settings will have an impact on the final soil carbon change estimates.*
 
> *Furthermore, biomass fractions and N concentrations can be overwritten by the user, and in this way ‘custom’ crops can be added that are currently not available.*


<div style="page-break-after: always"></div>

## Native Pasture

The cow-calf operation (defined later on) relies on native pasture for the summer months (May through October).

1. Drag a new **Field component** to your list of components. Enter the name **"Native Pasture"** in the **Field name** input box.

2. Enter **"100"** ha as the total area of the field.

3. Select **"Rangeland (Native)"** from the drop-down crop list in the **Crop** column under **Step 2**. Please note that Holos auto-populates the **Winter/Cover/Undersown Crop** area when a perennial crop is selected.

4. Keep **"0"** as the amount of irrigation and number of pesticide passes.

5. No fertilizer is used for this crop.

<br>

<p align="center">
    <img src="../../Images/Training/AnaerobicDigestion/figure11.png" alt="Figure 11" width="950"/>
    <br>
    <em>Figure 11: Native Pasture information.</em>
</p> 

<br>



<div style="page-break-after: always"></div>

# Beef Finishing Operation

Click on **View** menu item and uncheck the **Hide List of Available Components** option.

Adding animal components follows the exact same approach that was used for land management components. Under the **Beef Production** category in the available components, drag and drop one **Beef Finisher** component to the **My farm** section on the right. Both **Heifers** and **Steers** will be used in our example under **Step 1**. (Users can remove a group by clicking the **X** icon to the right of the Group name if that entry if not needed).

<br>

<p align="center">
    <img src="../../Images/Training/AnaerobicDigestion/figure12.gif" alt="Figure 12" width="950"/>
    <br>
    <em>Figure 12: The Beef Finisher Component.</em>
</p>

<br>

### Entering Heifers and Steers Information.

Following a typical beef finishing feeding cycle, the beef farm we are working with has a **single management (production) period**. We can go to **Step 2** and rename 'Management period #1' to **"Winter Feeding"**. As we have Native Pasture on our farm we can add an additional management period and place our animals on pasture for the end of the feeding cycle. Again under **Step 2** click the 'Add Management Period' box to add an additional period, and rename it **"Summer Grazing"**. We can now enter production and management data corresponding to these two management periods. 

<br>

#### Heifers - Winter Feeding


1. Under **Step 1**, make sure that the **'Heifers'** row is selected in order to enter the associated management information for that group.

2. Click the management period named **'Winter Feeding'** in **Step 2** to activate that management period.

3. Ensure **"January 19, 2025"** is set as the **Start date** and that **"July 07, 2025"** is set as the **End date**. The 'Number of days' shown will be 169.

Next, we can enter data related to the number of animals, diet, manure system, and housing type.

4. Under the **General Tab:**
    * Enter **"300"** as the number of animals.
    * Keep the remaining entries at their default values.

<br>

> *Note: The number of animals, average daily gain, and feed quality are the minimum required inputs for calculating methane and nitrous oxide emissions. Length of management periods (e.g., duration of grazing) will also be needed. Housing and manure management information are also important inputs but are relatively more impactful on the emissions of monogastrics.*

<br>

5. Under the **Diet Tab:**

    * We are going to create our own custom diet for our group of heifers during the **'Winter feeding'** management period. 
    
    * Click on the **"Custom Diet Creator"** button. Note that Holos provides a default set of animal diets that can be used as well.
    
  > *Note: Holos incorporates feed ingredient information from the recently published Nutrient Requirements of Beef Cattle book (2016).*

6. **Custom-Diet Creator:**

    * Click the **'Add Custom Diet'** button in the **Step 1** section of the screen to create a new custom diet.
    * Rename this diet to **"My Custom Cow Diet"** then press the Enter key to save the name.
    * To add ingredients to our new diet, under **Step 2** select **"Alfalfa hay"** from the ingredient list, and then click the **'Add Selected Ingredient to Diet'** button.
    * We will add one more ingredient to our diet. Using the 'Full Text Search' window type in and select **"Corn silage"** from the ingredient list and click the **'Add Selected Ingredient to Diet'** button once again.
    * Enter **"50%"** for **Alfalfa hay** and **"50%"** for **Corn silage** in **Step 3**. Note that Holos now reports the diet being complete since all ingredients total to 100%.

7. Click the **'OK'** button to save the new custom diet
8. Select the **"My Custom Cow Diet"** from the drop down-down menu in **'Diet Type'**.


 > *Note: Diet quality information such as crude protein, total digestible nutrient, and fat are required inputs so that Holos can estimate enteric methane emissions from an animal group.*

<br>

<p align="center">
    <img src="../../Images/Training/AnaerobicDigestion/figure13.png" alt="Figure 13" width="850"/>
    <br>
    <em>Figure 13: Custom diet creator for Heifers animal group.</em>
</p> 

<br>

9. Under the **Housing Tab:** 
    * Select **'Confined no barn'** as the housing type.


10. Under the **Manure Tab:**
    * Select **'Deep Bedding'** as the manure handling system.
    

<br>

#### Heifers - Summer Grazing

1. Click on the management period named **'Summer Grazing'**. Ensure **"July 08, 2025"** is set as the **Start date** and that **"September 15, 2025"** is set as the **End date**. The 'Number of days' shown will be 70.


2. Under the **General Tab:**
    * Enter **"300"** as the number of animals.
    

3. Under the **Diet Tab:**
    * Select **'High energy protein'** as the diet type.

4. Under the **Housing Tab:**
    * Select **Pasture/range/paddock** as the housing type.
    * Select **Native Pasture** as the pasture location.
 
5. Under the **Manure Tab:
    * Select **Pasture/range/paddock** as the manure handling system.

<br>

<p align="center">
    <img src="../../Images/Training/AnaerobicDigestion/figure14.png" alt="Figure 14" width="950"/>
    <br>
    <em>Figure 14: Beef Finisher, Heifers group</em>
</p>

<br> 


#### Steers

Click on the **Steers** row in the animal group section **Step 1** to enter information related to diet, housing and manure management. This group will be the exact same as the heifers group.

- Right click on the **Steers** animal group. A menu will appear allowing you to select the option to copy management periods from another animal group. Since the management for the steers is identical to the management for the heifers, click the **Copy Management From** -> **Heifers** sub-menu item.
- The **Steers** group is now auto-populated with the appropriate entry information for all tabs.

<br>

<p align="center">
    <img src="../../Images/Training/AnaerobicDigestion/figure15.png" alt="Figure 15" width="950"/>
    <br>
    <em>Figure 15: Copying data from another animal group</em>
</p> 

<br>

### Adding a Manure Application to the Wheat Field

In Holos, the user can apply livestock manure to a field using either manure produced by the livestock on the farm or using manure imported from off-farm. Since we have now defined our animal components, we can apply manure to any field on our farm.

1. Select the **Wheat & hairy vetch** field from the list of components added to our farm.

2. Click on the **Manure tab** and then click the **Add Manure Application** button. 
    * Add **'October 01, 2025'** as the application date. 
    * Select **'Beef cattle'** as the Manure type.
    * Select **'Livestock'** as the Origin of manure.
    * Select **'Deep Bedding'** as the Manure handling system.
    * Select **'Solid spread'** as the Application method.
    * Enter **'1,500 kg ha<sup>-1</sup>'** as the amount of manure applied to this field.
3. Notice the dial which visually shows the **Amount of Stored Manure N Remaining (Beef cattle)(kg N)** on the farm. It will move for any amount of removals/adjustments to the manure produced on-farm. This value will decrease once manure is added to the Anaerobic digestor component.       
4. Note that both chemical fertilizers and manure applications can be made on the same field

<br>

<p align="center">
    <img src="../../Images/Training/AnaerobicDigestion/figure16.gif" alt="Figure 16" width="950"/>
    <br>
    <em>Figure 16: Adding a Manure Application to a Field</em>
</p> 

<br>


### Adding supplemental hay/forage for grazing animals

We can also add additional hay/forage for animals that are grazing on a particular field. Since we have now placed a group of animals on the “Native Pasture” field component, we can then add an additional forage supplement for these grazing animals. Now shown on the **Grazing tab** are **'Grazing Details'**, highlighting the grazing events for the farm. These details can also be edited here by the user if desired.

1. Select the **Native Pasture** field component we created earlier.

2. From the **Grazing tab.**
    - Click the **Add Supplemental Hay** button to add additional forage for the animals on this field.
    - Enter **"July 15, 2025"** as the date.
    - Enter **"Off-farm"** as the **Sources of bales**.
    - Choose **"Native Pasture"** as the field.
    - Change the **Number of bales** to **"10".
    - Enter **"500"** as the wet bale weight.
    - Keep the moisture content as the default value.
3. On the field receiving the supplemental forage, there is a chart showing how much forage is still available on the right of the screen for reference.
  
> *Note: It is not recommended to mix different species of grasses together. Here, we are only demonstrating the ability of Holos to add supplemental hay to a field that has grazing animals.*

<br>

<p align="center">
    <img src="../../Images/Training/AnaerobicDigestion/figure17.png" alt="Figure 17" width="950"/>
    <br>
    <em>Figure 17 - Adding supplemental hay/forage for grazing animals.</em>
</p>
 
<br>

## Anaerobic Digestion Component:

We will now add the last animal component to our farm. In addition to the beef cattle operations of this farm, we will also be adding a **"Anaerobic Digestion"** component found in the 'Available Components' under the **"Infrastructure"** drop-down menu. 

> *Note: The anaerobic digestion (AD) component in Holos has the following assumptions: 1. The system is a wet anaerobic continuously stirred tank reactor (CSTR), 2. The primary feedstock is livestock manure with an optional co-feedstock of crop residues, 3. Biogas valorisation: combined heat and power (CHP) or direct injection to the gas grid, and 4. the default digestate treatment is solid-liquid separation.* 

The overall structure of the AD component is presented in Figure 18. Blue boxes represent unit processes; green pools represent valorisable substrates and products; arrows represent nutrient and C flows. 

Currently, in Holos V4, the option to add stored manure to an AD system is possible only for the major livestock groups, i.e., beef cattle, dairy cattle, sheep, swine and the major poultry groups (broilers, layers, pullets, turkeys), as the model does not currently estimate all required parameters for the AD calculations for other livestock types. The addition of stored manure from other livestock groups may be incorporated into a future version of Holos. 

<br>

<p align="center">
    <img src="../../Images/Training/AnaerobicDigestion/figure18.png" alt="Figure 18" width="950"/>
    <br>
    <em>Figure 18 - Flow diagram representing the structure of the anaerobic digestion component in Holos.</em>
</p>
 
<br>

All manure produced by animals on the farm can be added to the anaerobic digestor. If there are no animals on the farm, it is also possible to use imported manure as a feedstock for the digestor. To add imported manure, the user must enter a daily amount fed into the digestor and the manure type. 

1. Drag the **"Anaerobic Digestion"** component to the farm. 

2. Under **Step 1** leave the default setting of 'Yes' for liquid/solid seperation of the digestate.

3. Under **Step 2** click the **'Add'** checkbox to choose the management periods which will be the sources of manure for the digestor. Select both the Heifers and Steers groups. By default, all daily manure (100%) produced by the selected management period is added to the digestor. This daily pertcentage amount can be changed. Adjust the 'Daily percentage of manure added (%) to **"50"**. Leave the 'Use imported manure' choice as **'No'** in this case. 

4. Under **Step 3** we can add both/either 'Crop Residue Substrate' and 'Farm Residue Subsrtate' to the digestor by selecting each button and filling in the respective information.
   - Click the 'Add Crop Residue Substrate Type' button and under the **Crop** column choose **"Wheat"**.
   - Add a **Flow rate** of **"100"** (kg wet weight day<sup>-1</sup>)
   - Enter the 'Start date' as **"January 01, 2025"** and the 'End Date' as **"December 31, 2025"**.
   - Click the 'Add Farm Residue Substrate Type' button and under the **Farm Residue** column choose **"Vegetable Oil"**.
   - Add a **Flow rate** of **"10"** (kg wet weight day<sup>-1</sup>)
   - Enter the 'Start date' as **"March 01, 2025"** and the 'End Date' as **"October 31, 2025"**.
   
5. Under **Step 4** finally select the type of energy to be the 'Target digestor output'. Here we can chose **'Methane injection to gas grid'** as the option.

<br>

<p align="center">
    <img src="../../Images/Training/AnaerobicDigestion/figure19.png" alt="Figure 19" width="950"/>
    <br>
    <em>Figure 19 - The Anaerobic Digestion component.</em>
</p>
 
<br>

> *Note: The addition of fresh/raw manure is not currently enabled in Holos V4, but will be activated in a future update of the model. The model user has the option to add manure from a storage system to the anaerobic digester – this includes both solid and liquid manure types. (This type of manure or AD substrate refers to manure removed from a manure storage system after all C and N losses during the housing and storage stages have been accounted for).*
 
# Timeline Screen

We are now finishing the process of defining our farm. Click the **Next** button to go forward to the **'Timeline'** screen. 

The Timeline screen provides a visual layout of all the fields from 1985 to the specified end year for each field. This screen also allows the user to add historical and projected production systems under **'Step 2'**. 

The **'Add Historical Production System'** button enables the user to add a different cropping history to individual fields whereas the **'Add Projected Production System'** button enables the user to add a future (projected) cropping system to individual fields.

As our demonstration farm does not have a historical production system, we can leave the screen as showing 'Native Pasture' and 'Wheat and Hairy Vetch' for their respective 40 year durations (1985-2025). 

We can now proceed by selecting the **'Next'** button on the bottom of the screen.

<br>

# Details Screen

Going to go forward to the details screen.

To avoid the requirement that a user needs to provide annual crop yields going back to 1985 (or the specified start year, if different) for each field on the farm, the model will use default year- and crop-specific yield data from *Statistics Canada* (where available). Changes in crop yield affect various model outputs, including soil carbon sequestration rates and soil N<sub>2</sub>O emissions. The following steps demonstrate how adjusting the crop yield affects the above- and below-ground carbon inputs to the soil.

We will adjust this grid so that we can view the above-ground and below-ground carbon inputs for our wheat field and then we will adjust the crop yield for one specific year.

1. We will set a filter on the first column **'Field name'** so that we only display information for our **'Wheat and Hairy Vetch'** field. Beside the column heading, click the **'funnel'** icon to set a filter. Check the box beside **"Wheat and Hairy Vetch"**.

2. On the far left of this screen, click the **"Enable Columns"** sidebar ribbon (located to the left of the 'Field name' column).

3. Place a check beside **"Above-ground carbon input"** to show the column and remove the check beside the **"Notes"** column to hide it.

4. Click the **'Enable Columns'** sidebar again to collapse it.

5. We can now (optionally) adjust the yields for our wheat field for any given year if actual measured yields are available.

6. Adjust the yield for **1987** to be **4,100** kg/ha.

7. Note that Holos has updated the above-ground carbon inputs for this.


<br>

<p align="center">
    <img src="../../Images/Training/AnaerobicDigestion/figure20.png" alt="Figure 20" width="950"/>
    <br>
    <em>Figure 20: Details screen</em>
</p> 

<br>

#  Discover Results

Click the **"Next"** button to move to the final results report. Results will now be displayed in a variety of reports and charts. Here are a few highlighted:

1. Click on the tab named **'Emissions Pie Chart'** to display its graphic.

   - Starting with the **Emissions pie chart** allows us to see an overall breakdown of the enteric CH4, manure CH4, direct and indirect N2O. We are also able to see a detailed breakdown of the sources of these emissions.
   
   - The **"Overall Emissions"** and **"Component Emissions"** tabs provide further graphical breakdowns of sources of emissions for each GHG and the components responsible for their generation.

2. Click **"Yes"** on the **'Show details'** button at the top.

   - We can see that the biggest source of emissions from our farm is the cows. If you hover your mouse pointer over any slice of this chart you can get an isolated look at the different emission sources.
    
<br>

<p align="center">
    <img src="../../Images/Training/AnaerobicDigestion/figure21.png" alt="Figure 21" width="950"/>
    <br>
    <em>Figure 21: Emissions Pie Chart showing Details.</em>
</p> 

<br>

3. Click on the tab named **"Detailed Emission Report"**.

   - The **'Detailed Emission Report'** will display a monthly or annual GHG emission report. The detailed emission report will report on enteric methane, manure methane, direct & indirect N2O, and CO2 emissions from the farm.

   - Click the **'Report Format (Monthly)'** button to switch to a monthly report. Now we can see a monthly breakdown of all emissions from the farm and the emission source.
   - The **'Report year'** can be selected from the drop down menu.
   - In the **'Unit of measurement'** drop-down menu, one can choose to have the results displayed as CO2 equivalents (CO2e) or as unconverted greenhouse gas (GHG), and you can also choose the unit of measurement as either tonnes or kilograms.
   - We can export this data by clicking the **"Export to Excel"** button.

<br>

<p align="center">
    <img src="../../Images/Training/AnaerobicDigestion/figure22.png" alt="Figure 22" width="950"/>
    <br>
    <em>Figure 22: Detailed Emission report.</em>
</p> 

<br>

4. Click on the **"Anaerobic Digestion"** tab to get the results of our on farm digestor. We can chose the results frequency from the 'Report Format' drop-down menu. We will display it as 'Monthly'.
   - The data is categorized into the sections: 'Digestate Flows', 'Biogas & Methane Potential', 'Storage Emissions', and 'Land Application' for ferilizer use.
   - Click the **'Report Format (Monthly)'** button to switch to a monthly report.
   - Again, we can export this data by clicking the **"Export to Excel"** button.

<br>

<p align="center">
    <img src="../../Images/Training/AnaerobicDigestion/figure23.png" alt="Figure 23" width="950"/>
    <br>
    <em>Figure 23: Anaerobic Digestion report.</em>
</p> 

<br>

5. The **'Estimates of Production'** report provides total harvest yields for crops, farm land information, and the available amount of land applied manure.

   The **'Feed Estimate'** report provides an estimate of dry matter intake based on energy requirements of the animal and the energy in the feed.

   The **'Manure Management'** tab provides detailed information of the manure produced on the farm. It also has an 'Enable columns' ribbon on the left to select for columns to be included in the report. Again, we can export this data by clicking the **"Export to Excel"** button to the left. 

<br>

## Soil carbon modelling results

On the results screen we can see the change in soil carbon over time by clicking the **“Multiyear Carbon Modelling”** tab. This tab displays a graph showing the change in soil carbon over time for each one of our fields.

For each field on the graph, you can hover your mouse over the series to get more information for each historical year of the field.

If we click on one of these points, we can then view a more detailed breakdown of these results via the 'Grid' report format. We can also export this data by clicking the **"Export to Excel"** button.

<br>

<p align="center">
    <img src="../../Images/Training/AnaerobicDigestion/figure24.png" alt="Figure 24" width="950"/>
    <br>
    <em>Figure 24: Multiyear Carbon Modelling report.</em>
</p> 

<br>


> If you would like to export your entire farm file, from **File** on the main taskbar select **'Export'**. Click the arrow to highlight your farm and save it as a .json file.

<div style="page-break-after: always"></div>

# Finally...

## Whole-systems approach 

> An ecosystem consists of not only the organisms and the environment they live in but also the interactions within and between. A whole systems approach seeks to describe and understand the entire system as an integrated whole, rather than as individual components. This holistic approach can be very complex and describing the process can be difficult. One method to conceptualize a whole system is with a mathematical model.
> 
> The whole-systems approach ensures the effects of management changes are transferred throughout the entire system to the resulting net farm emissions. In some cases, reducing one GHG will actually increase the emissions of another. The whole-systems approach avoids potentially ill-advised practices based on preoccupation with one individual GHG.


To download Holos, for more information, or to access a recent list of Holos related publications, visit: www.agr.gc.ca

To contact us, email:
aafc.holos.acc@canada.ca

