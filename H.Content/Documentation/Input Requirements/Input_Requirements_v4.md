
<p align="center">
 <img src="../../Images/logo.png" alt="Holos Logo" width="650"/>
    <br>
</p>

# Input Requirements for the Holos V4 Model (advanced mode)


## Contents
    
1. Location data                                       
    
2. Land management	 

     2.1.	Field/Crop Rotation components	
    
     2.2.	Field-Perennial/Grassland	               
    
     2.3	Shelterbelt	 
    
3.	Beef production	                                   
4.	Dairy cattle	                                   
5.	Swine	                                           
6.	Sheep	                                           
7.	Poultry	                                           
8.	Other livestock	                                   
9.	Infrastructure	  

     9.1 Anaerobic digestion	                           


</p> 
<br>

This file lists the input parameter that the Holos model will ask for at different stages of the interface (or will offer the opportunity to specify). In order to provide and overview to our Living Lab partners (and other potential Holos model users) we have made an effort to determine which: 

    
* Inputs are key to the calculations (***required***), 
* which ones would be good to have to further specify the model outputs (***optional***), 
* some entries are ***recommended*** for ease of input
* which ones are key default parameter from the National Inventory that should generally not be changed (***operational***) unless there is scientific consensus 

* and others that the model automatically determines (calculated) 
* there is also some parameters listed that are ***not editable*** within that current list

</p> 
<br>

# 1.	Location data

|Data type                     | Importance    | Link for download data /comments | 
|------------------------------|---------------|----------------------------------|
|Historic daily weather data |Optional (acceptable defaults) |         https://power.larc.nasa.gov/data-access-viewer/
|Soil data |Optional (defaults provided, user specification suggested) |	https://open.canada.ca/data/en/dataset/5ad5e20c-f2bb-497d-a2a2-440eec6e10cd
|Plant hardiness zones |Optional (good default) |	http://www.planthardiness.gc.ca/?m=1
</p> 
<br>

### *Settings – Farm Defaults*

<body>
<p style = "color:green">
Farm
<body>


|Data type                     | Importance    | Link for download data /comments | 
|------------------------------|---------------|----------------------------------|
|Holos operation model (basic/advanced) |Required |Advanced mode is recommended for users exploring the effect of alternative management practices.
|Name |Optional	|   |
|Comments |Optional |    |
|Growing Season Precipitation (mm) | Calculated from default climate data |Upload custom climate data to adjust
Evapotranspiration (mm)	 |Calculated from default climate data |Upload custom climate data to adjust
|Polygon |Not editable |Determined in the map screen
|Coordinates |Not editable |Determined in the map screen
|Province |Not editable	|Determined in the map screen
|Hardiness zone	|Not editable |Determined in the map screen

</p> 
<br>
<body>
<p style = "color:green">
Soil
<body>

|Data type                     | Importance    | Link for download data /comments |
|------------------------------|---------------|----------------------------------|
|Soil texture |Optional |**Overwrite if farm specific data are available**
|Top layer thickness (cm) |Optional	|**Overwrite if farm specific data are available**
|Bulk density (g cm<sup>-3</sup>) |Optional |**Overwrite if farm specific data are available**	
|Proportion of clay in soil (%) |Optional |**Overwrite if farm specific data are available**	
|Proportion of sand in soil (%) |Optional |**Overwrite if farm specific data are available**	
|Proportion of soil organic carbon (%) |Optional |**Overwrite if farm specific data are available**	
|Soil pH |Optional |**Overwrite if farm specific data are available**	
|Soil cation exchange capacity (mEq 100g<sup>-1</sup>) |Optional|**Overwrite if farm specific data are available**	    |	
|Carbon modelling equilibrium year |Operational |**Changes the starting year for all new fields**
|Ecodistrict |Not editable |Determined in the map screen
|Soil functional category |Not editable |Determined in the map screen	
|Soil great group |Not editable |Determined in the map screen	

</p> 
<br>

<body>
<p style = "color:green">
 Precipitation / Temperature / Evapotranspiration (mm, ℃, mm)
<body>

 *Climate data can be specified (if either the default NASA data are not desired, or climate data prior to 1981 are needed). In that case, it is recommended to upload custom daily climate data during the map screen. Adjusting the climate normals in this interface will force the model to use the updated values, but the model will has to extrapolate daily values from these monthly averages/sums.*

|Data type                     | Importance    | Link for download data /comments |
|------------------------------|---------------|----------------------------------|
|January |Operational |Can be user specified
|February |Operational |Can be user specified	
|March |Operational |Can be user specified
|April |Operational |Can be user specified
|May |Operational |Can be user specified	 
|June |Operational |Can be user specified	
|July |Operational |Can be user specified	
|August |Operational |Can be user specified	
|September |Operational |Can be user specified
|October |Operational |Can be user specified	
|November |Operational |Can be user specified	
|December |Operational |Can be user specified	

</p> 
<br>

<body>
<p style ="color:green">
 Soil N<sub>2</sub>O breakdown (%)
<body>

*The distribution of annual emissions into monthly rates is based on expert opinion. It can be changed (but will not change the total overall emissions) if better data are available. Ensure that the monthly fractions sum up to 100%.*

|Data type                     | Importance    | Link for download data /comments |
|------------------------------|---------------|----------------------------------|
|January |Operational |Can be user specified
|February |Operational |Can be user specified	
|March |Operational |Can be user specified	
|April |Operational |Can be user specified	
|May |Operational |Can be user specified	
|June |Operational |Can be user specified	
|July |Operational |Can be user specified	
|August |Operational |Can be user specified	
|September |Operational |Can be user specified	
|October |Operational |Can be user specified	
|November |Operational |Can be user specified	
|December |Operational |Can be user specified	

</p> 
<br>

<body>
<p style ="color:green">
Default Bedding Composition
<body>

|Data type                     | Importance    | Link for download data /comments |
|------------------------------|---------------|----------------------------------|
|Total N (kg N kg DM<sup>-1</sup>) |Operational |Updates defaults for all bedding applications. Reliable measurements should be used.
|Total P (kg P kg DM<sup>-1</sup>) |Operational	|Updates defaults for all bedding applications. Reliable measurements should be used.   
Total C (kg C kg DM<sup>-1</sup>) |Operational |Updates defaults for all bedding applications. Reliable measurements should be used.     

</p> 
<br>

<body>
<p style ="color:green">
Default Manure Composition
<body>

|Data type                     | Importance    | Link for download data /comments |
|------------------------------|---------------|----------------------------------|
|Moisture content (%) |Operational |Updates defaults for all manure applications. Reliable measurements should be used. 
|Total N (% wet weight) |Operational |Updates defaults for all manure applications. Reliable measurements should be used.   
|Total C (% wet weight) |Operational |Updates defaults for all manure applications. Reliable measurements should be used.   
|C:N |Operational |Updates defaults for all manure applications. Reliable measurements should be used.    

</p> 
<br>

### *Settings – User Defaults*

<body>
<p style = "color:green">
Carbon modelling
<body>		

|Data type                     | Importance    | Link for download data /comments |
|------------------------------|---------------|----------------------------------|
|Carbon modelling strategy |Operational |Permits to switching to carbon change simulations using the Introductory Carbon Balance Model (**IPCC Tier 2 is the current National GHG Inventory Method)**
|Carbon concentration (kg kg<sup>-1</sup>) |Operational |Changes C concentration in plant biomass throughout the model. **Do not change.**
|Equilibrium year yield calculation strategy |Operational |Bases starting soil carbon content on the basis of different input defaults.
|Use custom equilibrium value (kg C ha<sup>-1</sup>) |Operational |Forces the model to use custom carbon stock values instead.

</p> 
<br>

<body>
<p style = "color:green">
Biomass (%)
<body>

|Data type                     | Importance    | Link for download data /comments |
|------------------------------|---------------|----------------------------------|
|Product returned to soil for annuals |Operational |Changes default biomass return rates for new crops. 
|Straw returned to soil for annuals |Operational |Changes default biomass return rates for new crops.  
|Roots returned to soil for annuals |Operational |Changes default biomass return rates for new crops.  
|Product returned to soil for fodder corn |Operational |Changes default biomass return rates for new crops.  
|Roots returned to soil for fodder corn |Operational |Changes default biomass return rates for new crops.  
|Product returned to soil for perennials |Operational |Changes default biomass return rates for new crops.  
|Roots returned to soil for perennials |Operational |Changes default biomass return rates for new crops.  
|Product returned to soil for root crops |Operational |Changes default biomass return rates for new crops.  
|Straw returned to soil for root crops |Operational |Changes default biomass return rates for new crops.  
|Supplemental feeding loss |Operational |Changes default biomass return rates for new crops.  

</p> 
<br>

<body>
 <p style="color:green">
 N<sub>2</sub>O (kg N<sub>2</sub>O-N kg N<sup>-1</sup>)
<body>


|Data type                     | Importance    | Link for download data /comments |
|------------------------------|---------------|----------------------------------|
|Volatilization emission factor |Operational |These values are aligned with the National GHG inventory. Only change if reliable measurements suggest different emission factors.
|Emission factor for leaching and runoff |Operational |These values are aligned with the National GHG inventory. Only change if reliable measurements suggest different emission factors.    

</p> 
<br>

<body>
<p style = "color:green">
Irrigation
<body>

|Data type                     | Importance    | Link for download data /comments |
|------------------------------|---------------|----------------------------------|
|Pump type |Optional |     |	

<br>

### *Settings – Energy*

