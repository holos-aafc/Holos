<p align="center">
<img src="../../Images/logo.png" alt="Holos Logo" width="550" />
<br>
</p>

# Frequently Asked Questions

The purpose of this document is to allow for users to find answers to their questions in one place. 

<br>

# General FAQ

#### 1. I do not know how to use the Holos program.
The training guide is available. Following step by steps would help you to learn how to use the program. The links are below:

<a href="https://github.com/holos-aafc/Holos/blob/main/H.Content/Documentation/Training/Holos_4_Training_Guide.md">Training Guide [ENG]</a>

<a href="https://github.com/holos-aafc/Holos/blob/main/H.Content/Documentation/Training/Holos_4_Training_Guide-fr.md">Training Guide [FR]</a>

<br>

### 2. Are there any videos I can watch?
Yes, Holos has a Youtube channel that offers tutorials to new users.

Click the link:&nbsp;&nbsp;<a href="https://www.youtube.com/channel/UCHDORmZ73VICHzqm_yVpM_Q">Tutorial Videos</a>

<br>

### 3. How can I participate in the Holos discussion forum?
Holos has a discussion board that users can leave feedback and ask questions. To begin using the discussion forum, a GitHub account needs to be created. After signing up for the GitHub account, you can start to chat in the forum. There are step by step guides for how to sign up for a GitHub account and how to create a simple post in the discussion forum:        

<a href="https://github.com/holos-aafc/Holos/discussions">Holos Discussion Forum</a>

<a href="https://github.com/holos-aafc/Holos/blob/main/H.Content/Documentation/GitHub%20Guide/GitHub%20Guide.md#creating-an-account">How to sign up for a Github account</a>

<a href="https://github.com/holos-aafc/Holos/blob/main/H.Content/Documentation/GitHub%20Guide/GitHub%20Guide.md#how-to-write-a-post-in-the-discussion-forum">How to create a simple post in the discussion forum</a>

<br>

### 4. How do I add/edit this FAQ?
Users can add and edit the FAQ. Note that this page uses markdown. To add and edit the FAQ, two steps are required: 1. Sign up for a GitHub account 2. Pull request to Holos repository. Once the admin of Holos repository appoves your pull request, the changes you made would apply to the FAQ page.

<a href="https://github.com/holos-aafc/Holos/blob/main/H.Content/Documentation/GitHub%20Guide/GitHub%20Guide.md#creating-an-account">How to sign up for a Github account</a>

<a href="https://github.com/holos-aafc/Holos/blob/main/H.Content/Documentation/GitHub%20Guide/GitHub%20Guide.md#contributing-changes-to-the-original-repository">How to make a pull request to Holos repository</a>

---

# Cropping FAQ

### 1. Where is my crop?
We do not have all crops.

---

# Livestock FAQ

### 1.	I cannot find options to input my grazing systems, how do I represent grazing system X in Holos?
Currently Holos does not offer the option to simulate different grazing systems, as there still is a lack of scientific clarity on what the exact effects of such systems are. Furthermore, there is also some confusion related to terminology. Our team is involved in several projects that attempt to provide more clarity and future updates to the model are intended to provide appropriate options. In the meantime, using Holos, the model user can place animal groups on specific pasture fields. In this way, multiple fields could be created to represent different paddocks for a rotational grazing system, with the management history for each field detailed. However, for grazed fields/paddocks, Holos estimates the aboveground biomass productivity based on the animal vegetation biomass consumption in combination with estimates of biomass utilization (grazing efficiency), thus more efficient grazing systems could be represented using a single pasture, rather than subdividing it into parcels. 
>*Please note: more intensive grazing systems have been shown to improve the feed quality, but this must be specified for each animal group grazing on a specific pasture and for each relevant management period, using the Diet tab for that animal group/management period, e.g., by creating a custom pasture diet using the ‘Custom Diet Creator’ tool.*

<br>

### 2.	I want to compare livestock management options, how can I do that in Holos?
There are three options to do this:
	* Set up two different farms
	* Set up two livestock components within a single farm
	* Set up two livestock groups within a single livestock component on a single farm
