using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sams.Extensions.Utility
{
    public class HtmlUtilityConstants
    {
        public static readonly string DefaultHeaderFormat = "<th style='background-color: #B8DBFD;border: 1px solid #ccc'>{0}</th>";
        public static readonly string BeginTableTag = "<table border='1' cellpadding='1' cellspacing='1' style='border: 1px solid black;font-family: Verdana; font-size: 10pt; margin-top:3px;'>";
        public static readonly string EndTableTag = "</table>";
        public static readonly string BeginRowTag = "<tr>";
        public static readonly string EndRowTag = "</tr>";
        public static readonly string HtmlBeginTag = "<html>";
        public static readonly string HtmlEndTag = "</html>";
        public static readonly string BodyBeginTag = "<body>";
        public static readonly string BodyEndTag = "</body>";
        public static readonly string HeadBeginTag = "<head>";
        public static readonly string HeadEndTag = "</head>";
        public static readonly string ColumnFormat = "<td style='border: 1px solid #ccc;padding:5px;'>{0}</td>";
        public static readonly string ImageColumnFormat = "<td style='border: 1px solid #ccc;padding:5px;width:150px;height:150px;'>{0}</td>";

        public static readonly string ImageFormat = "<img height='100px' width='100px' src=\'data:image/jpg;base64,@Image ></img>";
        public static readonly string DefaultCellBeginFormat = "<td style='border: 1px solid #ccc;padding:5px;min-Width:300px;max-Width:400px;'>";
        public static readonly string DefaultImageCellBeginFormat = "<td style='border: 1px solid #ccc;padding:5px;width:150px;height:150px;'>";

        public static readonly string DefaultCellEndingTag = "</td>";
        public static readonly string ImageStartingTag = "<img height='75px' width='75px' src=\'data:image/jpg;base64,";
        public static readonly string ImageUrlFormat = "<img height='150px' width='150px' src=@imageUrl ></img>";
        public static readonly string noimageformat = "<img src ='ImageUrl' height='150px' width='150px' />";
        //public static readonly string FcaReport = "<td style='border: 1px solid #ccc;padding:5px;width:100%;'>";
        //public static readonly string DefaultCellBeginFormat = "<td style='border: 1px solid #ccc;padding:5px;width:100%;'>";
        //public static readonly string DefaultCellBeginFormat = "<td style='border: 1px solid #ccc;padding:5px;width:100%;'>";



    }
}
