<p align="center">
 <img src="../../Images/logo.png" alt="Holos Logo" width="650"/>
    <br>
</p>
The purpose of this document is to provide an introduction on how to use the Holos model (version 4) and the required vs. optional inputs.

For the purpose of this training, we are going to create a farm that has an annual swine production system, and a feed crop production system. The farm is located in Manitoba near Portage La Prairie. 

<br>

# Launch Holos

Please note that Holos 4 can be installed on a Microsoft Windows PC only. Mac OS will be supported in the next version.

Launch Holos by double-clicking on the Holos desktop icon. Holos will ask the user to open an existing farm, create a new farm, or import a saved farm file (Figure 1). If there is already a saved farm in the system, the user can click **Open**. If there are no saved farms in the system Holos will ask the user if they want to create a **New** farm or **Import** a saved farm file (i.e., a .json file). If the user creates a new farm, they are asked for the farm name and an optional comment (Figure 2).  

Enter **"Holos 2023"** as the Name and **"Training Version"** as the Comments.  Click **OK** to proceed to the next screen.

Ensure “**Metric**” is selected as the unit of measurement type and then click the **Next** button at the bottom of the screen (Figure 3). 

<br>

<p align="center">
    <img src="../../Images/SwineGuide/en/figure1.png" alt="Figure 1" width="650"/>
    <br>
    <em>Figure 1: If a farm has been previously saved, Holos will prompt to re-open that farm.</em>
</p>

<br>

<p align="center">
    <img src="../../Images/SwineGuide/en/figure2.png" alt="Figure 2" width="750"/>
    <br>
    <em>Figure 2: Entering a name for the new farm. </em>
</p>

<br>

<p align="center">
    <img src="../../Images/SwineGuide/en/figure3.png" alt="Figure 3" width="550"/>
    <br>
    <em>Figure 3: Select metric as the unit of measurement.</em>
</p>

<br>

# Creating and Locating the New Swine Farm

The swine farm that we will create for this exercise is located in the province of Manitoba. Select **"Manitoba"** on the **"Select a province"** screen, and then click the **Next** button.

<br>

<p align="center">
    <img src="../../Images/SwineGuide/en/figure4.png" alt="Figure 4" width="550"/>
    <br>
    <em>Figure 4: Select Manitoba as the province.</em>
</p>

<br>

Holos uses **Soil Landscapes of Canada** (SLC), which are a series of GIS coverages that show the major characteristics of soils and land for all of Canada (compiled at a scale of 1:1 million). SLC polygons may contain one or more distinct soil landscape components.

The **Farm Location** screen brings up a map of Canada with the province of Manitoba centered on the screen (Figure 5). 

The map contains red colored polygons that can be selected by moving the cursor over the region that contains the location of your farm. You can zoom in or out of the map by using the mouse wheel or by hovering the cursor over the zoom icon at the bottom of the screen.

The swine farm for this example is located between Winnipeg and Portage la Prairie (Portage) with SLC polygon number **851003**. 


