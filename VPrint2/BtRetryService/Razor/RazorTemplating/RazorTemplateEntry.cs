using System;

namespace BtRetryService.Razor.RazorTemplating
{
    public class RazorTemplateEntry
    {
        public Type ModelType { get; set; }
        public string TemplateString { get; set; }
        public string TemplateName { get; set; }
    }
}