Each of these allows the model user to compare model outputs for the different management options. 
>*Please note: if the model user sets up two (or more) different farms, they can compare the model outputs for these farms by selecting ‘Yes’ for ‘Compare Multiple Farms’ on the Results screen and selecting the farms they wish to compare from the list available.*

<br>

### 3. I want to know what the carbon footprint of my livestock system is, what do I need to do?
The Holos model is set up to calculate a farm’s greenhouse gas (GHG) budget, meaning it accounts for all farm-based sources of GHG that we can estimate based on available information and data . To calculate the carbon footprint of a product, we need to account for all emissions generated as a result of the production of this product. For a livestock system, that means accounting for the feed production, whether that feed is grown on the actual farm or not. Before adding feed-producing fields to the simulated farm, the user must first calculate the area of each pasture or crop field required to sustain the animals on the farm – Holos will generate a warning message if not enough feed is being “grown” to satisfy animal requirements, as an internal check. Emissions generated as a result of inputs to the feed production system (e.g., fertilizer and pesticide production) are also accounted for. In Holos, upstream emissions for these farm inputs are also reported, i.e., CO2 generated from the upstream production of synthetic For the livestock system itself, emissions related to the breeding stock must be included in the calculations, as well as those relating to their progeny. Holos then outputs all of the emissions for this system up until the farm gate – any emissions related to transport, processing, etc. will need to be estimated by the user outside of Holos and added to the Holos outputs, if so desired.
>*Please note: it is up to the user to allocate the emissions according to the product, e.g., in a beef production system the outputs could easily be broken down to CO2eq per animal carcass, but for a CO2eq per kg meat it needs to be decided whether all emissions are assigned to the meat part of the carcass, or whether a portion of the emissions are allocated to the different parts of the carcass (Consulting an LCA expert is advised.)*

<br>

### 4. I want to add an ingredient to my custom diet that is not in the ingredient list, how can I do this?
Using the Custom Diet Creator, the model user can create new feed ingredients, that can then be added to a custom diet. Open the Custom Diet Creator on the Diet tab and under Step 2, click on ‘Create Custom Ingredient’. A new row should appear at the top of the ingredient list – you can click on the ingredient name to change it. To define this ingredient, you will need to enter the relevant data in the rest of the row. 
>*Please note: not all data columns in this table are necessary for the Holos calculations and the data required vary depending on the animal group under consideration.* 

At a minimum, the following data are required for different animal groups :

 - for ***beef and dairy cattle***: DM (Dry matter content of ingredient (as fed), % AF), Forage (% DM – this value will be either 0 (if the custom ingredient is not a forage ingredient) or 100 (if it is a forage ingredient)), CP (Crude protein content, % of DM), TDN (Total digestible nutrient, % of DM), Starch (Starch concentration in the ingredient, % of DM), Ash (Ash content of feed, % of DM), NEma (Net energy for maintenance, Mcal kg-1), NEga (Net energy for growth, Mcal kg-1) – these last two parameters are needed only for the estimation of methane emissions for calves not fed on a milk diet;- for ***sheep, swine, poultry and other livestock***: DM (% AF), Forage (% DM), CP (% DM), TDN (% DM), Ash (% DM).

Once you are finished, click ‘OK’ and your changes will be saved automatically.

<br>

### 5. Where can I see the full details of the default diets built into Holos?
Some of the data for the selected diet is visible when you select ‘Show Additional Information’ on the Diet tab, however you can see the full details for this diet if you open the Custom Diet Creator. Once open, select ‘Yes’ for ‘Show Default Diets’ under Step 1 – you will now see data related to the nutritional content of each default diet available for the relevant livestock type in this section, as well as data related to the percentage of the total dietary DM that is composed of the different diet ingredients (under Step 3). 

---

To download Holos, for more information, or to access a recent list of Holos related publications, visit:
https://agriculture.canada.ca/en/agricultural-science-and-innovation/agricultural-research-results/holos-software-program

To contact us, email:
aafc.holos.acc@canada.ca