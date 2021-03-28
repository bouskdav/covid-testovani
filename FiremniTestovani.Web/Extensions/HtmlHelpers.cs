using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace FiremniTestovani.Web.Extensions
{
    public static class HtmlHelpers
    {
        public static IHtmlContent DisplayStringOrNullValue(this IHtmlHelper helper, string id)
        {
            TagBuilder tb = new TagBuilder("input");
            tb.Attributes.Add("type", "file");
            tb.Attributes.Add("id", id);
            return new HtmlString(tb.ToString());
        }

        public static IHtmlContent CheckBoxSimple(this IHtmlHelper htmlHelper, string name)
        {
            TextWriter writer = new StringWriter();

            IHtmlContent html = htmlHelper.CheckBox(name);
            html.WriteTo(writer, HtmlEncoder.Default);

            string checkBoxWithHidden = writer.ToString();

            var hiddenIndex = checkBoxWithHidden.IndexOf("<input", 1);

            if (hiddenIndex == -1)
                return html;

            string pureCheckBox = checkBoxWithHidden.Substring(0, checkBoxWithHidden.IndexOf("<input", 1));
            return new HtmlString(pureCheckBox);
        }

        public static IHtmlContent CheckBoxSimple(this IHtmlHelper htmlHelper, string name, bool isChecked)
        {
            TextWriter writer = new StringWriter();

            IHtmlContent html = htmlHelper.CheckBox(name, isChecked);
            html.WriteTo(writer, HtmlEncoder.Default);

            string checkBoxWithHidden = writer.ToString();

            var hiddenIndex = checkBoxWithHidden.IndexOf("<input", 1);

            if (hiddenIndex == -1)
                return html;

            string pureCheckBox = checkBoxWithHidden.Substring(0, checkBoxWithHidden.IndexOf("<input", 1));
            return new HtmlString(pureCheckBox);
        }

        public static IHtmlContent CheckBoxSimple(this IHtmlHelper htmlHelper, string name, object htmlAttributes)
        {
            TextWriter writer = new StringWriter();

            IHtmlContent html = htmlHelper.CheckBox(name, htmlAttributes);
            html.WriteTo(writer, HtmlEncoder.Default);

            string checkBoxWithHidden = writer.ToString();

            var hiddenIndex = checkBoxWithHidden.IndexOf("<input", 1);

            if (hiddenIndex == -1)
                return html;

            string pureCheckBox = checkBoxWithHidden.Substring(0, checkBoxWithHidden.IndexOf("<input", 1));
            return new HtmlString(pureCheckBox);
        }

        public static IHtmlContent CheckBoxSimple(this IHtmlHelper htmlHelper, string name, bool isChecked, object htmlAttributes)
        {
            TextWriter writer = new StringWriter();

            IHtmlContent html = htmlHelper.CheckBox(name, isChecked, htmlAttributes);
            html.WriteTo(writer, HtmlEncoder.Default);

            string checkBoxWithHidden = writer.ToString();

            var hiddenIndex = checkBoxWithHidden.IndexOf("<input", 1);

            if (hiddenIndex == -1)
                return html;

            string pureCheckBox = checkBoxWithHidden.Substring(0, checkBoxWithHidden.IndexOf("<input", 1));
            return new HtmlString(pureCheckBox);
        }

        public static IHtmlContent CheckBoxSimpleFor<TModel>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, bool>> expression)
        {
            TextWriter writer = new StringWriter();

            IHtmlContent html = htmlHelper.CheckBoxFor(expression);
            html.WriteTo(writer, HtmlEncoder.Default);

            string checkBoxWithHidden = writer.ToString();

            var hiddenIndex = checkBoxWithHidden.IndexOf("<input", 1);

            if (hiddenIndex == -1)
                return html;

            string pureCheckBox = checkBoxWithHidden.Substring(0, checkBoxWithHidden.IndexOf("<input", 1));
            return new HtmlString(pureCheckBox);
        }

        public static IHtmlContent CheckBoxSimpleFor<TModel>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, bool>> expression, object htmlAttributes)
        {
            TextWriter writer = new StringWriter();

            IHtmlContent html = htmlHelper.CheckBoxFor(expression, htmlAttributes);
            html.WriteTo(writer, HtmlEncoder.Default);

            string checkBoxWithHidden = writer.ToString();

            var hiddenIndex = checkBoxWithHidden.IndexOf("<input", 1);

            if (hiddenIndex == -1)
                return html;

            string pureCheckBox = checkBoxWithHidden.Substring(0, checkBoxWithHidden.IndexOf("<input", 1));
            return new HtmlString(pureCheckBox);
        }

        //public static string Content(this UrlHelper urlHelper, string contentPath, bool toAbsolute = false)
        //{
        //    var path = urlHelper.Content(contentPath);
        //    var url = new Uri(Context.Request.Url, path);

        //    return toAbsolute ? url.AbsoluteUri : path;
        //}

        public static IHtmlContent TextBoxWithDescriptionFor<TModel, TValue>(this IHtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression)
        {
            ModelExpressionProvider expressionProvider = new ModelExpressionProvider(helper.MetadataProvider);
            ModelExpression modelExpression = expressionProvider.CreateModelExpression(helper.ViewData, expression);
            ModelMetadata metaData = modelExpression.Metadata;

            Dictionary<string, object> attributes = new Dictionary<string, object>();

            if (metaData.DisplayName != null)
            {
                attributes.Add("data-title", metaData.DisplayName);
            }

            if (metaData.Description != null)
            {
                attributes.Add("placeholder", metaData.Description);
            }

            return helper.TextBoxFor(expression, attributes);
            //return InputExtensions.TextBoxFor(helper, expression, attributes);
        }

        public static IHtmlContent TextBoxWithDescriptionFor<TModel, TValue>(this IHtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression, object htmlAttributes)
        {
            ModelExpressionProvider expressionProvider = new ModelExpressionProvider(helper.MetadataProvider);
            ModelExpression modelExpression = expressionProvider.CreateModelExpression(helper.ViewData, expression);
            ModelMetadata metaData = modelExpression.Metadata;

            Type objType = htmlAttributes.GetType();
            var propertyNames = objType.GetProperties();

            Dictionary<string, object> attributes = new Dictionary<string, object>();

            foreach (var propName in propertyNames)
            {
                attributes.Add(propName.Name.Replace("_", "-"), propName.GetValue(htmlAttributes));
            }

            if (metaData.DisplayName != null)
            {
                attributes.Add("data-title", metaData.DisplayName);
            }

            if (metaData.Description != null)
            {
                attributes.Add("placeholder", (string)metaData.Description);
            }

            return helper.TextBoxFor(expression, attributes);
        }

        public static IHtmlContent TextAreaWithDescriptionFor<TModel, TValue>(this IHtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression)
        {
            ModelExpressionProvider expressionProvider = new ModelExpressionProvider(helper.MetadataProvider);
            ModelExpression modelExpression = expressionProvider.CreateModelExpression(helper.ViewData, expression);
            ModelMetadata metaData = modelExpression.Metadata;

            Dictionary<string, object> attributes = new Dictionary<string, object>();

            if (metaData.DisplayName != null)
            {
                attributes.Add("data-title", metaData.DisplayName);
            }

            if (metaData.Description != null)
            {
                attributes.Add("placeholder", (string)metaData.Description);
            }

            return helper.TextAreaFor(expression, attributes);
        }

        public static IHtmlContent TextAreaWithDescriptionFor<TModel, TValue>(this IHtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression, object htmlAttributes)
        {
            ModelExpressionProvider expressionProvider = new ModelExpressionProvider(helper.MetadataProvider);
            ModelExpression modelExpression = expressionProvider.CreateModelExpression(helper.ViewData, expression);
            ModelMetadata metaData = modelExpression.Metadata;

            Type objType = htmlAttributes.GetType();
            var propertyNames = objType.GetProperties();

            Dictionary<string, object> attributes = new Dictionary<string, object>();

            foreach (var propName in propertyNames)
            {
                attributes.Add(propName.Name.Replace("_", "-"), propName.GetValue(htmlAttributes));
            }

            if (metaData.DisplayName != null)
            {
                attributes.Add("data-title", metaData.DisplayName);
            }

            if (metaData.Description != null)
            {
                attributes.Add("placeholder", (string)metaData.Description);
            }

            return helper.TextAreaFor(expression, attributes);
        }

        public static IHtmlContent DescriptionFor<TModel, TValue>(this IHtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression)
        {
            ModelExpressionProvider expressionProvider = new ModelExpressionProvider(helper.MetadataProvider);
            ModelExpression modelExpression = expressionProvider.CreateModelExpression(helper.ViewData, expression);
            ModelMetadata metaData = modelExpression.Metadata;

            return new HtmlString(metaData.Description);
        }

        ////public static IHtmlContent UpdatableHiddenFor<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        ////{
        ////    ReplacePropertyState(htmlHelper, expression);
        ////    return htmlHelper.HiddenFor(expression);
        ////}

        ////public static IHtmlContent UpdatableHiddenFor<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes)
        ////{
        ////    ReplacePropertyState(htmlHelper, expression);
        ////    return htmlHelper.HiddenFor(expression, htmlAttributes);
        ////}

        ////public static IHtmlContent UpdatableHiddenFor<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IDictionary<string, object> htmlAttributes)
        ////{
        ////    ReplacePropertyState(htmlHelper, expression);
        ////    return htmlHelper.HiddenFor(expression, htmlAttributes);
        ////}

        //private static void ReplacePropertyState<TModel, TProperty>(HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression)
        //{
        //    ModelExpressionProvider expressionProvider = new ModelExpressionProvider(helper.MetadataProvider);
        //    ModelExpression modelExpression = expressionProvider.CreateModelExpression(helper.ViewData, expression);
        //    ModelMetadata metaData = modelExpression.Metadata;

        //    string text = expressionProvider.GetExpressionText(expression);
        //    string fullName = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(text);
        //    ModelStateDictionary modelState = helper.ViewContext.ViewData.ModelState;

        //    if (modelState.ContainsKey(fullName))
        //    {
        //        var currentValue = modelState[fullName].RawValue;
        //        modelState[fullName].RawValue = metaData

        //        ValueProviderResult currentValue = modelState[fullName].Value;
        //        modelState[fullName].Value = new ValueProviderResult(metadata.Model, Convert.ToString(metadata.Model), currentValue.Culture);
        //    }
        //}
    }
}
