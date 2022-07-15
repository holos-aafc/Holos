# Introduction
We have set out a template that you can follow to fill out missing data. As long as you have a valid source, and you follow this guide, you should have no issues filling out the blank template!

Steps:
1) Figure out what data we need from the `Data\Crops\Holos.Grid.csv` file
2) Fill out the template with the associated sources
3) Create a Pull Request for to the Holos.Grid using your filled in template
4) Submit your sources to the `Data\Crops\Sources.csv`

# Structure
The Holos Model has 8 different crop groups as detailed on the table below.

| Group               |
| ------------------- |
| Small-grain cereals |
| Silage              |
| Oilseeds            |
| Pulse crops         |
| Root crops          |
| Other field crops   |
| Cover crops         |
| Perennial forages   |

From these 8 groups, we have 69 crops that we need to fill out, as seen in the following table.

| Group               | Crop ID | Crop                                                                |
|---------------------|---------|---------------------------------------------------------------------|
| Small-grain cereals | 1       | Summer fallow                                                       |
| Small-grain cereals | 2       | Small grain cereals                                                 |
| Small-grain cereals | 3       | Wheat                                                               |
| Small-grain cereals | 4       | Durum Wheat                                                         |
| Small-grain cereals | 5       | Barley                                                              |
| Small-grain cereals | 6       | Undersown Barley                                                    |
| Small-grain cereals | 7       | Oats                                                                |
| Small-grain cereals | 8       | Triticale                                                           |
| Small-grain cereals | 9       | Sorghum                                                             |
| Small-grain cereals | 10      | Canary seed                                                         |
| Small-grain cereals | 11      | Buckwheat                                                           |
| Small-grain cereals | 12      | Grain corn                                                          |
| Small-grain cereals | 13      | Mixed grains                                                        |
| Silage              | 14      | Corn Silage                                                         |
| Silage              | 15      | Barley Silage                                                       |
| Silage              | 16      | Oat Silage                                                          |
| Silage              | 17      | Triticale Silage                                                    |
| Silage              | 18      | Wheat Silage                                                        |
| Oilseeds            | 19      | Canola                                                              |
| Oilseeds            | 20      | Mustard                                                             |
| Oilseeds            | 21      | Flax                                                                |
| Pulse crops         | 22      | Soybean                                                             |
| Pulse crops         | 23      | Beans (dry field)                                                   |
| Pulse crops         | 24      | Whitebeans                                                          |
| Pulse crops         | 25      | Fababean                                                            |
| Pulse crops         | 26      | Chickpea                                                            |
| Pulse crops         | 27      | Dry Pea                                                             |
| Pulse crops         | 28      | Field Pea                                                           |
| Pulse crops         | 29      | Lentil                                                              |
| Root crops          | 30      | Potato                                                              |
| Root crops          | 31      | Sugar Beets                                                         |
| Other field crops   | 32      | Safflower                                                           |
| Other field crops   | 33      | Sunflower seed                                                      |
| Other field crops   | 34      | Tobacco                                                             |
| Other field crops   | 35      | Vegetables                                                          |
| Other field crops   | 36      | Berries & grapes                                                    |
| Cover crops         | 37      | Winter weeds                                                        |
| Cover crops         | 38      | Red clover (Trifolium pratense L.)                                  |
| Cover crops         | 39      | Berseem clover (Trifolium alexandrium L.)                           |
| Cover crops         | 40      | Sweet clover (Melilotus officinalis)                                |
| Cover crops         | 41      | Crimson clover (Trifolium incarnatum)                               |
| Cover crops         | 42      | Hairy Vetch (Vicia villosa roth)                                    |
| Cover crops         | 43      | Alfalfa (Medicago sativa L.)                                        |
| Cover crops         | 44      | Faba bean/broad bean (Vicia faba)                                   |
| Cover crops         | 45      | Cowpea (Vigna unguiculata)                                          |
| Cover crops         | 46      | Austrian winter pea                                                 |
| Cover crops         | 47      | Rapeseed (Brassica Napus L.)                                        |
| Cover crops         | 48      | Winter turnip rape [Brassica Rapa spp. oleifera L. (cv.   "Largo")] |
| Cover crops         | 49      | Phacelia [Phacelia tanacetifolia (cv. 'Phaci')]                     |
| Cover crops         | 50      | Forage radish (Raphanus sativus L.)                                 |
| Cover crops         | 51      | Mustard (Sinapus alba L. subsp. Mairei (H. Lindb.) Maire)           |
| Cover crops         | 52      | Barley (Hordeum vulgare)                                            |
| Cover crops         | 53      | Oat (Avena sativa)                                                  |
| Cover crops         | 54      | Rye (Secale cereale) / Winter rye / Cereal rye                      |
| Cover crops         | 55      | Sesame (Sesamum indicum)                                            |
| Cover crops         | 56      | Flax (Linum usitatissimum)                                          |
| Cover crops         | 57      | Ryegrass (Lolium Perenne L.)                                        |
| Cover crops         | 58      | Annual Ryegrass (Lolium multiflorum)                                |
| Cover crops         | 59      | Sorghum (Sorghum bicolour)                                          |
| Cover crops         | 60      | Pigeon Bean                                                         |
| Cover crops         | 61      | Shepherd's purse                                                    |
| Cover crops         | 62      | Winter wheat (Triticum aestivum)                                    |
| Cover crops         | 63      | (Fall) Rye                                                          |
| Perennial forages   | 64      | Rangeland (native)                                                  |
| Perennial forages   | 65      | Seeded grassland                                                    |
| Perennial forages   | 66      | Tame grass                                                          |
| Perennial forages   | 67      | Tame legume                                                         |
| Perennial forages   | 68      | Tame mixed (grass/legume)                                           |
| Perennial forages   | 69      | Forage for seed                                                     |