|Data type                     | Importance    | Link for download data /comments |
|------------------------------|---------------|----------------------------------|
|Electricity usage conversion (kg CO<sub>2</sub> kWh<sup>-1</sup>) |Operational|Changed defaults for all future application/animal additions. Replace only on the basis of reliable measurements.
|Diesel fuel production conversion (kg CO<sub>2</sub> GJ<sup>-1</sup>) |Operational |Changed defaults for all future application/animal additions. Replace only on the basis of reliable measurements.	
|Herbicide production conversion (kg CO<sub>2</sub> GJ<sup>-1</sup>) |Operational|Changed defaults for all future application/animal additions. Replace only on the basis of reliable measurements.	
|Nitrogen fertilizer production conversion (kg CO<sub>2</sub> kg N<sup>-1</sup>) |Operational |Changed defaults for all future application/animal additions. Replace only on the basis of reliable measurements.	
|Phosphorus fertilizer production conversion (kg CO<sub>2</sub> kg P<sub>2</sub>O<sub>5</sub><sup>-1</sup>) |Operational|Changed defaults for all future application/animal additions. Replace only on the basis of reliable measurements.	
|Irrigation conversion (kg CO<sub>2</sub> mm<sup>-1</sup>) |Operational |Changed defaults for all future application/animal additions. Replace only on the basis of reliable measurements.	
|kWh per cattle per year for electricity (kWh animal<sup>-1</sup>) |Operational |Changed defaults for all future application/animal additions. Replace only on the basis of reliable measurements.	
|kWh per dairy per year for electricity (kWh animal<sup>-1</sup>) |Operational |Changed defaults for all future application/animal additions. Replace only on the basis of reliable measurements.	
|kWh per pig per year for electricity (kWh animal<sup>-1</sup>) |Operational |Changed defaults for all future application/animal additions. Replace only on the basis of reliable measurements.	
|kWh per poultry placement per year for electricity (kWh poultry placement<sup>-1</sup>) |Operational|Changed defaults for all future application/animal additions. Replace only on the basis of reliable measurements.	
|GJ of energy per 1000 litres of solid manure applied (GJ 1000 litre<sup>-1</sup>) |Operational|Changed defaults for all future application/animal additions. Replace only on the basis of reliable measurements.	
|GJ of energy per 1000 litres of liquid manure applied (GJ 1000 litre<sup>-1</sup>) |Operational|Changed defaults for all future application/animal additions. Replace only on the basis of reliable measurements.	


### *Settings – Crop Defaults*

 *Permits changing biomass carbon fractions and their nitrogen concentrations for each crop.*
</p> 
<br>

## 2	Land management

### 2.1	Field/Crop Rotation components
The field and crop rotation component are structured exactly the same with respect to their input, the only difference is that in the field component, a crop rotation is defined for this field. In the crop rotation component, the crop rotation comes first, and the model assigns the necessary number of fields to the rotation (assuming that each phase of the rotation should be present every year).
</p> 
<br>
<body>
<p style ="color:blue">
Step 1
<body>


|Parameter      | Unit    | Importance         | Description                      |
|---------------|---------|--------------------|----------------------------------|
|Field Name     |         |	 Recommended       |		                          |
|Total area of the field| ha |Required |direct and indirect N<sub>2</sub>O emissions /	carbon change
|Start year of field history |  | Required |Start year of the simulation. Year for which the soil carbon equilibrium state is calculated (if no carbon content is specified) / carbon change.
|End year|   | Required |Final year of the field history. Specifies in which year the details screen/timeline screens stop copying the rotation forward. Year for which land management emissions and carbon change are reported for.
</p> 
<br>
<body>
<p style = "color:blue"> 
Step 2
<body>			

|Parameter      | Unit    | Importance         | Description                      |
|---------------|---------|--------------------|----------------------------------|
|Year           |         |	 Required          |		                          |	
|Crop |   |Required |direct and indirect N<sub>2</sub>O emissions / carbon change
|Winter/Cover/Undersown Crop |    |Optional (if present) |carbon change
</p> 
<br>
	
<body>
<p style = "color:blue">
Step 3
<body>			

<body>
<p style = "color:green">
General properties
<body>	

|Parameter      | Unit    | Importance         | Description                      |
|---------------|---------|--------------------|----------------------------------|
|Yield (wet weight) |kg  ha<sup>-1</sup> |Required (default available) |direct and indirect N<sub>2</sub>O emissions / carbon change
|Yield (dry weight) |kg DM ha<sup>-1</sup>| See above |direct and indirect N<sub>2</sub>O emissions / carbon change
|<p style = "color:red">*Tillage:*|
|Intensive tillage|   | Required |CO<sub>2</sub> emissions / direct N<sub>2</sub>O emissions 
|Reduced tillage|   | Required |CO<sub>2</sub> emissions / direct N<sub>2</sub>O emissions
|No tillage        |   | Required |CO<sub>2</sub> emissions / direct N<sub>2</sub>O emissions 
|<p style = "color:red">*Harvest method:*|
|Cash crop |   |Recommended	|Predetermines residue management / direct and indirect N<sub>2</sub>O emissions / carbon change  
|Green manure |   |Recommended	|Predetermines residue management / direct and indirect N<sub>2</sub>O emissions / carbon change  
|Silage |   |Recommended	|Predetermines residue management / direct and indirect N<sub>2</sub>O emissions / carbon change 
|Swathing |   |Recommended	|Predetermines residue management / direct and indirect N<sub>2</sub>O emissions / carbon change 
|    |
|Moisture content of crop |% |Optional (default available) |direct and indirect N<sub>2</sub>O emissions / carbon change
|Amount of irrigation |mm |Optional (only if applied) |CO<sub>2</sub> emissions
|No of pesticides passes |   | Optional (only if applied) |CO<sub>2</sub> emissions
|Fuel energy |GJ ha<sup>-1</sup> |Operational (National defaults used) |CO<sub>2</sub> emissions
|Herbicide energy |GJ ha<sup>-1</sup> |Operational (National defaults used) |CO<sub>2</sub> emissions

</p> 
<br>

<body>
<p style = "color:green">
Fertilizer management (if applicable)
<body>

|Parameter      | Unit    | Importance         | Description                      |
|---------------|---------|--------------------|----------------------------------|
|Season of application |    |Required |indirect N<sub>2</sub>O emissions
|Blend |    |Required |indirect N<sub>2</sub>O emissions / CO<sub>2</sub> emissions
|Method	 |    |Required |indirect N<sub>2</sub>O emissions 
|Rate of application |kg ha<sup>-1</sup> |Required |indirect N<sub>2</sub>O emissions
|Atmospheric nitrogen deposition |kg N ha<sup>-1</sup> |Optional (reduces default fertilizer rate) |indirect N<sub>2</sub>O emissions (default =  5 kg N ha<sup>-1</sup>)
|Nitrogen fixation |kg N ha <sup>-1</sup>year<sup>-1</sup> |Optional (reduces default fertilizer rate) |indirect N<sub>2</sub>O emissions (default of 70% of Crop N requirement)
|Soil test nitrogen |kg N ha<sup>-1</sup> |Optional (reduces default fertilizer rate)|    |	
|Fertilizer efficiency |% |Optional (reduces default fertilizer rate) |indirect N<sub>2</sub>O emissions (default =  75%)
|Nitrogen |% |Operational (custom blends) |indirect N<sub>2</sub>O emissions
|Phosphorus |% |Operational (custom blends) |Not yet used in the model
|Potassium |% |Operational (custom blends) |Not yet used in the model
|Sulfur |% |Operational (custom blends) |Not yet used in the model
</p> 
<br>

<body>
<p style = "color:green">
Manure management (if applicable)
<body>

|Parameter      | Unit    | Importance         | Description                      |
|---------------|---------|--------------------|----------------------------------|
|Date |   |Required |indirect N<sub>2</sub>O 
|Manure type |    |Required |manure CH<sub>4</sub> / direct and indirect  N<sub>2</sub>O emissions
|Origin of manure |    |Required |manure CH<sub>4</sub> / direct and indirect N<sub>2</sub>O emissions
|Manure handling system |    |Required |manure CH<sub>4</sub> / direct and indirect N<sub>2</sub>O emissions
|Application method |    |Required |manure CH<sub>4</sub> / direct and indirect N<sub>2</sub>O emissions
|Amount of manure |kg ha<sup>-1</sup> |Required |manure CH<sub>4</sub> / direct and indirect N<sub>2</sub>O emissions
|Amount of N from applied manure |kg N ha<sup>-1</sup> |Optional |direct and indirect N<sub>2</sub>O emissions
|Fraction of N in manure |% |Optional |direct and indirect N<sub>2</sub>O emissions
</p> 
<br>

<body>
<p style = "color:green">
Winter and cover crops (if applicable)
<body>

|Parameter      | Unit    | Importance         | Description                      |
|---------------|---------|--------------------|----------------------------------|
|Crop type |    |Required |CO<sub>2</sub> emissions (soil carbon change)
|Yield |kg DM ha<sup>-1</sup> |Required |CO<sub>2</sub> emissions (soil carbon change)
</p> 
<br>

<body>
<p style = "color:green">
Residue management (for further specification)
<body>

|Parameter      | Unit    | Importance         | Description                      |
|---------------|---------|--------------------|----------------------------------|
|Product returned to soil |% |Optional |**Pre-set defaults based on harvest mgmt.** /CO<sub>2</sub> emissions (soil carbon change)
|Straw returned to soil |% |Optional | **Pre-set defaults based on harvest mgmt.** /CO<sub>2</sub> emissions (soil carbon change)
|Roots returned to soil |% |Optional |**Pre-set defaults based on harvest mgmt.** /CO<sub>2</sub> emissions (soil carbon change) 
|Carbon coefficient of product |    |Operational |CO<sub>2</sub> emissions (soil carbon change)
|Carbon coefficient of straw |    |Operational |CO<sub>2</sub> emissions (soil carbon change)
|Carbon coefficient of roots |    |Operational |CO<sub>2</sub> emissions (soil carbon change)
|Carbon coefficient of extra-roots |    |Operational |CO<sub>2</sub> emissions (soil carbon change)
|Lignin content |% |Operational |CO<sub>2</sub> emissions (soil carbon change) in the IPCC Tier 2 carbon model
|Nitrogen content in product |kg N ha<sup>-1</sup> |Operational |direct and indirect N<sub>2</sub>O emissions
|Nitrogen content in straw |kg N ha<sup>-1</sup> |Operational |direct and indirect N<sub>2</sub>O emissions
|Nitrogen content in roots |kg N ha<sup>-1</sup> |Operational |direct and indirect N<sub>2</sub>O emissions
|Nitrogen content in extra-roots |kg N ha<sup>-1</sup> |Operational |direct and indirect N<sub>2</sub>O emissions

