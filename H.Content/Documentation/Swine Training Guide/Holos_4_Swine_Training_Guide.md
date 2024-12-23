<p align="center">
 <img src="../../Images/logo.png" alt="Holos Logo" width="650"/>
    <br>
</p>
The purpose of this document is to provide an introduction on how to use the Holos model (version 4) and the required vs. optional inputs.

For the purpose of this training, we are going to create a farm that has a swine production system and a feed crop production system. The farm is located in Manitoba near Portage La Prairie. 

<br>

# Launch Holos

Please note that Holos 4 can be installed on a Microsoft Windows PC only. Mac OS will be supported in the next version.

Launch Holos by double-clicking on the Holos desktop icon. Holos will ask the user to open an existing farm, create a new farm, or import a saved farm file (Figure 1). If there is already a saved farm in the system, the user can click **Open**. If there are no saved farms in the system Holos will ask the user if they want to create a **New** farm or **Import** a saved farm file (i.e., a .json file). If the user creates a new farm, they are asked for the farm name and an optional comment (Figure 2).  

Enter **"Holos 2024"** as the Name and **"Training Version"** as the Comments.  Click **OK** to proceed to the next screen.

Ensure “**Metric**” is selected as the unit of measurement type and then click the **Next** button at the bottom of the screen (Figure 3). 

<br>

<p align="center">
    <img src="../../Images/SwineGuide/en/Figure 1.png" alt="Figure 1" width="650"/>
    <br>
    <em>Figure 1: If a farm has been previously saved, Holos will prompt to re-open that farm.</em>
</p>

<br>

<p align="center">
    <img src="../../Images/SwineGuide/en/Figure 2.png" alt="Figure 2" width="750"/>
    <br>
    <em>Figure 2: Entering a name for the new farm. </em>
</p>

<br>

<p align="center">
    <img src="../../Images/SwineGuide/en/Figure 3.png" alt="Figure 3" width="550"/>
    <br>
    <em>Figure 3: Select "metric" as the unit of measurement.</em>
</p>

<br>

# Creating and Locating the New Swine Farm

The swine farm that we will create for this exercise is located in the province of Manitoba. Select **"Manitoba"** on the **"Select a province"** screen, and then click the **Next** button.

<br>

<p align="center">
    <img src="../../Images/SwineGuide/en/Figure 4.png" alt="Figure 4" width="550"/>
    <br>
    <em>Figure 4: Select Manitoba as the province.</em>
</p>

<br>

Holos uses **Soil Landscapes of Canada** (SLC) polygons, which are a series of GIS coverages that report the soil characteristics for all of Canada (compiled at a scale of 1:1 million). SLC polygons may contain one or more distinct soil landscape components.

The **Farm Location** screen brings up a map of Canada with the province of Manitoba centered on the screen (Figure 5). 

The map contains red colored polygons that can be selected by moving the cursor over the region that contains the location of your farm. You can zoom in or out of the map by using the mouse wheel or by hovering the cursor over the zoom icon at the bottom of the screen.

The swine farm for this example is located between Winnipeg and Portage la Prairie (Portage), in SLC polygon number **851003**. 


