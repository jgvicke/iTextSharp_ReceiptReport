using System;
using System.Collections.Generic;
using System.Linq;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace PDF_Report
{
    public static class PdfExtentions
    {
        public static PdfPTable CreateTable(string title, float[] columnWidths)
        {
            var table = new PdfPTable(numColumns: columnWidths.Length)
            {
                WidthPercentage = 100f,
                DefaultCell =
                {
                    Padding = 4,
                    PaddingLeft = 5,
                    PaddingRight = 5,
                    HorizontalAlignment = 0,
                    VerticalAlignment = 1
                }
            };

            table.AddCell(new PdfPCell
            {
                Padding = 4,
                PaddingLeft = 5,
                PaddingRight = 5,
                BackgroundColor = BaseColor.LIGHT_GRAY,
                Phrase = new Phrase(title, FontFactory.GetFont("Courier", 9, BaseColor.BLACK)),
                Colspan = columnWidths.Length
            });

            var totalSize = columnWidths.AsEnumerable().Sum();
            columnWidths[columnWidths.Length - 1] += 100f - totalSize;

            table.SetWidths(columnWidths);

            return table;
        }

        public static PdfPTable CreateTable(string title, int columns)
        {
            var columnWidth = (float)(100.0 / columns);
            var columnWidthList = new List<float>(Enumerable.Repeat(columnWidth, columns));
            return CreateTable(title, columnWidthList.ToArray());
        }

        public static PdfPTable CreateTable(string title, List<string> headers, float[] columnWidths = null)
        {
            PdfPTable table;

            if (columnWidths != null)
            {
                if (headers.Count > columnWidths.Length)
                    return null;

                table = CreateTable(title, columnWidths);
            }
            else
            {
                table = CreateTable(title, headers.Count);
            }

            for (var i = 0; i < headers.Count; i++)
            {
                var header = headers[i];

                var colspan = 1;
                while (i + colspan < headers.Count && headers[i + colspan] == null)
                    colspan++;

                table.AddCell(new PdfPCell
                {
                    Padding = 4,
                    PaddingLeft = 5,
                    PaddingRight = 5,
                    BackgroundColor = BaseColor.LIGHT_GRAY,
                    Phrase = new Phrase(header, FontFactory.GetFont("Courier", 9, BaseColor.BLACK)),
                    Colspan = colspan
                });

                i += colspan - 1;
            }

            return table;
        }

        public static bool AddRow(this PdfPTable table, string[] columnData)
        {
            var blankCols = table.NumberOfColumns - columnData.Length;
            if (blankCols < 0)
                return false;

            foreach (var data in columnData)
            {
                table.AddCell(new Phrase(data, FontFactory.GetFont("Courier", 7, BaseColor.BLACK)));
            }
            for (var i = 0; i < blankCols; i++)
            {
                table.AddCell(new Phrase(""));
            }

            return true;
        }

        public static void AddTable(this Document document, PdfPTable table)
        {
            document.Add(table);
            document.Add(new Phrase("\r\n", FontFactory.GetFont("Courier", 4, BaseColor.BLACK)));
        }

    }
}
