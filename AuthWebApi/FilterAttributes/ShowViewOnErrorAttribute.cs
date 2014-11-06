using System;

namespace AuthWebApi.FilterAttributes
{
    public class ShowViewOnErrorAttribute : Attribute
    {
        public string ControllerName { get; set; }

        public string ViewName { get; set; }
    }
}