using System;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

using WebStore.Domain.ViewModels;

namespace WebStore.TagHelpers
{
    public class Pagination : TagHelper
    {
        private readonly IUrlHelperFactory _UrlHelperFactory;

        public string PageAction { get; set; }

        [HtmlAttributeName(DictionaryAttributePrefix = "page-url-")]
        public Dictionary<string, object> PageUrlValues { get; set; } = new(StringComparer.OrdinalIgnoreCase);

        public PageViewModel PageModel { get; set; }

        [ViewContext, HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public Pagination(IUrlHelperFactory UrlHelperFactory) => _UrlHelperFactory = UrlHelperFactory;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var ul = new TagBuilder("ul");
            ul.AddCssClass("pagination");

            var urlHelper = _UrlHelperFactory.GetUrlHelper(ViewContext);
            for (var i = 1; i <= PageModel.TotalPages; i++)
                ul.InnerHtml.AppendHtml(CreateElement(i, urlHelper));

            output.Content.AppendHtml(ul);
        }

        private TagBuilder CreateElement(int PageNumber, IUrlHelper Url)
        {
            var li = new TagBuilder("li");
            var a = new TagBuilder("a");
            a.InnerHtml.AppendHtml(PageNumber.ToString());

            if (PageNumber == PageModel.Page)
                li.AddCssClass("active");
            else
            {
                PageUrlValues["page"] = PageNumber;
                a.Attributes["href"] = Url.Action(PageAction, PageUrlValues);
            }

            li.InnerHtml.AppendHtml(a);
            return li;
        }
    }
}