using iTextSharp.text;
using iTextSharp.text.pdf;
using Sams.Extensions.Dal;
using Sams.Extensions.Model;
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


    public class PdfReportBuilder
    {
        private readonly IBranchCodeData _branchCodeData;
       
        Document _pdfDoc;
        private static readonly CellPadding TopLabelHeader = new CellPadding { Bottom = 10, Top = 5, Left = 250, Right = 0 };
        private static readonly System.Drawing.Color HeaderBackgroundColor = System.Drawing.Color.LightBlue;
        private static readonly CellPadding TopHeader = new CellPadding { Bottom = 10, Top = 5, Left = 10, Right = 0 };
        private static readonly System.Drawing.Color DetailBackgroundColor = System.Drawing.Color.White;
        private static readonly System.Drawing.Color GroupBackgroundColor = System.Drawing.Color.Gray;
        private static readonly CellPadding DetailDefaultPadding = new CellPadding { Bottom = 5, Top = 5, Left = 50, Right = 10 };
        private static readonly CellPadding DetailDefaultLongPadding = new CellPadding { Bottom = 10, Top = 10, Left = 30, Right = 10 };
        private static readonly CellPadding DetailDefaultRemarksPadding = new CellPadding { Bottom = 5, Top = 5, Left = 25, Right = 10 };


        public PdfReportBuilder(Document pdfDoc, IBranchCodeData branchCodeData)
        {
            _branchCodeData = branchCodeData;
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

        private PdfPCell DefaultImageCell(byte[] ImageStream, CellPadding padding,string viewMoreUrl)
        {
            PdfPCell imagecell = new PdfPCell();

            try
            {
                if (ImageStream != null)
                {
                    Image image = Image.GetInstance(ImageStream);
                    image.ScaleAbsolute(100, 50);
                    imagecell.AddElement(image);
                    Paragraph paragraph = new Paragraph();
                    Anchor anchor = new Anchor("View");
                    anchor.Reference = viewMoreUrl;
                    paragraph.Add(anchor);
                    imagecell.AddElement(paragraph);
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
            }
            catch(Exception ex)
            {

            }
            return imagecell;
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
            var topHeader = DefaultTable(string.IsNullOrEmpty(header.CheckListType)?4:5);
            topHeader.AddCell(HeaderCell($"Branch Code", HeaderBackgroundColor, TopHeader));
            topHeader.AddCell(HeaderCell($"Branch Name", HeaderBackgroundColor, TopHeader));
            topHeader.AddCell(HeaderCell($"FO Name", HeaderBackgroundColor, TopHeader));
            topHeader.AddCell(HeaderCell($"Audit date", HeaderBackgroundColor, TopHeader));
            if(!string.IsNullOrEmpty(header.CheckListType))
            {
                topHeader.AddCell(HeaderCell($"Checklist Type", HeaderBackgroundColor, TopHeader));

            }

            _pdfDoc.Add(topHeader);
            var topHeaderValue = DefaultTable(string.IsNullOrEmpty(header.CheckListType) ? 4 : 5);
            topHeaderValue.AddCell(HeaderCell($"{header.BranchCode}", DetailBackgroundColor, TopHeader));
            topHeaderValue.AddCell(HeaderCell($"{header.BranchName}", DetailBackgroundColor, TopHeader));
            topHeaderValue.AddCell(HeaderCell($"{header.FOName}", DetailBackgroundColor, TopHeader));
            topHeaderValue.AddCell(HeaderCell($"{header.AuditDate}", DetailBackgroundColor, TopHeader));
            if (!string.IsNullOrEmpty(header.CheckListType))
            {
                topHeaderValue.AddCell(HeaderCell($"{header.CheckListType}", DetailBackgroundColor, TopHeader));

            }
            _pdfDoc.Add(topHeaderValue);
        }
        public void CreateDetails(ILookup<string, ReportBody> details,string viewMoreUrl)
        {
            var baseUrl = ConfigurationManager.AppSettings["BaseUrl"].ToString();

            var table = DefaultTable(5);
            table.SetWidths(new float[] {10,30,18,25,17 });
            table.AddCell(HeaderCell("S.No.", HeaderBackgroundColor, new CellPadding { Bottom = 10, Top = 5, Left = 15, Right = 0 }));
            table.AddCell(HeaderCell("Question", HeaderBackgroundColor, new CellPadding { Bottom = 10, Top = 5, Left = 60, Right = 0 }));
            table.AddCell(HeaderCell("Response", HeaderBackgroundColor, new CellPadding { Bottom = 10, Top = 5, Left = 25, Right = 0 }));
            table.AddCell(HeaderCell("Photo", HeaderBackgroundColor, new CellPadding { Bottom = 10, Top = 5, Left = 60, Right = 0 }));
            table.AddCell(HeaderCell("Comments", HeaderBackgroundColor, new CellPadding { Bottom = 10, Top = 5, Left = 20, Right = 0 }));
            foreach (IGrouping<string, ReportBody> packageGroup in details)
            {
                try
                {
                    string header = packageGroup.Key;//.Substring(0, packageGroup.Key.LastIndexOf(","));
                    var cell = HeaderCell(header, GroupBackgroundColor, TopLabelHeader);
                    cell.Colspan = 5;
                    cell.HorizontalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);
                    foreach (ReportBody checkListItem in packageGroup)
                    {
                        table.AddCell(DetailCell(checkListItem.ChecklistId.ToString(), DetailBackgroundColor, new CellPadding { Bottom = 5, Top = 5, Left = 20, Right = 0 }));
                        table.AddCell(DetailCell(checkListItem.SubHeader, DetailBackgroundColor, DetailDefaultLongPadding));
                        table.AddCell(DetailCell(checkListItem.Text, DetailBackgroundColor, new CellPadding { Bottom = 5, Top = 5, Left = 40, Right = 0 }));
                        var newviewMoreUrl = viewMoreUrl + $"&CheckListId={checkListItem.NewChecklistId}";
                        var imageArray = _branchCodeData.GetImageById(checkListItem.ImageAutoId, !string.IsNullOrEmpty(checkListItem.ChecklistType));
                        table.AddCell(DefaultImageCell(imageArray, DetailDefaultLongPadding, newviewMoreUrl));
                        table.AddCell(DetailCell(checkListItem.Remarks, DetailBackgroundColor, DetailDefaultRemarksPadding));
                    }
                }
                catch
                {

                }
            }
            _pdfDoc.Add(table);
        }

    }


}