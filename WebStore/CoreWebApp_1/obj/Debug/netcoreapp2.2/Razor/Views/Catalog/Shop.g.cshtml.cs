#pragma checksum "E:\fallingsappy\Data\Programming\portfolio\WebStore\CoreWebApp_1\Views\Catalog\Shop.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "def1eeb402936209bfe8ed8306209abc9739effc"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Catalog_Shop), @"mvc.1.0.view", @"/Views/Catalog/Shop.cshtml")]
[assembly:global::Microsoft.AspNetCore.Mvc.Razor.Compilation.RazorViewAttribute(@"/Views/Catalog/Shop.cshtml", typeof(AspNetCore.Views_Catalog_Shop))]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#line 1 "E:\fallingsappy\Data\Programming\portfolio\WebStore\CoreWebApp_1\Views\_ViewImports.cshtml"
using WebStore.Models;

#line default
#line hidden
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"def1eeb402936209bfe8ed8306209abc9739effc", @"/Views/Catalog/Shop.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"67066408fc6b1bdc8b40a1cbe0d084334331fdab", @"/Views/_ViewImports.cshtml")]
    public class Views_Catalog_Shop : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<WebStore.Models.Product.CatalogViewModel>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#line 2 "E:\fallingsappy\Data\Programming\portfolio\WebStore\CoreWebApp_1\Views\Catalog\Shop.cshtml"
  
    ViewData["Title"] = "Shop | E-Shopper";

#line default
#line hidden
            BeginContext(101, 2, true);
            WriteLiteral("\r\n");
            EndContext();
            BeginContext(104, 54, false);
#line 6 "E:\fallingsappy\Data\Programming\portfolio\WebStore\CoreWebApp_1\Views\Catalog\Shop.cshtml"
Write(await Html.PartialAsync("Partial/Shop/_Advertisement"));

#line default
#line hidden
            EndContext();
            BeginContext(158, 123, true);
            WriteLiteral("\r\n\r\n<section>\r\n    <div class=\"container\">\r\n        <div class=\"row\">\r\n            <div class=\"col-sm-3\">\r\n                ");
            EndContext();
            BeginContext(282, 47, false);
#line 12 "E:\fallingsappy\Data\Programming\portfolio\WebStore\CoreWebApp_1\Views\Catalog\Shop.cshtml"
           Write(await Html.PartialAsync("Partial/_LeftSideBar"));

#line default
#line hidden
            EndContext();
            BeginContext(329, 90, true);
            WriteLiteral("\r\n            </div>\r\n\r\n            <div class=\"col-sm-9 padding-right\">\r\n                ");
            EndContext();
            BeginContext(420, 69, false);
#line 16 "E:\fallingsappy\Data\Programming\portfolio\WebStore\CoreWebApp_1\Views\Catalog\Shop.cshtml"
           Write(await Html.PartialAsync("Partial/Shop/_ProductItems", Model.Products));

#line default
#line hidden
            EndContext();
            BeginContext(489, 358, true);
            WriteLiteral(@"
                <ul class = ""pagination"">
                    <li class = ""active"" ><a href = """" > 1 </a></li>
                    <li><a href = """" > 2 </a></li>
                    <li><a href = """" > 3 </a></li>
                    <li><a href = """" > &raquo; </a></li>
                </ul>
            </div>
        </div>
    </div>
</section>");
            EndContext();
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<WebStore.Models.Product.CatalogViewModel> Html { get; private set; }
    }
}
#pragma warning restore 1591