</p> 
<br>

## 2.2	Field-Perennial/Grassland

<body>
<p style = "color:blue">
Step 1
<body>	

|Parameter      | Unit    | Importance         | Description                      |
|---------------|---------|--------------------|----------------------------------|
|Field Name |    |Recommended |	
|Total area of the field |ha |Required |direct and indirect N<sub>2</sub>O emissions / carbon change
|Start year of field history |    |Required |**Start year of the simulation.** Year for which the soil carbon equilibrium state is calculated (if no carbon content is specified) / carbon change
|End year |    |Required |**Final year of the field history.** Specifies in which year the details screen/timeline screens stop copying the rotation forward. Year for which land management emissions and carbon change are reported for.
</p> 
<br>			
<body>
<p style ="color:blue">
Step 2
<body>				

|Parameter      | Unit    | Importance         | Description                      |
|---------------|---------|--------------------|----------------------------------|
|Field Name |    |Recommended |				
|<p style = "color:red">*Crops grown in this field:* |
|Forage for seed |    |Required |Impact on carbon change
|Rangeland (native) |   |Required |Impact on carbon change
|Seeded grassland |    |Required |Impact on carbon change
|Tame grass |    |Required |Impact on carbon change
|Tame legume |    |Required |Impact on carbon change
|Tame mixed	(grass/legume) |    |Required |Impact on carbon change
</p> 
<br>
<body>
<p style = "color:blue">
Step 3
<body>

<body>
<p style = "color:green">
General properties
<body>

|Parameter      | Unit      | Importance         | Description                    |
|---------------|-----------|--------------------|--------------------------------|	
|Undersown crop Y/N |    |Optional |**Only applicable if an annual crop is grown the year prior. Add carbon from the growing perennial to the residues of the annual crop.** / carbon change
|Moisture content of crop |% |Optional (default available) |direct and indirect N<sub>2</sub>O emissions / carbon change
|Amount of irrigation |mm |Optional (only if applied) |CO<sub>2</sub> emissions
|No of pesticides passes |    |Optional (only if applied) |CO<sub>2</sub> emissions
|Fuel energy |GJ ha<sup>-1</sup> |Operational (National defaults used) |CO<sub>2</sub> emissions
|Herbicide energy |GJ ha<sup>-1</sup> |Operational (National defaults used) |CO<sub>2</sub> emissions
|Rate constant |    |Operational |**Not for multi-year carbon model**
|Maximum carbon produced |g m<sup>2</sup> |Operational |**Not for multi-year carbon model**
</p> 
<br>

<body>
<p style = "color:green">
Fertilizer management (if applicable)
<body>

|Parameter      | Unit      | Importance         | Description                    |
|---------------|-----------|--------------------|--------------------------------|	
|Season of application |   |Required |indirect N<sub>2</sub>O emissions
|Blend |    |Required |indirect N<sub>2</sub>O emissions / CO<sub>2</sub> emissions
|Method |    |Required |indirect N<sub>2</sub>O emissions 
|Rate of application |kg ha<sup>-1</sup> |Required |direct and indirect N<sub>2</sub>O emissions
|Atmospheric nitrogen deposition |kg N ha<sup>-1</sup> |Optional (reduces default fertilizer rate) |direct and indirect N<sub>2</sub>O emissions (default =  5 kg N ha<sup>-1</sup>)
|Nitrogen fixation |kg N ha <sup>-1</sup> year<sup>-1</sup> |Optional (reduces default fertilizer rate) |indirect N<sub>2</sub>O emissions (default of 70% of Crop N requirement)
|Soil test nitrogen |kg N ha<sup>-1</sup> |Optional (reduces default fertilizer rate)|    |	
|Fertilizer efficiency |% |Optional (reduces default fertilizer rate) |direct and indirect N<sub>2</sub>O emissions (default =  75%)
|Nitrogen |% |Operational (custom blends) |direct and indirect N<sub>2</sub>O emissions
|Phosphorus |% |Operational (custom blends) |Not yet used in the model
|Potassium |% |Operational (custom blends) |Not yet used in the model
|Sulfur |% |Operational (custom blends) |Not yet used in the model
</p> 
<br>

<body>
<p style = "color:green">
Manure management (additions by the farmer, depositions from grazing animals added automatically)
<body>

|Parameter      | Unit      | Importance         | Description                    |
|---------------|-----------|--------------------|--------------------------------|	
|Date |    |Required |indirect N<sub>2</sub>O
|Manure type |    |Required |manure CH<sub>4</sub> / direct and indirect N<sub>2</sub>O emissions
|Origin of manure |    |Required | manure CH<sub>4</sub> / direct and indirect N<sub>2</sub>O emissions
|Manure handling system |    |Required |manure CH<sub>4</sub> / direct and indirect N<sub>2</sub>O emissions
|Application method |    |Required |manure CH<sub>4</sub> / direct and indirect N<sub>2</sub>O emissions
|Amount of manure |kg ha<sup>-1</sup> |Required |manure CH<sub>4</sub> / direct and indirect N<sub>2</sub>O emissions
|Amount of N from applied manure |kg N ha<sup>-1</sup> |Optional |direct and indirect N<sub>2</sub>O emissions
|Fraction of N in manure |%	 |Optional |direct and indirect N<sub>2</sub>O emissions
</p> 
<br>

<body>
<p style = "color:green">
Harvest (if applicable)
<body>

|Parameter      | Unit      | Importance         | Description                    |
|---------------|-----------|--------------------|--------------------------------|	
|Total number of bales |    |Required |carbon change
|Wet bale weight |kg |Required |    |	
|Moisture content |% |Required |    |	
|Date |    |Optional |No relevance so far
|Forage growth stage |     |Optional |Not yet utilized in the model, should determine default feed quality in future / enteric CH<sub>4</sub>
|Harvest loss |% |Optional |carbon change
|Total biomass |kg |Optional |carbon change
</p> 
<br>

<body>
<p style = "color:green">
Grazing (if applicable)
<body>

|Parameter      | Unit      | Importance         | Description                    |
|---------------|-----------|--------------------|--------------------------------|	
|Animal groups/numbers|    |    |**derived from livestock component**
|Grazing timing |    |    |**derived from livestock component**
|<p style = "color:red">*Supplemental hay:* |
|Source of bales |    |Required |**Checks whether enough bales are available**
|Field |    |Required |direct and indirect N<sub>2</sub>O emissions / carbon change
|number of bales |    |Required |carbon change
|Wet bale weight |kg |Required |carbon change
|Moisture content |% |Required |carbon change 
|Date |    |Optional |No relevance so far

</p> 
<br>

<body>
<p style = "color:green">
Residue management (for further specification)
<body>

|Parameter      | Unit      | Importance         | Description                    |
|---------------|-----------|--------------------|--------------------------------|	
|Product returned to soil |% |Optional |Pre-set defaults based on harvest mgmt. /	CO<sub>2</sub> emissions (soil carbon change)
|Straw returned to soil |% |Optional |	Pre-set defaults based on harvest mgmt. /	CO<sub>2</sub> emissions (soil carbon change)
|Roots returned to soil |% |Optional |Pre-set defaults based on harvest mgmt. /	CO<sub>2</sub> emissions (soil carbon change)
|Carbon coefficient of product |    |Operational |CO<sub>2</sub> emissions (soil carbon change)
|Carbon coefficient of straw |    |Operational |CO<sub>2</sub> emissions (soil carbon change)
|Carbon coefficient of roots |    |Operational |CO<sub>2</sub> emissions (soil carbon change)
|Carbon coefficient of extra-roots |    |Operational |CO<sub>2</sub> emissions (soil carbon change)
|Lignin content |% |Operational |CO<sub>2</sub> emissions (soil carbon change) in the IPCC Tier 2 carbon model
|Nitrogen content in product |kg N ha<sup>-1</sup> |Operational |direct and indirect N<sub>2</sub>O emissions
|Nitrogen content in straw |kg N ha<sup>-1</sup> |Operational |direct and indirect N<sub>2</sub>O emissions
|Nitrogen content in extra-roots |kg N ha<sup>-1</sup> |Operational |direct and indirect N<sub>2</sub>O emissions
</p> 
<br>

### 2.3	Shelterbelt

|Parameter      | Unit      | Importance         | Description                    |
|---------------|-----------|--------------------|--------------------------------|
|Year of observation |    |Required |    |	
|Row length |m |Required |carbon change
|No of trees |    |Required |carbon change
|Average circumferences |cm	|Required |Average of tree stem circumferences at 1.3m height along the individual stem outside bark. / carbon change
|Species |    |Required |White Spruce, Scots Pine, Hybrid Poplar, Manitoba Maple, Green Ash, and Caragana
|Additional rows |     |Optional |Add rows to enter more complex shelterbelts
|Average spacing between trees |m |Optional |carbon change

</p> 
<br>

# 3	Beef production
Note that the beef production is subdivided into Cow-calf, backgrounding and finishing. The components require the same information details, but do have different default setups and lookups.

<body>
<p style = "color:blue">
Step 1
<body>	

