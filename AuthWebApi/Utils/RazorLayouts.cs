using RazorEngine;
using System.IO;
using System.Web;

namespace AuthWebApi.Utils
{
    public static class RazorLayouts
    {
        public static void AddMainaLayout()
        {
            AddLayout("_Layout.cshtml", "~/Views/Shared/_Layout.cshtml");
        }

        private static void AddLayout(string layoutName, string layoutPath)
        {
            string layoutContent = File.ReadAllText(HttpContext.Current.Server.MapPath(layoutPath));
            Razor.Compile(layoutContent, layoutName);
        }
    }
}