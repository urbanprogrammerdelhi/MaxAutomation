using iTextSharp.text;
using iTextSharp.text.pdf;
using PdfDemo.Data;
using PdfDemo.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace PdfDemo
{


    public class PdfReportBuilder
    {
        Document _pdfDoc;
        private static readonly CellPadding TopLabelHeader = new CellPadding { Bottom = 10, Top = 5, Left = 250, Right = 0 };
        private static readonly System.Drawing.Color HeaderBackgroundColor = System.Drawing.Color.LightBlue;
        private static readonly CellPadding TopHeader = new CellPadding { Bottom = 10, Top = 5, Left = 40, Right = 0 };
        private static readonly System.Drawing.Color DetailBackgroundColor = System.Drawing.Color.White;
        private static readonly System.Drawing.Color GroupBackgroundColor = System.Drawing.Color.Gray;
        private static readonly CellPadding DetailDefaultPadding = new CellPadding { Bottom = 5, Top = 5, Left = 40, Right = 0 };
        private static readonly CellPadding DetailDefaultLongPadding = new CellPadding { Bottom = 10, Top = 10, Left = 10, Right = 10 };

        public PdfReportBuilder(Document pdfDoc)
        {
            _pdfDoc = pdfDoc;
        }
        private PdfPTable DefaultTable(int cellSize)
        {
            PdfPTable table = new PdfPTable(cellSize);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 1;
            table.SpacingBefore = 0f;
            table.SpacingAfter = 0f;
            return table;
        }
        private PdfPCell DefaultCell(string cellValue)
        {
            var chunk = new Chunk(cellValue);
            var cell = new PdfPCell();
            cell.AddElement(chunk);
            return cell;
        }
        private PdfPCell DefaultImageCell(string imageUrl)
        {
            PdfPCell imagecell = new PdfPCell();
            Image image = Image.GetInstance(imageUrl);
            image.ScaleAbsolute(100, 50);
            imagecell.AddElement(image);
            return imagecell;
        }

        private PdfPCell DefaultImageCell(byte[] ImageStream, CellPadding padding)
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
        public class CellPadding
        {
            public float Left { get; set; }
            public float Right { get; set; }

            public float Top { get; set; }

            public float Bottom { get; set; }


        }
        private PdfPCell HeaderCell(string cellValue, System.Drawing.Color color, CellPadding padding)
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
        private PdfPCell DetailCell(string cellValue, System.Drawing.Color color, CellPadding padding)
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
        public void CreateHeader(Reportheader header)
        {
            var headerLabel = DefaultTable(1);
            headerLabel.WidthPercentage = 100f;
            headerLabel.AddCell(HeaderCell("Max Audit Report", HeaderBackgroundColor, TopLabelHeader));
            _pdfDoc.Add(headerLabel);
            var topHeader = DefaultTable(4);
            topHeader.AddCell(HeaderCell($"Branch Code", HeaderBackgroundColor, TopHeader));
            topHeader.AddCell(HeaderCell($"Branch Name", HeaderBackgroundColor, TopHeader));
            topHeader.AddCell(HeaderCell($"FO Name", HeaderBackgroundColor, TopHeader));
            topHeader.AddCell(HeaderCell($"Audit date", HeaderBackgroundColor, TopHeader));

            _pdfDoc.Add(topHeader);
            var topHeaderValue = DefaultTable(4);
            topHeaderValue.AddCell(HeaderCell($"{header.BranchCode}", DetailBackgroundColor, TopHeader));
            topHeaderValue.AddCell(HeaderCell($"{header.BranchName}", DetailBackgroundColor, TopHeader));
            topHeaderValue.AddCell(HeaderCell($"{header.FOName}", DetailBackgroundColor, TopHeader));
            topHeaderValue.AddCell(HeaderCell($"{header.AuditDate}", DetailBackgroundColor, TopHeader));

            _pdfDoc.Add(topHeaderValue);
        }
        public void CreateDetails(ILookup<string, ReportBody> details)
        {
            var baseUrl = ConfigurationManager.AppSettings["BaseUrl"].ToString();
            var table = DefaultTable(5);
            table.AddCell(HeaderCell("S.No.", HeaderBackgroundColor, TopHeader));
            table.AddCell(HeaderCell("Question", HeaderBackgroundColor, TopHeader));
            table.AddCell(HeaderCell("Response", HeaderBackgroundColor, TopHeader));
            table.AddCell(HeaderCell("Photo", HeaderBackgroundColor, TopHeader));
            table.AddCell(HeaderCell("Comments", HeaderBackgroundColor, TopHeader));
            foreach (IGrouping<string, ReportBody> packageGroup in details)
            {
                string header = packageGroup.Key;//.Substring(0, packageGroup.Key.LastIndexOf(","));
                var cell = HeaderCell(header, GroupBackgroundColor, TopLabelHeader);
                cell.Colspan = 5;
                cell.HorizontalAlignment = Element.ALIGN_MIDDLE;
                table.AddCell(cell);
                foreach (ReportBody checkListItem in packageGroup)
                {
                    table.AddCell(DetailCell(checkListItem.ChecklistId.ToString(), DetailBackgroundColor, DetailDefaultPadding));
                    table.AddCell(DetailCell(checkListItem.SubHeader, DetailBackgroundColor, DetailDefaultLongPadding));
                    table.AddCell(DetailCell(checkListItem.Text, DetailBackgroundColor, DetailDefaultPadding));
                    var imageArray = DataAccessLayer.GetImageById(checkListItem.ImageAutoId);
                    table.AddCell(DefaultImageCell(imageArray, DetailDefaultLongPadding));
                    table.AddCell(DetailCell(checkListItem.Remarks, DetailBackgroundColor, DetailDefaultPadding));
                }
            }
            _pdfDoc.Add(table);
        }

    }


}