|Parameter      | Unit      | Importance         | Description                    |
|---------------|-----------|--------------------|--------------------------------|
|<p style = "color:red">*Group name:*|
|         Cow|    |Required |enteric CH<sub>4</sub> emissions
|        Calf|    |Required |enteric CH<sub>4</sub> emissions 
|       Bulls|    |Required |enteric CH<sub>4</sub> emissions
| Replacement heifers |    |Required |enteric CH<sub>4</sub> emissions	
			
</p> 
<br>

<body>
<p style = "color:blue">
Step 2
<body>	

|Parameter      | Unit      | Importance         | Description                    |
|---------------|-----------|--------------------|--------------------------------|
|No of mgmt. periods |    |Required |**Determined by adding mgmt. periods as needed**
|Start date |    |Required |    |	
|End date |    |Required |    |	
|Management period name |    |Optional |    |	
|No of days	|    |Optional |    |	
			
</p> 
<br>

<body>
<p style = "color:blue">
Step 3
<body>	

<body>
<p style = "color:green">
General management
<body>

|Parameter      | Unit      | Importance         | Description                    |
|---------------|-----------|--------------------|--------------------------------|	
|No of animals |    |Required |enteric CH<sub>4</sub> emissions
|Start weight |kg	|Required |enteric CH<sub>4</sub> emissions
|End weight |kg	|Required |enteric CH<sub>4</sub> emissions
|Daily gain |kg day<sup>-1</sup> |Optional |Calculated from Start and End weight
|Gain coefficient of bulls |     |Operational |    |	
</p> 
<br>
<body>
<p style = "color:green">
Diet management
<body>

|Parameter      | Unit      | Importance         | Description                    |
|---------------|-----------|--------------------|--------------------------------|	
| <p style = "color:red"> *Diet type:*         
| Low energy protein |    |Required | enteric CH<sub>4</sub> emissions / manure CH<sub>4</sub>  / direct and indirect N<sub>2</sub>O emissions 
|Medium energy protein |    |Required  |enteric CH<sub>4</sub> emissions / manure CH<sub>4</sub>  / direct and indirect N<sub>2</sub>O emissions 
|High energy protein |    |Required  |enteric CH<sub>4</sub> emissions / manure CH<sub>4</sub>  / direct and indirect N<sub>2</sub>O emissions
|<p style = "color:red">*Diet additives:*| **More additives will become available in future** |  
|None |    | Optional |enteric CH<sub>4</sub> emissions|
|2% fat |    | Optional |enteric CH<sub>4</sub> emissions|
|4% fat |    | Optional |enteric CH<sub>4</sub> emissions|
|Ionophore |    | Optional |enteric CH<sub>4</sub> emissions|
|Ionophore + 2%fat |    | Optional |enteric CH<sub>4</sub> emissions|
|Ionophore + 4% fat	|    | Optional |enteric CH<sub>4</sub> emissions|
|    |
|Forage |% |Info only, can be changed using the custom diet creator |enteric CH<sub>4</sub> emissions / manure CH<sub>4</sub> / direct and indirect N<sub>2</sub>O emissions
|Crude protein (CP) |% DM |Info only, can be changed using the custom diet creator |enteric CH<sub>4</sub> emissions / manure CH<sub>4</sub> / direct and indirect N<sub>2</sub>O emissions	
|Fat |% DM |Info only, can be changed using the custom diet creator |enteric CH<sub>4</sub> emissions / manure CH<sub>4</sub> / direct and indirect N<sub>2</sub>O emissions		
|Ash content of manure |% DM |Info only, can be changed using the custom diet creator |enteric CH<sub>4</sub> emissions / manure CH<sub>4</sub> / direct and indirect N<sub>2</sub>O emissions	
|Starch |% DM  |Info only, can be changed using the custom diet creator |enteric CH<sub>4</sub> emissions / manure CH<sub>4</sub> / direct and indirect N<sub>2</sub>O emissions			
|Neutral detergent fiber (NDF) |% DM  |Info only, can be changed using the custom diet creator |enteric CH<sub>4</sub> emissions / manure CH<sub>4</sub> / direct and indirect N<sub>2</sub>O emissions			
|Total digestible nutrient (TDN) |% DM  |Info only, can be changed using the custom diet creator |enteric CH<sub>4</sub> emissions / manure CH<sub>4</sub> / direct and indirect N<sub>2</sub>O emissions			
|Metabolizable energy |Mcal kg<sup>-1</sup>  |Info only, can be changed using the custom diet creator |enteric CH<sub>4</sub> emissions / manure CH<sub>4</sub> / direct and indirect N<sub>2</sub>O emissions			
|Methane conversion factor |kg CH<sub>4</sub> (kg CH<sub>4</sub>)<sup>-1</sup> |Info only, can be changed using the custom diet creator |enteric CH<sub>4</sub> emissions / manure CH<sub>4</sub> / direct and indirect N<sub>2</sub>O emissions
</p> 
<br>

<body>
<p style = "color:green">
Custom diet creator
<body>

|Parameter      | Unit      | Importance         | Description                    |
|---------------|-----------|--------------------|--------------------------------|	
|Crude protein (CP) |% DM 	|Required |direct and indirect N<sub>2</sub>O emissions
|Total digestible nutrient (TDN) |% DM |Required |enteric CH<sub>4</sub> emissions
|Ash |%DM |Required |anaerobic digestion
|Dry matter |% AF |Optional |Required for alternative CH<sub>4</sub> equations and for the formulation of custom diets / enteric CH<sub>4</sub> emissions
|Lignin |% DM |Optional |Required for alternative CH<sub>4</sub> equations and for the formulation of custom diets / enteric CH<sub>4</sub> emissions
|Forage |%	|Optional |Required for alternative CH<sub>4</sub> equations and for the formulation of custom diets / enteric CH<sub>4</sub> emissions	
|Starch |% DM |Optional |Required for alternative CH<sub>4</sub> equations and for the formulation of custom diets / enteric CH<sub>4</sub> emissions	
|Fat (esther extracted) |% DM |Optional |Required for alternative CH<sub>4</sub> equations and for the formulation of custom diets / enteric CH<sub>4</sub> emissions
|Neutral detergent fiber (NDF) |% DM |Optional |Required for alternative CH<sub>4</sub> equations and for the formulation of custom diets / enteric CH<sub>4</sub> emissions
|Methane conversion factor |kg CH<sub>4</sub> (kg CH<sub>4</sub>)<sup>-1</sup>| Operational |Automatically calculated / enteric CH<sub>4</sub> emissions
</p> 
<br>

<body>
<p style = "color:green">
Housing management
<body>

|Parameter      | Unit      | Importance         | Description                    |
|---------------|-----------|--------------------|--------------------------------|	
| <p style = "color:red"> *Housing type:*| 
|Confined no barn |    |Required |manure CH<sub>4</sub> / direct and indirect N<sub>2</sub>O emissions
|Housed in barn (solid) |    |Required |manure CH<sub>4</sub> / direct and indirect N<sub>2</sub>O emissions
|Housed in barn (slurry) |    |Required |manure CH<sub>4</sub> / direct and indirect N<sub>2</sub>O emissions
|Pasture |    |Required |**If pasture is selected, the field component can be selected on which the grazing takes place.** manure CH<sub>4</sub> / direct and indirect N<sub>2</sub>O emissions
| <p style = "color:red"> *Bedding type:*|
|Straw |    |Optional |direct and indirect N<sub>2</sub>O emissions / carbon change
|Wood chip |    |Optional |direct and indirect N<sub>2</sub>O emissions / carbon change
|    |
|Bedding application rate |kg head<sup>-1</sup>day<sup>-1</sup> |Optional |direct and indirect N<sub>2</sub>O emissions / carbon change
|Activity coefficient of feeding situation |    |Operational |enteric CH<sub>4</sub> emissions
|Maintenance coefficient |MJ day<sup>-1</sup> kg<sup>-1</sup> |Operational |enteric CH<sub>4</sub> emissions
</p> 
<br>

<body>
<p style = "color:green">
Bedding application calculator (if applicable)
<body>

|Parameter      | Unit      | Importance         | Description                    |
|---------------|-----------|--------------------|--------------------------------|
|Total number of bales |    |Optional |direct and indirect N<sub>2</sub>O emissions / carbon change 
|Average weight of bales |    |Optional |direct and indirect N<sub>2</sub>O emissions / carbon change 	
|Number of days |    |Optional	|direct and indirect N<sub>2</sub>O emissions / carbon change 
|Number of animals |    |Optional |direct and indirect N<sub>2</sub>O emissions / carbon change 	
|Bedding applied per day |kg |Calculated |direct and indirect N<sub>2</sub>O emissions / carbon change 	
|Bedding application rate |kg head<sup>-1</sup>day<sup>-1</sup> |Calculated	|direct and indirect N<sub>2</sub>O emissions / carbon change 
</p> 
<br>

<body>
<p style = "color:green">
Manure management
<body>

