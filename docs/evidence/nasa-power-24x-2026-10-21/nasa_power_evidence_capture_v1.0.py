"""
NASA POWER precipitation evidence capture, v1.0 (2026-10-21).

Saves NASA POWER API responses exactly as received (raw bytes), with the HTTP status, response headers, the request
URL and the capture time, and writes a SHA-256 manifest. Run it again later to show what the API returns then.

Context: holos-aafc/Holos#499. PRECTOTCORR is returned 24 times too large whenever a daily request reaches the months
served from GEOS-IT. The same day at the same grid cell comes back at its true value from a request that does not.

Usage:  python -I nasa_power_evidence_capture_v1.0.py [output folder]
"""
import hashlib
import json
import os
import sys
import time
import urllib.request
from datetime import datetime, timezone

LAT = "53.9125034103343"   # the central Alberta test point used throughout #499
LON = "-114.08203125"
DAILY = "https://power.larc.nasa.gov/api/temporal/daily/point"
CLIMATOLOGY = "https://power.larc.nasa.gov/api/temporal/climatology/point"
HOLOS_PARAMETERS = "T2M,PRECTOTCORR,RH2M,ALLSKY_SFC_SW_DWN,T2M_MIN,T2M_MAX"   # as in NasaClimateProvider.GetCorrectApiUrl
TODAY = datetime.now(timezone.utc).strftime("%Y%m%d")


def daily(parameters, start, end):
    return f"{DAILY}?parameters={parameters}&community=AG&longitude={LON}&latitude={LAT}&start={start}&end={end}&format=JSON"


REQUESTS = [
    ("A_short_clean", "Short request, the #499 original: 10 days of January 2020.",
     daily("PRECTOTCORR", "20200101", "20200110")),
    ("B_long_to_today", "1981 to today, PRECTOTCORR only. Same days as A.",
     daily("PRECTOTCORR", "19810101", TODAY)),
    ("C_holos_request", "The request Holos makes: Holos's six parameters, 1981 to today.",
     daily(HOLOS_PARAMETERS, "19810101", TODAY)),
    ("D_boundary_end_20261002", "2020 to 2026-10-02 (clean when first checked on 2026-10-21).",
     daily("PRECTOTCORR", "20200101", "20261002")),
    ("E_boundary_end_20261003", "2020 to 2026-10-03 (inflated when first checked on 2026-10-21).",
     daily("PRECTOTCORR", "20200101", "20261003")),
    ("F_climatology", "NASA POWER's own climatology for the same point, for comparison.",
     f"{CLIMATOLOGY}?parameters=PRECTOTCORR,PRECTOTCORR_SUM&community=AG&longitude={LON}&latitude={LAT}&format=JSON"),
]

KEY_DAYS = ["20200101", "20200102", "20200107", "20200108"]


def fetch(url):
    request = urllib.request.Request(url, headers={"User-Agent": "Holos-evidence-capture/1.0"})
    started = time.time()
    with urllib.request.urlopen(request, timeout=300) as response:
        body = response.read()
        return response.status, response.reason, list(response.headers.items()), body, time.time() - started


def main():
    out = sys.argv[1] if len(sys.argv) > 1 else os.path.dirname(os.path.abspath(__file__))
    stamp = datetime.now(timezone.utc).strftime("%Y%m%dT%H%M%SZ")
    folder = os.path.join(out, f"capture_{stamp}")
    os.makedirs(folder)

    manifest = []
    summary = [f"# Capture {stamp}", "", f"Point: latitude {LAT}, longitude {LON}", ""]
    values = {}

    for name, description, url in REQUESTS:
        captured = datetime.now(timezone.utc).isoformat()
        status, reason, headers, body, elapsed = fetch(url)

        body_path = os.path.join(folder, name + ".json")
        with open(body_path, "wb") as handle:
            handle.write(body)

        meta_path = os.path.join(folder, name + ".request.txt")
        with open(meta_path, "w", encoding="utf-8", newline="\n") as handle:
            handle.write(f"Description: {description}\nRequest URL: {url}\nCaptured (UTC): {captured}\n")
            handle.write(f"Elapsed: {elapsed:.1f} s\nHTTP: {status} {reason}\n\nResponse headers:\n")
            for key, value in headers:
                handle.write(f"{key}: {value}\n")

        for path in (body_path, meta_path):
            with open(path, "rb") as handle:
                manifest.append((hashlib.sha256(handle.read()).hexdigest(), os.path.basename(path)))

        payload = json.loads(body)
        units = payload.get("parameters", {}).get("PRECTOTCORR", {}).get("units")
        sources = payload.get("header", {}).get("sources")
        messages = payload.get("messages")
        series = payload.get("properties", {}).get("parameter", {}).get("PRECTOTCORR", {})
        values[name] = series
        summary.append(f"## {name}")
        summary.append(f"{description}  ")
        summary.append(f"`{url}`  ")
        summary.append(f"HTTP {status}; units `{units}`; sources `{sources}`; messages `{messages}`; {len(body):,} bytes")
        summary.append("")

    clean = values["A_short_clean"]
    summary.append("## PRECTOTCORR on the same days")
    summary.append("")
    summary.append("| Day | " + " | ".join(n for n, _, _ in REQUESTS[:5]) + " |")
    summary.append("|---|" + "---|" * 5)
    for day in KEY_DAYS:
        row = [str(values[n].get(day)) for n, _, _ in REQUESTS[:5]]
        summary.append(f"| {day} | " + " | ".join(row) + " |")
    summary.append("")
    for name, _, _ in REQUESTS[1:5]:
        ratios = [values[name][d] / clean[d] for d in KEY_DAYS if clean.get(d) and values[name].get(d) is not None]
        if ratios:
            summary.append(f"- {name}: ratio to A on those days = {', '.join(f'{r:.3f}' for r in ratios)}")
    climatology = values["F_climatology"]
    summary.append(f"- F_climatology: PRECTOTCORR annual mean {climatology.get('ANN')} mm/day")
    summary.append("")

    with open(os.path.join(folder, "SHA256SUMS.txt"), "w", encoding="utf-8", newline="\n") as handle:
        for digest, name in manifest:
            handle.write(f"{digest}  {name}\n")
    with open(os.path.join(folder, "SUMMARY.md"), "w", encoding="utf-8", newline="\n") as handle:
        handle.write("\n".join(summary) + "\n")

    print(folder)
    print("\n".join(summary))


if __name__ == "__main__":
    main()
