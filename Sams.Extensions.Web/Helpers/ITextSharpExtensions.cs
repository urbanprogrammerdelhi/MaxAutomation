using iTextSharp.text;
using iTextSharp.text.pdf;
using Sams.Extensions.Dal;
using Sams.Extensions.Model;
using Sams.Extensions.Utility;
using Sams.Extensions.Web.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace Sams.Extensions
{


    public static class ITextSharpExtensisos
    {


        private static readonly CellPadding TopLabelHeader = new CellPadding { Bottom = 10, Top = 5, Left = 250, Right = 0 };
        private static readonly System.Drawing.Color HeaderBackgroundColor = System.Drawing.Color.LightBlue;
        private static readonly CellPadding TopHeader = new CellPadding { Bottom = 10, Top = 5, Left = 10, Right = 0 };
        private static readonly System.Drawing.Color DetailBackgroundColor = System.Drawing.Color.White;
        private static readonly System.Drawing.Color GroupBackgroundColor = System.Drawing.Color.Gray;
        private static readonly CellPadding DetailDefaultPadding = new CellPadding { Bottom = 5, Top = 5, Left = 50, Right = 0 };
        private static readonly CellPadding DetailDefaultLongPadding = new CellPadding { Bottom = 10, Top = 10, Left = 30, Right = 10 };

        //public PdfReportBuilder(Document pdfDoc, IBranchCodeData branchCodeData)
        //{
        //    _branchCodeData = branchCodeData;
        //    _pdfDoc = pdfDoc;
        //}
        private static PdfPTable DefaultTable(int cellSize)
        {
            PdfPTable table = new PdfPTable(cellSize);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 1;
            table.SpacingBefore = 0f;
            table.SpacingAfter = 0f;
            return table;
        }
        private static PdfPCell DefaultCell(string cellValue)
        {
            var chunk = new Chunk(cellValue);
            var cell = new PdfPCell();
            cell.AddElement(chunk);
            return cell;
        }
        private static PdfPCell DefaultImageCell(string imageUrl)
        {
            PdfPCell imagecell = new PdfPCell();
            Image image = Image.GetInstance(imageUrl);
            image.ScaleAbsolute(100, 50);
            imagecell.AddElement(image);
            return imagecell;
        }

        private static PdfPCell DefaultImageCell(byte[] ImageStream, CellPadding padding)
        {

            PdfPCell imagecell = new PdfPCell();
            if (ImageStream != null)
            {
                Image image = Image.GetInstance(ImageStream);
                image.ScaleAbsolute(100, 50);
                imagecell.AddElement(image);
            }
            else
            {
                string defaultImage = ConfigurationManager.AppSettings["BaseUrl"].ToString() + @"NoImagesFound.jpg";
                Image image = Image.GetInstance(defaultImage);
                image.ScaleAbsolute(100, 40);
                imagecell.AddElement(image);
            }
            imagecell.PaddingTop = padding.Top;
            imagecell.PaddingBottom = padding.Bottom;
            imagecell.PaddingLeft = padding.Left;

            return imagecell;
        }
       
        private static PdfPCell HeaderCell(string cellValue, System.Drawing.Color color, CellPadding padding)
        {

            var chunk = new Chunk(cellValue);

            var cell = new PdfPCell();
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_MIDDLE;
            cell.PaddingTop = padding.Top;
            cell.PaddingBottom = padding.Bottom;
            cell.PaddingLeft = padding.Left;
            cell.BackgroundColor = new BaseColor(color);
            cell.AddElement(chunk);
            return cell;
        }
        private static PdfPCell DetailCell(string cellValue, System.Drawing.Color color, CellPadding padding)
        {

            var chunk = new Chunk(cellValue);

            var cell = new PdfPCell();
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_MIDDLE;
            cell.PaddingTop = padding.Top;
            cell.PaddingBottom = padding.Bottom;
            cell.PaddingLeft = padding.Left;
            cell.BackgroundColor = new BaseColor(color);
            cell.AddElement(chunk);
            return cell;
        }
       
        public static Document CreatePdfDocument<T>(this List<T> source,string[] requiredFields,Document document)
        {
            var result = DefaultTable(requiredFields.Length);
            foreach(var field in requiredFields)
            {
                result.AddCell(HeaderCell(field, HeaderBackgroundColor, TopHeader));
            }
            foreach (var row in source)
            {
                foreach (var field in requiredFields)
                {
                    var property = row.GetType().GetProperty(field.Replace(" ", string.Empty).Trim());
                    if (property != null)
                    {
                        if (property.PropertyType == typeof(byte[]))
                        {
                            result.AddCell(DefaultImageCell((byte[])property.GetValue(row, null), DetailDefaultLongPadding));
                        }
                        else
                        {
                            var txt = property.GetValue(row, null).ParseToText();
                            result.AddCell(DetailCell(txt, DetailBackgroundColor, DetailDefaultPadding));
                        }
                    }
                    else
                    {
                        result.AddCell(DetailCell(string.Empty, DetailBackgroundColor, DetailDefaultPadding));
                    }
                }
            }

            document.Add(result);
            return document;
        }
    }
    

}