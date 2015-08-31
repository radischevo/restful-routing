﻿using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace RestfulRouting
{
    public static class HtmlHelperExtensions
    {
        public static MvcHtmlString PutOverrideTag(this HtmlHelper html)
        {
            return html.Hidden("X-Http-Method-Override", "put");
        }

        public static MvcHtmlString DeleteOverrideTag(this HtmlHelper html)
        {
            return html.Hidden("X-Http-Method-Override", "delete");
        }
    }
}