|Parameter      | Unit      | Importance         | Description                    |
|---------------|-----------|--------------------|--------------------------------|
|<p style = "color:red">*Manure handling system*:
|Pasture |    |Required |manure CH<sub>4</sub> / direct and indirect N<sub>2</sub>O emissions
|Solid storage |    |Required |manure CH<sub>4</sub> / direct and indirect N<sub>2</sub>O emissions 
|Compost passive |    |Required |manure CH<sub>4</sub> / direct and indirect N<sub>2</sub>O emissions
|Compost intensive |    |Required |manure CH<sub>4</sub> / direct and indirect N<sub>2</sub>O emissions
|Deep bedding |    |Required |manure CH<sub>4</sub> / direct and indirect N<sub>2</sub>O emissions
|Anaerobic digester	|    |Required |manure CH<sub>4</sub> / direct and indirect N<sub>2</sub>O emissions	
|    |
|Ash content of manure |% |Optional	|direct and indirect N<sub>2</sub>O emissions
|Fraction of N in manure |% wet weight |Optional |direct and indirect N<sub>2</sub>O emissions
|Fraction of C in manure |% wet weight |Optional |manure CH<sub>4</sub> emissions
|Fraction of P in manure |% wet weight |Optional |    |	
|Leaching Fraction |kg N (kg N)<sup>-1</sup> |Operational |indirect N<sub>2</sub>O emissions
|Direct emission factor| kg N2O-N (kg N)<sup>-1</sup> |Operational |direct N<sub>2</sub>O emissions
|Volatilization emission factor |kg N2O-N (kg N)<sup>-1</sup> |Operational |indirect N<sub>2</sub>O emissions
|Methane producing capacity of manure |m<sup>3</sup> CH<sub>4</sub> (kg VS)<sup>-1</sup> |Operational |manure CH<sub>4</sub> emissions
|Emission factor for leaching |kg N2O-N (kg N)<sup>-1</sup> |Operational |indirect N<sub>2</sub>O emissions
</p> 
<br>

# 4	Dairy cattle

<body>
<p style = "color:blue">
Step 1
<body>

|Parameter      | Unit      | Importance         | Description                    |
|---------------|-----------|--------------------|--------------------------------|
|<p style = "color:red">*Group name*:
|Dairy calves |    |Required |enteric CH<sub>4</sub> emissions
|Dairy heifers |    |Required |enteric CH<sub>4</sub> emissions
|Dairy lactating |    |Required |enteric CH<sub>4</sub> emissions
|Dairy dry |    |Required |enteric CH<sub>4</sub> emissions
</p> 
<br>

<body>
<p style = "color:blue">
Step 2
<body>	

|Parameter      | Unit      | Importance         | Description                    |
|---------------|-----------|--------------------|--------------------------------|			
|No of mgmt. periods |    |Required |**Determined by adding mgmt. periods as needed**
|Start date |    |Required	|
|End date |    |Required	|		
|Management period name	|    |Optional |
|No of days |    |Optional |	
</p> 
<br>

<body>
<p style = "color:blue">
Step 3
<body>	

<body>
<p style = "color:green">
Manure management
<body>	

|Parameter      | Unit      | Importance         | Description                    |
|---------------|-----------|--------------------|--------------------------------|	
|No of animals |    |Required |enteric CH<sub>4</sub> emissions
|Start weight |kg |Required |enteric CH<sub>4</sub> emissions
|End weight |kg	|Required |enteric CH<sub>4</sub> emissions
|Milk production |kg day<sup>-1</sup> |Required |enteric CH<sub>4</sub> emissions
|Daily gain |kg day<sup>-1</sup> |Optional |Calculated from Start and End weight
|Milk fat content |% |Optional |enteric CH<sub>4</sub> emissions
|Milk protein |% |Optional	|enteric CH<sub>4</sub> emissions
|Gain coefficient |    |Operational	|enteric CH<sub>4</sub> emissions
</p> 
<br>

<body>
<p style = "color:green">
Diet management
<body>

|Parameter      | Unit      | Importance         | Description                    |
|---------------|-----------|--------------------|--------------------------------|
|<p style = "color:red">*Diet type:*
|Legume forage based diet |    |Required |enteric CH<sub>4</sub> emissions / manure CH<sub>4</sub> / direct and indirect N<sub>2</sub>O emissions
|Barley silage based diet |    |Required |enteric CH<sub>4</sub> emissions / manure CH<sub>4</sub> / direct and indirect N<sub>2</sub>O emissions 
Corn silage based diet |    |Required |enteric CH<sub>4</sub> emissions / manure CH<sub>4</sub> / direct and indirect N<sub>2</sub>O emissions
Low fiber |    |Required |enteric CH<sub>4</sub> emissions / manure CH<sub>4</sub> / direct and indirect N<sub>2</sub>O emissions
High fiber |    |Required |enteric CH<sub>4</sub> emissions / manure CH<sub>4</sub> / direct and indirect N<sub>2</sub>O emissions
|<p style = "color:red">*Diet additives:*
|None |    |Optional |enteric CH<sub>4</sub> emissions 
|5% fat |    |Optional |enteric CH<sub>4</sub> emissions
|Ionophore |    |Optional |enteric CH<sub>4</sub> emissions
|Ionophore + 5% fat |    |Optional |enteric CH<sub>4</sub> emissions
|    |
|Crude protein (CP) |% DM |   |Info only, can be changed using the custom diet creator. enteric CH<sub>4</sub> emissions / manure CH<sub>4</sub> / direct and indirect N<sub>2</sub>O emissions
|Ash content of the manure |% DM |    |Info only, can be changed using the custom diet creator. enteric CH<sub>4</sub> emissions / manure CH<sub>4</sub> / direct and indirect N<sub>2</sub>O emissions	
|Acid detergent fiber |% DM |    |Info only, can be changed using the custom diet creator. enteric CH<sub>4</sub> emissions / manure CH<sub>4</sub> / direct and indirect N<sub>2</sub>O emissions		
|Total digestible nutrient (TDN) |% |    |Info only, can be changed using the custom diet creator. enteric CH<sub>4</sub> emissions / manure CH<sub>4</sub> / direct and indirect N<sub>2</sub>O emissions		
|Neutral detergent fiber |% DM |    |Info only, can be changed using the custom diet creator. enteric CH<sub>4</sub> emissions / manure CH<sub>4</sub> / direct and indirect N<sub>2</sub>O emissions		
|Ether extract |% DM |    |Info only, can be changed using the custom diet creator. enteric CH<sub>4</sub> emissions / manure CH<sub>4</sub> / direct and indirect N<sub>2</sub>O emissions		
|Digestible energy |Mcal kg<sup>-1</sup> |    |Info only, can be changed using the custom diet creator. enteric CH<sub>4</sub> emissions / manure CH<sub>4</sub> / direct and indirect N<sub>2</sub>O emissions		
|Net energy for lactation measured at 3X maintenance |Mcal kg<sup>-1</sup> |  |enteric CH<sub>4</sub> emissions / manure CH<sub>4</sub> / direct and indirect N<sub>2</sub>O emissions
|Methane conversion factor |kg CH<sub>4</sub> (kg CH<sub>4</sub>)<sup>-1</sup>||enteric CH<sub>4</sub> emissions / manure CH<sub>4</sub> / direct and indirect N<sub>2</sub>O emissions
</p> 
<br>

<body>
<p style = "color:green">
Custom diet creator
<body>

|Parameter      | Unit      | Importance         | Description                    |
|---------------|-----------|--------------------|--------------------------------|
|Total digestible nutrient (TDN) |% |Required |enteric CH<sub>4</sub> emissions
|Crude protein (CP) |% DM |Required |direct and indirect N<sub>2</sub>O emissions
|Ash |%DM |Required |anaerobic digestion
|Neutral detergent fiber (NDF) |% DM |Optional |Required for alternative CH<sub>4</sub> equations and custom diet formulations / enteric CH<sub>4</sub> emissions
|Acid detergent fiber |% DM |Optional |    |	
|Ether extract |% DM |Optional |    |	
|Methane conversion factor |kg CH<sub>4</sub> (kg CH<sub>4</sub>)<sup>-1</sup> |Operational |Automatically calculated. / enteric CH<sub>4</sub> emissions
Digestible energy |Mcal kg<sup>-1</sup> |    |    |		
Net energy lactation |Mcal kg<sup>-1</sup> |    |    |	
</p> 
<br>

<body>
<p style = "color:green">
Housing management
<body>  

|Parameter      | Unit      | Importance         | Description                    |
|---------------|-----------|--------------------|--------------------------------|
|<p style = "color:red">*Housing type:*   
|Tie stall (solid litter) |   |Required |manure CH<sub>4</sub> / direct and indirect N<sub>2</sub>O emissions
|Tie stall (slurry) |   |Required |manure CH<sub>4</sub> / direct and indirect N<sub>2</sub>O emissions
|Free stall barn (solid litter) |   |Required |manure CH<sub>4</sub> / direct and indirect N<sub>2</sub>O emissions
|Free stall barn (slurry scraping) |   |Required |manure CH<sub>4</sub> / direct and indirect N<sub>2</sub>O emissions
|Free stall barn (flushing) |   |Required |manure CH<sub>4</sub> / direct and indirect N<sub>2</sub>O emissions
|Free stall barn (milk parlour- slurry flushing) |   |Required |manure CH<sub>4</sub> / direct and indirect N<sub>2</sub>O emissions
|Drylot |   |Required |manure CH<sub>4</sub> / direct and indirect N<sub>2</sub>O emissions
|Pasture |   |Required |**If pasture is selected, the field component can be selected on which the grazing takes place.** manure CH<sub>4</sub> / direct and indirect N<sub>2</sub>O emissions
|<p style = "color:red">*Bedding type:* 
|Sand |    |Optional |direct and indirect N<sub>2</sub>O emissions / carbon change
|Separated manure solid |    |Optional |direct and indirect N<sub>2</sub>O emissions / carbon change
|Straw (long) |    |Optional |direct and indirect N<sub>2</sub>O emissions / carbon change
|Straw (chopped) |    |Optional |direct and indirect N<sub>2</sub>O emissions / carbon change
|Shavings |    |Optional |direct and indirect N<sub>2</sub>O emissions / carbon change
|Sawdust |    |Optional |direct and indirect N<sub>2</sub>O emissions / carbon change			
|Bedding application rate |kg head<sup>-1</sup>day<sup>-1</sup> |Optional |direct and indirect N<sub>2</sub>O emissions / carbon change
|Activity coefficient of feeding situation |Operational |    |    |	
|Maintenance coefficient |MJ day<sup>-1</sup> kg<sup>-1</sup> |Operational |    |	
</p> 
<br>

