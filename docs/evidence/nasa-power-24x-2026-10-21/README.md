# NASA POWER precipitation returned 24 times too large

Evidence for holos-aafc/Holos#499. Captured 2026-10-21.

## The finding

The NASA POWER daily point API returns `PRECTOTCORR` (precipitation, labelled `mm/day`) at exactly 24 times its true
value whenever the request reaches the months served from GEOS-IT. A request for the same days at the same grid cell
that does not reach those months returns the true value. Neither response carries a warning.

| Day (2020) | Short request (`end=20200110`) | Request through today (`end=20261021`) | Ratio |
|---|---|---|---|
| Jan 1 | 0.77 | 18.48 | 24.000 |
| Jan 2 | 0.4 | 9.6 | 24.000 |
| Jan 7 | 3.63 | 87.12 | 24.000 |
| Jan 8 | 3.13 | 75.12 | 24.000 |

Point: latitude 53.9125034103343, longitude -114.08203125 (central Alberta). The exact request Holos makes (file
`C_holos_request.json`) shows the same values.

Which value is true: NASA POWER's own climatology for the point gives 1.45 mm/day, about 530 mm a year. ERA5 (ECMWF,
via the Open-Meteo archive) gives 475 mm a year for 1991-2020. The short request agrees with both. The long request
sums to about 12,700 mm a year.

## What is in this folder

- `capture_20261021T174322Z/` - the responses exactly as received:
  - `*.json` - the raw response body, byte for byte
  - `*.request.txt` - request URL, capture time (UTC), HTTP status and NASA's response headers (including its own `Date`)
  - `SHA256SUMS.txt` - checksums of every file, so any copy can be checked against this one
  - `SUMMARY.md` - the values above, extracted from the files
- `nasa_power_evidence_capture_v1.0.py` - the script that made the capture. Run it again to capture what the API
  returns at that time: `python -I nasa_power_evidence_capture_v1.0.py <output folder>`

To verify a copy of the capture: `sha256sum -c SHA256SUMS.txt` inside the capture folder.

## Where the switch falls (all at the same point, `start=20200101`)

| Checked | Clean through end date | Inflated from end date |
|---|---|---|
| 2026-10-19/20 | 2026-08-31 | 2026-09-01 |
| 2026-10-21, earlier in the day | 2026-10-02 | 2026-10-03 |
| 2026-10-21 17:43 UTC | 2026-09-30 | 2026-10-01 |

The boundary is not a fixed date. It moves in both directions, and two automated checks 18 seconds apart on
2026-10-21 (17:47 UTC) placed it at 2026-09-30/10-01 and at 2026-10-13/10-14, so it may depend on which server
answers. The factor does not vary: every inflated response is exactly 24 times the clean one. In the captures made on
2026-10-21, every inflated response lists `GEOSIT` among its sources and every clean one lists only `MERRA2`.

## Also recorded elsewhere

- The issue and its comments: https://github.com/holos-aafc/Holos/issues/499
- Holos's correction: https://github.com/holos-aafc/Holos/pull/500
- An independent record of the same factor by other researchers (2026-10-02): https://zenodo.org/records/23111223
- Internet Archive (Wayback Machine) copies of the request URLs: see `WAYBACK.md`
- Daily check of whether it is still happening, kept on the project share (`Z:\Projects\Holos\AI`):
  `nasa_power_monitor_v1.0.py`, logging to `nasa_power_24x_monitor_log.csv`
