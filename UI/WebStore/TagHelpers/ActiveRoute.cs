using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace WebStore.TagHelpers
{
    [HtmlTargetElement(Attributes = AttributeName)]
    public class ActiveRoute : TagHelper
    {
        private const string AttributeName = "ws-is-active-route";

        private const string IgnoreAction = "ws-ignore-action";

        [HtmlAttributeName("ws-is-active-route-active")]
        public string ActiveCssClass { get; set; } = "active";

        [HtmlAttributeName("asp-controller")]
        public string Controller { get; set; }

        [HtmlAttributeName("asp-action")]
        public string Action { get; set; }

        [HtmlAttributeName("asp-all-route-data", DictionaryAttributePrefix = "asp-route-")]
        public Dictionary<string, string> RouteValues { get; set; } = new(StringComparer.OrdinalIgnoreCase);

        [ViewContext, HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var isIgnoreAction = output.Attributes.RemoveAll(IgnoreAction);

            if (IsActive(isIgnoreAction))
                MakeActive(output);

            output.Attributes.RemoveAll(AttributeName);
        }

        private bool IsActive(bool IgnoreAction)
        {
            var routeValues = ViewContext.RouteData.Values;

            var routeController = routeValues["controller"]?.ToString();
            var routeAction = routeValues["action"]?.ToString();

            if (!IgnoreAction && Action is { Length: > 0 } action && !string.Equals(action, routeAction))
                return false;

            if (Controller is { Length: > 0 } controller && !string.Equals(controller, routeController))
                return false;

            foreach (var (key, value) in RouteValues)
                if (!routeValues.ContainsKey(key) || routeValues[key]?.ToString() != value)
                    return false;

            return true;
        }

        private void MakeActive(TagHelperOutput output)
        {
            var classAttribute = output.Attributes.FirstOrDefault(attr => attr.Name == "class");

            if (classAttribute is null)
                output.Attributes.Add("class", ActiveCssClass);
            else
            {
                if (classAttribute.Value?.ToString()?.Contains(ActiveCssClass) ?? false)
                    return;

                output.Attributes.SetAttribute("class", $"{classAttribute.Value} {ActiveCssClass}");
            }
        }
    }
}