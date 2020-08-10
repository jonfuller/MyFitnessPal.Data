using System.Linq;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;

namespace MyFitnessPal.Data.Utility
{
    public static class LoginPageParser
    {
        public static bool IsLoggedIn(IDocument document) => document.Descendents<IHtmlAnchorElement>().Any(h => h.Href.EndsWith("/account/logout"));
    }
}