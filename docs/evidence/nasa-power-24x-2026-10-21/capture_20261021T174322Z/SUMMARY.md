# Capture 20261021T174322Z

Point: latitude 53.9125034103343, longitude -114.08203125

## A_short_clean
Short request, the #499 original: 10 days of January 2020.  
`https://power.larc.nasa.gov/api/temporal/daily/point?parameters=PRECTOTCORR&community=AG&longitude=-114.08203125&latitude=53.9125034103343&start=20200101&end=20200110&format=JSON`  
HTTP 200; units `mm/day`; sources `['MERRA2']`; messages `[]`; 646 bytes

## B_long_to_today
1981 to today, PRECTOTCORR only. Same days as A.  
`https://power.larc.nasa.gov/api/temporal/daily/point?parameters=PRECTOTCORR&community=AG&longitude=-114.08203125&latitude=53.9125034103343&start=19810101&end=20261021&format=JSON`  
HTTP 200; units `mm/day`; sources `['GEOSIT', 'MERRA2']`; messages `[]`; 272,304 bytes

## C_holos_request
The request Holos makes: Holos's six parameters, 1981 to today.  
`https://power.larc.nasa.gov/api/temporal/daily/point?parameters=T2M,PRECTOTCORR,RH2M,ALLSKY_SFC_SW_DWN,T2M_MIN,T2M_MAX&community=AG&longitude=-114.08203125&latitude=53.9125034103343&start=19810101&end=20261021&format=JSON`  
HTTP 200; units `mm/day`; sources `['SYN1DEG', 'FLASHFLUX', 'SRB', 'GEOSIT', 'MERRA2', 'POWER']`; messages `[]`; 1,675,082 bytes

## D_boundary_end_20261002
2020 to 2026-10-02 (clean when first checked on 2026-10-21).  
`https://power.larc.nasa.gov/api/temporal/daily/point?parameters=PRECTOTCORR&community=AG&longitude=-114.08203125&latitude=53.9125034103343&start=20200101&end=20261002&format=JSON`  
HTTP 200; units `mm/day`; sources `['GEOSIT', 'MERRA2']`; messages `[]`; 40,579 bytes

## E_boundary_end_20261003
2020 to 2026-10-03 (inflated when first checked on 2026-10-21).  
`https://power.larc.nasa.gov/api/temporal/daily/point?parameters=PRECTOTCORR&community=AG&longitude=-114.08203125&latitude=53.9125034103343&start=20200101&end=20261003&format=JSON`  
HTTP 200; units `mm/day`; sources `['GEOSIT', 'MERRA2']`; messages `[]`; 40,594 bytes

## F_climatology
NASA POWER's own climatology for the same point, for comparison.  
`https://power.larc.nasa.gov/api/temporal/climatology/point?parameters=PRECTOTCORR,PRECTOTCORR_SUM&community=AG&longitude=-114.08203125&latitude=53.9125034103343&format=JSON`  
HTTP 200; units `mm/day`; sources `['MERRA2', 'POWER']`; messages `['The requested parameters are retrieved from a pre-computed climatological period (January 2001 - December 2025)']`; 1,095 bytes

## PRECTOTCORR on the same days

| Day | A_short_clean | B_long_to_today | C_holos_request | D_boundary_end_20261002 | E_boundary_end_20261003 |
|---|---|---|---|---|---|
| 20200101 | 0.77 | 18.48 | 18.48 | 18.48 | 18.48 |
| 20200102 | 0.4 | 9.6 | 9.6 | 9.6 | 9.6 |
| 20200107 | 3.63 | 87.12 | 87.12 | 87.12 | 87.12 |
| 20200108 | 3.13 | 75.12 | 75.12 | 75.12 | 75.12 |

- B_long_to_today: ratio to A on those days = 24.000, 24.000, 24.000, 24.000
- C_holos_request: ratio to A on those days = 24.000, 24.000, 24.000, 24.000
- D_boundary_end_20261002: ratio to A on those days = 24.000, 24.000, 24.000, 24.000
- E_boundary_end_20261003: ratio to A on those days = 24.000, 24.000, 24.000, 24.000
- F_climatology: PRECTOTCORR annual mean 1.45 mm/day

