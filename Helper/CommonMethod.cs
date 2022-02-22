using System;
using System.Collections.Generic;

namespace SIPERPUS.Helper
{
    public class CommonMethod
    {
        public static DateTime JakartaTimeZone(DateTime date)
        {
            TimeZoneInfo timeInfo = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime result = TimeZoneInfo.ConvertTime(date, timeInfo);
            return result;
        }
        public static string Alert(string type, string msg)
        {
            var alertHtml = "";
            switch (type)
            {
                case "success":
                    alertHtml = "<div class=\"alert alert-success d-flex align-items-center\" role=\"alert\">"
                                   + "<svg class=\"bi flex-shrink-0 me-2\" width=\"24\" height=\"24\" role=\"img\" aria-label=\"Success:\"><use xlink:href=\"#check-circle-fill\"/></svg>"
                                   + "<div>" + msg + "</div>"
                             + "</div>";
                    break;
                case "fail":
                    alertHtml = "<div class=\"alert alert-danger d-flex align-items-center\" role=\"alert\">"
                                   + "<svg class=\"bi flex-shrink-0 me-2\" width=\"24\" height=\"24\" role=\"img\" aria-label=\"Danger:\"><use xlink:href=\"#exclamation-triangle-fill\"/></svg>"
                                   + "<div>" + msg + "</div>"
                             + "</div>";
                    break;
            }
            return alertHtml;
        }
    }
}