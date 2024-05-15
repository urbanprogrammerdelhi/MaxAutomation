using ClosedXML.Excel;
using Sams.Extensions.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sams.Extensions.Utility
{
    public static class Extensions
    {
       
        public static int ParseInt(this string input)
        {
            if (int.TryParse(input, out int output))
            {
                return output;
            }
            return -1;
        }
        public static string NullifEmpty(this string input)
        {
            return input.NullIf(x => string.IsNullOrEmpty(x) || x.Trim().Length <= 0);
        }
        public static T NullIf<T>(this T @this, Func<T, bool> predicate) where T : class
        {
            if (predicate(@this))
            {
                return null;
            }
            return @this;
        }
        private static T CreateItemFromRow<T>(DataRow row, List<PropertyInfo> properties) where T : new()
        {
            T item = new T();
            foreach (var property in properties)
            {
                if (row.Table.Columns.Contains(property.Name))
                {
                    if (row[property.Name] != DBNull.Value)
                        property.SetValue(item, row[property.Name], null);
                }
            }
            return item;
        }
        public static T ConvertFromDataRow<T>(this DataRow row) where T : new()
        {
            T item = new T();
            List<PropertyInfo> properties = typeof(T).GetProperties().ToList();

            foreach (var property in properties)
            {
                if (row.Table.Columns.Contains(property.Name))
                {
                    if (row[property.Name] != DBNull.Value)
                        property.SetValue(item, row[property.Name], null);
                }
            }
            return item;
        }
        public static List<T> ToList<T>(this DataTable table) where T : new()
        {
            List<PropertyInfo> properties = typeof(T).GetProperties().ToList();
            List<T> results = new List<T>();

            foreach (var row in table.Rows)
            {
                var item = CreateItemFromRow<T>((DataRow)row, properties);
                results.Add(item);
            }
            return results;
        }

        public static string GenerateHtmlTable<T>(this List<T> list, IEnumerable<string> requiredFields = null) where T : new()
        {
            try
            {
                if (list == null || list.Count <= 0) { return string.Empty; }
                if (requiredFields == null || requiredFields.Count() <= 0)
                {
                    requiredFields = typeof(T).GetProperties().Select(p => p.Name);
                }
                StringBuilder sb = new StringBuilder();
                sb.Append(HtmlUtilityConstants.HtmlBeginTag)
                    .Append(HtmlUtilityConstants.HeadBeginTag)
                    .Append(HtmlUtilityConstants.BodyBeginTag);
                sb.Append(HtmlUtilityConstants.BeginTableTag).Append(HtmlUtilityConstants.BeginRowTag);
                foreach (var field in requiredFields)
                {
                    sb.Append(string.Format(HtmlUtilityConstants.DefaultHeaderFormat, field));
                }
                sb.Append(HtmlUtilityConstants.EndRowTag);
                int counter = 1;
                foreach (var row in list)
                {
                    sb.Append(HtmlUtilityConstants.BeginRowTag);
                    foreach (var field in requiredFields)
                    {

                        var property = row.GetType().GetProperty(field.Replace(" ", string.Empty).Trim());
                        if (property != null)
                        {
                            if (property.PropertyType == typeof(byte[]))
                            {
                                var image = property.GetValue(row, null);
                                if (image != null)
                                {
                                    sb.Append(HtmlUtilityConstants.DefaultCellBeginFormat);
                                    sb.Append(HtmlUtilityConstants.ImageStartingTag);
                                    sb.Append(Convert.ToBase64String((byte[])property.GetValue(row, null)) + "' />");
                                    sb.Append(HtmlUtilityConstants.DefaultCellEndingTag);
                                }
                                else
                                {
                                    sb.Append(string.Format(HtmlUtilityConstants.ColumnFormat, property.GetValue(row, null)));
                                }
                            }
                            else
                            {
                                sb.Append(string.Format(HtmlUtilityConstants.ColumnFormat, property.GetValue(row, null)));
                            }
                        }
                        else
                        {

                            sb.Append(string.Format(HtmlUtilityConstants.ColumnFormat, string.Empty));

                        }
                    }
                    sb.Append(HtmlUtilityConstants.EndRowTag);
                    counter++;
                }
                sb.Append(HtmlUtilityConstants.EndTableTag).Append(HtmlUtilityConstants.BodyEndTag).Append(HtmlUtilityConstants.HeadEndTag).Append(HtmlUtilityConstants.HtmlEndTag);
                return sb.ToString();

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static T ConvertFromExcelRow<T>(this IXLCells cells, string[] VerifyingColumns) where T : new()
        {
            try
            {
                T item = new T();
                List<PropertyInfo> properties = typeof(T).GetProperties().ToList();
                string verifyingString = string.Join(",", VerifyingColumns);
                var comparisonString = verifyingString.Replace(" ", string.Empty);
                comparisonString = comparisonString.Replace(".", string.Empty);
                var comparisionArray = comparisonString.Split(',').ToList();

                foreach (var property in properties)
                {
                    string propertyName = property.Name.Replace("Day", string.Empty).Trim();
                    int index = comparisionArray.ToList().IndexOf(propertyName);
                    if (index >= 0)
                    {
                        if (index < cells.ToList().Count)
                        {
                            if (cells.ToList()[index] != null && cells.ToList()[index].Value != DBNull.Value)
                            {
                                property.SetValue(item, cells.ToList()[index].Value.ToString(), null);
                            }
                        }
                        else
                        {
                            property.SetValue(item, string.Empty, null);

                        }
                    }
                }
                return item;
            }
            catch (Exception ex)
            {
                throw;
            }
        }




        public static int ParseToInteger(this string str)
        {
            if (int.TryParse(str, out int result)) return result;
            return 0;
        }
        public static int ParseToInteger(this Object obj)
        {
            if (obj == null) return -1;
            if (int.TryParse(obj.ParseToText(), out int result)) return result;
            return -1;
        }
        public static string ParseToText(this Object obj)
        {
            if (obj == null)
                return string.Empty;
            return Convert.ToString(obj);
        }
        public static dynamic[] ToPivotArray<T, TColumn, TRow, TData>(
this IEnumerable<T> source,
Func<T, TColumn> columnSelector,
Expression<Func<T, TRow>> rowSelector,
Func<IEnumerable<T>, TData> dataSelector)
        {

            var arr = new List<object>();
            var cols = new List<string>();
            String rowName = ((MemberExpression)rowSelector.Body).Member.Name;
            var columns = source.Select(columnSelector).Distinct();

            cols = (new[] { rowName }).Concat(columns.Select(x => x.ToString())).ToList();


            var rows = source.GroupBy(rowSelector.Compile())
                             .Select(rowGroup => new
                             {
                                 Key = rowGroup.Key,
                                 Values = columns.GroupJoin(
                                     rowGroup,
                                     c => c,
                                     r => columnSelector(r),
                                     (c, columnGroup) => dataSelector(columnGroup))
                             }).ToArray();


            foreach (var row in rows)
            {
                var items = row.Values.Cast<object>().ToList();
                items.Insert(0, row.Key);
                var obj = GetAnonymousObject(cols, items);
                arr.Add(obj);
            }
            return arr.ToArray();
        }
        private static dynamic GetAnonymousObject(IEnumerable<string> columns, IEnumerable<object> values)
        {
            IDictionary<string, object> eo = new ExpandoObject() as IDictionary<string, object>;
            int i;
            for (i = 0; i < columns.Count(); i++)
            {
                eo.Add(columns.ElementAt<string>(i), values.ElementAt<object>(i));
            }
            return eo;
        }

        public static string GenerateHtmlTableV2<T>(this List<T> list, IEnumerable<string> requiredFields = null, IEnumerable<string> comparisionFields = null) where T : new()
        {
            try
            {
                if (list == null || list.Count <= 0) { return string.Empty; }
                if (requiredFields == null || requiredFields.Count() <= 0)
                {
                    requiredFields = typeof(T).GetProperties().Select(p => p.Name);
                }
                StringBuilder sb = new StringBuilder();
                sb.Append(HtmlUtilityConstants.HtmlBeginTag)
                    .Append(HtmlUtilityConstants.HeadBeginTag)
                    .Append(HtmlUtilityConstants.BodyBeginTag);
                sb.Append(HtmlUtilityConstants.BeginTableTag).Append(HtmlUtilityConstants.BeginRowTag);
                foreach (var field in requiredFields)
                {
                    sb.Append(string.Format(HtmlUtilityConstants.DefaultHeaderFormat, field));
                }
                sb.Append(HtmlUtilityConstants.EndRowTag);
                int counter = 1;
                foreach (var row in list)
                {
                    sb.Append(HtmlUtilityConstants.BeginRowTag);
                    foreach (var field in comparisionFields)
                    {
                        var property = row.GetType().GetProperty(field.Replace(" ", string.Empty).Trim());
                        if (property != null)
                        {
                            if (property.PropertyType == typeof(byte[]))
                            {
                                var image = property.GetValue(row, null);
                                if (image != null)
                                {
                                    sb.Append(HtmlUtilityConstants.DefaultCellBeginFormat);
                                    sb.Append(HtmlUtilityConstants.ImageStartingTag);
                                    sb.Append(Convert.ToBase64String((byte[])image) + "' />");
                                    sb.Append(HtmlUtilityConstants.DefaultCellEndingTag);
                                }
                                else
                                {
                                    sb.Append(string.Format(HtmlUtilityConstants.ColumnFormat, property.GetValue(row, null)));
                                }
                            }
                            else
                            {
                                sb.Append(string.Format(HtmlUtilityConstants.ColumnFormat, property.GetValue(row, null)));
                            }
                        }
                        else
                        {

                            sb.Append(string.Format(HtmlUtilityConstants.ColumnFormat, string.Empty));

                        }
                    }
                    sb.Append(HtmlUtilityConstants.EndRowTag);
                    counter++;
                }
                sb.Append(HtmlUtilityConstants.EndTableTag).Append(HtmlUtilityConstants.BodyEndTag).Append(HtmlUtilityConstants.HeadEndTag).Append(HtmlUtilityConstants.HtmlEndTag);
                return sb.ToString();

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> sourceList, int ListSize)
        {
            while (sourceList.Any())
            {
                yield return sourceList.Take(ListSize);
                sourceList = sourceList.Skip(ListSize);
            }
        }

        public static string ConcatinateCells(this ClosedXML.Excel.IXLRow row)
        {
            var cells = row.Cells(false, XLCellsUsedOptions.All)
                .Where(cl => !string.IsNullOrEmpty(cl.Value.ParseToText())).Select(cl1 => cl1.Value.ParseToText());
            return string.Join(",", cells);
        }
        public static string RemoveDateColumnName(this string columnName)
        {
            if (columnName.Contains("_"))
            {
                return columnName.Substring(columnName.LastIndexOf("_") + 1);
            }
            return columnName;
        }


        public static string GenerateSimpleHeader<T>(this T item,IEnumerable<string> requiredFields, IEnumerable<string> comparisionFields) where T:new()
        {
            try
            {
                if (item == null) { return string.Empty; }
                if (requiredFields == null || requiredFields.Count() <= 0)
                {
                    requiredFields = typeof(T).GetProperties().Select(p => p.Name);
                }
                StringBuilder sb = new StringBuilder();
                int counter = 0;
                sb.Append(HtmlUtilityConstants.BeginTableTag);
                foreach (var field in comparisionFields)
                {
                    sb.Append(HtmlUtilityConstants.BeginRowTag);
                    var fieldName = field.Replace(" ", string.Empty).Trim();
                    var property = item.GetType().GetProperty(fieldName);
                    if (property != null)
                    {
                        sb.Append(string.Format(HtmlUtilityConstants.ColumnFormat, $"{requiredFields.ToArray()[counter]}:"));
                        if (property.PropertyType == typeof(byte[]))
                        {
                            var image = property.GetValue(item, null);
                            if (image != null)
                            {
                                sb.Append(HtmlUtilityConstants.DefaultCellBeginFormat);
                                sb.Append(HtmlUtilityConstants.ImageStartingTag);
                                sb.Append(Convert.ToBase64String((byte[])image) + "' />");
                                sb.Append(HtmlUtilityConstants.DefaultCellEndingTag);
                            }
                            else
                            {
                                sb.Append(string.Format(HtmlUtilityConstants.ColumnFormat, property.GetValue(item, null)));
                            }
                        }
                        else
                        {
                            sb.Append(string.Format(HtmlUtilityConstants.ColumnFormat, property.GetValue(item, null)));
                        }
                    }
                    else
                    {

                        sb.Append(string.Format(HtmlUtilityConstants.ColumnFormat, string.Empty));

                    }
                    sb.Append(HtmlUtilityConstants.EndRowTag);
                    counter++;
                }              

                sb.Append(HtmlUtilityConstants.EndTableTag);
                return sb.ToString();

            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public static string GenerateHtmlTableV4<TAny>(this List<TAny> list, IEnumerable<string> requiredFields = null, IEnumerable<string> comparisionFields = null) //--where TAny : class()
        {
            try
            {
                if (list == null || list.Count <= 0) { return string.Empty; }
                if (requiredFields == null || requiredFields.Count() <= 0)
                {
                    requiredFields = typeof(TAny).GetProperties().Select(p => p.Name);
                }
                StringBuilder sb = new StringBuilder();
                sb.Append(HtmlUtilityConstants.BeginTableTag).Append(HtmlUtilityConstants.BeginRowTag);
                foreach (var field in requiredFields)
                {
                    sb.Append(string.Format(HtmlUtilityConstants.DefaultHeaderFormat, field));
                }
                sb.Append(HtmlUtilityConstants.EndRowTag);
                int counter = 1;
                foreach (var row in list)
                {
                    sb.Append(HtmlUtilityConstants.BeginRowTag);
                    foreach (var field in comparisionFields)
                    {
                        var property = row.GetType().GetProperty(field.Replace(" ", string.Empty).Trim());
                        if (property != null)
                        {
                            if (property.PropertyType == typeof(byte[]))
                            {
                                var image = property.GetValue(row, null);
                                if (image != null)
                                {
                                    sb.Append(HtmlUtilityConstants.DefaultCellBeginFormat);
                                    sb.Append(HtmlUtilityConstants.ImageStartingTag);
                                    sb.Append(Convert.ToBase64String((byte[])image) + "' />");
                                    sb.Append(HtmlUtilityConstants.DefaultCellEndingTag);
                                }
                                else
                                {
                                    sb.Append(string.Format(HtmlUtilityConstants.ColumnFormat, property.GetValue(row, null)));
                                }
                            }
                            else if (property.PropertyType == typeof(string) && property.Name.Contains("Pictures"))
                            {
                                var image = property.GetValue(row, null);
                                if (image != null && !string.IsNullOrEmpty(image.ParseToText())) 
                                {
                                    sb.Append(HtmlUtilityConstants.DefaultImageCellBeginFormat);
                                    var imageCell = HtmlUtilityConstants.ImageUrlFormat;
                                    imageCell = imageCell.Replace("@imageUrl",$"https://www.ifm360.in/APS/FSAImages/{image}");
                                    sb.Append(imageCell);
                                    sb.Append(HtmlUtilityConstants.DefaultCellEndingTag);
                                }
                                else
                                {
                                    var noImageFormat = HtmlUtilityConstants.noimageformat.Replace("ImageUrl", ConfigurationManager.AppSettings["BaseUrl"] + "NoImagesFound.jpg");
                                    sb.Append(string.Format(HtmlUtilityConstants.ImageColumnFormat, noImageFormat));
                                }
                            }
                          
                            else
                            {
                                sb.Append(string.Format(HtmlUtilityConstants.ColumnFormat, property.GetValue(row, null)));
                            }
                        }
                        else
                        {

                            sb.Append(string.Format(HtmlUtilityConstants.ColumnFormat, string.Empty));

                        }
                    }
                    sb.Append(HtmlUtilityConstants.EndRowTag);
                    counter++;
                }
                sb.Append(HtmlUtilityConstants.EndTableTag);
                return sb.ToString();

            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public static string GenerateFsaHeader(this List<FsaReportHeader> list, IEnumerable<string> requiredFields = null, IEnumerable<string> comparisionFields = null) //--where TAny : class()
        {
            try
            {
                if (list == null || list.Count <= 0) { return string.Empty; }
                if (requiredFields == null || requiredFields.Count() <= 0)
                {
                    requiredFields = typeof(FsaReportHeader).GetProperties().Select(p => p.Name);
                }
                StringBuilder sb = new StringBuilder();
                sb.Append(HtmlUtilityConstants.FsaHeaderTableBeginFormat).Append(HtmlUtilityConstants.BeginRowTag);
                foreach (var field in requiredFields)
                {
                    sb.Append(string.Format(HtmlUtilityConstants.FsaHeaderThBeginFormat, field));
                }
                sb.Append(HtmlUtilityConstants.EndRowTag);
                int counter = 1;
                foreach (var row in list)
                {
                    sb.Append(HtmlUtilityConstants.BeginRowTag);
                    foreach (var field in comparisionFields)
                    {
                        var property = row.GetType().GetProperty(field.Replace(" ", string.Empty).Trim());
                        if (property != null)
                        {  
                           sb.Append(string.Format(HtmlUtilityConstants.ColumnFormat, property.GetValue(row, null)));                           
                        }
                        else
                        {
                            sb.Append(string.Format(HtmlUtilityConstants.ColumnFormat, string.Empty));

                        }
                    }
                    sb.Append(HtmlUtilityConstants.EndRowTag);
                    counter++;
                }
                sb.Append(HtmlUtilityConstants.EndTableTag);
                return sb.ToString();

            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public static string GenerateFsaDetails(this List<FsaReportDetails> list, IEnumerable<string> requiredFields = null, IEnumerable<string> comparisionFields = null) //--where TAny : class()
        {
            try
            {
                if (list == null || list.Count <= 0) { return string.Empty; }
                if (requiredFields == null || requiredFields.Count() <= 0)
                {
                    requiredFields = typeof(FsaReportHeader).GetProperties().Select(p => p.Name);
                }
                StringBuilder sb = new StringBuilder();
                sb.Append(HtmlUtilityConstants.FsaHeaderTableBeginFormat).Append(HtmlUtilityConstants.BeginRowTag);

                foreach (var field in requiredFields)
                {
                    var columnSize = 10;
                    switch(field)
                    {
                        case "SlNo.":
                            columnSize = 10;
                            break;
                        case "Category":
                            columnSize = 40;
                            break;
                        case "Audit":
                            columnSize = 15;
                            break;
                        case "Required Action":
                            columnSize = 10;
                            break;
                        case "Pictures":
                            columnSize = 35;
                            break;
                    }
                    
                    sb.Append(string.Format(HtmlUtilityConstants.FsaHeaderDetailsHeaderBeginFormat,columnSize, field));
                }
                sb.Append(HtmlUtilityConstants.EndRowTag);
                int counter = 1;
                foreach (var row in list)
                {
                    sb.Append(HtmlUtilityConstants.BeginRowTag);
                    foreach (var field in comparisionFields)
                    {
                        var property = row.GetType().GetProperty(field.Replace(" ", string.Empty).Trim());
                        if (property != null)
                        {
                            if (property.PropertyType == typeof(byte[]))
                            {
                                var image = property.GetValue(row, null);
                                if (image != null)
                                {
                                    sb.Append(HtmlUtilityConstants.DefaultCellBeginFormat);
                                    sb.Append(HtmlUtilityConstants.ImageStartingTag);
                                    sb.Append(Convert.ToBase64String((byte[])image) + "' />");
                                    sb.Append(HtmlUtilityConstants.DefaultCellEndingTag);
                                }
                                else
                                {
                                    sb.Append(string.Format(HtmlUtilityConstants.ColumnFormat, property.GetValue(row, null)));
                                }
                            }
                            else if (property.PropertyType == typeof(string) && property.Name.Contains("Pictures"))
                            {
                                var image = property.GetValue(row, null);
                                if (image != null && !string.IsNullOrEmpty(image.ParseToText()))
                                {
                                    sb.Append(HtmlUtilityConstants.DefaultImageCellBeginFormat);
                                    var imageCell = HtmlUtilityConstants.ImageUrlFormat;
                                    //imageCell = imageCell.Replace("@imageUrl", $"https://www.ifm360.in/APS/FSAImages/{image}");
                                    imageCell = imageCell.Replace("@imageUrl", $"{ConfigurationManager.AppSettings["FSAImagePath"].ParseToText()}/{image}");

                                    sb.Append(imageCell);
                                    sb.Append(HtmlUtilityConstants.DefaultCellEndingTag);
                                }
                                else
                                {
                                    var noImageFormat = HtmlUtilityConstants.noimageformat.Replace("ImageUrl", ConfigurationManager.AppSettings["BaseUrl"] + "NoImagesFound.jpg");
                                    sb.Append(string.Format(HtmlUtilityConstants.ImageColumnFormat, noImageFormat));
                                }
                            }

                            else
                            {
                                sb.Append(string.Format(HtmlUtilityConstants.ColumnFormat, property.GetValue(row, null)));
                            }
                        }
                        else
                        {

                            sb.Append(string.Format(HtmlUtilityConstants.ColumnFormat, string.Empty));

                        }
                    }
                    sb.Append(HtmlUtilityConstants.EndRowTag);
                    counter++;
                }
                sb.Append(HtmlUtilityConstants.EndTableTag);
                return sb.ToString();

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static string GenerateFsaFooter(this List<FsaReportFooter> list, IEnumerable<string> requiredFields = null, IEnumerable<string> comparisionFields = null) //--where TAny : class()
        {
            try
            {
                if (list == null || list.Count <= 0) { return string.Empty; }
                if (requiredFields == null || requiredFields.Count() <= 0)
                {
                    requiredFields = typeof(FsaReportHeader).GetProperties().Select(p => p.Name);
                }
                StringBuilder sb = new StringBuilder();
                sb.Append(HtmlUtilityConstants.FsaHeaderTableBeginFormat).Append(HtmlUtilityConstants.BeginRowTag);

                foreach (var field in requiredFields)
                {
                    var columnSize = 10;
                    switch (field)
                    {
                        case "SlNo.":
                            columnSize = 10;
                            break;
                        case "Category":
                            columnSize = 80;
                            break;
                        case "Qty":
                            columnSize = 10;
                            break;
                       
                    }

                    sb.Append(string.Format(HtmlUtilityConstants.FsaHeaderDetailsHeaderBeginFormat, columnSize, field));
                }
                sb.Append(HtmlUtilityConstants.EndRowTag);
                int counter = 1;
                foreach (var row in list)
                {
                    sb.Append(HtmlUtilityConstants.BeginRowTag);
                    foreach (var field in comparisionFields)
                    {
                        var property = row.GetType().GetProperty(field.Replace(" ", string.Empty).Trim());
                        if (property != null)
                        {
                            if (property.PropertyType == typeof(byte[]))
                            {
                                var image = property.GetValue(row, null);
                                if (image != null)
                                {
                                    sb.Append(HtmlUtilityConstants.DefaultCellBeginFormat);
                                    sb.Append(HtmlUtilityConstants.ImageStartingTag);
                                    sb.Append(Convert.ToBase64String((byte[])image) + "' />");
                                    sb.Append(HtmlUtilityConstants.DefaultCellEndingTag);
                                }
                                else
                                {
                                    sb.Append(string.Format(HtmlUtilityConstants.ColumnFormat, property.GetValue(row, null)));
                                }
                            }
                            else if (property.PropertyType == typeof(string) && property.Name.Contains("Pictures"))
                            {
                                var image = property.GetValue(row, null);
                                if (image != null && !string.IsNullOrEmpty(image.ParseToText()))
                                {
                                    sb.Append(HtmlUtilityConstants.DefaultImageCellBeginFormat);
                                    var imageCell = HtmlUtilityConstants.ImageUrlFormat;
                                    imageCell = imageCell.Replace("@imageUrl", $"https://www.ifm360.in/APS/FSAImages/{image}");
                                    sb.Append(imageCell);
                                    sb.Append(HtmlUtilityConstants.DefaultCellEndingTag);
                                }
                                else
                                {
                                    var noImageFormat = HtmlUtilityConstants.noimageformat.Replace("ImageUrl", ConfigurationManager.AppSettings["BaseUrl"] + "NoImagesFound.jpg");
                                    sb.Append(string.Format(HtmlUtilityConstants.ImageColumnFormat, noImageFormat));
                                }
                            }

                            else
                            {
                                sb.Append(string.Format(HtmlUtilityConstants.ColumnFormat, property.GetValue(row, null)));
                            }
                        }
                        else
                        {

                            sb.Append(string.Format(HtmlUtilityConstants.ColumnFormat, string.Empty));

                        }
                    }
                    sb.Append(HtmlUtilityConstants.EndRowTag);
                    counter++;
                }
                sb.Append(HtmlUtilityConstants.EndTableTag);
                return sb.ToString();

            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
