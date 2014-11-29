﻿using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace WebApplication.Helpers
{
    public static class SiteCheckboxHelper
    {
        /// <summary>
        /// Extension method to provide consistent check box layouts.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="expression"></param>
        /// <param name="help"></param>
        /// <returns></returns>
        public static MvcHtmlString SiteCheckbox<TModel, TValue>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TValue>> expression, string help)
        {
            //<div class="checkbox col-md-3 col-sm-6">
            //    <label>
            //        @Html.EditorFor(model => model.Gallery)
            //        @Html.LabelFor(model => model.Gallery)
            //        <a href="#" data-toggle="tooltip" data-title="Is there a gallery at your site?">
            //            <span class="glyphicon glyphicon-info-sign"></span>
            //        </a>
            //    </label>
            //</div>
            var divBuilder = new TagBuilder("div");
            divBuilder.AddCssClass("checkbox col-md-3 col-sm-6");

            var labelbuilder = new TagBuilder("label");
            labelbuilder.InnerHtml += htmlHelper.EditorFor(expression);
            labelbuilder.InnerHtml += " ";
            labelbuilder.InnerHtml += htmlHelper.LabelFor(expression);
            labelbuilder.InnerHtml += " ";

            var aBuilder = new TagBuilder("a");
            aBuilder.MergeAttribute("href", "#");

            var spanBuilder = new TagBuilder("span");
            spanBuilder.AddCssClass("glyphicon glyphicon-info-sign");
            spanBuilder.MergeAttribute("data-toggle", "tooltip");
            spanBuilder.MergeAttribute("data-title", help);

            aBuilder.InnerHtml += spanBuilder.ToString(TagRenderMode.SelfClosing);
            labelbuilder.InnerHtml += aBuilder.ToString();
            divBuilder.InnerHtml += labelbuilder.ToString();

            return MvcHtmlString.Create(divBuilder.ToString());
        }
    }
}