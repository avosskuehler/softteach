namespace SoftTeach.ViewModel.Helper.ODSSupport
{
  using Ionic.Zip;
  using SoftTeach.ViewModel.Jahrespläne;
  using SoftTeach.ViewModel.Personen;
  using SoftTeach.ViewModel.Termine;
  using System;
  using System.Data;
  using System.Globalization;
  using System.IO;
  using System.Linq;
  using System.Reflection;
  using System.Xml;

  internal sealed class OdsReaderWriter
  {
    // Namespaces. We need this to initialize XmlNamespaceManager so that we can search XmlDocument.
    private static string[,] namespaces = new string[,]
    {
            {"table", "urn:oasis:names:tc:opendocument:xmlns:table:1.0"},
            {"office", "urn:oasis:names:tc:opendocument:xmlns:office:1.0"},
            {"style", "urn:oasis:names:tc:opendocument:xmlns:style:1.0"},
            {"text", "urn:oasis:names:tc:opendocument:xmlns:text:1.0"},
            {"draw", "urn:oasis:names:tc:opendocument:xmlns:drawing:1.0"},
            {"fo", "urn:oasis:names:tc:opendocument:xmlns:xsl-fo-compatible:1.0"},
            {"dc", "http://purl.org/dc/elements/1.1/"},
            {"meta", "urn:oasis:names:tc:opendocument:xmlns:meta:1.0"},
            {"number", "urn:oasis:names:tc:opendocument:xmlns:datastyle:1.0"},
            {"presentation", "urn:oasis:names:tc:opendocument:xmlns:presentation:1.0"},
            {"svg", "urn:oasis:names:tc:opendocument:xmlns:svg-compatible:1.0"},
            {"chart", "urn:oasis:names:tc:opendocument:xmlns:chart:1.0"},
            {"dr3d", "urn:oasis:names:tc:opendocument:xmlns:dr3d:1.0"},
            {"math", "http://www.w3.org/1998/Math/MathML"},
            {"form", "urn:oasis:names:tc:opendocument:xmlns:form:1.0"},
            {"script", "urn:oasis:names:tc:opendocument:xmlns:script:1.0"},
            {"ooo", "http://openoffice.org/2004/office"},
            {"ooow", "http://openoffice.org/2004/writer"},
            {"oooc", "http://openoffice.org/2004/calc"},
            {"dom", "http://www.w3.org/2001/xml-events"},
            {"xforms", "http://www.w3.org/2002/xforms"},
            {"xsd", "http://www.w3.org/2001/XMLSchema"},
            {"xsi", "http://www.w3.org/2001/XMLSchema-instance"},
            {"rpt", "http://openoffice.org/2005/report"},
            {"of", "urn:oasis:names:tc:opendocument:xmlns:of:1.2"},
            {"rdfa", "http://docs.oasis-open.org/opendocument/meta/rdfa#"},
            {"config", "urn:oasis:names:tc:opendocument:xmlns:config:1.0"}
    };

    // Read zip stream (.ods file is zip file).
    private static ZipFile GetZipFile(Stream stream)
    {
      return ZipFile.Read(stream);
    }

    // Read zip file (.ods file is zip file).
    private static ZipFile GetZipFile(string inputFilePath)
    {
      return ZipFile.Read(inputFilePath);
    }

    private static XmlDocument GetContentXmlFile(ZipFile zipFile)
    {
      // Get file(in zip archive) that contains data ("content.xml").
      ZipEntry contentZipEntry = zipFile["content.xml"];

      // Extract that file to MemoryStream.
      Stream contentStream = new MemoryStream();
      contentZipEntry.Extract(contentStream);
      contentStream.Seek(0, SeekOrigin.Begin);

      // Create XmlDocument from MemoryStream (MemoryStream contains content.xml).
      XmlDocument contentXml = new XmlDocument();
      contentXml.Load(contentStream);

      return contentXml;
    }

    private static XmlNamespaceManager InitializeXmlNamespaceManager(XmlDocument xmlDocument)
    {
      XmlNamespaceManager nmsManager = new XmlNamespaceManager(xmlDocument.NameTable);

      for (int i = 0; i < namespaces.GetLength(0); i++)
        nmsManager.AddNamespace(namespaces[i, 0], namespaces[i, 1]);

      return nmsManager;
    }

    /// <summary>
    /// Read .ods file and store it in DataSet.
    /// </summary>
    /// <param name="inputFilePath">Path to the .ods file.</param>
    /// <returns>DataSet that represents .ods file.</returns>
    public DataSet ReadOdsFile(string inputFilePath)
    {
      ZipFile odsZipFile = GetZipFile(inputFilePath);

      // Get content.xml file
      XmlDocument contentXml = GetContentXmlFile(odsZipFile);

      // Initialize XmlNamespaceManager
      XmlNamespaceManager nmsManager = InitializeXmlNamespaceManager(contentXml);

      DataSet odsFile = new DataSet(Path.GetFileName(inputFilePath));

      foreach (XmlNode tableNode in GetTableNodes(contentXml, nmsManager))
        odsFile.Tables.Add(this.GetSheet(tableNode, nmsManager));

      return odsFile;
    }

    // In ODF sheet is stored in table:table node
    private static XmlNodeList GetTableNodes(XmlDocument contentXmlDocument, XmlNamespaceManager nmsManager)
    {
      return contentXmlDocument.SelectNodes("/office:document-content/office:body/office:spreadsheet/table:table", nmsManager);
    }

    private DataTable GetSheet(XmlNode tableNode, XmlNamespaceManager nmsManager)
    {
      DataTable sheet = new DataTable(tableNode.Attributes["table:name"].Value);

      XmlNodeList rowNodes = tableNode.SelectNodes("table:table-row", nmsManager);

      int rowIndex = 0;
      foreach (XmlNode rowNode in rowNodes)
        this.GetRow(rowNode, sheet, nmsManager, ref rowIndex);

      return sheet;
    }

    private void GetRow(XmlNode rowNode, DataTable sheet, XmlNamespaceManager nmsManager, ref int rowIndex)
    {
      //XmlAttribute rowsRepeated = rowNode.Attributes["table:number-rows-repeated"];
      //if (rowsRepeated == null || Convert.ToInt32(rowsRepeated.Value, CultureInfo.InvariantCulture) == 1)
      //{
      while (sheet.Rows.Count < rowIndex)
        sheet.Rows.Add(sheet.NewRow());

      DataRow row = sheet.NewRow();

      XmlNodeList cellNodes = rowNode.SelectNodes("table:table-cell", nmsManager);

      int cellIndex = 0;
      foreach (XmlNode cellNode in cellNodes)
        this.GetCell(cellNode, row, nmsManager, ref cellIndex);

      sheet.Rows.Add(row);

      rowIndex++;
      //}
      //else
      //{
      //  rowIndex += Convert.ToInt32(rowsRepeated.Value, CultureInfo.InvariantCulture);
      //}

      // sheet must have at least one cell
      if (sheet.Rows.Count == 0)
      {
        sheet.Rows.Add(sheet.NewRow());
        sheet.Columns.Add();
      }
    }

    private void GetCell(XmlNode cellNode, DataRow row, XmlNamespaceManager nmsManager, ref int cellIndex)
    {
      XmlAttribute cellRepeated = cellNode.Attributes["table:number-columns-repeated"];

      if (cellRepeated == null)
      {
        DataTable sheet = row.Table;

        while (sheet.Columns.Count <= cellIndex)
          sheet.Columns.Add();

        row[cellIndex] = ReadCellValue(cellNode);

        cellIndex++;
      }
      else
      {
        cellIndex += Convert.ToInt32(cellRepeated.Value, CultureInfo.InvariantCulture);
      }
    }

    private static string ReadCellValue(XmlNode cell)
    {
      XmlAttribute cellVal = cell.Attributes["office:value"];

      if (cellVal == null)
        return String.IsNullOrEmpty(cell.InnerText) ? null : cell.InnerText;
      else
        return cellVal.Value;
    }

    /// <summary>
    /// Writes a Jahresplan into an .ods file.
    /// </summary>
    /// <param name="lerngruppe">The <see cref="JahresplanViewModel"/> with the Jahresplandata to export.</param>
    /// <param name="outputFilePath">The name of the file to save to.</param>
    public void WriteOdsFile(LerngruppeViewModel lerngruppe, string outputFilePath)
    {
      ZipFile templateFile = GetZipFile(Assembly.GetExecutingAssembly().GetManifestResourceStream("SoftTeach.ViewModel.Helper.ODSSupport.template.ods"));

      XmlDocument contentXml = GetContentXmlFile(templateFile);

      XmlNamespaceManager nmsManager = InitializeXmlNamespaceManager(contentXml);

      XmlNode sheetsRootNode = this.GetSheetsRootNodeAndRemoveChildrens(contentXml, nmsManager);

      // Tabelle anlegen
      XmlDocument ownerDocument = sheetsRootNode.OwnerDocument;
      XmlNode sheetNode = ownerDocument.CreateElement("table:table", GetNamespaceUri("table"));
      XmlAttribute sheetName = ownerDocument.CreateAttribute("table:name", GetNamespaceUri("table"));
      sheetName.Value = lerngruppe.LerngruppeSchuljahr.ToString();
      sheetNode.Attributes.Append(sheetName);

      // Spalten anlegen
      XmlNode columnDefinition = ownerDocument.CreateElement("table:table-column", GetNamespaceUri("table"));
      XmlAttribute columnsCount = ownerDocument.CreateAttribute("table:number-columns-repeated", GetNamespaceUri("table"));
      columnsCount.Value = "5";
      columnDefinition.Attributes.Append(columnsCount);
      sheetNode.AppendChild(columnDefinition);

      // Zeilen anlegen
      foreach (var stunde in lerngruppe.Lerngruppentermine.OfType<StundeViewModel>().OrderBy(o => o.LerngruppenterminDatum))
      {
        XmlNode rowNode = ownerDocument.CreateElement("table:table-row", GetNamespaceUri("table"));

        // Datum schreiben
        XmlElement cellNode = ownerDocument.CreateElement("table:table-cell", GetNamespaceUri("table"));
        XmlAttribute valueType = ownerDocument.CreateAttribute("office:value-type", GetNamespaceUri("office"));
        valueType.Value = "string";
        cellNode.Attributes.Append(valueType);
        XmlElement cellValue = ownerDocument.CreateElement("text:p", GetNamespaceUri("text"));
        cellValue.InnerText = stunde.LerngruppenterminDatumKurz;
        cellNode.AppendChild(cellValue);

        rowNode.AppendChild(cellNode);

        // Titel des Stundenentwurfs schreiben
        cellNode = ownerDocument.CreateElement("table:table-cell", GetNamespaceUri("table"));
        valueType = ownerDocument.CreateAttribute("office:value-type", GetNamespaceUri("office"));
        valueType.Value = "string";
        cellNode.Attributes.Append(valueType);
        cellValue = ownerDocument.CreateElement("text:p", GetNamespaceUri("text"));
        cellValue.InnerText = stunde.TerminBeschreibung;
        cellNode.AppendChild(cellValue);

        rowNode.AppendChild(cellNode);

        // Modul des Stundenentwurfs schreiben
        cellNode = ownerDocument.CreateElement("table:table-cell", GetNamespaceUri("table"));
        valueType = ownerDocument.CreateAttribute("office:value-type", GetNamespaceUri("office"));
        valueType.Value = "string";
        cellNode.Attributes.Append(valueType);
        cellValue = ownerDocument.CreateElement("text:p", GetNamespaceUri("text"));
        cellValue.InnerText = stunde.StundeModul.ModulBezeichnung;
        cellNode.AppendChild(cellValue);

        rowNode.AppendChild(cellNode);

        // Phasen des Stundenentwurfs schreiben
        cellNode = ownerDocument.CreateElement("table:table-cell", GetNamespaceUri("table"));
        valueType = ownerDocument.CreateAttribute("office:value-type", GetNamespaceUri("office"));
        valueType.Value = "string";
        cellNode.Attributes.Append(valueType);
        cellValue = ownerDocument.CreateElement("text:p", GetNamespaceUri("text"));
        cellValue.InnerText = stunde.StundePhasenLangform;
        cellNode.AppendChild(cellValue);

        rowNode.AppendChild(cellNode);

        // ggf. Datein für den Stundenentwurf schreiben
        cellNode = ownerDocument.CreateElement("table:table-cell", GetNamespaceUri("table"));
        valueType = ownerDocument.CreateAttribute("office:value-type", GetNamespaceUri("office"));
        valueType.Value = "string";
        cellNode.Attributes.Append(valueType);
        cellValue = ownerDocument.CreateElement("text:p", GetNamespaceUri("text"));
        cellValue.InnerText = stunde.StundeDateiliste; 
        cellNode.AppendChild(cellValue);

        rowNode.AppendChild(cellNode);

        sheetNode.AppendChild(rowNode);
      }

      sheetsRootNode.AppendChild(sheetNode);

      SaveContentXml(templateFile, contentXml);

      templateFile.Save(outputFilePath);

      App.OpenFile(outputFilePath);
    }

    private void WriteFloatCell(XmlDocument ownerDocument, XmlNode rowNode, string floatValue)
    {
      XmlElement cellNode = ownerDocument.CreateElement("table:table-cell", GetNamespaceUri("table"));
      XmlAttribute valueType = ownerDocument.CreateAttribute("office:value-type", GetNamespaceUri("office"));
      valueType.Value = "float";
      cellNode.Attributes.Append(valueType);
      XmlAttribute cellAttributeValue = ownerDocument.CreateAttribute("office:value", GetNamespaceUri("office"));
      cellAttributeValue.Value = floatValue;
      cellNode.Attributes.Append(cellAttributeValue);
      rowNode.AppendChild(cellNode);
    }

    private void WriteStringCell(XmlDocument ownerDocument, XmlNode rowNode, string cellContent, bool withColor = false)
    {
      XmlElement cellNode = ownerDocument.CreateElement("table:table-cell", GetNamespaceUri("table"));
      XmlAttribute valueType = ownerDocument.CreateAttribute("office:value-type", GetNamespaceUri("office"));
      valueType.Value = "string";
      cellNode.Attributes.Append(valueType);
      if (withColor)
      {
        XmlAttribute stylename = ownerDocument.CreateAttribute("table:style-name", GetNamespaceUri("table"));
        stylename.Value = "Notiz";
        cellNode.Attributes.Append(stylename);
      }
      XmlElement cellValue = ownerDocument.CreateElement("text:p", GetNamespaceUri("text"));
      cellValue.InnerText = cellContent;
      cellNode.AppendChild(cellValue);
      rowNode.AppendChild(cellNode);
    }

    private void WriteZeroCell(XmlDocument ownerDocument, XmlNode rowNode)
    {
      XmlElement cellNode = ownerDocument.CreateElement("table:table-cell", GetNamespaceUri("table"));
      XmlAttribute valueType = ownerDocument.CreateAttribute("office:value-type", GetNamespaceUri("office"));
      valueType.Value = "float";
      cellNode.Attributes.Append(valueType);
      XmlAttribute cellAttributeValue = ownerDocument.CreateAttribute("office:value", GetNamespaceUri("office"));
      cellAttributeValue.Value = "0";
      cellNode.Attributes.Append(cellAttributeValue);
      rowNode.AppendChild(cellNode);
    }

    private void SaveStyleSheet(XmlNode sheetsRootNode)
    {
      XmlDocument ownerDocument = sheetsRootNode.OwnerDocument;

      XmlNode sheetNode = ownerDocument.CreateElement("number:date-style", GetNamespaceUri("number"));

      XmlAttribute styleName = ownerDocument.CreateAttribute("style:name", GetNamespaceUri("style"));
      styleName.Value = "N19";
      sheetNode.Attributes.Append(styleName);

      XmlElement numberDay = ownerDocument.CreateElement("number:day", GetNamespaceUri("number"));
      XmlAttribute numberStyle = ownerDocument.CreateAttribute("number:style", GetNamespaceUri("number"));
      numberStyle.Value = "long";
      numberDay.Attributes.Append(numberStyle);
      sheetNode.AppendChild(numberDay);

      XmlElement numberText = ownerDocument.CreateElement("number:text", GetNamespaceUri("number"));
      numberText.InnerText = "/";
      sheetNode.AppendChild(numberText);

      XmlElement numberMonth = ownerDocument.CreateElement("number:month", GetNamespaceUri("number"));
      XmlAttribute numberStyle2 = ownerDocument.CreateAttribute("number:style", GetNamespaceUri("number"));
      numberStyle2.Value = "long";
      numberMonth.Attributes.Append(numberStyle2);
      sheetNode.AppendChild(numberMonth);

      XmlElement numberText2 = ownerDocument.CreateElement("number:text", GetNamespaceUri("number"));
      numberText2.InnerText = "/";
      sheetNode.AppendChild(numberText2);

      XmlElement numberYear = ownerDocument.CreateElement("number:year", GetNamespaceUri("number"));
      XmlAttribute numberStyle3 = ownerDocument.CreateAttribute("number:style", GetNamespaceUri("number"));
      numberStyle3.Value = "long";
      numberYear.Attributes.Append(numberStyle3);
      sheetNode.AppendChild(numberYear);

      sheetsRootNode.AppendChild(sheetNode);
    }

    private void SaveAutomaticStyleSheet(XmlNode sheetsRootNode)
    {
      XmlDocument ownerDocument = sheetsRootNode.OwnerDocument;

      XmlNode sheetNode = ownerDocument.CreateElement("style:style", GetNamespaceUri("style"));

      XmlAttribute styleName = ownerDocument.CreateAttribute("style:name", GetNamespaceUri("style"));
      styleName.Value = "ce2";
      sheetNode.Attributes.Append(styleName);

      XmlAttribute styleFamily = ownerDocument.CreateAttribute("style:family", GetNamespaceUri("style"));
      styleFamily.Value = "table-cell";
      sheetNode.Attributes.Append(styleFamily);

      XmlAttribute styleParentStyleName = ownerDocument.CreateAttribute("style:parent-style-name", GetNamespaceUri("style"));
      styleParentStyleName.Value = "Default";
      sheetNode.Attributes.Append(styleParentStyleName);

      XmlAttribute styleDataStyleName = ownerDocument.CreateAttribute("style:data-style-name", GetNamespaceUri("style"));
      styleDataStyleName.Value = "N19";
      sheetNode.Attributes.Append(styleDataStyleName);

      sheetsRootNode.AppendChild(sheetNode);
    }

    /// <summary>
    /// Writes DataSet as .ods file.
    /// </summary>
    /// <param name="odsFile">DataSet that represent .ods file.</param>
    /// <param name="outputFilePath">The name of the file to save to.</param>
    public void WriteOdsFile(DataSet odsFile, string outputFilePath)
    {
      ZipFile templateFile = GetZipFile(Assembly.GetExecutingAssembly().GetManifestResourceStream("Kurssystem.template.ods"));

      XmlDocument contentXml = GetContentXmlFile(templateFile);

      XmlNamespaceManager nmsManager = InitializeXmlNamespaceManager(contentXml);

      XmlNode sheetsRootNode = this.GetSheetsRootNodeAndRemoveChildrens(contentXml, nmsManager);

      foreach (DataTable sheet in odsFile.Tables)
        this.SaveSheet(sheet, sheetsRootNode);

      SaveContentXml(templateFile, contentXml);

      templateFile.Save(outputFilePath);
    }

    private XmlNode GetSheetsRootNodeAndRemoveChildrens(XmlDocument contentXml, XmlNamespaceManager nmsManager)
    {
      XmlNodeList tableNodes = GetTableNodes(contentXml, nmsManager);

      XmlNode sheetsRootNode = tableNodes.Item(0).ParentNode;
      // remove sheets from template file
      foreach (XmlNode tableNode in tableNodes)
        sheetsRootNode.RemoveChild(tableNode);

      return sheetsRootNode;
    }

    private void SaveSheet(DataTable sheet, XmlNode sheetsRootNode)
    {
      XmlDocument ownerDocument = sheetsRootNode.OwnerDocument;

      XmlNode sheetNode = ownerDocument.CreateElement("table:table", GetNamespaceUri("table"));

      XmlAttribute sheetName = ownerDocument.CreateAttribute("table:name", GetNamespaceUri("table"));
      sheetName.Value = sheet.TableName;
      sheetNode.Attributes.Append(sheetName);

      this.SaveColumnDefinition(sheet, sheetNode, ownerDocument);

      this.SaveRows(sheet, sheetNode, ownerDocument);

      sheetsRootNode.AppendChild(sheetNode);
    }

    private void SaveColumnDefinition(DataTable sheet, XmlNode sheetNode, XmlDocument ownerDocument)
    {
      XmlNode columnDefinition = ownerDocument.CreateElement("table:table-column", GetNamespaceUri("table"));

      XmlAttribute columnsCount = ownerDocument.CreateAttribute("table:number-columns-repeated", GetNamespaceUri("table"));
      columnsCount.Value = sheet.Columns.Count.ToString(CultureInfo.InvariantCulture);
      columnDefinition.Attributes.Append(columnsCount);

      sheetNode.AppendChild(columnDefinition);
    }

    private void SaveRows(DataTable sheet, XmlNode sheetNode, XmlDocument ownerDocument)
    {
      DataRowCollection rows = sheet.Rows;
      for (int i = 0; i < rows.Count; i++)
      {
        XmlNode rowNode = ownerDocument.CreateElement("table:table-row", GetNamespaceUri("table"));

        this.SaveCell(rows[i], rowNode, ownerDocument);

        sheetNode.AppendChild(rowNode);
      }
    }

    private void SaveCell(DataRow row, XmlNode rowNode, XmlDocument ownerDocument)
    {
      object[] cells = row.ItemArray;

      for (int i = 0; i < cells.Length; i++)
      {
        XmlElement cellNode = ownerDocument.CreateElement("table:table-cell", GetNamespaceUri("table"));

        if (row[i] != DBNull.Value)
        {
          // We save values as text (string)
          XmlAttribute valueType = ownerDocument.CreateAttribute("office:value-type", GetNamespaceUri("office"));
          valueType.Value = "string";
          cellNode.Attributes.Append(valueType);

          XmlElement cellValue = ownerDocument.CreateElement("text:p", GetNamespaceUri("text"));
          cellValue.InnerText = row[i].ToString();
          cellNode.AppendChild(cellValue);
        }

        rowNode.AppendChild(cellNode);
      }
    }

    private static void SaveContentXml(ZipFile templateFile, XmlDocument contentXml)
    {
      templateFile.RemoveEntry("content.xml");

      MemoryStream memStream = new MemoryStream();
      contentXml.Save(memStream);
      memStream.Seek(0, SeekOrigin.Begin);

      templateFile.AddEntry("content.xml", memStream);
    }

    private static string GetNamespaceUri(string prefix)
    {
      for (int i = 0; i < namespaces.GetLength(0); i++)
      {
        if (namespaces[i, 0] == prefix)
          return namespaces[i, 1];
      }

      throw new InvalidOperationException("Can't find that namespace URI");
    }
  }
}