Find and right-click on this polygon to select it on the map (Figure 6). Note that at this point daily climate data will be downloaded from [NASA](https://power.larc.nasa.gov/data-access-viewer/). 

<br>

> *Note: Climate data are central to most calculations performed by Holos. For the most accurate estimation of farm emissions, measured climate data should be provided by the user which will override the default data obtained from the NASA weather API. If the user chooses to use the default NASA climate data, these data are available in a 10 km grid, and so can vary throughout the SLC polygon, depending on the precise location of the farm. Therefore, if possible, the user should choose the location of their farm as precisely as possible. Doing so can be aided by using different views (e.g., the Aerial view), which can be selected via the eye icon at the bottom of the map on the Farm Location screen.*
> 

> *Holos will use daily precipitation, temperature, and potential evapotranspiration values to model soil carbon (C) change (climate parameter), nitrous oxide (N<sub>2</sub>O) emissions, as well as ammonia (NH<sub>3</sub>) volatilization.*

<br>

<p align="center">
    <img src="../../Images/SwineGuide/en/Figure 5.png" alt="Figure 5" width="950"/>
    <br>
    <em>Figure 5: Map of the Manitoba province showing the different selectable polygons.</em>
</p>

<br>

<p align="center">
    <img src="../../Images/SwineGuide/en/Figure 6.png" alt="Figure 6" width="950"/>
    <br>
    <em>Figure 6: Selecting the SLC polygon for the farm location.</em>
</p> 

<br>

Once the farm location is selected, soil information (texture, sand, and clay proportions) for the types of soils found in this polygon are displayed on the right side of the screen. It is possible that more than one soil type per polygon will be found and the user is expected to select their soil type from this list or use the default selection (Figure 7). The default soil type selected represents the dominant soil type for the chosen polygon.

For this tutorial, keep the default **Soil Zone** as 'Black' soil, and the default "**Hardiness Zone**" as '3b'. 

<br>
<p align="center">
    <img src="../../Images/SwineGuide/en/Figure 7.png" alt="Figure 7" width="950"/>
    <br>
    <em>Figure 7: Multiple soil types will be displayed for the selected SLC polygon.</em>
</p> 

<br>

> *Note: Soil data obtained from the user’s selected location will be used in the calculation of location-specific N<sub>2</sub>O emission factors. Properties such as soil texture, top layer thickness, and soil pH are required for these calculations, and can be overwritten on the Component Selection screen, under Settings > Farm Defaults > Soil.*

<br>

Click the **Next** button to proceed to the next step.

<div style="page-break-after: always"></div>

# Selecting Farm Components

Now that the farm location has been selected, we can move on to the **Component Selection** screen. This is where the user can select different components for their farm. Holos will display all available components on the left side of the screen under the **All Available Components** column (Figure 8). These components are grouped into various categories including Land Management, Beef Production, Dairy Cattle, Swine, Sheep, Poultry and Other Livestock.

If we click on the drop-down button next to a category's name, we can then see the available components in that category.  For this portion of the training section, we will be working with the “Land management” and “Swine” categories. 

<br>

<p align="center">
    <img src="../../Images/SwineGuide/en/Figure 8.png" alt="Figure 8" width="950"/>
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
    <img src="../../Images/SwineGuide/en/Figure 9.gif" alt="Figure 9" width="950"/>
    <br>
    <em>Figure 9: Adding a component to the farm.</em>
</p> 

<br>

### Wheat with Cover Crop

Our first field on the farm will grow continuous wheat with a cover crop of hairy vetch. Change the following elements in the **"Field #1"** component.

1. Rename the field to **"Wheat & Hairy Vetch"** in the **Step 1** section of the screen. Change the area of the field to **18 ha**.

2. Select **"Wheat"** as the main crop and **"Hairy Vetch"** as the cover crop in **Step 2**.

3. Under the "**General**" tab:
    * Enter a yield of **"3,000 kg ha<sup>-1</sup>"** (wet weight). The dry weight value will be calculated automatically based on the moisture content of crop value..
    * Select "**Reduced Tillage**" as the tillage type.
    * Enter "**200**" as the amount of irrigation.
    * Select **"0"** as the number of pesticide passes.
    * Leave "**Cash crop**" as the harvest method.
    

<br>

<p align="center">
    <img src="../../Images/SwineGuide/en/Figure 10.png" alt="Figure 10" width="950"/>
    <br>
    <em>Figure 10: Field Component of the farm.</em>
</p> 

<br> 

4. Select the **Fertilizer** tab and click the **"Add Fertilizer Application"** button. Holos has now added a new fertilizer application for this field and will suggest "Urea" as the fertilizer blend. A default application rate is calculated based on the yield value entered for this field. Details of this fertilizer application can be changed by clicking the **Show Additional Information** button (e.g., season of application, blend, method of application, etc.).

<br>

<p align="center">
    <img src="../../Images/SwineGuide/en/Figure 11.gif" alt="Figure 11" width="950"/>
    <br>
    <em>Figure 11: Adding fertilizer to a field.</em>
</p> 

<br>

> *Note: It is not necessary to enter a crop for each individual year going back to 1985 (or an alternative user-defined start year), only enough crops to describe a single phase of the rotation will need to be entered by the user. Holos will then copy this phase information and back-populate the field history (i.e., Holos will copy the specified rotation back to the start year on behalf of the user).*

> *At a minimum, Holos requires the area of the field, type of crop grown, and a field-specific fertilizer application rate to calculate direct and indirect N<sub>2</sub>O emissions.*

> *Residue management of each crop (and cover crop) can be adjusted in Holos (see ‘Residue’ tab). Holos provides default values depending on the type of crop being grown and will set a value for percentage of product returned to soil, percentage of straw returned to soil, etc. These residue input settings will have an impact on the final soil C change estimates, as well as soil N<sub>2</sub>O emissions.*

> *Furthermore, biomass fractions and N concentrations can be overwritten by the user, and in this way ‘custom’ crops can be added that are currently not available in the crop drop-down menus.*

<div style="page-break-after: always"></div>

### Native Grasslands 

The swine operation (defined later on) places some swine groups on pasture during the summer months (May through October).

1. Drag a new **Field** component to your list of components. Enter the name **"Native Grassland"** in the **Field name** input box.

2. Enter **"100"** ha as the total area of the field.

3. Select **"Rangeland (Native)"** from the drop-down crop list in the **Crop** column under **Step 2**. Please note that Holos auto-populates the **Winter/Cover/Undersown Crop** area when a perennial crop is selected.

4. Set **"0"** as the amount of irrigation and number of pesticide passes.

5. No fertilizer is used for this crop.

<br>

<p align="center">
    <img src="../../Images/SwineGuide/en/Figure 12.png" alt="Figure 12" width="950"/>
    <br>
    <em>Figure 12: Native Grasslands information.</em>
</p> 

<br>

# Swine Operation

Adding animal components follows the same approach used for the land management components. Under the **Swine** category in the available components, drag and drop one **Farrow-to-Wean** component to the **My farm** section on the right. In this example, all animal groups listed under this component will be used. This means we will not have to remove any animal group by clicking the **X** icon next to it under **Step 1**. We can now begin entering information into each Animal Group.

*Note: Click on the **View** menu item in the top taskbar and check/uncheck the **Hide List of Available Components** option. Checking this option hides the list of **Available Components**, allowing more room on the screen for the **My Farm** section.*

<br>

<p align="center">
    <img src="../../Images/SwineGuide/en/Figure 14.gif" alt="Figure 14" width="950"/>
    <br>
    <em>Figure 14: Adding Swine Farm Components.</em>
</p> 

<br>

## Farrow-to-Wean

The farrow-to-wean component is divided into four **Animal groups** (under Step 1), and each of these groups is then divided into a number of **Management periods** (under Step 2). We will now enter information into each of these management periods. 

### Gilts

1. Under **Step 1**, make sure that the **"Gilts"** row is selected in order to enter the associated management information for this group.

2. Click the management period named **"Open gilts"** in **Step 2** to activate that management period.

3. Ensure **"January 1, 2024"** is set as the 'Start date' and **"January 6, 2024"** is set as the 'End date' (5 days). The default start and end dates that are provided in Holos can be changed by the user and the ‘Number of days’ will adjust accordingly. 

4. Enter data related to the number of animals, diet, manure system, and housing type in **Step 3**. Under the **General** Tab:
    * Select **"Open (not lactating or pregnant)"** as the production stage.
    * Enter **"83"** as the number of animals.
    * Enter **"9"** for the litter size, **"1.4"** for the weight of piglets at birth and **"6"** for the weight of weaned piglets.
    * For all other fields keep the default values.

5. Under the **Diet Tab** select **"Gestation"** from the diet drop down menu. From the **Housing Tab** select **"Housed in Barn"** and set the Bedding type as **"Straw (Chopped)"** (keep the default application rate). From the **Manure Tab** select **"Liquid/Slurry with natural crust"**.

6. For each of the three management periods named **"Bred Gilts (Stages 1-3)"** adjust the 'Start' and 'End' dates so that each management period lasts **38** days, i.e., Stage #1: January 7, 2024 to February 14, 2024; Stage #2: February 15, 2024 to March 24, 2024; Stage #3: March 25, 2024 to May 2, 2024.

7. Highlight the **"Bred Gilts (Stage #1)"** management period and on the **General Tab:**
    * Select **"Gestating"** as the production stage.
    * Enter **"83"** as the number of animals.
    * For all other fields keep the default values.
    
8. Under the **Diet Tab** select **"Gestation"** from the diet drop down menu. From the **Housing Tab** select **"Housed in Barn"** with **"Straw (Chopped)"** bedding (keep the default application rate), and from the **Manure Tab** select **"Liquid/Slurry with natural crust"**.

9. Repeat Steps 7 and 8 for **"Bred Gilts (Stage #2) and Bred Gilts (Stage #3)"**.
  
10. Lastly, select the **"Farrowing gilts"** management period, and enter **"May 3, 2024"** as the 'Start date' and **"May 24, 2024"** as the 'End date' (21 days).

11. Under the **General Tab:**
    * Select **"Lactating"** as the production stage.
    * Enter **"83"** as the number of animals.
    * For all other fields keep the default values.
    
12. Under the **Diet Tab** select **"Lactation"** from the diet drop down menu. From the **Housing Tab** select **"Housed in Barn"** with **"Straw (Chopped)"** bedding (keep the default application rate), and from the **Manure Tab** select **"Liquid/Slurry with natural crust"**.

<br>

### Sows

We will now move on to the **"Sow"** animal group in **Step 1** by highlighting it. 

1. Click the management period named **"Open sows"** in **Step 2** to activate that management period.

2. Ensure **"January 1, 2024"** is set as the 'Start date' and **"January 6, 2024"** is set as the 'End date' (5 days). The default start and end dates that are provided in Holos can be changed by the user and the ‘Number of days’ will adjust accordingly. 

3. Enter data related to the number of animals, diet, manure system, and housing type in **Step 3**. Under the **General** Tab:
    * Select **"Open (not lactating or pregnant)"** as the production stage.
    * Enter **"83"** as the number of animals.
    * Enter **"9"** for the litter size, **"1.4"** for the weight of piglets at birth and **"6"** for the weight of weaned piglets.
    * For all other fields keep the default values.

4. Under the **Diet Tab** select **"Gestation"** from the diet drop down menu. From the **Housing Tab** select **"Housed in Barn"** and set the Bedding type as **"Straw (Chopped)"** (keep the default application rate). From the **Manure Tab** select **"Liquid/Slurry with natural crust"**.

5. For each of the three management periods in **Step 2** named **"Bred Sows (Stages 1-3)"**, adjust the 'Start' and 'End' dates so that each management period lasts **38** days, i.e., Stage #1: January 7, 2024 to February 14, 2024; Stage #2: February 15, 2024 to March 24, 2024; Stage #3: March 25, 2024 to May 2, 2024.

6. Under the **General** tab for **all three** 'Bred Sows' management periods:
    * Select **"Gestating"** as the production stage.
    * Enter **"83"** as the number of animals.
    * Enter **"9"** for the litter size, **"1.4"** for the weight of piglets at birth and **"6"** for the weight of weaned piglets
    * For all other fields keep the default values.  
    
3. Under the **Diet Tab** select **"Gestation"** from the diet drop down menu. From the **Housing Tab** select **"Housed in Barn"** with **"Straw (Chopped)"** bedding (keep the default application rate), and from the **Manure Tab** select **"Liquid/Slurry with natural crust"**.

7. Moving to the **"Farrowing lactating sows"** management period in **Step 2**, set the 'Start date' to **"May 3, 2024"** and the 'End date' to **"June 24, 2024"** (21 days).

8. Under the **General** tab, select **"Lactating"** as the production stage and enter **"83"** as the number of animals. For all other fields keep the default values.
    
9. Under the **Diet Tab** select **"Lactation"** from the diet drop down menu. From the **Housing Tab** select **"Housed in Barn"** with **"Straw (Chopped)"** bedding (keep the default application rate), and from the **Manure Tab** select **"Liquid/Slurry with natural crust"**.

<br>

### Boars

Moving on to the **"Boars"** animal group, ensure this group is highlighted in **Step 1**. In **Step 2** we will have a single management period with a 'Start date' of **"January 1, 2024"** and an 'End date' of  **"December 31, 2024"** (365 days).

1. Under the **General Tab:**
    * Select **"Breeding stock"** as the production stage.
    * Enter **"2"** as the number of animals.
    * For all other fields keep the default values.
      
2. For the Boar animal group, we will create a custom diet.
    
#### **Creating a New Diet**

As an alternative to the default set of standard animal diets that are found in Holos, users can create custom diets. *(Note: Holos incorporates feed ingredient information from D. Beaulieu, University of Saskatchewan, for swine diets.* 

We will create a custom diet for our group of breeding boars that are housed in the barn. Click the **Diet** tab followed by the **"Custom Diet Creator"** button.

On the **Custom Diet Creator** screen:
    * Click the **"Add Custom Diet"** button under **Step 1** to begin to create our new custom diet. 
    * Rename the diet **"Custom Boar Diet"** then press the Enter to save.
    * Add ingredients to the new diet in **Step 2**. Select **"Add Selected Ingredients to Diet"**. Search for **"Wheat, Hard Red"** from the ingredient list, highlight the choice, and then click the **OK** button below to have it appear in **Step 3**.
    * Enter the **Percentage in diet (%DM)** value for this ingredient; here we will input **38%**.
    * Continue to add more ingredients to the diet. Select **"Wheat Shorts"** from the “**Add Selected Ingredient to Diet**” list, highlight, and click **OK**. Enter **2%** in **Step 3** for **Percentage in diet (%DM)**.
    * Add the remaining ingredients to the diet as follows: "Corn distiller's dried grains" (10.6%); "Soybean meal, fermented" (8.15%), "Canola, Full fat" (1%), "Field peas" (5.0%), "Canola Meal, Expelled" (15%), and "Barley" (20%). 
    * If any ingredient is not found in the **Full Text Search**, the user can enter the ingredient information independently using the **"Create Custom Ingredient"** button. In this example, we will add the combination of lysine/vitamins (0.25%) as a custom ingredient. 
    * Holos will now report the diet as being complete when **all ingredients total 100%**.
    * Under **Step 1** input **"3"** as the value for 'Feed intake'. 
    * Click the **OK** button to save the new custom diet.
    * Now from the Diet tab, **"Custom Boar Diet"** can be selected from the drop down-down menu as the new diet for this animal group.

3. From the **Housing Tab** select **"Housed in Barn"** with **"Straw (Chopped)"** bedding (keep the default application rate), and from the **Manure Tab** select **"Liquid/Slurry with natural crust"**. 
 <br>
 
<p align="center">
    <img src="../../Images/SwineGuide/en/Figure 15.png" alt="Figure 15" width="850"/>
    <br>
    <em>Figure 15: The Custom diet creator for the Boar animal group.</em>
</p> 

<br>

*Note: For the other Swine components, the animal groups and management periods are added/defined in the same way as for the Farrow-to-Wean component.*

## Grower-to-Finish 

1. Click on the **Hogs** animal group in **Step 1** to enter the associated management information for this group. As we will have just three management periods for this animal group, we can go ahead and remove the fourth Management period by clicking the **X**.
 
2. Under **Step 2**, select the first management period and set the 'Start date' to **"January 01, 2024"** and the 'End date' to **"April 30, 2024"** (121 days). Name this period **"Spring Hogs"**. 

3. Highlight the 'Spring Hogs' management period, and on the **General** tab leave 'Growing/finishing' as the production stage and enter **1000** as the number of animals.

5. On the **Diet** tab, select **"Grower/Finisher diet 2"** as the diet type. On the **Housing** tab, select **"Housed in barn"** as the housing type and **"Straw (Chopped)"** as the bedding type (keep the default application rate) and on the **Manure** tab, select **"Liquid/Slurry with natural crust"** as the manure handling system.
<br>  

6. For the second management period, the hogs will be placed on pasture - rename this management period **"Summer Pasture Hogs"** and set the 'Start date' to **"May 01, 2024"** and the 'End date' to **"October 31, 2024"** (184 days). 

7. Highlight the 'Summer Pasture Hogs' management period and on the **General"** tab, leave the production stage as **"Growing/finishing"** and enter **1000** as the number of animals.

8. For the diet for these hogs on pasture, we will create a new diet. Select the **"Custom Diet Creator"** button. In **Step 1** on this screen, select **"Add Custom Diet"** and name the new diet **"Hog Supplement"**. 

<br>

> *It is important to note that pigs cannot be raised year-round on pasture alone and will require additional feed for proper health, growth, and development.*

<br>

3. In **Step 2**  and **Step 3**, add the diet ingredients as follows: 'Barley' (99.5%) and 'Lysine/vitamins' (0.5%). Lysine/vitamins will now appear in the text search as it was previously added for the 'Custom Boar Diet'.

4. Under **Step 1** enter **"3"** as the 'Feed intake' for the 'Hog Supplement' diet. Click **OK** to save.

5. On the **Diet tab** select 'Hog Supplement' from the drop down menu.

6. On the **Housing** tab, select **Pasture/range/paddock** as the housing type and select **Native Grassland** as the pasture location. 
 
7. On the **Manure** tab, select **Pasture** as the manure handling system.

8. Finally, set the 'Start date' and 'End date' for the third management period to **"November 01, 2024"** to and **"December 31, 2024"** (61 days) and rename this period **"Winter Hogs"**.

10. Highlight the 'Winter Hogs' management period, and on the **General** tab leave **"Growing/finishing"** as the production stage and enter **1000** as the number of animals.\

11. On the **Diet** tab, select 'Grower/Finisher diet #4' as the diet type. On the **Housing** tab, select **"Housed in barn"** as the housing type and leave **"Straw (Chopped)"** as the bedding type (keep the default application rate, and on the **Manure** tab, select **"Liquid/Slurry with natural crust"** as the manure handling system.

<br>

<p align="center">
    <img src="../../Images/SwineGuide/en/Figure 17.png" alt="Figure 17" width="950"/>
    <br>
    <em>Figure 17: Grower-to-finish, Hogs group</em>
</p> 

<br> 


### Adding a Manure Application to the Wheat Field

In Holos, the user can apply livestock manure to a field using either manure produced by the livestock on the farm or using manure imported from off-farm. Since we have now defined our animal components, we can apply manure to any field on our farm.

1. Select the **Wheat & Hairy Vetch** field from the list of components added to our farm.

2. Click on the **Manure tab** and then click the **Add Manure Application** button. 
    * Select **Livestock** as the **Origin of manure**.
    * Select **Swine** as the **Manure type**. 
    * Select **Liquid/Slurry with natural crust** as the **Manure handling system**.
    * Select **Slurry broadcasting** as the **Application method**.
    * Enter **200 kg/ha** as the amount of manure applied to this field.

3. **Both** chemical fertilizers and manure applications can be made on the same field.

<br>

<p align="center">
    <img src="../../Images/SwineGuide/en/Figure 18.gif" alt="Figure 18" width="950"/>
    <br>
    <em>Figure 18: Adding a manure application to the Wheat & Hairy Vetch component</em>
</p>

<br>

# Timeline Screen

We are now finishing the process of defining our farm. Click the **Next** button to go forward to the timeline screen.
The timeline screen provides a visual layout of all the fields from 1985 to the specified end year for each field. This screen also allows the user to add historical and projected production systems. 

The **Add Historical Production System** button enables the user to add a different cropping history to individual fields whereas the **Add Projected Production System** button enables the user to add a future (projected) cropping system to individual fields.


### Adding a historical production system

We will assume that the **Wheat & Hairy Vetch** field was used to grow a **Barley** grain - **Tame mixed (grass/legume)** hay rotation between between **1985 and 2000.**

1. To add a new historical cropping system, select the **Wheat & Hairy Vetch** field. To select an item, click on the timeline bar to activate that field.

2. Click on the **Add Historical Production System** button which will add a new row to the table under the **"Step 1"** section in the upper left of the screen. Note that this new entry has the words **"Historical management practice"** added.

3. We will set the end year of this historical management practice to the year **2000**. To adjust this we use the numeric up/down buttons within the cell.

4. Select the newly added **'Historical management practice'** and then click the **"Edit Selected"** button. This will open a new screen that allows us to adjust the crops grown and their management during this period.

5. As we want to make this a three-crop rotation, under **"Step 2"** click the **"Add Crop"** button twice to add two more crops.

6. Change the first crop type to **"Barley"** (with no cover crop). Enter a yield of **"3,500 kg ha<sub>-1</sub>"** (wet weight) and keep all other settings as default.

7. Change the second and third rows to **"Tame mixed (grass/legume)"** - note that when a perennial crop is selected as the main crop, Holos automatically selects the same crop type as the cover crop. Keep all other settings as default for both years.

8. Click "OK" to save these adjustments.

<br>

<p align="center">
    <img src="../../Images/PoultryGuide/en/Figure14_Revised.png" alt="Figure 14" width="950"/>
    <br>
    <em>Figure 14: Customized Timeline screen</em>
</p> 

<br>  

<p align="center">
    <img src="../../Images/PoultryGuide/en/Figure15_Revised.png" alt="Figure 15" width="550"/>
    <br>
    <em>Figure 15: Adjusted start and end year for productions systems on the timeline screen.</em>
</p> 

<br> 

<p align="center">
    <img src="../../Images/PoultryGuide/en/Figure16_Revised.png" alt="Figure 16" width="650"/>
    <br>
    <em>Figure 16: Editing crops in a historical period of the rotation. </em>
</p> 

<div style="page-break-after: always"></div>

# Details Screen

Click the **"Next"** button to go forward to the details screen.

To avoid the requirement that a user needs to provide annual crop yields going back to 1985 (or the specified start year, if different) for each field on the farm, the model will use default year- and crop-specific yield data from *Statistics Canada* (where available). Changes in crop yield affect various model outputs, including soil C sequestration rates and soil N<sub>2</sub>O emissions. The following steps demonstrate how adjusting the crop yield affects the above- and below-ground C inputs to the soil.

We will adjust this grid so that we can view the above-ground and below-ground C inputs for the **Wheat & Hairy Vetch** and then we will adjust the wheat crop yield for one specific year.

1. We will set a filter on the first column named **'Field name'** so that we only display information for our **Wheat and hairy vetch field**. Beside the column heading, click the **'funnel'** icon to set a filter. Check the box beside **Wheat & hairy vetch**.

2. On the far left of this screen, click the **"Enable Columns"** sidebar (located near the “Field name” column).

3. Place a check beside **Above-ground carbon input** to show the column and remove the check beside the **Notes** column to hide it.

4. Click the **Enable Columns** sidebar again to collapse it.

5. We can now (optionally) adjust the yields for our wheat field for any given year if actual measured yields are available.

6. Adjust the yield for **2006** to be **4,100** kg/ha.

7. Note that Holos has updated the above-ground C inputs for this.

<br>

<p align="center">
    <img src="../../Images/PoultryGuide/en/Figure17_Revised.png" alt="Figure 17" width="950"/>
    <br>
    <em>Figure 17: Details screen</em>
</p> 

<br>

# Results Screen

Click the **"Next"** button to move to the final results report. Results will now be displayed in a variety of reports and charts. 

1. Click on the tab named **"Detailed Emission Report"**.

    The **Detailed Emission Report** displays a monthly or annual GHG emission report. The detailed emission report will report on enteric methane (CH<sub>4</sub>), manure CH<sub>4</sub>, direct and indirect N<sub>2</sub>O, and carbon dioxide (CO<sub>2</sub>) emissions from the farm.

2. Click the **"Yes"** button beside **'Show details'**.

    We can see that the biggest source of emissions from our farm is the crop components. If you hover your mouse pointer over any slice of this chart you can get an isolated look at the different emission sources.

3. Click on the tab named **"Detailed Emission Report"**.

    The **Detailed Emission Report** will display a monthly or annual GHG emission report. The detailed emission report will report on enteric methane (CH<sub>4</sub>), manure CH<sub>4</sub>, direct and indirect N<sub>2</sub>O, and carbon dioxide (CO<sub>2</sub>) emissions from the farm.

2. Click the **Report Format (Monthly)** button to switch to a monthly report. Now we can see a monthly breakdown of all emissions from the farm and the emission source.

    In the **Unit of measurement** drop-down menu, you can choose to have the results displayed as CO<sub>2</sub> equivalents (CO<sub>2</sub>e) or as unconverted greenhouse gases (GHG), and you can also choose the unit of measurement as either tonnes/megagrammes (Mg) or kilograms (kg).
<br>

<p align="center">
    <img src="../../Images/PoultryGuide/en/Figure18_Revised.png" alt="Figure 18" width="950"/>
    <br>
    <em>Figure 18: Detailed emissions report.</em>
</p> 

<br>

3. Click on the **Estimates of Production** report which provides total harvest yields for the farm's fields and of the amount of N available in manure produced by the farm's livestock.

<br>

<p align="center">
    <img src="../../Images/PoultryGuide/en/Figure19_Revised.png" alt="Figure 19" width="950"/>
    <br>
    <em>Figure 19: Estimates of production report.</em>
</p> 

<br>

## Soil carbon modeling results

On the results screen we can see the change in soil carbon over time by clicking the “Multiyear Carbon Modelling” tab. This tab displays a graph showing the change in soil carbon over time for each one of our fields.

For each field on the graph, you can hover your mouse over the series to get more information for each year of the simulation.

If we click on one of these points, we can then view a more detailed breakdown of these results via the “Grid” report format. We can also export this data by clicking the **"Export to Excel"** button.

If you would like to export your entire farm file, from **File** on the main taskbar select 'Export'. Click the arrow to highlight your farm and save it as a .json file.

<br>

<p align="center">
    <img src="../../Images/SwineGuide/en/Figure 27.png" alt="Figure 27" width="950"/>
    <br>
    <br>
    <em>Figure 27: Carbon report section. Allows switching between graph shown here and table format shown below.</em>
  </p>
  
<br>
    
<p align="center">
    <img src="../../Images/SwineGuide/en/Figure 28.png" alt="Figure 28" width="950"/>
    <br>
    <em>Figure 28: Carbon report section. The table format.</em>
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
