using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Razor.Runtime.TagHelpers;

namespace Sandbox.Web.TagHelpers
{
    [TargetElement("p", Attributes = "data-th-color-or-size1")]
    [TargetElement(Attributes = "data-th-color-or-size2")]
    public class ColorOrSizeTagHelper : TagHelper
    {
        [HtmlAttributeName("data-th-color-or-size1")]
        public string Style1 { get; set; }

        [HtmlAttributeName("data-th-color-or-size2")]
        public string Style2 { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.Attributes.Add("style", Style1 + " " + Style2);
        }
    }
}