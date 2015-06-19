using System;
using Microsoft.AspNet.Razor.Runtime.TagHelpers;

namespace Sandbox.Web.TagHelpers
{
    [TargetElement("p", Attributes = "data-th-order-before")]
    public class OrderBeforeTagHelper : TagHelper
    {
        public override int Order
        {
            get
            {
                return -1;
            }
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            TagHelperAttribute style;
            if (output.Attributes.TryGetAttribute("style", out style))
            {
                return;
            }
            else
            { 
                output.Attributes.Add("style", "font-size: 24px;");
            }
        }
    }
}