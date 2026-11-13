# Internet Archive copies

Saved through the Wayback Machine on 2026-10-21 (timestamps are the Archive's own, UTC). Each copy was fetched back
from the Archive afterwards and checked. Add `id_` after the timestamp to get the raw JSON instead of the Archive's
viewer, e.g. `https://web.archive.org/web/20261021174414id_/https://power.larc.nasa.gov/...`

| Request | Archived (UTC) | Sources in response | 2020-01-01 | 2020-01-07 |
|---|---|---|---|---|
| [`start=20200101&end=20200110`](https://web.archive.org/web/20261021174414/https://power.larc.nasa.gov/api/temporal/daily/point?parameters=PRECTOTCORR&community=AG&longitude=-114.08203125&latitude=53.9125034103343&start=20200101&end=20200110&format=JSON) | 2026-10-21 17:44:14 | MERRA2 | 0.77 | 3.63 |
| [`start=19810101&end=20261021`](https://web.archive.org/web/20261021174528/https://power.larc.nasa.gov/api/temporal/daily/point?parameters=PRECTOTCORR&community=AG&longitude=-114.08203125&latitude=53.9125034103343&start=19810101&end=20261021&format=JSON) | 2026-10-21 17:45:28 | GEOSIT, MERRA2 | 18.48 | 87.12 |
| [`start=20200101&end=20260930`](https://web.archive.org/web/20261021174549/https://power.larc.nasa.gov/api/temporal/daily/point?parameters=PRECTOTCORR&community=AG&longitude=-114.08203125&latitude=53.9125034103343&start=20200101&end=20260930&format=JSON) | 2026-10-21 17:45:49 | MERRA2 | 0.77 | 3.63 |
| [`start=20200101&end=20261001`](https://web.archive.org/web/20261021174611/https://power.larc.nasa.gov/api/temporal/daily/point?parameters=PRECTOTCORR&community=AG&longitude=-114.08203125&latitude=53.9125034103343&start=20200101&end=20261001&format=JSON) | 2026-10-21 17:46:11 | GEOSIT, MERRA2 | 18.48 | 87.12 |

All four report `"units": "mm/day"` and no messages. The last two differ only by one day in the end date.
