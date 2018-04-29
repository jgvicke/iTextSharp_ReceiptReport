using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Collections.Generic;
using System.Net;
using System.Drawing.Imaging;
using System;
using System.Linq;

namespace PDF_Report
{
    class Program
    {
        static void Main(string[] args)
        {
            var fileStream = new FileStream("test.pdf", FileMode.Create);
            var document = new Document(PageSize.LETTER);
            var writer = PdfWriter.GetInstance(document, fileStream);

            var pageLandscape = false;
            var pageHeight = 680;
            var pageWidth = 520;

            document.Open();
            document.AddAuthor("Document Author");
            document.AddCreator("Sample application using iTextSharp");
            document.AddKeywords("PDF tutorial education");
            document.AddSubject("Document subject - Describing the steps creating a PDF document");
            document.AddTitle("The document title - PDF creation using iTextSharp");

            if (pageLandscape)
            {
                var temp = pageWidth;
                pageWidth = pageHeight;
                pageHeight = temp;
                document.SetPageSize(PageSize.LETTER.Rotate());
            }

            document.NewPage();
            PdfPTable table;
            Image imagePDF;
            
            var images = new List<System.Drawing.Image>();
            var names = new List<string>();
            var files = Directory.GetFiles(@"..\..\Images");
            for (var i = 0; i < files.Length; i++)
            {
                if (!files[i].Contains("Large"))
                    continue;

                var image = System.Drawing.Image.FromFile(files[i]);

                images.Add(ResizeImage(image, 1600));
                names.Add(Path.GetFileName(files[i]));
            }

            for (var i = 0; i < images.Count; i++)
            {
                document.NewPage();
                table = PdfExtentions.CreateTable($"Reciept #{i + 1}: {names[i]}", 1);

                imagePDF = Image.GetInstance(images[i], System.Drawing.Imaging.ImageFormat.Jpeg);

                // if aspectRatio > 1 then it is a horizontal image, if < 1 it is portrait
                var aspectRatio = imagePDF.Width / (double)imagePDF.Height;
                var aspectRatioTable = pageWidth / (double)pageHeight;

                if (aspectRatio > aspectRatioTable)
                {
                    // image is wider than page, so scale off the width
                    imagePDF.ScaleAbsoluteWidth(pageWidth-20);
                    imagePDF.ScaleAbsoluteHeight(imagePDF.Height * imagePDF.ScaledWidth / imagePDF.Width);
                }
                else
                {
                    // image is taller than page, so scale off the height
                    imagePDF.ScaleAbsoluteHeight(pageHeight-20);
                    imagePDF.ScaleAbsoluteWidth(imagePDF.Width * imagePDF.ScaledHeight / imagePDF.Height);
                }

                imagePDF.CompressionLevel = PdfStream.BEST_COMPRESSION;

                table.AddCell(new PdfPCell(imagePDF)
                {
                    Padding = 10,
                    HorizontalAlignment = 1,
                    VerticalAlignment = 1,
                    FixedHeight = pageHeight
                });

                document.Add(table);
            }
            
            document.Close();
            writer.Close();
            fileStream.Close();

            System.Diagnostics.Process.Start("test.pdf");

        }

        static private System.Drawing.Image ResizeImage(System.Drawing.Image image, int maxDimension = 800)
        {
            // if aspectRatio > 1 then it is a horizontal image, if < 1 it is portrait
            var aspectRatio = image.Width / (double)image.Height;

            // scale the image
            var maxDimensionCurrent = (double)Math.Max(image.Width, image.Height);
            var scaleFactor = maxDimension / maxDimensionCurrent;
            image = new System.Drawing.Bitmap(image, new System.Drawing.Size((int)(image.Width * scaleFactor), (int)(image.Height * scaleFactor)));
            
            //// rotate if needed
            //if ((horizontal && aspectRatio < 1) || (!horizontal && aspectRatio > 1))
            //    image.RotateFlip(System.Drawing.RotateFlipType.Rotate90FlipNone);

            return image;
        }

    }
}