Find and right-click on this polygon to select it on the map (Figure 6). Note that at this point daily climate data will be downloaded from [NASA](https://power.larc.nasa.gov/data-access-viewer/). 

<br>

> *Note: Climate data is central to most calculations performed by Holos. For the most accurate estimation of farm emissions, measured climate data should be provided by the user which will override the default data obtained from the NASA weather API. If the user chooses to use the default NASA climate data, these data are available in a 10 km grid, and so can vary throughout the SLC polygon, depending on the precise location of the farm. Therefore, if possible, the user should choose the location of their farm as precisely as possible. Doing so can be aided by using different views (e.g., the Aerial view), which can be selected via the eye icon at the bottom of the map on the Farm Location screen.*
> 

> *Holos will use daily precipitation, temperature, and potential evapotranspiration values to model soil carbon change (climate parameter), nitrous oxide emissions, as well as ammonia volatilization.*

<br>

<p align="center">
    <img src="../../Images/SwineGuide/en/figure5.png" alt="Figure 5" width="950"/>
    <br>
    <em>Figure 5: Map of the Manitoba province showing the different selectable polygons.</em>
</p>

<br>

Once the farm location is selected, soil information (texture, sand, and clay proportions) for the types of soils found in this polygon are displayed on the right side of the screen. It is possible that more than one soil type per polygon will be found and the user is expected to select their soil type from this list or use the default selection (Figure 7). The default soil type selected represents the dominant soil type for the chosen polygon.

For this tutorial, keep the default **Soil Zone** as 'Black' soil, and the default "**Hardiness Zone**" as '3b'. 

<br>

<p align="center">
    <img src="../../Images/SwineGuide/en/figure6.png" alt="Figure 6" width="950"/>
    <br>
    <em>Figure 6: Selecting the SLC polygon for the farm location.</em>
</p> 

<br>

<p align="center">
    <img src="../../Images/SwineGuide/en/figure7.png" alt="Figure 7" width="950"/>
    <br>
    <em>Figure 7: Multiple soil types will be displayed for the selected SLC polygon.</em>
</p> 

<br>

> *Note: Soil data obtained from the user’s selected location will be used in the calculation of location-specific N<sub>2</sub>O emission factors. Properties such as soil texture, top layer thickness, and soil pH are required for these calculations, and can be overwritten on the Component Selection screen, under Settings > Farm Defaults > Soil.*

<br>

Click the **Next** button to proceed to the next step.

<div style="page-break-after: always"></div>

# Selecting Farm Components

Now that the farm location has been selected, we can move on to the **Component Selection** screen. This is where the user can select different components for their farm. Holos will display all available components on the left side of the screen under the **All Available Components** column (Figure 8). These components are grouped into various categories including Land Management, Beef Production, Dairy Cattle, and Swine.

If we click on the drop-down button next to a category's name, we can then see the available components in that category.  For this portion of the  training section, we will be working with the “Land management” and “Swine” categories. 

<br>

<p align="center">
    <img src="../../Images/SwineGuide/en/figure8.png" alt="Figure 8" width="950"/>
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
    <img src="../../Images/SwineGuide/en/figure9.gif" alt="Figure 9" width="950"/>
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
    * Leave 'Harvest method' as the default selection.
    

<br>

<p align="center">
    <img src="../../Images/SwineGuide/en/figure10.png" alt="Figure 10" width="950"/>
    <br>
    <em>Figure 10: Field Component of the farm.</em>
</p> 

<br> 

4. Select the **Fertilizer** tab and click the **"Add Fertilizer Application"** button. Holos has now added a new fertilizer application for this field and will suggest Urea as the fertilizer blend. A default application rate is calculated based on the yield value entered for this field. Details of this fertilizer application can be changed by clicking the **Show Additional Information** button (e.g., season of application, different fertilizer blend, etc.).

<br>

<p align="center">
    <img src="../../Images/SwineGuide/en/figure11.gif" alt="Figure 11" width="950"/>
    <br>
    <em>Figure 11: Adding fertilizer to a field.</em>
</p> 

<br>

> *Note: At a minimum, Holos requires the area of the field, type of crop grown, and a field-specific fertilizer application rate to calculate direct and indirect nitrous oxide emissions.*

> *Residue management of each crop (and cover crop) can be adjusted in Holos (see ‘Residue’ tab). Holos provides default values depending on the type of crop being grown and will set a value for percentage of product returned to soil, percentage of straw returned to soil, etc. These residue input settings will have an impact on the final soil carbon change estimates.*

> *Furthermore, biomass fractions and N concentrations can be overwritten by the user, and in this way ‘custom’ crops can be added that are currently not available.*

<div style="page-break-after: always"></div>

### Native Grasslands 

The swine operation (defined later on) relies on native pasture for the summer months (May through October) as a supplement to feed.

1. Drag a new **Field** component to your list of components. Enter the name **"Native Grassland"** in the **Field name** input box.

2. Enter **"100"** ha as the total area of the field.

3. Select **"Rangeland (Native)"** from the drop-down crop list in the **Crop** column under **Step 2**. Please note that Holos auto-populates the **Winter/Cover/Undersown Crop** area when a perennial crop is selected.

4. Keep **"0"** as the amount of irrigation and number of pesticide passes.

5. No fertilizer is used for this crop.

<br>

<p align="center">
    <img src="../../Images/SwineGuide/en/figure12.png" alt="Figure 12" width="950"/>
    <br>
    <em>Figure 12: Native Grasslands information.</em>
</p> 

<br>

### Barley Grain and Mixed Hay Rotation

To demonstrate the crop rotation component (as opposed to using individual field components), we will assume that barley grain and mixed hay are grown in rotation, with the mixed hay under-seeded to the barley so that it can be harvested in both main years (example derived from University of Alberta’s Breton plots). 

When using the **Crop Rotation** component, any sequence of crops that are input into this component will be applied to each individual field that is part of the rotation setup. This means one field is added for each rotation phase, and the rotation shifts so that each rotation phase is present on one field. Since each field can have a different management history, soil carbon algorithms will run for each field.

For this example, we assume that the farm requires **70 ha** of barley grain and mixed hay, which are grown in rotation. We will need to set up three fields where barley grain is rotated in each field every two years (Figure 12). When using the crop rotation component, the crop management input of a specific crop is repeated on each field in the rotation where the crop is grown. 

**To set up the rotation:** 

1. Add one **"Crop Rotation"** component from the available components.

2. To expand the horizontal space available in Holos, click on **View** from the top menu bar and select **"Hide List of Available Components"**.

3. The rotation of this field begins in **"1985"** and ends in **"2023"**. Under **Step 1**, please ensure that these two values are set as the start and end year, respectively.

4. Enter **"70"** ha as the **total area** of this field.

5. Under **Step 2** change the crop to **"Barley"**. The year for this crop should be **"2023"**.
    * Under the **General** Tab enter **"3,000 kg ha<sup>-1</sup>"** (wet weight) as the yield for this crop.
    * Change the tillage type to **"Reduced Tillage"**.
    * Keep **"0"** as the amount of irrigation and number of pesticide passes.

6. Now add another crop to this rotation. Click on **"Add Crop"** under **Step 2** to add a second crop to the rotation. 

<br>

> *Note: Holos sets the year for this new crop to 2022 or one before the previous crop's year. This means that Holos is expecting the user to enter crops that have been grown in reverse order back to 1985.*

> *It is not necessary to enter a crop for each individual year going back to 1985, only enough crops to describe a single phase of the rotation will need to be entered by the user. Holos will then copy the phase information and back-populate the field history (i.e., Holos will copy the rotation back to 1985 on behalf of the user).*

<br>

7. For this newly added crop select **"Tame Mixed (grass/legume)"** as the crop type.

8. Click on the **Add crop** button one more time. For this third crop, select **"Tame Mixed (grass/legume)"** once again as the crop type.

9. Now add harvest data to each of the tame mixed crops. You will need to select each **tame mixed** crop and add the harvest data to that specific crop. So select the first tame mixed crop (2022) and then:
      * Under the **Harvest Tab** click the **"Add Harvest Date"** button to create a new harvest.
    * Select a Harvest date of **"August 31, 2022"**, assuming the harvest is done on the same day every year.
    * Select **"Mid"** for Forage growth stage.
    * Enter **"5"** as the total number of bales.
    * Enter **"500"** as the Wet bale weight.
    
10. **Repeat** Step 9 for the second tame mixed crop.

If the tame mixed field is harvested more than once, the **Add Harvest Date** button can be used to add subsequent harvests.

<br>

<p align="center">
    <img src="../../Images/SwineGuide/en/figure13.png" alt="Figure 13" width="950"/>
    <br>
    <em>Figure 13: An example of a crop rotation of three crops.</em>
</p> 

<div style="page-break-after: always"></div>

# Swine Operation

Click on **View** menu item and uncheck the **Hide List of Available Components** option.

Adding animal components follows the exact same approach that was used for land management components. Under the **Swine** category in the available components, drag and drop one **Grower-to-Finish** component to the **My farm** section on the right. Add a **Farrow-to-Wean**, followed by an **Iso-Wean** component. In this example, all groups listed under each component will be used. This means we will not have to remove any animal group by clicking the **X** icon next to it under **Step 1**. We can now begin entering information into each Animal Group from our three Swine Components.

<br>

<p align="center">
    <img src="../../Images/SwineGuide/en/figure14.gif" alt="Figure 14" width="950"/>
    <br>
    <em>Figure 14: The Swine Farm Components.</em>
</p> 

<br>

> *Note: The number of animals, average daily gain, and feed quality are the minimum required inputs for calculating methane and nitrous oxide emissions. Length of management periods (e.g., duration of grazing) will also be needed. Housing and manure management information are also important inputs but are **relatively more impactful on the emissions of monogastrics.***

<br>

## Farrow-to-Wean

The farrow-to-wean component is divided into **three animal groups**, and each of these animal group is then divided into **a number of corresponding management (production) periods**. We will now enter information into each of these management period. 

### Gilts

1. Under **Step 1**, make sure that the **"Gilts"** row is selected in order to enter the associated management information for this group.

2. Click the management period named **"Open gilts"** in **Step 2** to activate that management period.

3. Ensure **"January 1, 2022"** is set as the 'Start date' and **"January 5, 2022"** is set as the 'End date' (5 days). The default start and end dates that are provided in Holos can be changed by the user and the ‘Number of days’ will adjust accordingly. 

4. Enter data related to the number of animals, diet, manure system, and housing type in **Step 3**. Under the **General** Tab:
    * Select 'Open (not lactating or pregnant)' as the Production stage.
    * Enter **"83"** as the number of animals.
    * Keep the remaining entries at their default values.

5. Under the **Diet Tab** select 'Gestation based diet' from the drop down menu. From the **Housing Tab** select 'Housed in Barn' and leave the Bedding type as 'Straw'. From the **Manure Tab**, select 'Liquid/Slurry with natural crust'.

6.  For each of the three management periods named **"Bred Gilts (Stages 1-3)"** set the 'Start' and 'End' dates to **38** days, respectively (i.e., Stage #1: 'January 06, 2022' to 'February 12, 2022', etc.).

7. Highlight **"Bred Gilts (Stage #1)"** management period and from the **General Tab:**
    * Select 'Gestating' as the Production stage.
    * Enter **"83"** as the number of animals.
    * Keep the remaining entries at their default values.
    
8. Under the **Diet Tab** select 'Gestation based diet' from the drop down menu. From the **Housing Tab** select 'Housed in Barn' with 'Straw' bedding, and from the **Manure Tab** select 'Liquid/Slurry with natural crust'.

9. Repeat the Steps 7 and 8 for **"Bred Gilts (Stage#2) and (Stage #3)"** management periods.

10. Lastly select the **"Farrowing gilts"**, and enter **"April 04, 2022"** as the 'Start date' and **"May 07, 2022"** as the 'End date' (21 days).

11. Under the **General Tab:**
    * Select 'Lactating' as the Production stage.
    * Enter **"83"** as the number of animals.
    * Keep the remaining entries at their default values.
    
12. Under the **Diet Tab** select 'Lactation based diet' from the drop down menu. From the **Housing Tab** select 'Housed in Barn' with 'Straw' bedding, and from the **Manure Tab** select 'Liquid/Slurry with natural crust'.

<br>

### Sows

We will now move on to the **"Sow"** animal group in **Step 1** by highlighting it. 

13. For each of the three management periods in **Step 2** named **"Bred Sows (Stages 1-3)"**, set the 'Start' and 'End' dates to **38** days, respectively (i.e., Stage #1: 'January 31, 2022' to 'March 10, 2022', etc.). 

14. Under the **General Tab** for **all three** 'Bred Sows' management periods:
    * Select 'Gestating' as the Production stage.
    * Enter **"83"** as the number of animals.
    * Keep the remaining entries at their default values.
    * **"Litter size"** can be adjusted accordingly in all Management periods for Sows; here we will leave the default value of **13.5**.  
    
15. Under the **Diet Tab** select 'Gestation based diet' from the drop down menu.     From the **Housing Tab** select 'Housed in Barn' with 'Straw' bedding, and from the **Manure Tab** select 'Liquid/Slurry with natural crust'.

16. Moving to the **"Farrowing lactating sows"** management period in **Step 2**, set the 'Start date' to **"May 28, 2022"** and 'End date' to **"June 09, 2022"**(21 days).

17. Under the **General Tab:** 
    * Select 'Lactating' as the Production stage.
    * Enter **"83"** as the number of animals.
    * Keep the remaining entries at their default values.
    
18. Under the **Diet Tab** select 'Lactation based diet' from the drop down menu. From the **Housing Tab** select 'Housed in Barn' with 'straw' bedding, and from the **Manure Tab** select 'Liquid/Slurry with natural crust'.

19. Lastly, in the **"Open sows"** management period under the **General Tab:**
    * Select 'Open (not lactating or pregnant)' as the Production stage.
    * Enter **"370"** as the number of animals.
    * Keep the remaining entries at their default values.
    
20. Under the **Diet Tab** select 'Gestation based diet' from the drop down menu.     From the **Housing Tab** select 'Housed in Barn' with 'Straw' bedding, and from the **Manure Tab** select 'Liquid/Slurry with natural crust'.

<br>

### Boars

Moving on to the **"Boars"** section, ensure this group is highlighted in **Step 1**. In **Step 2** we will have a single Management period setting **"January 1, 2022"** as the 'Start date' and **"December 31,2022"** as the 'End date'(365 days).

20. Under the **General Tab:**
    * Select 'Breeding stock' as the Production stage.
    * Enter **"2"** as the number of animals.
    * Keep the remaining entries at their default values.
    
#### **Creating a New Diet**

As an alternative to the default set of standard animal diets that are found in Holos users can create custom diets. *(Note: Holos incorporates feed ingredient information from D. Beaulieu, University of Saskatchewan for swine diets (20XX).* 

21. We will create a custom diet for our group of breeding boars that are housed in the barn. Click the **Diet** tab followed by the **"Custom Diet Creator"** button. 

    **Custom Diet Creator Screen:**

    * Click the **"Add Custom Diet"** button in **Step 1** to begin to create our new custom diet. 
    * Rename the diet **"Boar Diet"** then press the Enter to save.
    * Add ingredients to the new diet in **Step 2**. Select **"Add Selected Ingredients to Diet"**. Search for **"Wheat, Hard Red"** from the ingredient list, highlight the choice, and then click the **OK** button below to have it appear in **Step 3** where the diet is being comprised.
    * Enter the **Percentage in diet (%DM)** that this ingredient will hold; here we will input **38%**.
    * Continue to add more ingredients to the diet. Select **"Wheat Shorts"** from the “**Add Selected Ingredient to Diet**” list, highlight, and click **OK**. Enter **2%** in **Step 3** for **Percentage in diet (%DM)**.
    * Enter **"Corn distiller's dried grains (10.6%)"**. Continue to add all the various ingredients to your diet. For our boar diet, add ingredients as follows: "Soybean meal, fermented (7.1%)", "Canola, Full fat (0.5%)", "Field peas (5.0%)", "salt (0.26%)", "phosphorus (1.3%)", "Canola Meal, Expelled (15%)", and "Barley (20%)". 
    * If any ingredient is not found in the **Full Text Search**, the user can enter the ingredient information independently using the **"Create Custom Ingredient"** button. For our example we will add the combination of lysine/vitamins (0.24%) following these steps.  
    * Holos will now report the diet as being complete when **all ingredients total 100%**.
    * Click the **OK** button to save the new custom diet
    * Now from the Diet tab, **"Boar Diet"** can be selected from the drop down-down menu as the new diet for this animal group.
 
 <br>
 
> *Note: Diet quality information such as crude protein, total digestible nutrient, and fat are required inputs so that Holos can estimate enteric methane emissions from an animal group.*

<br>

<p align="center">
    <img src="../../Images/SwineGuide/en/figure15.png" alt="Figure 15" width="850"/>
    <br>
    <em>Figure 15: Custom diet creator for Swine animal group.</em>
</p> 

<br>

22.  From the **Housing Tab** select 'Housed in Barn' with 'Straw' bedding, and from the **Manure Tab** select 'Liquid/Slurry with natural crust'.

<br>

## Iso-Wean

Click on the **Piglets** animal group in **Step 1**. Click on the **"Weaned piglets Stage #1"** management period in **Step 2**. We will leave the default dates 'January 01, 2022' to 'January 19, 2022' (19 days).

23. From the **General Tab:**
       * Enter **"100:** as the number of animals.
       * Keep the remaining entries at their default values.

24. From the **Diet Tab:**
    * Select 'Nursery weaners (starter diet #1)' as the diet type. 
    
25.  From the **Housing Tab** select 'Housed in Barn' with 'Straw' bedding, and from the **Manure Tab** select 'Liquid/Slurry with natural crust'.

26. Information related to diet, housing and manure management is entered in the same manner as for the **"Weaned piglets Stage #2"** management period. We will leave the default dates 'January 20, 2022' to 'February 04, 2022' (16 days). Repeat steps 23 to 25, with the exception of selecting 'Nursery weaners (starter diet #2)'.

<br>

<p align="center">
    <img src="../../Images/SwineGuide/en/figure16.png" alt="Figure 16" width="950"/>
    <br>
    <em>Figure 16: Iso-Wean - Piglet group</em>
</p> 

<br>

## Grower-to-Finish 

Click on the **Hogs** animal group in **Step 1**. In **Step 2** we will create three Management periods for this group. The first is the winter period, setting **"January 01, 2022"** as the 'Start date' and **"April 30, 2022"** as the ‘End date’ (120 days). Name this period **"Spring Hogs"**. We can go ahead and remove the 4th Management period by clicking the **X**.

1. Highlight the 'Spring Hogs' management period, and on the **General Tab:**
      * Leave the default 'Growing/finishing' as the production stage.
      * Enter **1000** as the number of animals.

   **Diet Tab:**
    * Select 'Grower/Finisher diet 2' as the diet type.

    **Housing Tab:**
    * Select 'Housed in barn' as the housing type and leave 'Straw' as the bedding type.
    
    **Manure Tab:**
    * Select 'Liquid/Slurry with natural crust' as the manure handling system.
<br>  

### Hogs - On Pasture

The second management period in **Step 2** will be named **"Summer Pasture Hogs"**. Our warm season grazing will be set from **"May 01, 2022"**  to **"October 31, 2022"** (184 days). 

2. Highlight 'Summer Pasture Hogs', and on the **General Tab:"** 
      * Leave the production stage as 'Growing/finishing' 
      * Enter **1000** as the number of animals.  

<br>

> *It is important to note that pigs cannot be raised year-round on pasture alone and will require additional feed for proper health, growth, and development.*

<br>


3. We will create a new diet as a supplement to grazing on pasture. Select the 'Custom Diet Creator' button. In **Step 1** on the this screen, select 'Add Custom Diet' and name it **"Hog Supplement"**. 

4. In **Step 2** add the ingredients Barley (97.5%), phosphorus (2%) and lysine/vitamins (0.5%). The last two ingredients will now appear in the text search as they were previously added for the 'Boar Diet'. 

5. Adjust the 'Percentage in diet' values in **Step 3**. Click **OK** to save.

6. On the **Diet tab** select 'Hog Diet Supplement' from the drop down menu.

7. On the **Housing Tab:**
    * Select **Pasture/range/paddock** as the housing type.
    * Select **Native Grassland** as the pasture location.
 
8. On the **Manure Tab:**
    * Select **Pasture** as the manure handling system.

Lastly set the fall period as **"November 01, 2022"** to **"December 31,2022"** (61 days) and name this **"Winter Hogs"**.

9. Highlight the 'Winter Hogs' management period, and on the **General Tab:**
      * Leave the default 'Growing/finishing' as the production stage.
      * Enter **1000** as the number of animals.

   **Diet Tab:**
    * Select 'Grower/Finisher diet #4' as the diet type.

    **Housing Tab:**
    * Select 'Housed in barn' as the housing type and leave 'Straw' as the bedding type.
    
    **Manure Tab:**
    * Select 'Liquid/Slurry with natural crust' as the manure handling system.

<br>

<p align="center">
    <img src="../../Images/SwineGuide/en/figure17.png" alt="Figure 17" width="950"/>
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
    <img src="../../Images/SwineGuide/en/figure18.gif" alt="Figure 18" width="950"/>
    <br>
    <em>Figure 18: Adding a manure application to the Wheat & Hairy Vetch component</em>
</p>

<br>

### Adding supplemental hay/forage for grazing animals

We can also add additional hay/forage for animals that are grazing on a particular field. Since we have now placed a group of animals on the “Native Grassland” field component, and we have also provided harvest information for our mixed hay crops on the crop rotation component, we can then add an additional forage supplement for these grazing animals.

1. Select the **Native Grassland** field component we created earlier.

2. Click on the **Grazing tab.**
    - Click the **Add Supplemental Hay** button to add additional forage for the animals on this field.
    - Enter "**On-farm**" as the **Sources of bales**.
    - Choose **Crop rotation #1 [Field #2] - Tame Mixed (grass/legume)** under **Field** to select the source of the supplemental hay.
    - Change the **Number of bales** to **1**
    - Enter **500** as the wet bale weight.
    - Keep the moisture content as the default value.

3. On the field receiving the supplemental forage, there is a chart showing how much forage is still available on the right of the screen for reference.

<br>
  
 > *Note: It is not recommended to mix different species of grasses together. Here, we are only demonstrating the ability of Holos to add supplemental hay to a field that has grazing animals.*

<br>

<p align="center">
    <img src="../../Images/SwineGuide/en/figure19.png" alt="Figure 19" width="950"/>
    <br>
    <em>Figure 19: Adding supplemental hay/forage for grazing animals.</em>
</p>

<br>

# Timeline Screen

We are now finishing the process of defining our farm. Click the **Next** button to go forward to the timeline screen.
The timeline screen provides a visual layout of all the fields from 1985 to the specified end year for each field. This screen also allows the user to add historical and projected production systems. 

The **Add Historical Production System** button enables the user to add a different cropping history to individual fields whereas the **Add Projected Production System** button enables the user to add a future (projected) cropping system to individual fields.


### Adding a historical production system

We will assume that the barley grain and mixed hay rotation fields were previously in a continuous wheat cropping system between **1985 and 2000.**

1. To add a new historical cropping system, select one of the fields that are in the barley grain and mixed hay rotation. To select an item, click on the timeline bar to activate that field. We will select the first field in this rotation (i.e., the field with the name of **"Crop rotation #1 [Field #1] - Barley"**).

2. Click on the **Add Historical Production System** button which will add a new row to the table under the **"Step 1"** section in the upper left section of the screen. Notice that this new entry has the words **"Historical management practice"** added.

3. We will set the end year of this historical management practice to the year **2000**. To adjust this we use the numeric up/down buttons within the cell.

4. Select the newly added **'Historical management practice'** and then click the **"Edit Selected"** button. This will open a new screen that allows us to adjust the crops grown and the management during this period.

5. Click on the **"Barley"** crop under the **"Step 2"** section. Change the crop type to **"Wheat"** and on the **'General'** tab change the yield to **3,500** kg/ha. We will keep the other settings unchanged.

6. We also need to remove the **"Tame mixed"** crops from this historical period. Click the **'X'**’ icon beside each of the **"Tame mixed"** crops under the **"Step 2"** section. Clicking the **'X'** icon will remove these crops from the rotation for this period of time.

7. Click **'OK'** to save adjustments we just made to this field.

8. Repeat these same steps so that the other fields in this rotation also have continuous wheat from **1985 to 2000** using the same steps we used for the first field.

<br>

<p align="center">
    <img src="../../Images/SwineGuide/en/figure20.png" alt="Figure 20" width="950"/>
    <br>
    <em>Figure 20: Customized Timeline Screen</em>
</p> 

<br>  

<p align="center">
    <img src="../../Images/SwineGuide/en/figure21.png" alt="Figure 21" width="550"/>
    <br>
    <em>Figure 21: Adjusted start and end year for production systems on the timeline screen.</em>
</p> 

<br>

<p align="center">
    <img src="../../Images/SwineGuide/en/figure22.png" alt="Figure 22" width="650"/>
    <br>
    <em>Figure 22: Editing crops in a historical period of the rotation. </em>
</p> 

<div style="page-break-after: always"></div>

# Details Screen

Click the **"Next"** button to go forward to the details screen.

To avoid the requirement that a user needs to provide annual crop yields going back to 1985 (or the specified start year, if different) for each field on the farm, the model will use default year- and crop-specific yield data from *Statistics Canada* (where available). Changes in crop yield affect various model outputs, including soil carbon sequestration rates and soil N<sub>2</sub>O emissions. The following steps demonstrate how adjusting the crop yield affects the above- and below-ground carbon inputs to the soil.

We will adjust this grid so that we can view the above-ground and below-ground carbon inputs for our wheat field and then we will adjust the crop yield for one specific year.

1. We will set a filter on the first column named **'Field name'** so that we only display information for our **Wheat and hairy vetch field**. Beside the column heading, click the **'funnel'** icon to set a filter. Check the box beside **Wheat & hairy vetch**.

2. On the far left of this screen, click the **"Enable Columns"** sidebar (located near the “Field name” column).

3. Place a check beside **Above-ground carbon input** to show the column and remove the check beside the **Notes** column to hide it.

4. Click the **Enable Columns** sidebar again to collapse it.

5. We can now (optionally) adjust the yields for our wheat field for any given year if actual measured yields are available.

6. Adjust the yield for **1987** to be **4,100** kg/ha.

7. Note that Holos has updated the above-ground carbon inputs for this.

<br>

<p align="center">
    <img src="../../Images/SwineGuide/en/figure23.png" alt="Figure 23" width="950"/>
    <br>
    <em>Figure 23: Details screen</em>
</p>

<br>

# Discover Results

Click the **"Next"** button to move to the final results report. Results will now be displayed in a variety of reports and charts. 

1. Click on the tab named **"Emissions Pie Chart"**.

    Starting with the **Emissions pie chart** we can see an overall breakdown of the enteric CH<sub>4</sub>, manure CH<sub>4</sub>, direct and indirect N<sub>2</sub>O. We are also able to see a detailed breakdown of the sources of these emissions.

2. Click the **"Yes"** button beside **'Show details'**.

    We can see that the biggest source of emissions from our farm is the crop components. If you hover your mouse pointer over any slice of this chart you can get an isolated look at the different emission sources.

3. Click on the tab named **"Detailed Emission Report"**.

    The **Detailed Emission Report** will display a monthly or annual GHG emission report. The detailed emission report will report on enteric methane, manure methane, direct & indirect N<sub>2</sub>O, and CO<sub>2</sub> emissions from the farm.

Click the **"Report Format (Monthly)"** button to switch to a monthly report. Now we can see a monthly breakdown of all emissions from the farm and the emission source.

In the **'Unit of measurements'** drop-down menu, you can choose to have the results displayed as CO<sub>2</sub> equivalents (CO<sub>2</sub>e) or as unconverted greenhouse gas (GHG), and you can also choose the unit of measurement as either tonnes or kilograms.

The **'Estimates of Production'** report provides total harvest yields, farm land information, and amount of land-applied manure produced from each of the swine animal components.

The **'Feed Estimate'** report provides an estimate of dry matter intake based on energy requirements of the animal and the energy in the feed.

<br>

<p align="center">
    <img src="../../Images/SwineGuide/en/figure24.png" alt="Figure 24" width="950"/>
    <br>
    <em>Figure 24: Detailed emissions report.</em>
</p> 

<br>

<p align="center">
    <img src="../../Images/SwineGuide/en/figure25.png" alt="Figure 25" width="950"/>
    <br>
    <em>Figure 25: Estimates of production report.</em>
</p> 

<br>

<p align="center">
    <img src="../../Images/SwineGuide/en/figure26.png" alt="Figure 26" width="950"/>
    <br>
    <em>Figure 26: Feed estimate report.</em>
</p> 

<br>

## Soil carbon modeling results

On the results screen we can see the change in soil carbon over time by clicking the “Multiyear Carbon Modelling” tab. This tab displays a graph showing the change in soil carbon over time for each one of our fields.

For each field on the graph, you can hover your mouse over the series to get more information for each year of the simulation.

If we click on one of these points, we can then view a more detailed breakdown of these results via the “Grid” report format. We can also export this data by clicking the **"Export to Excel"** button.

If you would like to export your entire farm file, from **File** on the main taskbar select 'Export'. Click the arrow to highlight your farm and save it as a .json file.

<br>

<p align="center">
    <img src="../../Images/SwineGuide/en/figure27.png" alt="Figure 27" width="950"/>
    <br>
    <br>
    <em>Figure 27: Carbon report section. Allows switching between graph shown here and table format shown below.</em>
  </p>
  
<br>
    
<p align="center">
    <img src="../../Images/SwineGuide/en/figure28.png" alt="Figure 28" width="950"/>
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