<body>
<p style = "color:green">
Bedding application calculator
<body> 

|Parameter      | Unit      | Importance         | Description                    |
|---------------|-----------|--------------------|--------------------------------|
|Total number of bales |    |Optional |direct and indirect N<sub>2</sub>O emissions / carbon change
|Average weight of bales | kg |Optional |direct and indirect N<sub>2</sub>O emissions / carbon change	
|Number of days |    |Optional |direct and indirect N<sub>2</sub>O emissions / carbon change	
|Number of animals |    |Optional |direct and indirect N<sub>2</sub>O emissions / carbon change	
|Bedding applied per day |kg |Calculated |direct and indirect N<sub>2</sub>O emissions / carbon change	
|Bedding application rate |kg head<sup>-1</sup>day<sup>-1</sup> |Calculated	|direct and indirect N<sub>2</sub>O emissions / carbon change
</p> 
<br>

<body>
<p style = "color:green">
Manure management
<body> 

|Parameter      | Unit      | Importance         | Description                    |
|---------------|-----------|--------------------|--------------------------------|
|<p style = "color:red">*Manure handling system:* 
|Pasture |    |Required | manure CH<sub>4</sub> / direct and indirect N<sub>2</sub>O emissions
|Daily spread |    |Required | manure CH<sub>4</sub> / direct and indirect N<sub>2</sub>O emissions
|Solid storage |    |Required | manure CH<sub>4</sub> / direct and indirect N<sub>2</sub>O emissions
|Compost passive |    |Required | manure CH<sub>4</sub> / direct and indirect N<sub>2</sub>O emissions
|Compost intensive |    |Required | manure CH<sub>4</sub> / direct and indirect N<sub>2</sub>O emissions
|Deep bedding |    |Required | manure CH<sub>4</sub> / direct and indirect N<sub>2</sub>O emissions
|Liquid with natural crust |    |Required | manure CH<sub>4</sub> / direct and indirect N<sub>2</sub>O emissions
|Liquid no crust |    |Required | manure CH<sub>4</sub> / direct and indirect N<sub>2</sub>O emissions
|Liquid with solid cover |    |Required | manure CH<sub>4</sub> / direct and indirect N<sub>2</sub>O emissions
| Anaerobic digester |    |Required | manure CH<sub>4</sub> / direct and indirect N<sub>2</sub>O emissions
|    |
|Ash content of manure |% |Optional |    |	
|Fraction of N in manure |% wet weight |Optional | direct and indirect N<sub>2</sub>O emissions
|Fraction of C in manure |% wet weight |Optional |manure CH<sub>4</sub> emissions
|Fraction of P in manure |% wet weight |Optional |    |	
|Leaching Fraction |kg N (kg N)<sup>-1</sup> |Operational |indirect N<sub>2</sub>O emissions
|Direct emission factor |kg N<sub>2</sub>O-N (kg N)<sup>-1</sup> |Operational |	direct N<sub>2</sub>O emissions
|Volatilization emission factor |kg N<sub>2</sub>O-N (kg N)<sup>-1</sup> |Operational |indirect N<sub>2</sub>O emissions
|Methane producing capacity of manure |m<sup>3</sup> CH<sub>4</sub> (kg VS)<sup>-1</sup> |Operational |manure CH<sub>4</sub> emissions
|Emission factor for leaching |kg N<sub>2</sub>O-N (kg N)<sup>-1</sup> |Operational |indirect N<sub>2</sub>O emissions
</p> 
<br>

# 5	Swine

<body>
<p style = "color:blue">
Step 1
<body>

|Parameter      | Unit      | Importance         | Description                    |
|---------------|-----------|--------------------|--------------------------------|
|<p style = "color:red">*Group name:* 
|Piglets |    |Required | manure CH<sub>4</sub> emissions
|Gilts |    |Required | manure CH<sub>4</sub> emissions
|Sows |    |Required | manure CH<sub>4</sub> emissions
|Boars |    |Required | manure CH<sub>4</sub> emissions
|Hogs |    |Required | manure CH<sub>4</sub> emissions			
</p> 
<br>

<body>
<p style = "color:blue">
Step 2
<body>

|Parameter      | Unit      | Importance         | Description                    |
|---------------|-----------|--------------------|--------------------------------|		
|No of mgmt. periods |    |Required |**Determined by adding mgmt. periods as needed**
|Start date |    |Required |    |	
|End date |    |Required	|    |
|Management period name |    |Optional |    |	
|No of days |    |Optional |    |	
</p> 
<br>

<body>
<p style = "color:blue">
Step 3
<body>		

<body>
<p style = "color:green">
General management
<body> 

|Parameter      | Unit      | Importance         | Description                    |
|---------------|-----------|--------------------|--------------------------------|
|No of animals |    |Required |enteric CH<sub>4</sub> / manure CH<sub>4</sub> emissions / direct and indirect N<sub>2</sub>O emissions
|<p style = "color:red">*Production stage:* 			
|Growing/finishing |    |Required |manure CH<sub>4</sub> emissions
|Weaning |    |Required |manure CH<sub>4</sub> emissions
|Gestating |    |Required |manure CH<sub>4</sub> emissions
|Lactating |    |Required |manure CH<sub>4</sub> emissions
|Growing/finishing |    |Required |manure CH<sub>4</sub> emissions
|Breeding stock |    |Required |manure CH<sub>4</sub> emissions
|Open (Not lactating or pregnant) |    |Required |manure CH<sub>4</sub> emissions
|        |   
|Start weight |kg |Required |    |	
|End weight |kg |Required |    |	
|Daily gain |kg day<sup>-1</sup> |Optional |Calculated using Start and End weight
|Yearly enteric methane rate |kg head<sup>-1</sup> year<sup>-1</sup> |Operational |	enteric CH<sub>4</sub>
</p> 
<br>

<body>
<p style = "color:green">
Diet management
<body> 

|Parameter      | Unit      | Importance         | Description                    |
|---------------|-----------|--------------------|--------------------------------|
|<p style = "color:red">*Diet:* 
|Gestation based diet |    |Required |manure CH<sub>4</sub> / direct and indirect N<sub>2</sub>O emissions
|Lactation based diet |    |Required |manure CH<sub>4</sub> / direct and indirect N<sub>2</sub>O emissions
|Nursery weaners (starter diet 1) |    |Required |manure CH<sub>4</sub> / direct and indirect N<sub>2</sub>O emissions
|Nursery weaners (starter diet 2) |    |Required |manure CH<sub>4</sub> / direct and indirect N<sub>2</sub>O emissions
|Grower finisher diet 1 |    |Required |manure CH<sub>4</sub> / direct and indirect N<sub>2</sub>O emissions
|Grower finisher diet 2 |    |Required |manure CH<sub>4</sub> / direct and indirect N<sub>2</sub>O emissions
|Grower finisher diet 3 |    |Required |manure CH<sub>4</sub> / direct and indirect N<sub>2</sub>O emissions
|Grower finisher diet 4 |    |Required |manure CH<sub>4</sub> / direct and indirect N<sub>2</sub>O emissions
|    |
|Feed intake |kg head<sup>-1</sup> year<sup>-1</sup> |Operational |manure CH<sub>4</sub> / direct and indirect N<sub>2</sub>O emissions
|Crude protein |kg kg<sup>-1</sup> |Operational |     |	 
|Volatile solid adjustment factor |kg kg<sup>-1</sup> |Operational |    |	
|Nitrogen excretion adjustment factor |kg kg<sup>-1</sup> |Operational |    |
</p> 
<br>

<body>
<p style = "color:green">
Custom diet creator
<body> 

|Parameter      | Unit      | Importance         | Description                    |
|---------------|-----------|--------------------|--------------------------------|
|Crude protein (CP) |% DM |Required |direct and indirect N<sub>2</sub>O emissions
</p> 
<br>

<body>
<p style = "color:green">
Manure management
<body> 

|Parameter      | Unit      | Importance         | Description                    |
|---------------|-----------|--------------------|--------------------------------|
|<p style = "color:red">*Manure handing system:* 
|Solid storage 
|Liquid with natural crust
|Liquid no crust     
|Liquid with solid cover
|Anaerobic digester
|Deep pit |    |Required |manure CH<sub>4</sub> / direct and indirect N<sub>2</sub>O emissions
|    |
|Ash content of manure |% |Optional |    |	
|Fraction of N in manure |% wet weight |Optional |direct and indirect N<sub>2</sub>O emissions
|Fraction of C in manure |% wet weight |Optional |manure CH<sub>4</sub> emissions
|Fraction of P in manure |% wet weight |Optional |    |	
|Leaching Fraction |kg N (kg N)<sup>-1</sup> |Operational |indirect N<sub>2</sub>O emissions
|Direct emission factor |kg N<sub>2</sub>O-N (kg N)<sup>-1</sup> |Operational |direct N<sub>2</sub>O emissions
|Volatilization emission factor |kg N<sub>2</sub>O-N (kg N)<sup>-1</sup> |Operational |indirect N<sub>2</sub>O emissions
|Methane producing capacity of manure |m<sup>3</sup> CH<sub>4</sub> (kg VS)<sup>-1</sup> |Operational |manure CH<sub>4</sub> emissions
|Emission factor for leaching |kg N<sub>2</sub>O-N (kg N)<sup>-1</sup> |Operational |indirect N<sub>2</sub>O emissions
</p> 
<br>

# 6	Sheep

<body>
<p style = "color:blue">
Step 1
<body>

|Parameter      | Unit      | Importance         | Description                    |
|---------------|-----------|--------------------|--------------------------------|
|<p style = "color:red">*Group name:* 
|Lambs |    |Required |enteric CH<sub>4</sub> emissions
|Ewes |    |Required |enteric CH<sub>4</sub> emissions
|Rams |    |Required |enteric CH<sub>4</sub> emissions
|  Sheep feedlot |    |Required |enteric CH<sub>4</sub> emissions
</p> 
<br>

