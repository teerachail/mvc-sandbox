using System;
using Microsoft.AspNet.Razor.Runtime.TagHelpers;

namespace Sandbox.Web.TagHelpers
{
    [TargetElement("p", Attributes = "data-th-order-after")]
    public class OrderAfterTagHelper : TagHelper
    {
        public override int Order
        {
            get
            {
                return 1;
            }
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            TagHelperAttribute style;
            if (output.Attributes.TryGetAttribute("style", out style))
            {
                output.Attributes["style"] = style + "font-size: 6px;";
                return;
            }
        }
    }
}