These values need to be filled out for every province and for every soil type, we will show you how to deal with different soil types in the next section. Furthermore, there may be some crops that are close to the names we have below but not quite, we will also address this in the next section.   
This last table  shows all the variables we need to fill in. If you need some help figuring out how to map the values from the crop production guide to the holos variables, please refer to this document. [Holos Mapping](https://github.com/Espartaco-Gonzalez-Arteaga/Holos_economic_data/blob/b72939c38d0cbaa0d224ada4f61a69d6294a3aa7/H.Economic.Data/Templates/Crops/Holos.Crops.Mapping.md)

| Variable               | Definition                                                                                |
| ---------------------- | ----------------------------------------------------------------------------------------- |
|    ID                |     Automatically generated index [leave blank]                                                                  |
|    Year                  |     The year of the source                                         |
| Province               | Province of the source                                                                    |
| Soil Type              |                                                                                           |
| Group                  |                                                                                           |
| Crop ID                |                                                                                           |
| Crop                   |                                                                                           |
| Variant / Species      | Write about the crop's special characteristics if applicable.                             |
| Unit (bu, t, etc)      | what unit is the measurement given?                                                       |
| Yield                  |                                                                                           |
| Crop Price             |                                                                                           |
| Seed                   | Seed, Innoculants, and other Treatments                                                   |
| Fertilizer             |                                                                                           |
| Chemical               | Herbicide, Pesticide, and all other chemicals added                                       |
| Trucking and Marketing | trucking fees, and marketing fees, as well as any related crop fees, like membership fees |
| Fuel Oil Lube          | Allocation for fuel, oil, lube and other machine-related consumables                      |
| Machinery Repairs      |                                                                                           |
| Crop Insurance         | Hail, Crop Production, and all other insurance directly applicable to the   crop.         |
| Building Repairs       |                                                                                           |
| Custom Work            | Any crop specific extra work applied, like drying, twining, pruning   etc...              |
| Labour                 |                                                                                           |
| Utilities              | Utilities and Miscellanous                                                                |
| Operating Interest     | Interest on operating loans, short term loans for purchase of inputs                      |
| Fixed Cost $/Acre      |                                                                                           |
| Variable Cost $/acre   |                                                                                           |
| Total Costs $/Acre     |                                                                                           |
| Source                 | Source of your document                                                                   |

# Examples and Instructions

This is an example of what a filled out template should look like. Some variables have been ommited for simplicity.

> Missing Data & Duplicates

Take note on the first three rows, you can see that even though there is no data for small grain cereals, we still keep them. If a value is not avaliable, instead of filling it with a '0' just leave it blank.  

| ID | Year | Province | Soil Type | Group               | Crop ID | Crop                | Variant / Species     | unit (bu, t, etc) | Yield | Crop Price | ... |
|----|------|----------|-----------|---------------------|---------|---------------------|-----------------------|-------------------|-------|------------|-----|
| 1  | 2021 | Manitoba | Black     | Small-grain cereals | 1       | Summer fallow       |                       |                   |       |            | ... |
| 2  | 2021 | Manitoba | Black     | Small-grain cereals | 2       | Small grain cereals |                       |                   |       |            | ... |
| 3  | 2021 | Manitoba | Black     | Small-grain cereals | **3**   | Wheat               | **Hard Red Spring**   | bu                | 61    | 6.75       | ... |
| 4  | 2021 | Manitoba | Black     | Small-grain cereals | **3**   | Wheat               | **Northern Hard Red** | bu                | 70    | 6.25       | ... |


Look at the bottom two rows, see how they seem like a duplicate of the Black Soil wheat? Look at the `Variant \ Species`, you will notice that one is Hard Red Spring Wheat, and the other is Northern Hard Red Wheat.... This is the suggested way to fill out the template, please remember to duplicate the `Crop ID` as well as all the other variables to the left of the `Crop`

> Multiple Soil Types

What to do when you have more that one soil type? Start with one Soil Type, then fill out another template right below your previous template

| Year | Province | Soil Type | Group               | Crop ID | Crop                      |
|------|----------|-----------|---------------------|---------|---------------------------|
| 2020 | Alberta  | Black     | Perennial forages   | 73      | Tame mixed (grass/legume) |
| 2020 | Alberta  | Black     | Perennial forages   | 74      | Forage for seed           |
| 2020 | Alberta  | Brown     | Small-grain cereals | 1       | Summer fallow             |
| 2020 | Alberta  | Brown     | Small-grain cereals | 2       | Small grain cereals       |

> Validation and Ordering
>  *This step is optional but should help you make sure you are doing it right*

Once you finish, and Sort By `Crop ID`, the final product should look like this!  Once all the Crops and Soil Types have been filled, and they have been sorted by `Crop ID`, now we can automatically add an `ID` like so:

| ID | Year | Province | Soil Type   | Group               | Crop ID | Crop          |
|----|------|----------|-------------|---------------------|---------|---------------|
| 1  | 2020 | Alberta  | Black       | Small-grain cereals | 1       | Summer fallow |
| 2  | 2020 | Alberta  | Brown       | Small-grain cereals | 1       | Summer fallow |
| 3  | 2020 | Alberta  | Dark Brown  | Small-grain cereals | 1       | Summer fallow |
| 4  | 2020 | Alberta  | Grey-Wooded | Small-grain cereals | 1       | Summer fallow |
| 5  | 2020 | Alberta  | Irrigated   | Small-grain cereals | 1       | Summer fallow |

Lastly, this is the expected input data types for each of the variables.

| Variable               | Expected Data Type |
|------------------------|--------------------|
| ID                     | Integer            |
| Year                   | Integer            |
| Province               | Character/String   |
| Soil Type              | Character/String   |
| Group                  | Character/String   |
| Crop ID                | Integer            |
| Crop                   | Character/String   |
| Variant / Species      | Character/String   |
| Unit (bu, t, etc)      | Character/String   |
| Yield                  | Numeric/Float      |
| Crop Price             | Numeric/Float      |
| Seed                   | Numeric/Float      |
| Fertilizer             | Numeric/Float      |
| Chemical               | Numeric/Float      |
| Trucking and Marketing | Numeric/Float      |
| Fuel Oil Lube          | Numeric/Float      |
| Machinery Repairs      | Numeric/Float      |
| Crop Insurance         | Numeric/Float      |
| Building Repairs       | Numeric/Float      |
| Custom Work            | Numeric/Float      |
| Labour                 | Numeric/Float      |
| Utilities              | Numeric/Float      |
| Operating Interest     | Numeric/Float      |
| Fixed Cost $/Acre      | Numeric/Float      |
| Variable Cost $/acre   | Numeric/Float      |
| Total Costs $/Acre     | Numeric/Float      |
| Source                 | Character/String   |
