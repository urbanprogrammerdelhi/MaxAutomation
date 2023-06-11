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
            return imagecell;        }

        private PdfPCell DefaultImageCell(byte[] ImageStream)
        {

            PdfPCell imagecell = new PdfPCell();
            if (ImageStream != null)
            {
                Image image = Image.GetInstance(ImageStream);
                image.ScaleAbsolute(100, 50);
                imagecell.AddElement(image);
            }
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
            cell.PaddingBottom =padding.Bottom;
            cell.PaddingLeft = padding.Left;
            cell.BackgroundColor = new BaseColor(color);
            cell.AddElement(chunk);
            return cell;
        }
        
        public void CreateHeader(Reportheader header)
        {
            var headerLabel = DefaultTable(1);
            headerLabel.WidthPercentage = 100f;
            headerLabel.AddCell(HeaderCell("Max Audit Report", System.Drawing.Color.LightBlue, new CellPadding { Bottom = 5, Top = 5, Left = 250, Right = 0 }));
            _pdfDoc.Add(headerLabel);
            var topHeader = DefaultTable(3);
            topHeader.AddCell(HeaderCell($"Branch Code", System.Drawing.Color.LightBlue, new CellPadding { Bottom = 5, Top = 5, Left = 40, Right = 0 }));
            topHeader.AddCell(HeaderCell($"Branch Name", System.Drawing.Color.LightBlue, new CellPadding { Bottom = 5, Top = 5, Left = 40, Right = 0 }));
            topHeader.AddCell(HeaderCell($"FO Name", System.Drawing.Color.LightBlue, new CellPadding { Bottom = 5, Top = 5, Left = 40, Right = 0 }));
            _pdfDoc.Add(topHeader);
            var topHeaderValue = DefaultTable(3);
            topHeaderValue.AddCell(HeaderCell($"{header.BranchCode}", System.Drawing.Color.White, new CellPadding { Bottom = 5, Top = 5, Left = 40, Right = 0 }));
            topHeaderValue.AddCell(HeaderCell($"{header.BranchName}", System.Drawing.Color.White, new CellPadding { Bottom = 5, Top = 5, Left = 40, Right = 0 }));
            topHeaderValue.AddCell(HeaderCell($"{header.FOName}", System.Drawing.Color.White, new CellPadding { Bottom = 5, Top = 5, Left = 40, Right = 0 }));
            _pdfDoc.Add(topHeaderValue);
        }
        public void CreateDetails(ILookup<string, ReportBody> details)
        {
            var baseUrl = ConfigurationManager.AppSettings["BaseUrl"].ToString();
            var table = DefaultTable(5);
            table.AddCell(HeaderCell("S.No.", System.Drawing.Color.LightBlue, new CellPadding { Bottom = 5, Top = 5, Left = 40, Right = 0 }));
            table.AddCell(HeaderCell("Question", System.Drawing.Color.LightBlue, new CellPadding { Bottom = 5, Top = 5, Left = 40, Right = 0 }));
            table.AddCell(HeaderCell("Response", System.Drawing.Color.LightBlue, new CellPadding { Bottom = 5, Top = 5, Left = 40, Right = 0 }));
            table.AddCell(HeaderCell("Photo", System.Drawing.Color.LightBlue, new CellPadding { Bottom = 5, Top = 5, Left = 40, Right = 0 }));
            table.AddCell(HeaderCell("Comments", System.Drawing.Color.LightBlue, new CellPadding { Bottom = 5, Top = 5, Left = 40, Right = 0 }));
            foreach (IGrouping<string, ReportBody> packageGroup in details)
            {
                string header = packageGroup.Key.Substring(0, packageGroup.Key.LastIndexOf(","));
                var cell = HeaderCell(header,System.Drawing.Color.Gray, new CellPadding { Bottom = 5, Top =5, Left = 250, Right = 0 });
                cell.Colspan = 5;
                cell.HorizontalAlignment = Element.ALIGN_MIDDLE;
                table.AddCell(cell);
                foreach (ReportBody checkListItem in packageGroup)
                {
                    table.AddCell(DefaultCell(checkListItem.ChecklistId.ToString()));
                    table.AddCell(DefaultCell(checkListItem.SubHeader));
                    table.AddCell(DefaultCell(checkListItem.Text));
                    var imageArray = DataAccessLayer.GetImageById(checkListItem.ImageAutoId);
                    table.AddCell(DefaultImageCell(imageArray));
                    table.AddCell(DefaultCell(checkListItem.Remarks));                   
                }
            }
            _pdfDoc.Add(table);
        }

    }

  
}