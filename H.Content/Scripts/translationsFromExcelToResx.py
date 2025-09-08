import openpyxl
from lxml import etree
import re

# Assign these variables to correct file paths and to the correct sheet name in the Excel file
# English and French resx files must be used to ensure keys exist in English file prior to adding French translations
# If key does not exist in English file but does in French file, this key will not be added as conflicts will be created in the project
resxEnglishFilePath = r"C:\Users\username\Source\Repos\Holos\H\H.Core\Properties\Resources.resx"
resxFrenchFilePath = r"C:\Users\username\Source\Repos\Holos\H\H.Core\Properties\Resources.fr-CA.resx"
xlsxFilePath = r"C:\Users\username\Desktop\Resource_Python_Script\Files\resources.xlsx"
excelFileSheetName = 'H.Core Resources.resx'

"""
Read translated values from the Excel file and return them as a dictionary.

Args:
    String xlsxFilePath: Path to the Excel file containing translations.

Returns:
    Dictionary with keys as resource names and values as their French translations.
"""
def readTranslatedValuesFromxlsx(xlsxFilePath):
    translatedValues = {}

    # Load correct sheet from Excel file
    workbook = openpyxl.load_workbook(xlsxFilePath)
    sheet = workbook[excelFileSheetName]
    # Set column indices for key and French translation
    keyColumn = 0
    frenchColumn = 2

    for row in sheet.iter_rows(values_only=True):
        key = row[keyColumn]
        translatedValues[key] = row[frenchColumn]

    return translatedValues

""" 
Verifies and enters translated values into the French resx file for project.

Args:
    String resxFrenchFilePath: Path to the French resx file where translations will be entered.
    String resxEnglishFilePath: Path to the English resx file used to verify that key exists before adding French translation.
    Dictionary translatedValues: Dictionary with keys as resource names and values as their French translations.

Returns: 
    None. Updates the French resx file with new and updated translations.
"""
def enterTranslatedValuesToResx(resxFrenchFilePath, resxEnglishFilePath, translatedValues):
    originalElements = []
    updatedElements = []
    newElements = []
    unalteredElements = []
    invalidElements = []

    parser = etree.XMLParser(remove_blank_text=False)

    # Get the XML tree from the French resx file
    treeFrench = etree.parse(resxFrenchFilePath, parser)
    rootFrench = treeFrench.getroot()
    resxDataFrench = rootFrench.findall('.//data')
    resxFrenchKeys = {data.get('name') for data in resxDataFrench}

    # Get the XML tree from the English resx file
    resxEnglishKeys = {data.get('name') for data in etree.parse(resxEnglishFilePath, parser).getroot().findall('.//data')}    

    # Iterate through existing data elements in resx and update the value if the key matches and if the value is different, if same, value will not be changed
    for keys in resxDataFrench:
        name = keys.get('name')
        if name in translatedValues:
            frenchTranslation = keys.find('value')      
            if frenchTranslation.text != translatedValues[name]:
                originalElements.append({"key": name, "value": str(frenchTranslation.text)})
                frenchTranslation.text = translatedValues[name]
                updatedElements.append({"key": name, "value": str(translatedValues[name])})
            else:
                unalteredElements.append({"key": name, "value": str(translatedValues[name])})

    # Find keys that exist in the Excel file but are missing in the .resx file
    missingKeys = set(translatedValues.keys()) - resxFrenchKeys

    # Iterate through key that only exist in the Excel file and not in the French resx file
    # If the key exists in the English resx file, it will be added to the French resx file, else it will not to avoid potential errors
    for key in missingKeys:
        if key not in resxEnglishKeys and key != None:
            invalidElements.append({"Key": key, "Value": str(translatedValues[key])})
        elif key in resxEnglishKeys:
            newData = etree.Element("data", name=key, attrib={"{http://www.w3.org/XML/1998/namespace}space": "preserve"})
            valueElement = etree.SubElement(newData, "value")
            valueElement.text = translatedValues[key]
            # Add proper indentation and line breaks to maintain formatting
            newData.text = "\n    "     # Puts the value tag on a new line
            valueElement.tail = "\n  "  # Puts closing data tag on new line
            newData.tail = "\n  "       # Puts opening data tag on new line
            rootFrench.append(newData)
            newElements.append({"Key": key, "Value": str(translatedValues[key])})

    # Preserve the XML declaration: "<?xml version="1.0" encoding="utf-8"?>"
    # read file and store file as string in variable
    with open(resxFrenchFilePath, encoding="utf-8") as f:
        XML_Declaration = f.readline()
        originalFileString = f.read()

    # Convert the updated XML tree to a string for easier manipulation
    updatedXMLTreeString = etree.tostring(treeFrench, encoding='utf8').decode('utf8')
    # Pattern before the first <data> element to find where to split the string
    endOfFileDeclarationPattern = r'  <resheader name="writer">\s+<value>System\.Resources\.ResXResourceWriter, System\.Windows\.Forms, Version=4\.0\.0\.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</value>\s+</resheader>'
    positionToSplitStrings = re.search(endOfFileDeclarationPattern, updatedXMLTreeString).end()

    with open(resxFrenchFilePath, "w", encoding="utf-8") as f:
        f.write(XML_Declaration) # <?xml version="1.0" encoding="utf-8"?>
        f.write(originalFileString[:positionToSplitStrings+16]) # Preserves the original file up to the first <data> element
        # replace (#1) to fix whitespace added from lxml, replace (#2) to fix closing tag at end of file, replace (#3) to fix indentation on first of new keys added
        f.write(updatedXMLTreeString[positionToSplitStrings:].replace("/>", " />").replace("  </root>", "</root>").replace("\n<data", "\n  <data")) # All keys and associated values + closing "</root>" tag

    print("Resources added: ", len(newElements))
    print("Resources updated: ", len(updatedElements))
    print("Unaltered resources: ", len(unalteredElements))
    print("Invalid resources: ", len(invalidElements))

    # Menu for displaying resources under each category
    options = ["Print added resources", "Print updated resources", "Print unaltered resources", "Print invalid resources", "Exit"]
    while True:
        print("\nOptions:")
        for i, option in enumerate(options, 1):
            print(f"{i}. {option}")

        choice = input("Select an option (1-5): ")
        if choice == "1":
            if not newElements:
                print("No new resources added.")
            else:
                print("\nAdded Resources:")
                for item in newElements:
                    print(f"Key: {item['Key']}\n   Value: {item['Value']}")
        elif choice == "2":
            if not updatedElements:
                print("No resources updated.")
            else:
                print("\nUpdated Resources:")
                original_dict = {item['key']: item['value'] for item in originalElements}
                for item in updatedElements:
                    key = item['key']
                    new_value = item['value']
                    original_value = original_dict.get(key, "N/A")
                    print(f"Key: {key}\n   Org Value: {original_value}\n   New Value: {new_value}")
        elif choice == "3":
            if not unalteredElements:
                print("No unaltered resources.")
            else:
                print("\nUnaltered Resources:")
                for item in unalteredElements:
                    print(f"Key: {item['key']}\n   Value: {item['value']}")
        elif choice == "4":
            if not invalidElements:
                print("No invalid resources.")
            else:
                print("\nInvalid Resources:")
                for item in invalidElements:
                    print(f"Key: {item['Key']}, Value: {item['Value']}")
        elif choice == "5":
            break
        else:
            print("Invalid input.")

def main():
    translatedValues = readTranslatedValuesFromxlsx(xlsxFilePath)
    enterTranslatedValuesToResx(resxFrenchFilePath, resxEnglishFilePath, translatedValues)

if __name__ == "__main__":
    main()