import os
from pathlib import Path
import itertools

# The folder needs to be in the same 'root' folder as the script. Change name accordingly
FOLDER_NAME = Path("files")
# The copyright file needs to be in the same folder as the script. The file MUST have a empty new line at the end of the file.
COPYRIGHT_FILE_NAME = "copyright.txt"
# The copyright region name. Change string based on contents of copyright notice.
COPYRIGHT_REGION_NAME = "#region Copyright"


def insert_copyright_into_files(file_path, copyright_notice, copyright_region_name):
    """
    This function inserts a copyright into a file. The function takes in the path of the file and a copyright notice and
    inserts the copyright notice at the top of the file. The function also checks if a copyright already exists in a file before
    adding a new copyright section. The function creates a new file, enters copyright information
    into that new file, moves the previous contents of this file into the new file and finally deletes/renames  files.

    Parameters
    -----------
    file_path : pathlib.Path module module object. 
        The file path of the file where the copyright needs to be inserted. Needs to be converted to string.
    copyright_notice : str [].
        An array of strings containing the copyright notice. Each entry in the array denotes a line in the copyright notice.
    copyright_region_name : str
        A string denoting the name of the copyright region. This is the starting region tag in VS and is used to check if a file already
        contains a copyright notice or not.
    """
    temp_file = str(file_path) + '.backup'
    first_line = ""

    with open(file_path, 'r') as file:
        first_line = next(file)

    if copyright_region_name not in first_line:
        with open(file_path, 'r') as original_file, open(temp_file, 'w') as new_file:
            for line in copyright_notice:
                new_file.write(line + '\n')
            
            for line in original_file:
                new_file.write(line)

        os.remove(file_path)
        os.rename(temp_file, file_path)



def count_copyright_region_lines(file):
    """
    This function counts the number of lines of a copyright notice in a file. 
    NOTE: The method assumes that the method will only be called when a copyright notice exists inside a file.
    The method searches for the first "endregion" tag as it assumes a starting region tag is already present.

    Parameters
    -----------
    file : Iterator file object created using open()

    Returns
    ---------

    count : int
        The count of the number of lines of the copyright notice region.
        NOTE: The method adds 1 to the return count total. This is because the copyright notice file is assumed to have
        an empty line at the end of the file.
    """
    count = 0
    for line in file:
        if ("#endregion" in line):
            count += 1
            break

        count += 1
    return count + 1


def delete_cropyright_from_files(file_path, copyright_region_name):
    """
    This function removes a copyright notice from a file. The function also performs a check of whether a copyright notice already exists.
    This is done by checking if the first line of the file contains the copyright region name. If it does, the method
    calls count_copyright_region_lines to find the number of lines of copyright notice, skips that count of lines in the current file
    and writes the remaining files into a new file with the same name.

    Parameters
    ----------
    file_path = pathlib.Path module module object.
        The file path of the file where the copyright needs to be deleted from. Needs to be converted to string.
    copyright_region_name:
        The name of the #region tag include the #region portion. E.g. #region Copyright. The region tag needs to be included so that the
        function doesn't find other lines in the file with a possible similar name.

    """
    temp_file = str(file_path) + '.backup'
    first_line = ""

    with open(file_path, 'r') as file:
        first_line = next(file)

    if copyright_region_name in first_line:
        with open(file_path, 'r') as original_file, open(temp_file, 'w') as new_file:
            
            num_copyright_region_lines = count_copyright_region_lines(original_file)
            original_file.seek(0)
            for line in itertools.islice(original_file, num_copyright_region_lines, None):
                new_file.write(line)

        os.remove(file_path)
        os.rename(temp_file, file_path)

            

def main():
    # The copyright file MUST have a blank empty file at the end.
    with open(COPYRIGHT_FILE_NAME) as copyright:
        copyright_lines = copyright.readlines()

    """
    Un-comment either method and the for loop to perform the desired function. Please remember to comment the method back after use
    to avoid unnecessary processing of files.

    insert_copyright_into_files = Inserts a copyright into each file inside the folder.
    delete_cropyright_from_files = Deletes a copyright from each file inside the folder.
    """
    #for file_path in FOLDER_NAME.glob('**/*.cs'):
        #insert_copyright_into_files(file_path, copyright_lines, COPYRIGHT_REGION_NAME)
        #delete_cropyright_from_files(file_path, COPYRIGHT_REGION_NAME)


if __name__ == '__main__':
    main()