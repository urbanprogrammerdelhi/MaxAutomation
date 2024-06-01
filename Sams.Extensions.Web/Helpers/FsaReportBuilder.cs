using iTextSharp.text;
using iTextSharp.text.pdf;
using Sams.Extensions.Business;
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


    public class FsaReportBuilder
    {
        private readonly IGroupLReportBusiness _groupLReportBusiness;

        Document _pdfDoc;
        private static readonly CellPadding TopLabelHeader = new CellPadding { Bottom = 10, Top = 5, Left = 250, Right = 0 };
        private static readonly System.Drawing.Color HeaderBackgroundColor = System.Drawing.Color.LightBlue;
        private static readonly CellPadding TopHeader = new CellPadding { Bottom = 10, Top = 5, Left = 10, Right = 0 };
        private static readonly System.Drawing.Color DetailBackgroundColor = System.Drawing.Color.White;
        private static readonly System.Drawing.Color GroupBackgroundColor = System.Drawing.Color.Gray;
        private static readonly CellPadding DetailDefaultPadding = new CellPadding { Bottom = 5, Top = 5, Left = 10, Right = 10 };
        private static readonly CellPadding DetailDefaultLongPadding = new CellPadding { Bottom = 10, Top = 10, Left = 10, Right = 10 };
        private static readonly CellPadding DetailDefaultRemarksPadding = new CellPadding { Bottom = 5, Top = 5, Left = 10, Right = 10 };

        private static readonly CellPadding DetailDefaultImagePadding = new CellPadding { Bottom = 5, Top = 5, Left = 20, Right = 10 };

        public FsaReportBuilder(Document pdfDoc, IGroupLReportBusiness groupLReportBusiness)
        {
            _groupLReportBusiness = groupLReportBusiness;
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

        private PdfPCell DefaultImageCell(string imageName, CellPadding padding)
        {
            PdfPCell imagecell = new PdfPCell();

            try
            {
                if (!string.IsNullOrEmpty(imageName))
                {
                    //var imageUrl = $"https://www.ifm360.in/APS/FSAImages/{imageName}";

                    //Image image = Image.GetInstance(new Uri(imageUrl, UriKind.RelativeOrAbsolute));

                    Image image = Image.GetInstance($@"{ConfigurationManager.AppSettings["FSAImagePath"]}\{imageName}");
                    image.ScaleToFit(120, 120);
                    imagecell.AddElement(image);
                    Paragraph paragraph = new Paragraph();
                    Anchor anchor = new Anchor("View");
                    anchor.Reference = $@"{ConfigurationManager.AppSettings["FsaReportImageViewer"].ParseToText()}?imageName={imageName}";
                    paragraph.Add(anchor);
                    imagecell.AddElement(paragraph);
                }
                else
                {
                    string defaultImage = ConfigurationManager.AppSettings["BaseUrl"].ToString() + @"NoImagesFound.jpg";
                    Image image = Image.GetInstance(defaultImage);
                    image.ScaleToFit(120,120);
                    imagecell.AddElement(image);
                }


                imagecell.PaddingTop = padding.Top;
                imagecell.PaddingBottom = padding.Bottom;
                imagecell.PaddingLeft = padding.Left;
            }
            catch (Exception ex)
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
        public void CreateHeader(FsaReportHeader header)
        {
            var headerLabel = DefaultTable(1);
            headerLabel.WidthPercentage = 100f;
            headerLabel.AddCell(HeaderCell("Fire Safety Audit Report", HeaderBackgroundColor, TopLabelHeader));
            _pdfDoc.Add(headerLabel);
            var currentTable = DefaultTable(2);
            currentTable.SetWidths(new float[] { 30, 70 });
            currentTable.AddCell(HeaderCell($"MLI Location Name & Code", HeaderBackgroundColor, TopHeader));
            currentTable.AddCell(HeaderCell($"Office Address", HeaderBackgroundColor, TopHeader));
            currentTable.AddCell(HeaderCell($"{header.ClientDetails}", DetailBackgroundColor, TopHeader));
            currentTable.AddCell(HeaderCell($"{header.OfficeAddress}", DetailBackgroundColor, TopHeader));
            _pdfDoc.Add(currentTable);
        }
        public void CreateDetails(List<FsaReportDetails> details, string viewMoreUrl)
        {
            var baseUrl = ConfigurationManager.AppSettings["BaseUrl"].ToString();

            var table = DefaultTable(5);
            table.SetWidths(new float[] { 8, 35, 15, 15, 27 });
            table.AddCell(HeaderCell("S.No.", HeaderBackgroundColor, new CellPadding { Bottom = 10, Top = 5, Left = 10, Right = 0 }));
            table.AddCell(HeaderCell("Category", HeaderBackgroundColor, new CellPadding { Bottom = 10, Top = 5, Left = 65, Right = 0 }));
            table.AddCell(HeaderCell("Audit", HeaderBackgroundColor, new CellPadding { Bottom = 10, Top = 5, Left = 25, Right = 0 }));
            table.AddCell(HeaderCell("Action", HeaderBackgroundColor, new CellPadding { Bottom = 10, Top = 5, Left = 25, Right = 0 }));
            table.AddCell(HeaderCell("Picture", HeaderBackgroundColor, new CellPadding { Bottom = 10, Top = 5, Left = 70, Right = 0 }));
            foreach (FsaReportDetails reportDetails in details)
            {

                table.AddCell(DetailCell(reportDetails.ShowID.ParseToText(), DetailBackgroundColor, new CellPadding { Bottom = 5, Top = 5, Left = 15, Right = 0 }));
                table.AddCell(DetailCell(reportDetails.Category.ParseToText(), DetailBackgroundColor, DetailDefaultLongPadding));
                table.AddCell(DetailCell(reportDetails.Audit.ParseToText(), DetailBackgroundColor, new CellPadding { Bottom = 5, Top = 5, Left = 20, Right = 0 }));
                table.AddCell(DetailCell(reportDetails.RequiredAction, DetailBackgroundColor, new CellPadding { Bottom = 5, Top = 5, Left = 25, Right = 10 }));
                table.AddCell(DefaultImageCell(reportDetails.Pictures.ParseToText(), new CellPadding { Bottom = 5, Top = 5, Left = 25, Right = 5 }));

            }
            _pdfDoc.Add(table);
        }
        public void CreateFooter(List<FsaReportFooter> footer)
        {
           

            var table = DefaultTable(3);
            table.SetWidths(new float[] { 10, 70, 20 });
            table.AddCell(HeaderCell("S.No.", HeaderBackgroundColor, new CellPadding { Bottom = 10, Top = 5, Left = 20, Right = 0 }));
            table.AddCell(HeaderCell("Category", HeaderBackgroundColor, new CellPadding { Bottom = 10, Top = 5, Left = 100, Right = 0 }));
            table.AddCell(HeaderCell("Qty", HeaderBackgroundColor, new CellPadding { Bottom = 10, Top = 5, Left = 40, Right = 0 }));

            foreach (FsaReportFooter reportFooter in footer)
            {

                table.AddCell(DetailCell(reportFooter.ChecklistID.ParseToText(), DetailBackgroundColor, new CellPadding { Bottom = 5, Top = 5, Left = 20, Right = 0 }));
                table.AddCell(DetailCell(reportFooter.Category.ParseToText(), DetailBackgroundColor, DetailDefaultLongPadding));
                table.AddCell(DetailCell(reportFooter.Qty.ParseToText(), DetailBackgroundColor, new CellPadding { Bottom = 5, Top = 5, Left = 40, Right = 0 }));
            }
            _pdfDoc.Add(table);
        }
        public void CreateBlankRow()
        {


            var table = DefaultTable(1);
            table.SetWidths(new float[] { 100 });
            _pdfDoc.Add(table);            
        }
    }


}