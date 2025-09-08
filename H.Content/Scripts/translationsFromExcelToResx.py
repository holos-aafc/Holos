import openpyxl
from lxml import etree
# Must use lxml in order to preserve comments and namespaces in the XML file (pip install lxml)

# Assign these variables to correct file paths and to the correct sheet name in the Excel file
resxFilePath = r"C:\Users\ExampleUser\source\repos\Holos\H\H.Core\Properties\Resources.fr-CA.resx"
xlsxFilePath = r"C:\Users\ExampleUser\Documents\Resources.xlsx"
excelFileSheetName = 'H.Core Resources.resx'

def readTranslatedValues(xlsxFilePath):
    translatedValues = {}

    # Load correct sheet from Excel file
    workbook = openpyxl.load_workbook(xlsxFilePath)
    sheet = workbook[excelFileSheetName]

    # Iterate through the rows in the sheet and extract the key and translated value
    # Assumes french column is the third column (index 2) and the key is in the first column (index 0)
    for row in sheet.iter_rows(values_only=True):
        key = row[0]
        translatedValues[key] = row[2]

    return translatedValues

def enterTranslatedValuesToResx(resxFilePath, translatedValues):
    matchedElements = []

    parser = etree.XMLParser(remove_blank_text=False)
    tree = etree.parse(resxFilePath, parser)
    root = tree.getroot()
    resxData = root.findall('.//data')

    # Iterate through the data elements and update the value if the key matches
    for keys in resxData:
        name = keys.get('name')
        if name in translatedValues:
            frenchTranslation = keys.find('value')      
            if frenchTranslation is not None:
                frenchTranslation.text = "translatedValues[name]"
                matchedElements.append(name)

    # Print elements from Excel file that did not match any elements in the .resx file, usually a slight mispelling of the key in the .resx file
    invalidElements = set(translatedValues)-set(matchedElements)
    print(f"This entries did not have match in the resx file: {invalidElements}")

    # Save the updated .resx file, preserving comments and namespaces
    tree.write(resxFilePath, encoding='utf-8', xml_declaration=True, pretty_print=True)

if __name__ == "__main__":
    translatedValues = readTranslatedValues(xlsxFilePath)
    enterTranslatedValuesToResx(resxFilePath, translatedValues)