<p align="center">
 <img src="../../Images/logo.png" alt="Holos Logo" width="650"/>
    <br>
</p>
The purpose of this document is to provide an introduction on how to use the Holos model (version 4) and the required vs. the optional inputs. We are going to create a farm that has a dairy production system, and a feed crop production system. The farm is located in Manitoba near Portage La Prairie.

<br>

# Launch Holos

Please note that Holos 4 can be installed on a Microsoft Windows PC only. Mac OS will be supported in the next version.

Launch Holos by double-clicking on the Holos desktop icon. If there are no saved farms in the system, Holos will create a new farm and ask the user for a farm name and an optional comment (Figure 1). If there is already a saved farm in the system, Holos will ask the user to open the existing farm or to create a new farm (Figure 2).

Enter "**Holos 2023**" as a farm name and "**training version**" as the Comment.  Click **OK** to proceed to the next screen.

Ensure “**Metric**” is selected as the unit of measurement type and then click the **Next** button at the bottom of the screen (Figure 3). 

<br>
<p align="center">
    <img src="../../Images/DairyGuide/Figure1.png" alt="Figure 1" width="650"/>
    <br>
    <em>Figure 1: Entering a name for the new farm.</em>
</p>
<br>
<p align="center">
    <img src="../../Images/DairyGuide/Figure2.png" alt="Figure 2" width="650"/>
    <br>
    <em>Figure 2: If a farm has been previously saved, Holos will prompt to re-open that farm.</em>
</p>
<br>
<p align="center">
    <img src="../../Images/DairyGuide/Figure3.png" alt="Figure 3" width="650"/>
    <br>
    <em>Figure 3: Select metric as the unit of measurement.</em>
</p>
<br>

# Creating and locating the new dairy farm

The dairy farm that we will create for this exercise is located in the province of Manitoba. Select “**Manitoba**” on the "**Select a province**" screen, and then click the **Next** button.

<br>
<p align="center">
    <img src="../../Images/DairyGuide/Figure4.png" alt="Figure 4" width="650"/>
    <br>
    <em>Figure 4: Select Manitoba as the province.</em>
</p>
<br>

Holos uses **Soil Landscapes of Canada** (SLC), which are a series of GIS coverages that show the major characteristics of soils and land for all of Canada (compiled at a scale of 1:1 million). SLC polygons may contains one or more distinct soil landscape components.

The **“Farm Location”** screen brings up a map of Canada with the province of Manitoba centered on the screen (Figure 5). 

The map contains red colored polygons that can be selected by moving the cursor over the region that contains the location of your farm. You can zoom in or out of the map by using the mouse wheel or by hovering the cursor over the zoom icon at the bottom of the screen.

The beef farm for this example is located between Winnipeg and Portage la Prairie (Portage) with SLC polygon number **851003**. 

