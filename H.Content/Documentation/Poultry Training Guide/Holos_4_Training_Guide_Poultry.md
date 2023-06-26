<p align="center">
 <img src="../../Images/logo.png" alt="Holos Logo" width="650"/>
    <br>
</p>
The purpose of this document is to provide an introduction on how to use the Holos model (version 4) and the required vs. optional inputs.

For the purpose of this training, we are going to create a farm that has an annual poultry production system (including both meat and eggs), and a feed crop production system. The farm is located in Manitoba near Portage La Prairie. 

<br>

# Launch Holos

Please note that Holos 4 can be installed on a Microsoft Windows PC only. Mac OS will be supported in the next version.

Launch Holos by double-clicking on the Holos desktop icon. Holos will ask the user to open an existing farm, create a new farm, or import a saved farm file (Figure 1). If there is already a saved farm in the system, the user can click **Open**. If there are no saved farms in the system Holos will ask the user if they want to create a **New** farm or **Import** a saved farm file (i.e., a .json file). If the user creates a new farm, they are asked for the farm name and an optional comment (Figure 2).  

Enter **"Holos 2023"** as the Name and **"Training Version"** as the Comments.  Click **OK** to proceed to the next screen.

Ensure **"Metric"** is selected as the unit of measurement type and then click the **Next** button at the bottom of the screen (Figure 3). 

<br>

<p align="center">
    <img src="../../Images/PoultryGuide/en/Figure1.png" alt="Figure 1" width="650"/>
    <br>
    <em>Figure 1: If a farm has been previously saved, Holos will prompt to re-open that farm.</em>
</p>

<br>

<p align="center">
    <img src="../../Images/PoultryGuide/en/Figure2.png" alt="Figure 2" width="750"/>
    <br>
    <em>Figure 2: Entering a name for the new farm. </em>
</p>

<br>

<p align="center">
    <img src="../../Images/PoultryGuide/en/Figure3.png" alt="Figure 3" width="550"/>
    <br>
    <em>Figure 3: Select metric as the unit of measurement.</em>
</p>

<br>

# Creating and Locating the New Poultry Farm

The poultry farm that we will create for this exercise is located in the province of Manitoba. Select **"Manitoba"** on the **Select a province** screen, and then click the **Next** button.

<br>

<p align="center">
    <img src="../../Images/PoultryGuide/en/Figure4.png" alt="Figure 4" width="550"/>
    <br>
    <em>Figure 4: Select Manitoba as the province.</em>
</p>

<br>

Holos uses **Soil Landscapes of Canada** (SLC), which are a series of GIS coverages that show the major characteristics of soils and land for all of Canada (compiled at a scale of 1:1 million). SLC polygons may contain one or more distinct soil landscape components.

The “**Farm Location**” screen brings up a map of Canada with the province of Manitoba centered on the screen (Figure 5). 

The map contains red colored polygons that can be selected by moving the cursor over the region that contains the location of your farm. You can zoom in or out of the map by using the mouse wheel or by hovering the cursor over the zoom icon at the bottom of the screen.

The beef farm for this example is located between Winnipeg and Portage la Prairie (Portage) with SLC polygon number **851003**. 