<body>
<p style = "color:blue">
Step 2
<body>		

			
|Parameter      | Unit      | Importance         | Description                    |
|---------------|-----------|--------------------|--------------------------------|	
|No of mgmt. periods |    |Required |**Determined by adding mgmt. periods as needed**
|Start date |    |Required |    |	
|End date |    |Required	|    |
|Management period name |    |Optional |    |	
|No of days |    |Optional |    |	
</p> 
<br>

<body>
<p style = "color:blue">
Step 3
<body>	

<body>
<p style = "color:green">
General management
<body> 

|Parameter      | Unit      | Importance         | Description                    |
|---------------|-----------|--------------------|--------------------------------|
|No of animals |    |Required |enteric CH<sub>4</sub> / manure CH<sub>4</sub> emissions / direct and indirect N<sub>2</sub>O emissions
|Start weight |kg |Required |enteric CH<sub>4</sub> / manure CH<sub>4</sub> emissions / direct and indirect N<sub>2</sub>O emissions		
|End weight	|kg |Required |enteric CH<sub>4</sub> / manure CH<sub>4</sub> emissions / direct and indirect N<sub>2</sub>O emissions	
|Wool production |kg year<sup>-1</sup>	|Required |enteric CH<sub>4</sub> / manure CH<sub>4</sub> emissions / direct and indirect N<sub>2</sub>O emissions	
|Milk production |kg head<sup>-1</sup>	|Required |enteric CH<sub>4</sub> / manure CH<sub>4</sub> emissions / direct and indirect N<sub>2</sub>O emissions	
|Energy required for wool |MJ kg<sup>-1</sup> |Operational |enteric CH<sub>4</sub> / manure CH<sub>4</sub> emissions / direct and indirect N<sub>2</sub>O emissions		
|Energy required for milk |MJ kg<sup>-1</sup> |Operational |enteric CH<sub>4</sub> / manure CH<sub>4</sub> emissions / direct and indirect N<sub>2</sub>O emissions		
|Gain coefficient A |MJ kg<sup>-1</sup> |Operational |enteric CH<sub>4</sub> / manure CH<sub>4</sub> emissions / direct and indirect N<sub>2</sub>O emissions
|Gain coefficient B |MJ kg<sup>-2</sup> |Operational |enteric CH<sub>4</sub> / manure CH<sub>4</sub> emissions / direct and indirect N<sub>2</sub>O emissions	

</p> 
<br>

<body>
<p style = "color:green">
Diet management
<body> 

|Parameter      | Unit      | Importance         | Description                    |
|---------------|-----------|--------------------|--------------------------------|
|<p style = "color:red">*Diet:* 
|Good quality forage |    |Required |enteric CH<sub>4</sub> emissions / manure CH<sub>4</sub> / direct and indirect N<sub>2</sub>O emissions
|Average quality forage |    |Required |enteric CH<sub>4</sub> emissions / manure CH<sub>4</sub> / direct and indirect N<sub>2</sub>O emissions
|Medium quality forage |    |Required |enteric CH<sub>4</sub> emissions / manure CH<sub>4</sub> / direct and indirect N<sub>2</sub>O emissions
|    |
|Total digestible nutrient |%  |Required |Values cannot be edited here. Use the diet formulator.
|Crude protein |kg kg<sup>-1</sup> |Required |Values cannot be edited here. Use the diet formulator.		
|Methane conversion factor |kg CH<sub>4</sub> (kg CH4)<sup>-1</sup> |Required |Values cannot be edited here. Use the diet formulator.		
</p> 
<br>

<body>
<p style = "color:green">
Custom diet creator
<body> 

|Parameter      | Unit      | Importance         | Description                    |
|---------------|-----------|--------------------|--------------------------------|
|Crude protein (CP) |% DM |Required | enteric CH<sub>4</sub> emissions
|Total digestible nutrient (TDN) |% DM |Required | enteric CH<sub>4</sub> emissions
|Ash |%DM |Required |anaerobic digestion
|Methane conversion factor |kg CH<sub>4</sub> (kg CH<sub>4</sub>)<sup>-1</sup> |Operational |enteric CH<sub>4</sub> emissions
</p> 
<br>
<body>
<p style = "color:green">
Housing management
<body> 

|Parameter      | Unit      | Importance         | Description                    |
|---------------|-----------|--------------------|--------------------------------| 
|<p style = "color:red">*Housing type:* 
|Confined |   |Required |manure CH4 / direct and indirect N<sub>2</sub>O emissions
|Housed ewes |    |Required |manure CH4 / direct and indirect N<sub>2</sub>O emissions
|Pasture |   |Required |I**f pasture is selected, the field component can be selected on which the grazing takes place.**
|<p style = "color:red">*Bedding type:* 
|None |    |Optional |direct and indirect N<sub>2</sub>O emissions / carbon change
|Straw |    |Optional |direct and indirect N<sub>2</sub>O emissions / carbon change
|Wood chip |    |Optional |direct and indirect N<sub>2</sub>O emissions / carbon change
|Separated manure solid |    |Optional |direct and indirect N<sub>2</sub>O emissions / carbon change
|Sand |    |Optional |direct and indirect N<sub>2</sub>O emissions / carbon change
|Straw (long) |    |Optional |direct and indirect N<sub>2</sub>O emissions / carbon change
|Straw (chopped) |    |Optional |direct and indirect N<sub>2</sub>O emissions / carbon change
|Shavings |    |Optional |direct and indirect N<sub>2</sub>O emissions / carbon change
|Sawdust |    |Optional |direct and indirect N<sub>2</sub>O emissions / carbon change
|Bedding application rate |kg head<sup>-1</sup>day<sup>-1</sup> |Optional |direct and indirect N<sub>2</sub>O emissions / carbon change
|Activity coefficient of feeding situation |    |Operational |    |	
|Maintenance coefficient |MJ day<sup>-1</sup> kg<sup>-1</sup>| Operational |    |	
</p> 
<br>
<body>
<p style = "color:green">
Bedding application calculator
<body> 

|Parameter      | Unit      | Importance         | Description                    |
|---------------|-----------|--------------------|--------------------------------|
|Total number of bales |   |Optional |carbon change
|Average weight of bales |    |Optional |carbon change
|Number of days |   |Optional |    |	
|Number of animals |    |Optional |enteric and manure CH<sub>4</sub> emissions
|Bedding applied per day |kg |Calculated |direct and indirect N<sub>2</sub>O emissions / carbon change
|Bedding application rate |kg head<sup>-1</sup>day<sup>-1</sup> |Calculated |direct and indirect N<sub>2</sub>O emissions / carbon change
</p> 
<br>

<body>
<p style = "color:green">
Manure management
<body> 

|Parameter      | Unit      | Importance         | Description                    |
|---------------|-----------|--------------------|--------------------------------|
|<p style = "color:red">*Manure handing system:* 
|Pasture |    |Required |manure CH<sub>4</sub> / direct and indirect N<sub>2</sub>O emissions
|Solid storage |    |Required |manure CH<sub>4</sub> / direct and indirect N<sub>2</sub>O emissions 
|Compost passive |    |Required |manure CH<sub>4</sub> / direct and indirect N<sub>2</sub>O emissions
|Compost intensive |    |Required |manure CH<sub>4</sub> / direct and indirect N<sub>2</sub>O emissions
|Deep bedding |    |Required |manure CH<sub>4</sub> / direct and indirect N<sub>2</sub>O emissions
|Anaerobic digester |    |Required |manure CH<sub>4</sub> / direct and indirect N<sub>2</sub>O emissions
|    |
|Ash content of manure |% |Optional |    |   	
|Fraction of N in manure |% wet weight |Optional | 	direct and indirect N<sub>2</sub>O emissions
|Fraction of C in manure |% wet weight |Optional |manure CH<sub>4</sub> emissions
|Fraction of P in manure |% wet weight |Optional |    |	
|Leaching Fraction |kg N (kg N)<sup>-1</sup> |Operational |indirect N<sub>2</sub>O emissions
|Direct emission factor |kg N<sub>2</sub>O-N (kg N)<sup>-1</sup> |Operational |direct N<sub>2</sub>O emissions
|Volatilization emission factor |kg N<sub>2</sub>O-N (kg N)<sup>-1</sup> |Operational |indirect N<sub>2</sub>O emissions
|Methane producing capacity of manure |m<sup>3</sup> CH<sub>4</sub> (kg VS)<sup>-1</sup> |Operational |manure CH<sub>4</sub> emissions
|Emission factor for leaching |kg N<sub>2</sub>O-N (kg N)<sup>-1</sup> |Operational |indirect N<sub>2</sub>O emissions

# 7	Poultry

<body>
<p style = "color:blue">
Step 1
<body>

|Parameter      | Unit      | Importance         | Description                    |
|---------------|-----------|--------------------|--------------------------------|
|<p style = "color:red">*Group name:* 
|Pullets |    |Required |manure CH<sub>4</sub> emissions
|Cockerels |    |Required |manure CH<sub>4</sub> emissions
|Chicken eggs |    |Required |manure CH<sub>4</sub> emissions
|Chicks |    |Required |manure CH<sub>4</sub> emissions
|Turkey eggs |    |Required |manure CH<sub>4</sub> emissions
|Poults |    |Required |manure CH<sub>4</sub> emissions
|Hens |    |Required |manure CH<sub>4</sub> emissions
|Roosters |    |Required |manure CH<sub>4</sub> emissions
|Young toms |    |Required |manure CH<sub>4</sub> emissions
|Young turkey hens |    |Required |manure CH<sub>4</sub> emissions
|Toms |    |Required |manure CH<sub>4</sub> emissions
|Turkey hens |    |Required |manure CH<sub>4</sub> emissions
</p> 
<br>

