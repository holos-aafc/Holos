"""
New SLC Normals Concatenation
This is a script to be used after BuildNewSLCClimateNorms to create merged
file from what BuildNewSLCClimateNorms creates.
"""

import os
from pathlib import Path
import threading
from typing import List, Dict


def get_files_in_dir(path: str) -> List[Path]:
    """ 
        return a list of file names in a directory
    """
    list_of_files = []
    p = Path(path)
    for entry in p.iterdir():
        list_of_files.append(entry)
    return list_of_files


def has_header(line: str) -> bool:
    """
        return true if line has a header, false otherwise
    """
    if "SLC" in line:
        return True
    return False


def get_file_cache(dirs: List[str]) -> Dict[str, List[Path]]:
    """
    make a cache of all the files in each directory
        return Dictionary[str, List[Path]]
    """
    cache = {}
    print("creating cache...")
    for dir in dirs:
        cache[dir] = get_files_in_dir(dir)
    return cache


def make_superfile(directory: str, list_of_files: List[Path]) -> None:
    """
    Take in a list of files and then create a merged file of all those lists in the given directory
    return None
    """
    print(f"making superfile for directory: {directory}")
    superfile: str = directory + "\\superfile.csv"
    header: str = "SLC,month,Tavg,PREC,PET\n"
    with open(superfile, 'w') as output:
        output.write(header)
        for file in list_of_files:
            with open(file) as input:
                for line in input:
                    if (has_header(line)):
                        continue
                    output.write(line)


def main() -> None:
    """
    The main method
    """
    # change these paths to fit your needs everything else should just work...I think
    dirnames = [r"C:\Users\bigbe\Holos\slcDaily\1950-1980",
                r"C:\Users\bigbe\Holos\slcDaily\1960-1990",
                r"C:\Users\bigbe\Holos\slcDaily\1970-2000",
                r"C:\Users\bigbe\Holos\slcDaily\1980-2010",
                r"C:\Users\bigbe\Holos\slcDaily\1990-2017",
                ]

    threads = 5
    jobs: list = []
    cache = get_file_cache(dirnames)
    for i in range(0, threads):
        thread = threading.Thread(target=make_superfile(
            dirnames[i], cache[dirnames[i]]))
        jobs.append(thread)

    for j in jobs:
        j.start()

    for j in jobs:
        j.join()

    print("processing complete")


if __name__ == "__main__":
    main()