Find and right-click on this polygon to select it on the map. Note that at this point daily climate data will be downloaded from [NASA](https://power.larc.nasa.gov/data-access-viewer/). 

<br>

> *Note: Climate data is central to most calculations performed by Holos. For the most accurate estimation of farm emissions, measured climate data should be provided by the user which will override the default data obtained from the NASA weather API.*

> *Holos will use daily precipitation, temperature, and potential evapotranspiration values to model soil carbon change (climate parameter), nitrous oxide emissions, as well as ammonia volatilization.*

<br>
<p align="center">
    <img src="../../Images/DairyGuide/Figure5.png" alt="Figure 5" width="950"/>
    <br>
    <em>Figure 5: Map of the Manitoba province showing the different selectable SCL polygons.</em>
</p>
<br>
<p align="center">
    <img src="../../Images/DairyGuide/Figure6.gif" alt="Figure 6" width="950"/>
    <br>
    <em>Figure 6: Entering the polygon ID for the farm location.</em>
</p>  
<br>
<p align="center">
    <img src="../../Images/DairyGuide/Figure7.png" alt="Figure 7" width="950"/>
    <br>
    <em>Figure 7: Multiple soil types will be displayed for the selected regions.</em>
</p> 
<br>

Once the farm location is selected, soil information (texture, sand, and clay proportions) for the types of soils found in this region are displayed on the right side of the screen. It’s possible that more than one soil type per region will be found and the user is expected to select their soil type from this list or use the default selection. (Figure 7)

For this tutorial, keep the default first selected soil type, and keep the default "**Hardiness zone**".  

> *Note: Soil data obtained from the user’s selected location will be used in the calculation of location-specific N2O emission factors. Properties such as soil texture, top layer thickness, and soil pH are required for these calculations, and can be overwritten.*

<br>

Click the **Next** button to proceed to the next step.

<div style="page-break-after: always"></div>

# Selecting Farm Components

Now that the farm location has been selected, we can move on to the **Component Selection** screen. This is where the user can select different components for their farm. Holos will display all available components on the left side of the screen under the **All Available Components** column (Figure 9). These components are grouped into various categories including Land Management, Beef Production and Dairy Cattle.

If we click on the drop-down button next to a categories' name, we can then see the available components in that category.  For this portion of the  training section, we will be are working with the “Land management” and “Dairy Cattle” categories. 

<br>
<p align="center">
    <img src="../../Images/DairyGuide/Figure8.png" alt="Figure 8" width="950"/>
    <br>
    <em>Figure 8: The available components screen. Specific components can be chosen here to include in the farm. </em>
</p> 
<br>

The Holos model is designed to define the land management before livestock. This is because we are allowing livestock to be placed onto a specific Pasture (field) for grazing, and that is easier done when a pasture field has been defined already (otherwise the user would have to interrupt the livestock setup to setup a field). 

## Crop and Hay Production

Now we can add our first component to the farm. Drag a **Field** component from the left side of the screen and drop it on the **My Farm** on the right side (Figure 9). The screen will now update to reflect this new component that you have added to your farm. Holos will  label the field as “**Field #1**”. At this point, we can now enter production information related to the crop being grown on this field.  
<br>
<p align="center">
    <img src="../../Images/DairyGuide/Figure9.gif" alt="Figure 9" width="950"/>
    <br>
    <em>Figure 9: Specific components can be chosen here to include in the farm.</em>
</p> 
<br>

### Wheat with Cover Crop

Our first field on the farm will grow continuous wheat with a cover crop of hairy vetch. Change the following elements in the "**Field #1**" component.

1. Rename the field to “**Wheat & Hairy Vetch**” in the **Step 1** section of the screen. Change the area of the field to **18 ha**.

2. Select "**Wheat**" as the main crop and "**Hairy Vetch**" as the cover crop in **Step 2**.

3. Under the **General** tab:
    * Enter a yield of **"3000 kg/ha"** (wet weight). The dry weight value will automatically be calculated based on the moisture content of crop value.
    * Select "**Reduced Tillage**" as the tillage type.
    * Enter "**200**" as the amount of irrigation.
    * Select "**0**" as the pesticide passes.
    * Leave "Harvest method" as the default selection.

<br>
<p align="center">
    <img src="../../Images/DairyGuide/Figure10.png" alt="Figure 10" width="950"/>
    <br>
    <em>Figure 10: Field Component of the farm.</em>
</p> 
<br>

4. Select the **Fertilizer** tab and click the “**Add Fertilizer Application**” button. Holos has now added a new fertilizer application for this field and will suggest Urea as the fertilizer blend. A default application rate is calculated based on the yield value entered for this field. Details of this fertilizer application can be changed by clicking the “**Show Additional Information**” button (e.g., season of application, different fertilizer blend, etc.).

<br>
<p align="center">
    <img src="../../Images/DairyGuide/Figure11.gif" alt="Figure 11" width="950"/>
    <br>
    <em>Figure 11: Adding fertilizer to a field.</em>
</p> 
<br>


> *Note: At a minimum, Holos requires the area of the field, type of crop grown, and a field-specific fertilizer application rate to calculate direct and indirect nitrous oxide emissions.*

> *Residue management of each crop (and cover crop) can be adjusted in Holos (see ‘Residue’ tab). Holos provides default values depending on the type of crop being grown and will set a value for percentage of product returned to soil, percentage of straw returned to soil, etc. These residue input settings will have an impact on the final soil carbon change estimates.*

> *Furthermore, biomass fractions and N concentrations can be overwritten by the user, and in this way ‘custom’ crops can be added that are currently not available.*

<div style="page-break-after: always"></div>


## Native Grasslands Information

1. Drag a new **Field** tab component to your list of components. Enter the name “**Native Grassland**” in the "**Field name**" input box.
2. Enter "**100**" as the total area of the field.
3. Select "**Rangeland (Native)**" from the drop -down crop list in the **Crop** column under **Step 2**. Please note that Holos auto populates the "**Winter/Cover/Undersown Crop**" area when a perennial crop is selected.
4. Keep "**0**" as the amount of irrigation and pesticide passes.
5. No fertilizer is used for this crop.

<br>
<p align="center">
    <img src="../../Images/DairyGuide/Figure12.png" alt="Figure 12" width="950"/>
    <br>
    <em>Figure 12: Native Grasslands information.</em>
</p> 
<br>

## Barley Grain and Mixed Hay Rotation

To demonstrate the crop rotation component (as opposed to using individual field components), we will assume that barley grain and mixed hay are grown in rotation, with the mixed hay under seeded to the barley so that it can be harvested in both main years (example derived from University of Alberta’s Breton plots). 

When using the **Crop Rotation** component, any sequence of crops that are input into this components will be applied to each individual field that is part of the rotation setup. This means one field is added for each rotation phase, and the rotation shifts so that each rotation phase is present on one field. Since each field can have a different historical management, soil carbon algorithms will run for each field.

For this example, we assume that the farm requires **70 ha** of barley grain and mixed hay, which are grown in rotation. We will need to setup three fields where barley grain is rotated in each field every two years. When using the crop rotation component, the crop management input of a specific crop is repeated on each field in the rotation where the crop is grown. 

**To setup the rotation:** 

1. Add one **Crop Rotation** component from the available components.
2. To expand the horizontal space available in Holos, click on "**View**" from the top menu bar and select "**Hide List of Available Components**".
3. The rotation of this field begins in "**1985**" and ends in "**2023**". Under **Step 1**, please ensure that these two values are set as the start and end year respectively.
4. Enter **"70 ha"** as the total area of this field.
5. Under **Step 2** change the crop to "**Barley**". The year for this crop should be "**2023**".
    * Under the **General** Tab enter **"3000 kg/ha"** (wet weight) as the yield for this crop.
    * Change the tillage type to **Reduced Tillage**.
    * Keep "**0**" as the amount of irriga
    tion and number of pesticide passes.
    
6. Now add another crop to this rotation. Click on **Add Crop** under **Step 2** to add a second crop to the rotation. 

> *Note: Holos sets the year for this new crop to 2022 or one before the previous crop's year. This means that Holos is expecting the user to enter crops that have been grown in reverse order back to 1985.*

> *It is not necessary to enter a crop for each individual year going back to 1985, only enough crops to describe a single phase of the rotation will need to be entered by the user. Holos will then copy the phase information and back-populate the field history (e.g., Holos will copy the rotation back to 1985 on behalf of the user).*

<br>

7. For this newly added crop select "**Tame Mixed(grass/legume)**" as the crop type.

8. Click on the **Add crop** button one more time. For this third crop, select "**Tame Mixed(grass/legume)**" once again as the crop type.

9. Now add harvest data to each of the tame mixed crops. You will need to select each "**tame mixed crop**" and add the harvest data to that specific crop. So select the first tame mixed crop **(2022)** and then:
    * Under the **Harvest Tab** and click the "**Add Harvest Date**" button to create a new harvest.
    * Select a Harvest date of "**August 31, 2022**", assuming the harvest is done on the same day every year.
    * Select "**Mid**"" for Forage growth stage.
    * Enter "**5**" as the total number of bales.
    * Enter "**500**" as the Wet bale weight.
    
10. **Repeat** the above steps (step 9) **for the second** tame mixed crop.

If the tame mixed field is harvested more than once, the **Add Harvest Date** button can be used to add subsequent harvests.

<br>
<p align="center">
    <img src="../../Images/DairyGuide/Figure13.png" alt="Figure 13" width="950"/>
    <br>
    <em>Figure 13: An example of a crop rotation of three crops.</em>
</p> 
<br>

<div style="page-break-after: always"></div>

# Dairy Operation

Click the view menu item again and uncheck **Hide List of Available Components** option so that we can see all of the available components again.

Adding animal components follows the exact same approach that was used for land management components. Under the "**Dairy cattle**" category, drag and drop one dairy component to the **My Farm** area. If we were not going to use each animal group listed for dairy, we could remove a group by clicking the "**X**" icon right next to its entry under **Step 1**.

Adding animal components follows the exact same approach that was used for land management components. Under the "**Dairy cattle**" category, drag and drop one dairy component to the **My Farm** area.

<br>
<p align="center">
    <img src="../../Images/DairyGuide/Figure14.gif" alt="Figure 14" width="950"/>
    <br>
    <em>Figure 14: The Dairy Cattle Component.</em>
</p> 
<br>

## **Lactating cows, calves, dairy heifers, and dry cow information**

**To set up for Lactating Cows:**

Holos provides the user the option to have more than one lactating cows group. We assumed that the calving interval is 14 months with a 12 - month lactation period.

1. Under the animal groups section in **Step 1**, make sure that the "**Dairy lactating**" row is selected in order to enter the associated management information for that group of animals.
2. Click the management period named "**Early lactation**" in **Step 2** to activate that management period. Ensure "**January 1, 2022**" is set as the 'Start date' and that "**May 31, 2022**" is set as the 'End date' (150 days). Note that the "Number of days" being shown will be inclusive of the start and end dates.
4. Next, we can enter data related to the number of animals, housing type, manure system, and diet for our group of lactating cows. Click on the **General** tab and enter "**65**" for "Number of animals".
5. We are going to create a custom diet for our group of lactating cows. Click on the **Diet** tab. Note that Holos provides a default set of animal diets that can be used. Since we are going to create our own custom diet, we will click on the "**Custom Diet Creator**" button.

> *Note: The number of animals, average daily gain, and feed quality are the minimum required inputs for calculating methane and nitrous oxide emissions. Length of management periods (i.e., duration of grazing) will also be needed. Housing and manure management information are also important inputs but are relatively more impactful on the emissions of monogastrics.*
 
 
6. Click the "**Add Custom Diet**" button in the **Step 1** section of the screen to create a new custom diet. Rename this diet to "**Custom Dairy Diet**" then press the Enter key to save the name.
7. To add ingredients to our new diet, move to the **Step 2** section and select “**ALFALFA**” from the ingredient list, and then click the "**Add Selected Ingredient to Diet”** button.
8. We will add one more ingredient to our diet. Select "**Barley grain, rolled**" from the ingredient list, and then click the “Add Selected Ingredient to Diet” button once again. 
9. Enter "**50%**" for ALFALFA and "**50%**" for Barley grain, rolled in **Step 3**. Click the “OK” button to save the new custom diet.
> *Note: Holos now reports the diet being complete since all ingredients total up to 100%.*

10.  Select the “Custom Dairy Diet” from the drop down-down menu on the "**Diet**" tab.

<br>

<br>
<p align="center">
    <img src="../../Images/DairyGuide/Figure15.png" alt="Figure 15" width="950"/>
    <br>
    <em>Figure 15: Custom diet creator for Dairy Lactating animal group.</em>
</p> 
<br>

> *Note: Diet quality information such as crude protein, total digestible nutrient, and fat are required inputs so that Holos can estimate enteric methane emissions from an animal group.*

11. Moving onto **Step 3**, click on the **"Housing"** tab and select “Tie stall (solid litter)”.
12. Click on the "**Manure**" tab and select “Solid storage (stockpiled)” from the list.

**To set up for Calves:**

Calves that are not used for replacement will enter into the veal system and are fed mainly milk replacer and corn grain. Factoring in fertility loss of 7.7%, 60 calves are produced with a sex ratio of 50:50 male:female. Out of the 30 female calves produced, the farm selected 15 as a replacement heifer (young heifer) for lactating dairy cows. With assumed death losses of 4.4% at four months of age, 43 veal calves
will be fed on a corn grain based diet. Veal calves are slaughtered at the age of 6 months (~270 kg body weight).

1. Under the animal groups section in **Step 1**, make sure that the "**Dairy calves**" row is selected in order to enter the associated management information for this group. 
2. Under **Step 2**, the first management period will be from "**January 1, 2021**" to “**March 31, 2021**".
3. Under **Step 3**, click on the **General** tab and enter "**45**" for "Number of animals". Click on the **Manure** tab and select "Solid storage (stockpiled)” from the list.
4. Returning to **Step 2**, click on "**Add Management Period**" button. This will add a second management period for our group of calves. Ensure the end date of "Management period #2" is "**June 30, 2021**". Since we assumed a death loss of 4.4% at four months of age, we will adjust the number of animals in the second management period to "**43**".

> *Note: Diet quality information such as crude protein, total digestible nutrient, and fat are required inputs so that Holos can estimate enteric methane emissions from an animal group.*

**To set up for Dairy Heifers:**

Out of the 30 female calves produced, the farm selected 15 as replacement heifers for lactating dairy cows.

1. Under the animal groups section in **Step 1**, make sure that the "**Dairy heifers**" row is selected in order to enter the associated management information for that group.
2. For “Management period #1”, enter “**January 1, 2023**” as the ‘Start date’ and   “**December 31,2023**” as the ‘End date’.
3. Click on the **General** tab and enter "**15**" for "Number of animals".
Change the ‘End weight’ to 687 kg.
4. On the **Diet** tab, ensure the “High fiber” diet is selected for the heifers.
5. On the **Housing** tab, select “Free stall barn (solid litter)” as the housing type.
6. On the **Manure** tab, select “Solid storage (stockpiled)”.

**To set up for Dairy dry cows:**

Under the animal groups section in **Step 1**, ensure the row "**Dairy lactating**" is selected, and select "**Dry period**" from **Step 2** in order to enter the associated management information for that group.

1. For “Dry period”, enter “**January 1, 2023**” as the ‘Start date’ and “**March 1, 2023**” as the ‘End date’.
2. Click on the **General** tab and enter "**65**" for "Number of animals".
3. On the **Diet** tab, ensure the “Close-up diet” diet is selected for the dry cows.
4. On the **Housing** tab, select “Drylot” as the housing type.
5. On the **Manure** tab, select "Solid storage (stockpiled)”.


## Discovering the Results Tabs

Click the **Next** button to move to the final results, past the **Timeline** and **Details** screens. **Results** will now be displayed in a variety of reports and charts.  
1. Click on the tab named “**Emissions Pie Chart**”. Starting with the “Emissions pie chart” we can see an overall breakdown of the enteric CH4, manure CH4, direct and indirect N2O. We are also able to see a detailed breakdown of the sources of these emissions.

2. Click the “**Yes**” button beside ‘Show details’. Here we can see that the biggest source of emissions from our farm is the cow-calf component. If you hover your mouse pointer over any slice of this chart you can get an isolated look at the different emission sources.

<br>

<br>
<p align="center">
    <img src="../../Images/DairyGuide/Figure16.gif" alt="Figure 16" width="950"/>
    <br>
    <em>Figure 16: Emissions Pie Chart.</em>
</p> 
<br>

 3. Click on the tab named “**Detailed Emission Report**” which will display a monthly or annual GHG emission report. This "Detailed emission report" will report on enteric methane, manure methane, direct & indirect N2O, and CO2 emissions from the farm.
 
 4. Click the “**Report Format**" tab, which allow the switch between 'Monthly' and 'Annual' reports. In the “**Unit of measurements**” drop-down menu, you can choose to have the results displayed as CO2 equivalents (CO2e) or as unconverted greenhouse gas (GHG), and you can also choose the unit of measurement as either tonnes or kilograms.
 

<br>
<p align="center">
    <img src="../../Images/DairyGuide/Figure17.png" alt="Figure 17" width="850"/>
    <br>
    <em>Figure 17: Detailed Emissions Report.</em>
</p> 
<br>

 
 5. The “**Estimate of Production**” report provides total harvest yields, amount of land applied manure, and estimates of milk production for dairy components.
 


<br>
<p align="center">
    <img src="../../Images/DairyGuide/Figure18.png" alt="Figure 18" width="950"/>
    <br>
    <em>Figure 18: Estimates of Production Report.</em>
</p> 
<br>

6. The “**Feed Estimate Report**" provides an estimate of dry matter intake based on energy requirements of the animal and the energy in the feed.

<br>
<p align="center">
    <img src="../../Images/DairyGuide/Figure19.png" alt="Figure 19" width="950"/>
    <br>
    <em>Figure 19: Feed Estimate Report.</em>
</p> 
<br>

## Soil carbon modelling results
On the results screen we can see the change in soil carbon over time by clicking the “**Multiyear Carbon Modelling**” tab. This tab displays a graph showing the change in soil carbon over time for each one of our fields.

For each field on the graph, you can hover your mouse over the series to get more information for each historical year of the field.

If we click on one of these points, we can then view a more detailed breakdown of these results. We can also export this data by clicking the “**Export to Excel**” button.

<br>
<p align="center">
    <img src="../../Images/DairyGuide/Figure20.png" alt="Figure 20" width="950"/>
    <br>
    <em>Figure 20: Multiyear Carbon Modelling.</em>
</p> 
<br>

# Finally...
**Whole-systems approach**

> An ecosystem consists of not only the organisms and the environment they live in but also the interactions within and between. A whole systems approach seeks to describe and understand the entire system as an integrated whole, rather than as individual components. This holistic approach can be very complex and describing the process can be difficult. One method to conceptualize a whole system is with a mathematical model. The whole-systems approach ensures the effects of management changes are
> transferred throughout the entire system to the resulting net farm emissions. In some cases, reducing one GHG will actually increase the emissions of another. The whole-systems approach avoids potentially ill-advised practices based on preoccupation with one individual GHG.


To download Holos, for more information, or to access a recent list of Holos related publications, visit:
https://agriculture.canada.ca/en/agricultural-science-and-innovation/agricultural-research-results/holos-software-program

To contact us, email:
aafc.holos.acc@canada.ca