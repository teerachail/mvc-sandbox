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
            string style;
            if (output.Attributes.TryGetValue("style", out style))
            {
                output.Attributes["style"] = style + "font-size: 6px;";
                return;
            }
        }
    }
}