Find and right-click on this polygon to select it on the map (Figure 6). Note that at this point daily climate data will be downloaded from [NASA](https://power.larc.nasa.gov/data-access-viewer/). 

<br>

> *Note: Climate data is central to most calculations performed by Holos. For the most accurate estimation of farm emissions, measured climate data should be provided by the user which will override the default data obtained from the NASA weather API. If the user chooses to use the default NASA climate data, these data are available in a 10 km grid, and so can vary throughout the SLC polygon, depending on the precise location of the farm. Therefore, if possible, the user should choose the location of their farm as precisely as possible. Doing so can be aided by using different views (e.g., the Aerial view), which can be selected via the eye icon at the bottom of the map on the Farm Location screen.*
> 

> *Holos will use daily precipitation, temperature, and potential evapotranspiration values to model soil carbon change (climate parameter), nitrous oxide emissions, as well as ammonia volatilization.*

<br>

<p align="center">
    <img src="../../Images/PoultryGuide/en/Figure5.png" alt="Figure 5" width="950"/>
    <br>
    <em>Figure 5: Map of the Manitoba province showing the different selectable polygons.</em>
</p>

<br>

<p align="center">
    <img src="../../Images/PoultryGuide/en/Figure6.png" alt="Figure 6" width="950"/>
    <br>
    <em>Figure 6: Selecting the SLC polygon for the farm location.</em>
</p>  

<br>

Once the farm location is selected, soil information (texture, sand, and clay proportions) for the types of soils found in this polygon are displayed on the right side of the screen. It is possible that more than one soil type per polygon will be found and the user is expected to select their soil type from this list or use the default selection (Figure 7). The default soil type selected represents the dominant soil type for the chosen polygon.

For this tutorial, keep the default **Soil Zone** as 'Black' soil, and the default "**Hardiness Zone**" as '3b'. 

<br>

<p align="center">
    <img src="../../Images/PoultryGuide/en/Figure7.png" alt="Figure 7" width="950"/>
    <br>
    <em>Figure 7: Multiple soil types might be available for a given region.</em>
</p>  

<br>

**Note: Soil data obtained from the user’s selected location will be used in the calculation of location-specific N<sub>2</sub>O emission factors. Properties such as soil texture, top layer thickness, and soil pH are required for these calculations, and can be overwritten on the Component Selection screen, under Settings > Farm Defaults > Soil.*

<br>

Click the **Next** button to proceed to the next step.

<div style="page-break-after: always"></div>

# Selecting Farm Components

Now that the farm location has been selected, we can move on to the “Component Selection” screen. This is where the user can select different components for their farm. Holos will display all available components on the left side of the screen under the “All Available Components” column (Figure 8). These components are grouped into various categories including Land Management, Beef and Dairy Cattle, Other Livestock and Poultry.

If we click on the drop-down button next to a category's name, we can then see the available components in that category.  For this portion of the  training section, we will be working with the “Land management” and “Poultry” categories. 

<br>

<p align="center">
    <img src="../../Images/PoultryGuide/en/Figure8.png" alt="Figure 8" width="950"/>
    <br>
    <em>Figure 8: The available components screen. Specific components can be chosen here to include in the farm.</em>
</p> 

<br>

The Holos model is designed so that the land management components are defined before the livestock components. This is because the model allows for the placement of livestock onto a specific field(s) (i.e., pasture(s)) for grazing. It is easier to do this if the pasture field has already been defined. However, the user can first set up their livestock components and then their field components, but will then need to return to their livestock components to specify and ‘place’ them on pasture.

<div style="page-break-after: always"></div>

## Crop and Hay Production

Now we can add our first component to the farm. Drag a **Field** component from the left side of the screen and drop it on the **My Farm** on the right side (Figure 9). The screen will now update to reflect this new component that you have added to your farm. Holos will  label the field as **“Field #1”**. At this point, we can enter production information related to the crop being grown on this field.

<br>

<p align="center">
    <img src="../../Images/PoultryGuide/en/Figure9.gif" alt="Figure 9" width="950"/>
    <br>
    <em>Figure 9: Adding a component to the farm.</em>
</p> 

<br>

### Wheat with Cover Crop

Our first field on the farm will grow continuous wheat with a cover crop of hairy vetch. Change the following elements in the "**Field #1**" component.

1. Rename the field to “**Wheat & Hairy Vetch**” in the “**Step 1**” section of the screen. Change the area of the field to **18 ha**.

2. Select "**Wheat**" as the main crop and "**Hairy Vetch**" as the cover crop in "**Step 2**".

3. Under the "**General**" tab:
    * Enter a yield of **"3,000 kg ha<sup>-1</sup>"** (wet weight). The dry weight value will be calculated automatically based on the moisture content of crop value..
    * Select "**Reduced Tillage**" as the tillage type.
    * Enter "**200**" as the amount of irrigation.
    * Select **"0"** as the number of pesticide passes.
    * Leave 'Harvest method' as the default selection.

<br>

<p align="center">
    <img src="../../Images/PoultryGuide/en/Figure10.png" alt="Figure 10" width="950"/>
    <br>
    <em>Figure 10: Field Component of the farm.</em>
</p> 

<br> 

4. Select the **Fertilizer** tab and click the **Add Fertilizer Application** button. Holos has now added a new fertilizer application for this field and will suggest Urea as the fertilizer blend. A default application rate is calculated based on the yield value entered for this field. Details of this fertilizer application can be changed by clicking the **Show Additional Information** button (e.g., season of application, different fertilizer blend, etc.).

<br>

<p align="center">
    <img src="../../Images/PoultryGuide/en/Figure11.gif" alt="Figure 11" width="950"/>
    <br>
    <em>Figure 11: Adding fertilizer to a field.</em>
</p> 

<br>

> *Note: At a minimum, Holos requires the area of the field, type of crop grown, and a field-specific fertilizer application rate to calculate direct and indirect nitrous oxide emissions.*
 
> *Residue management of each crop (and cover crop) can be adjusted in Holos (see ‘Residue’ tab). Holos provides default values depending on the type of crop being grown and will set a value for percentage of product returned to soil, percentage of straw returned to soil, etc. These residue input settings will have an impact on the final soil carbon change estimates.*
 
> *Furthermore, biomass fractions and N concentrations can be overwritten by the user, and in this way ‘custom’ crops can be added that are currently not available.*


<div style="page-break-after: always"></div>

## Native Grassland 

The poultry operation (defined later on) can utilize native pasture for free-range grazing in the summer months (May through October).

1. Drag a new **Field** component to your list of components. Enter the name **"Native Grassland"** in the **Field name** input box.

2. Enter **"100"** ha as the total area of the field.

3. Select **"Rangeland (Native)"** from the drop-down crop list in the **Crop** column under **Step 2**. Please note that Holos auto-populates the **Winter/Cover/Undersown Crop** area when a perennial crop is selected.

4. Keep **"0"** as the amount of irrigation and number of pesticide passes.

5. No fertilizer is used for this crop.

<br>

<p align="center">
    <img src="../../Images/PoultryGuide/en/Figure12.png" alt="Figure 12" width="950"/>
    <br>
    <em>Figure 12: Native Grasslands information.</em>
</p> 

> *Please note: In the Holos farm we are building all of the production components on 'My Farm' are to be housed in barns, thus they will not have a grazing management period. This step was included for information purposes only.*

<br>

## Barley Grain and Mixed Hay Rotation

To demonstrate the crop rotation component (as opposed to using individual field components), we will assume that barley grain and mixed hay are grown in rotation, with the mixed hay under-seeded to the barley so that it can be harvested in both main years (example derived from University of Alberta’s Breton plots). 

When using the “Crop Rotation” component, any sequence of crops that are input into this component will be applied to each individual field that is part of the rotation setup. This means one field is added for each rotation phase, and the rotation shifts so that each rotation phase is present on one field. Since each field can have a different management history, soil carbon algorithms will run for each field.

For this example, we assume that the farm requires **70 ha** of barley grain and mixed hay, which are grown in rotation. We will need to set up three fields where barley grain is rotated in each field every two years (Figure 13). When using the crop rotation component, the crop management input of a specific crop is repeated on each field in the rotation where the crop is grown. 

**To set up the rotation:** 

1. Add one "**Crop Rotation**" component from the available components.

2. To expand the horizontal space available in Holos, click on "**View**" from the top menu bar and select "**Hide List of Available Components**".

3. The rotation of this field begins in **"1985"** and ends in **"2023"**. Under **Step 1**, please ensure that these two values are set as the start and end year, respectively.

4. Enter **"70"** ha as the **total area** of this field.

5. Under **Step 2** change the crop to **"Barley"**. The year for this crop should be **"2023"**.
    * Under the **General** Tab enter **"3,000 kg ha<sup>-1</sup>"** (wet weight) as the yield for this crop.
     * Change the tillage type to **"Reduced Tillage"**.
    * Keep **"0"** as the amount of irrigation and number of pesticide passes.

6. Now add another crop to this rotation. Click on "**Add Crop**" under "**Step 2**" to add a second crop to the rotation. 

<br>

> *Note: Holos sets the year for this new crop to 2022 or one before the previous crop's year. This means that Holos is expecting the user to enter crops that have been grown in reverse order back to 1985.*

> *It is not necessary to enter a crop for each individual year going back to 1985, only enough crops to describe a single phase of the rotation will need to be entered by the user. Holos will then copy the phase information and back-populate the field history (i.e., Holos will copy the rotation back to 1985 on behalf of the user).*

<br>

7. For this newly added crop select **"Tame Mixed (grass/legume)"** as the crop type.

8. Click on the **Add crop** button one more time. For this third crop, select **"Tame Mixed (grass/legume)"** once again as the crop type.

9. Now add harvest data to each of the tame mixed crops. You will need to select each **tame mixed** crop and add the harvest data to that specific crop. So select the first tame mixed crop (2022) and then:
    * Under the **Harvest Tab** click the **"Add Harvest Date"** button to create a new harvest.
    * Select a Harvest date of "**"August 31, 2022"**", assuming the harvest is done on the same day every year.
    * Select **"Mid"** for Forage growth stage.
    * Enter **"5"** as the total number of bales.
    * Enter **"500"** as the Wet bale weight.
    
10. **Repeat** Step 9 for the second tame mixed crop.

If the tame mixed field is harvested more than once, the **Add Harvest Date** button can be used to add subsequent harvests.

<br>

<p align="center">
    <img src="../../Images/PoultryGuide/en/Figure13.png" alt="Figure 13" width="950"/>
    <br>
    <em>Figure 13: An example of a crop rotation of three crops.</em>
</p> 

<br>

<div style="page-break-after: always"></div>

# Poultry Operation

Adding animal components follows the exact same approach that was used for the land management components. Under the **Poultry** category, drag and drop the available components of **Chicken Meat Production**, **Chicken Egg Production**, and **Pullet Farm** to the **My farm** section on the right.

<br>

<p align="center">
    <img src="../../Images/PoultryGuide/en/Figure14.png" alt="Figure 14" width="950"/>
    <br>
    <em>Figure 14: The Poultry Components.</em>
</p>

<br>

## Entering 'Chicken Meat Production' Information

If you hover your mouse cursor over the “Chicken Meat Production” component under the “Poultry” category, Holos will display a tooltip that gives a brief description of a chicken meat production operation:
    
**“Chicks arriving in the operation from a multiplier hatchery are raised to market weight (1-4 kg, depending on bird type and end product) after approximately 30-56 days (depending on the bird type and rearing system).”**


Click on **View** menu item in the top taskbar and uncheck the **Hide List of Available Components** option. 

The poultry meat production farm is **divided into three  management (production) periods** for both pullet and cockerels. We will enter production and management data corresponding to these three management periods for both of these animal groups. 

1. Under **Step 1**, make sure that the “**Pullets**” row is selected.

2. Click the management period “**Brooding**” in **Step 2** to activate that management period.

3. Ensure “**January 1, 2023**” appears as the **Start date** and that “**January 15, 2022**” appears as the **End date**. These dates are set as the defaults.

 Keep the remaining entries as their default values for the two "**Rearing stage**" management periods. Thus, all three management periods will be 14 days in length. 

Next, we can enter data related to the number of animals, manure system, and housing type for each of the three management periods. Select "**Brooding stage**" first, and then repeat the steps for each of the "**Rearing stage**" periods following. This means the number of animals will be consistent throughout the management periods / year. 

* **General Tab:**
    * Enter **75,000** as the number of animals. 
    
* **Housing Tab**: 
    * Leave **Housed in barn** as the default housing type.

* **Manure Tab**:
    * Leave **Solid storage (with or without litter)** as the default manure handling system.
    
We can now go back and fill in the selections for the **Cockerels** group. Highlight the the "**Pullets**" group and right click on it to activate the **Copy Management From -> Pullets**. All of the previously entered information will appear within the **Cockerels** group. 

> *Note: The number of animals, average daily gain, and feed quality are the minimum required inputs for calculating methane and nitrous oxide emissions. Length of management periods (e.g., duration of grazing) will also be needed.*


## Entering 'Chicken Egg Production' Information

Click on the "**Hens**" row in the animal group section in **Step 1**. Information related to numbers, housing, and manure management is entered the same as it was for the cockerel and pullet groups above. As egg-laying occurs year round, there is only a single 365 day management period for hens.

* **General Tab:**

   * Enter **10,000** as the number of animals.

* **Housing Tab:**

    * Leave **Housed in barn** as the default housing type.

* **Manure Tab:**

    * Leave **Solid storage (with or without litter)** as the default manure handling system.


## Entering 'Pullet Farm' Information:
 We can enter data related to the number of animals, manure system, and housing type for each of the two management periods. Select "**Brooding stage**" first, and then repeat the steps for the "**Rearing stage**". The number of animals will be consistent throughout the management periods / year. 
 
1. Under **Step 1**, make sure that the “**Pullets**” row is selected.

2. Click the management period “**Brooding**” in **Step 2** to activate that management period.

3. Ensure “**January 1, 2023**” appears as the **Start date** and that “**January 15, 2022**” appears as the **End date**. These dates are set as the defaults.

* **General Tab:**:
    * Enter **150,000** as the number of animals. 
    
* **Housing Tab**: 
    * Leave **Housed in barn** as the default housing type.

* **Manure Tab**:
    * Leave **Solid storage (with or without litter)** as the default manure handling system.

Highlight the "**Rearing stage**" and enter the above information once more. The number of days for this stage should appear as 119 as a default.


### Adding a Manure Application to the Wheat Field

In Holos, the user can apply livestock manure to a field using either manure produced by the livestock on the farm or using manure imported from off-farm. Since we have now defined our animal components, we can apply manure to any field on our farm.

1. Select the **Wheat & hairy vetch** field from the list of components added to our farm.

2. Click on the **Manure tab** and then click the **Add Manure Application** button. 
    * Select **Livestock** as the **Origin of manure**.
    * Select **Poultry** as the **Manure type** .
    * Select **Solid storage (with or without litter)** as the **Manure handling system**.
    * Select **Slurry broadcasting** as the **Application method**
    * Enter **14000 kg/ha** as the amount of manure applied to this field.
3. Note that both chemical fertilizers and manure applications can be made on the same field.

> Holos provides an interactive gauge on the left of the manure tab to show the "**Amount of Stored Manure N Remaining (poultry) (kg N)**" which will move to reflect the users application choices and quantify manure availability.

<br>

<p align="center">
    <img src="../../Images/PoultryGuide/en/Figure15.png" alt="Figure 15" width="950"/>
    <br>
    <em>Figure 15 - Adding a Manure Application to a Field.</em>
</p>
 
<br>

# Timeline Screen

We are now finished the process of defining our farm. Click the **Next** button to go forward to the timeline screen.
The timeline screen provides a visual layout of all the fields from 1985 to the specified end year for each field. This screen also allows the user to add historical and projected production systems. 

The **Add Historical Production System** button enables the user to add a different cropping history to individual fields whereas the **Add Projected Production System** button enables the user to add a future (projected) cropping system to individual fields.


### Adding a historical production system


We will assume that the barley grain and mixed hay rotation fields were previously in a continuous wheat cropping system between **1985 and 2000**.

1. To add a new historical cropping system, select one of the fields that are in the barley grain and mixed hay rotation. To select an item, click on the timeline bar to activate that field. We will select the first field in this rotation (i.e., the field with the name of **"Crop rotation #1 [Field #1] - Barley"**).

2. Click on the **Add Historical Production System** button which will add a new row to the table under the **"Step 1"** section in the upper left section of the screen. Notice that this new entry has the words **"Historical management practice"** added.

3. We will set the end year of this historical management practice to the year **2000**. To adjust this we use the numeric up/down buttons within the cell.

4. Select the newly added **Historical management practice** and then click the **"Edit Selected"** button. This will open a new screen that allows us to adjust the crops grown and the management during this period.

5. Click on the **"Barley"** crop under the **"Step 2"** section. Change the crop type to **'Wheat'** and on the **'General'** tab change the yield to **3,500** kg/ha. We will keep the other settings unchanged.

6. We also need to remove the **"Tame mixed"** crops from this historical period. Click the **'X'** icon beside each of the **"Tame mixed"** crops under the **"Step 2"** section. Clicking the **'X'** icon will remove these crops from the rotation for this period of time.

7. Click **"OK"** to save adjustments we just made to this field.

8. Repeat these same steps so that the other fields in this rotation also have continuous wheat from **1985 to 2000** using the same steps we used for the first field.


<br>

<p align="center">
    <img src="../../Images/PoultryGuide/en/Figure16.png" alt="Figure 16" width="950"/>
    <br>
    <em>Figure 16: Customized Timeline Screen</em>
</p> 

<br>  

<p align="center">
    <img src="../../Images/PoultryGuide/en/Figure17.png" alt="Figure 17" width="550"/>
    <br>
    <em>Figure 17: Adjusted start and end year for productions systems on the timeline screen.</em>
</p> 

<br> 

<p align="center">
    <img src="../../Images/PoultryGuide/en/Figure18.png" alt="Figure 18" width="650"/>
    <br>
    <em>Figure 18: Editing crops in a historical period of the rotation. </em>
</p> 

<div style="page-break-after: always"></div>


# Details Screen

Click the **"Next"** button to go forward to the details screen.

To avoid the requirement that a user needs to provide annual crop yields going back to 1985 (or the specified start year, if different) for each field on the farm, the model will use default year- and crop-specific yield data from *Statistics Canada* (where available). Changes in crop yield affect various model outputs, including soil carbon sequestration rates and soil N<sub>2</sub>O emissions. The following steps demonstrate how adjusting the crop yield affects the above- and below-ground carbon inputs to the soil.

We will adjust this grid so that we can view the above-ground and below-ground carbon inputs for our wheat field and then we will adjust the crop yield for one specific year.

1. We will set a filter on the first column named **'Field name'** so that we only display information for our **'Wheat and hairy vetch field'**. Beside the column heading, click the **'funnel'** icon to set a filter. Check the box beside **'Wheat & hairy vetch'**.

2. On the far left of this screen, click the **"Enable Columns"** sidebar (located near the “Field name” column).

3. Place a check beside **"Above-ground carbon input"** to show the column and remove the check beside the **'Notes'** column to hide it.

4. Click the **'Enable Columns'** sidebar again to collapse it.

5. We can now (optionally) adjust the yields for our wheat field for any given year if actual measured yields are available.

6. Adjust the yield for **1987** to be **4,100** kg/ha.

7. Note that Holos has updated the above-ground carbon inputs for this.


<br>

<p align="center">
    <img src="../../Images/PoultryGuide/en/Figure19.png" alt="Figure 19" width="950"/>
    <br>
    <em>Figure 19: Details screen</em>
</p> 

<br>

#  Results Screen

Click the **"Next"** button to move on to discover the results in the final section. Results will now be displayed in a variety of reports and charts.

1. Click on the tab named **Detailed Emission Report**

    The **Detailed Emission Report** will display a monthly or annual GHG emission report. The detailed emission report will report on enteric methane, manure methane, direct & indirect N2O, and CO2 emissions from the farm.

2. Click the **Report Format (Monthly)** button to switch to a monthly report. Now we can see a monthly breakdown of all emissions from the farm and the emission source.

    In the **Unit of measurements** drop-down menu, you can choose to have the results displayed as CO2 equivalents (CO2e) or as unconverted greenhouse gas (GHG), and you can also choose the unit of measurement as either tonnes or kilograms.
<br>

<p align="center">
    <img src="../../Images/PoultryGuide/en/Figure20.png" alt="Figure 20" width="950"/>
    <br>
    <em>Figure 20: Detailed emissions report.</em>
</p> 

<br>

3. Click on the **Estimates of Production** report which provides total harvest yields, amount of land applied manure, and estimates of milk production for dairy components.
    
    Use the slide bar on the right of the screen to see the bar graph display of **Land Applied Manure**.

<br>

<p align="center">
    <img src="../../Images/PoultryGuide/en/Figure21.png" alt="Figure 21" width="950"/>
    <br>
    <em>Figure 21: Estimates of production report.</em>
</p> 

<br>

## Soil carbon modelling results

On the results screen we can see the change in soil carbon over time by clicking the “**Multiyear Carbon Modelling**” tab. This tab displays a graph showing the change in soil carbon over time for each one of our fields.

For each field on the graph, you can hover your mouse over the series to get more information for each historical year of the field.

If we click on one of these points, we can then view a more detailed breakdown of these results via the “Grid” report format. We can also export this data by clicking the "**Export to Excel**" button.

If you would like to export your entire farm file, from '**File**' on the main taskbar select '**Export**'. Click the arrow to highlight your farm and save it as a .json file.

<br>

<p align="center">
    <img src="../../Images/PoultryGuide/en/Figure22.png" alt="Figure 22" width="950"/>
    <br>
    <em>Figure 22: Carbon report section. Allows switching between graph and table format.</em>
</p> 

<br>

<div style="page-break-after: always"></div>

# Finally...

## Whole-systems approach 

> An ecosystem consists of not only the organisms and the environment they live in but also the interactions within and between. A whole systems approach seeks to describe and understand the entire system as an integrated whole, rather than as individual components. This holistic approach can be very complex and describing the process can be difficult. One method to conceptualize a whole system is with a mathematical model.
> 
> The whole-systems approach ensures the effects of management changes are transferred throughout the entire system to the resulting net farm emissions. In some cases, reducing one GHG will actually increase the emissions of another. The whole-systems approach avoids potentially ill-advised practices based on preoccupation with one individual GHG.


To download Holos, for more information, or to access a recent list of Holos related publications, visit: www.agr.gc.ca

To contact us, email:
aafc.holos.acc@canada.ca

