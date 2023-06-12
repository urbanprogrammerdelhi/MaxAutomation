using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace PdfDemo
{
    public static class MvcExtensions
    {
        public static MvcHtmlString LabelWithColonFor<TModel, TValue>(
              this HtmlHelper<TModel> helper,
              Expression<Func<TModel, TValue>> expression)
        {
            
            return helper.LabelFor(expression, string.Format("{0}:",
                                                      helper.DisplayNameFor(expression)));
        }


        public static TRes Transform<TSrc, TRes>(this TSrc src, Func<TSrc, TRes> selector)
        {
            return selector(src);
        }
        public static List<SelectListItem> ToSelectList<T>(this List<T> list, string idPropertyName, string namePropertyName = "Name")
       where T : class, new()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();

            list.ForEach(item =>
            {
                selectListItems.Add(new SelectListItem
                {
                    Text = item.GetType().GetProperty(namePropertyName).GetValue(item).ToString(),
                    Value = item.GetType().GetProperty(idPropertyName).GetValue(item).ToString()
                });
            });

            return selectListItems;
        }
    }
}