<body>
<p style = "color:blue">
Step 2
<body>		

|Parameter      | Unit      | Importance         | Description                    |
|---------------|-----------|--------------------|--------------------------------|	
|No of mgmt. periods |    |Required |**Determined by adding mgmt. periods as needed**
|Start date |    |Required |    |	
|End date |    |Required	|    |
|Management period name |    |Optional |    |	
|No of days |    |Optional |    |	
</p> 
<br>

<body>
<p style = "color:blue">
Step 3
<body>	

<body>
<p style = "color:green">
General management
<body> 	

|Parameter      | Unit      | Importance         | Description                    |
|---------------|-----------|--------------------|--------------------------------|	
|No of animals |    |Required |manure CH<sub>4</sub> emissions / direct and indirect N<sub>2</sub>O emissions
</p> 
<br>

<body>
<p style = "color:green">
Housing management
<body> 

|Parameter      | Unit      | Importance         | Description                    |
|---------------|-----------|--------------------|--------------------------------|
|<p style = "color:red">*Housing type:* 
|Housed in barn |    |Required |manure CH<sub>4</sub> emissions / direct and indirect N<sub>2</sub>O emissions
|<p style = "color:red">*Bedding type:* 
|None |    |Optional |direct and indirect N<sub>2</sub>O emissions / carbon change
|Straw |    |Optional |direct and indirect N<sub>2</sub>O emissions / carbon change
|Wood chip |    |Optional |direct and indirect N<sub>2</sub>O emissions / carbon change
|Separated manure solid |    |Optional |direct and indirect N<sub>2</sub>O emissions / carbon change
|Sand |    |Optional |direct and indirect N<sub>2</sub>O emissions / carbon change
|Straw (long) |    |Optional |direct and indirect N<sub>2</sub>O emissions / carbon change
|Straw (chopped) |    |Optional |direct and indirect N<sub>2</sub>O emissions / carbon change
|Shavings |    |Optional |direct and indirect N<sub>2</sub>O emissions / carbon change
|Sawdust |    |Optional |direct and indirect N<sub>2</sub>O emissions / carbon change
|Bedding application rate |kg head<sup>-1</sup>day<sup>-1</sup> |Optional |direct and indirect N<sub>2</sub>O emissions / carbon change
|Bedding application calculator |    |Optional |    |	
|Total number of bales |    |Optional |	direct and indirect N<sub>2</sub>O emissions / carbon change
|Average weight of bales |kg |Optional |    |	
|Number of days |   |Optional |    |	
|Number of animals |    |Optional |    |	
|Bedding applied per day |kg |Calculated |   |	
|Bedding application rate |kg head<sup>-1</sup>day<sup>-1</sup> |Calculated	|    |
</p> 
<br>

<body>
<p style = "color:green">
Manure management
<body> 

|Parameter      | Unit      | Importance         | Description                    |
|---------------|-----------|--------------------|--------------------------------|
|<p style = "color:red">*Manure handing system:* 
|Solid storage |    |Required |manure CH<sub>4</sub> / direct and indirect N<sub>2</sub>O emissions 
|Anaerobic digester |    |Required |manure CH<sub>4</sub> / direct and indirect N<sub>2</sub>O emissions 
|    |
|Fraction of N in manure |% wet weight |Optional | 	direct and indirect N<sub>2</sub>O emissions
|Fraction of C in manure |% wet weight |Optional |manure CH<sub>4</sub> emissions
|Fraction of P in manure |% wet weight |Optional |    |	
|Leaching Fraction |kg N (kg N)<sup>-1</sup> |Operational |indirect N<sub>2</sub>O emissions 
|Volatilization fraction |kg NH<sub>3</sub>-N (kg N)<sup>-1</sup> |Operational |indirect N<sub>2</sub>O emissions
|Yearly enteric methane rate |kg head<sup>-1</sup> year<sup>-1</sup> |Operational |enteric CH<sub>4</sub> emissions
|Direct emission factor |kg N<sub>2</sub>O-N (kg N)<sup>-1</sup> |Operational |direct N<sub>2</sub>O emissions
|Volatilization emission factor |kg N<sub>2</sub>O-N (kg N)<sup>-1</sup> |Operational |indirect N<sub>2</sub>O emissions
|Methane producing capacity of manure |m<sup>3</sup> CH<sub>4</sub> (kg VS)<sup>-1</sup> |Operational |manure CH<sub>4</sub> emissions
|Emission factor for leaching |kg N<sub>2</sub>O-N (kg N)<sup>-1</sup> |Operational |indirect N<sub>2</sub>O emissions
</p> 
<br>

# 8	Other livestock

<body>
<p style = "color:blue">
Step 1
<body>

|Parameter      | Unit      | Importance         | Description                    |
|---------------|-----------|--------------------|--------------------------------|
|<p style = "color:red">*Group name:* 
|Goats |    |Required | Impacts on enteric CH<sub>4</sub> emissions
|Deer |    |Required | Impacts on enteric CH<sub>4</sub> emissions
|Horses |    |Required | Impacts on enteric CH<sub>4</sub> emissions
|Mules |    |Required | Impacts on enteric CH<sub>4</sub> emissions
|Bison |    |Required | Impacts on enteric CH<sub>4</sub> emissions
|Llamas |    |Required | Impacts on enteric CH<sub>4</sub> emissions
</p> 
<br>

<body>
<p style = "color:blue">
Step 2
<body>

|Parameter      | Unit      | Importance         | Description                    |
|---------------|-----------|--------------------|--------------------------------|	
|No of mgmt. periods |    |Required |**Determined by adding mgmt. periods as needed**
|Start date |    |Required |    |	
|End date |    |Required	|    |
|Management period name |    |Optional |    |	
|No of days |    |Optional |    |	
</p> 
<br>

<body>
<p style = "color:blue">
Step 3
<body>	
			
<body>
<p style = "color:green">
General management
<body> 	

|Parameter      | Unit      | Importance         | Description                    |
|---------------|-----------|--------------------|--------------------------------|	
|No of animals |    |Required |enteric CH<sub>4</sub> emissions 
</p> 
<br>	

<body>
<p style = "color:green">
Manure management
<body> 

|Parameter      | Unit      | Importance         | Description                    |
|---------------|-----------|--------------------|--------------------------------|
|<p style = "color:red">*Manure handing system:* 
|Pasture |    |Required |manure CH<sub>4</sub> / direct and indirect N<sub>2</sub>O emissions
|Solid storage |    |Required |manure CH<sub>4</sub> / direct and indirect N<sub>2</sub>O emissions 
|    |
|Fraction of N in manure |% wet weight |Optional | 	direct and indirect N<sub>2</sub>O emissions
|Fraction of C in manure |% wet weight |Optional |manure CH<sub>4</sub> emissions
|Fraction of P in manure |% wet weight |Optional |    |	
|Leaching Fraction |kg N (kg N)<sup>-1</sup> |Operational |indirect N<sub>2</sub>O emissions	
|Yearly enteric methane rate |kg head<sup>-1</sup> year<sup>-1</sup> |Operational |enteric CH<sub>4</sub> emissions
|Direct emission factor |kg N<sub>2</sub>O-N (kg N)<sup>-1</sup> |Operational |direct N<sub>2</sub>O emissions
|Volatilization emission factor |kg N<sub>2</sub>O-N (kg N)<sup>-1</sup> |Operational |indirect N<sub>2</sub>O emissions
|Methane producing capacity of manure |m<sup>3</sup> CH<sub>4</sub> (kg VS)<sup>-1</sup> |Operational |manure CH<sub>4</sub> emissions
|Emission factor for leaching |kg N<sub>2</sub>O-N (kg N)<sup>-1</sup> |Operational |indirect N<sub>2</sub>O emissions
</p> 
<br>

# 9	Infrastructure

## 9.1 Anaerobic digestion

<body>
<p style = "color:green">
Manure substrate type
<body>

|Parameter      | Unit      | Importance         | Description                    |
|---------------|-----------|--------------------|--------------------------------|
|Manure type |    |Required |manure CH<sub>4</sub> / direct and indirect N<sub>2</sub>O emissions
|Bedding material type |    |Required |manure CH<sub>4</sub> / direct and indirect N<sub>2</sub>O emissions	
|Daily manure added to digester |kg |Required |manure CH<sub>4</sub> / direct and indirect N<sub>2</sub>O emissions	
|Total nitrogen |kg N t<sup>-1</sup> |Optional |    |
|Biomethane potential |Nm<sup>3</sup> t<sup>-1</sup> VS<sup>-1</sup> |Operational |    |	
|Methane fraction in biogas |    |Operational |    |	
|Total solids |(kg t<sup>-1</sup>)<sup>3</sup> |Operational |    |	
|Volatile solids |% |Operational |    |
</p> 
<br>

<body>
<p style = "color:green">
Farm residue substrate type
<body>

|Parameter      | Unit      | Importance         | Description                    |
|---------------|-----------|--------------------|--------------------------------|
|Total nitrogen |kg N t<sup>-1</sup> |Optional |    |	
|Biomethane potential |Nm<sup>3</sup> t<sup>-1</sup> VS<sup>-1</sup> |Operational |manure CH<sub>4</sub> / direct and indirect N<sub>2</sub>O emissions
|Methane fraction in biogas |    |Operational |manure CH<sub>4</sub> / direct and indirect N<sub>2</sub>O emissions	
|Total solids |	(kg t<sup>-1</sup>)<sup>3</sup> |Operational |manure CH<sub>4</sub> / direct and indirect N<sub>2</sub>O emissions
|Volatile solids |% of TS |Operational |manure CH<sub>4</sub> / direct and indirect N<sub>2</sub>O emissions
