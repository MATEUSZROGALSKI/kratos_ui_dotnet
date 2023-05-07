using Identity.Models;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

using Ory.Kratos.Client.Model;

namespace Identity.Extensions;

internal static class HtmlExtensions
{
    public static IHtmlContent OpenDiv(this IHtmlHelper helper, string styles) => helper.Raw($"<div class=\"{styles}\">");
    public static IHtmlContent CloseDiv(this IHtmlHelper helper) => helper.Raw("</div>");

    public static IHtmlContent BeginForm(this IHtmlHelper helper, string action, string method, string styles) => helper.Raw($@"<form class=""{styles}"" action=""{action}"" method=""{method}"">");
    public static IHtmlContent EndForm(this IHtmlHelper helper) => helper.Raw("</form>");

    public static IHtmlContent RenderCheckboxField(this IHtmlHelper helper, KratosUiNodeInputAttributes attr) =>
        helper.Partial("_CheckboxField", attr);

    public static IHtmlContent RenderHiddenField(this IHtmlHelper helper, KratosUiNodeInputAttributes attr) =>
        helper.Partial("_HiddenField", attr);
    
    public static IHtmlContent RenderInputField(this IHtmlHelper helper, KratosUiNodeInputAttributes attr, string text) => 
        helper.Partial("_InputField", new InputModel(attr, text));

    public static IHtmlContent RenderButton(this IHtmlHelper helper, KratosUiNodeInputAttributes attr, string text, string styles) =>
        helper.Partial("_Button", new InputModel(attr, text));

    public static IHtmlContent RenderImage(this IHtmlHelper helper, KratosUiNodeImageAttributes attr, string styles) => helper.Raw($@"<img class=""{styles}"" src=""{attr.Src}"" id=""{attr.Id}"" width=""{attr.Width}"" height=""{attr.Height}"" />");
    public static IHtmlContent RenderAnchor(this IHtmlHelper helper, KratosUiNodeAnchorAttributes attr, string text, string styles) => helper.Raw($@"<a class=""{styles}"" href=""{attr.Href}"" id=""{attr.Id}"" title=""{attr.Title}"">{text}</a>");
    public static IHtmlContent RenderScript(this IHtmlHelper helper, KratosUiNodeScriptAttributes attr) => helper.Raw($@"<script src=""{attr.Src}"" {(attr.Async ? "async" : "")} type=""{attr.Type}""></script>");
    public static IHtmlContent RenderText(this IHtmlHelper helper, KratosUiNodeTextAttributes attr, string styles) => helper.Raw($@"<p class=""{styles}"" id=""{attr.Id}"">{attr.Text.Text}</p>");

    public static IHtmlContent RenderLabel(this IHtmlHelper helper, string @for, string text) => helper.Raw($@"<label for=""{@for}""><span></span>{text}</label>");

    public static IHtmlContent RenderInputNode(this IHtmlHelper helper, KratosUiNodeInputAttributes attr, string text)
    {
        IHtmlContentBuilder builder = new HtmlContentBuilder();
        switch (attr.Type)
        {
            case KratosUiNodeInputAttributes.TypeEnum.Password:
            case KratosUiNodeInputAttributes.TypeEnum.Email:
            case KratosUiNodeInputAttributes.TypeEnum.Text:
            case KratosUiNodeInputAttributes.TypeEnum.Url:
            case KratosUiNodeInputAttributes.TypeEnum.Tel:
            case KratosUiNodeInputAttributes.TypeEnum.Number:
                builder.AppendHtml(helper.RenderInputField(attr, text));
                break;

            case KratosUiNodeInputAttributes.TypeEnum.Hidden:
                builder.AppendHtml(helper.RenderHiddenField(attr));
                break;

            case KratosUiNodeInputAttributes.TypeEnum.Checkbox:
                builder.AppendHtml(helper.OpenDiv("relative w-full"));
                builder.AppendHtml(helper.RenderCheckboxField(attr));
                builder.AppendHtml(helper.RenderLabel(attr.Name, text));
                builder.AppendHtml(helper.CloseDiv());
                break;

            case KratosUiNodeInputAttributes.TypeEnum.Submit:
                builder.AppendHtml(helper.OpenDiv("relative w-full flex flex-row justify-end"));
                builder.AppendHtml(helper.RenderButton(attr, text, "flex justify-center bg-purple-800 hover:bg-purple-700 text-gray-100 p-3 rounded-lg tracking-wide font-semibold  cursor-pointer transition ease-in duration-500"));
                builder.AppendHtml(helper.CloseDiv());
                break;
        }
        return builder;
    }

    public static IHtmlContent RenderKratosNodes(this IHtmlHelper helper, params KratosUiNode[] nodes)
    {
        IHtmlContentBuilder builder = new HtmlContentBuilder();
        foreach (var node in nodes)
        {
            switch (node.Type)
            {
                case KratosUiNode.TypeEnum.Input:
                    builder.AppendHtml(
                        helper.RenderInputNode(node.Attributes.GetKratosUiNodeInputAttributes(), node.Meta?.Label?.Text ?? ""));
                    break;

                case KratosUiNode.TypeEnum.A:
                    builder.AppendHtml(
                        helper.RenderAnchor(
                            node.Attributes.GetKratosUiNodeAnchorAttributes(),
                            node.Meta?.Label?.Text ?? "",
                            ""));
                    break;

                case KratosUiNode.TypeEnum.Text:
                    builder.AppendHtml(
                        helper.RenderText(
                            node.Attributes.GetKratosUiNodeTextAttributes(),
                            ""));
                    break;

                case KratosUiNode.TypeEnum.Img:
                    builder.AppendHtml(
                        helper.RenderImage(
                            node.Attributes.GetKratosUiNodeImageAttributes(),
                            ""));
                    break;

                case KratosUiNode.TypeEnum.Script:
                    builder.AppendHtml(
                        helper.RenderScript(
                            node.Attributes.GetKratosUiNodeScriptAttributes()));
                    break;
            }
        }
        return builder;
    }
    
    public static IHtmlContent BuildKratosUi(this IHtmlHelper helper, KratosUiContainer kratosUi)
    {
        var groups = kratosUi.Nodes
                .GroupBy(node => node.Group)
                .ToDictionary(k => k.Key, k => k.ToArray());

        IHtmlContentBuilder builder = new HtmlContentBuilder();
        builder.AppendHtml(helper.Raw($@"<form class=""space-y-6 w-full flex-col justify-evenly items-center"" action=""{kratosUi.Action}"" method=""{kratosUi.Method}"">"));
        foreach (var group in groups)
        { 
            builder.AppendHtml(helper.RenderKratosNodes(group.Value));
        }
        builder.AppendHtml(helper.Raw("</form>"));
        return builder;
    }

    //public static IHtmlContent BuildKratosUiInput(this IHtmlHelper helper, KratosUiNode node)
    //{
    //    var inputAttributes = node.Attributes.GetKratosUiNodeInputAttributes();
    //    Console.WriteLine($"Building {inputAttributes.Type} html control");
    //    switch (inputAttributes.Type)
    //    {
    //        case KratosUiNodeInputAttributes.TypeEnum.Password:
    //        case KratosUiNodeInputAttributes.TypeEnum.Email:
    //        case KratosUiNodeInputAttributes.TypeEnum.Text:
    //        case KratosUiNodeInputAttributes.TypeEnum.Url:
    //        case KratosUiNodeInputAttributes.TypeEnum.Tel:
    //        case KratosUiNodeInputAttributes.TypeEnum.Number:
    //            return helper.StyledInput(inputAttributes, node.Meta.Label.Text);

    //        case KratosUiNodeInputAttributes.TypeEnum.Checkbox:
    //            return helper.StyledCheckbox(inputAttributes, node.Meta.Label.Text);

    //        case KratosUiNodeInputAttributes.TypeEnum.Submit:
    //            return helper.StyledButton(inputAttributes, node.Meta.Label.Text);
    //    }
    //    return helper.Raw("");
    //}

    //public static IHtmlContent StyledInput(this IHtmlHelper helper, KratosUiNodeInputAttributes inputAttributes, string text)
    //{
    //    IHtmlContentBuilder builder = new HtmlContentBuilder();
    //    builder.AppendHtml(helper.OpenDiv("relative w-full"));
    //    builder.AppendHtml(helper.RenderInputField(
    //        inputAttributes,
    //        "w-full text-sm px-4 py-3 bg-gray-200 focus:bg-gray-100 border border-gray-200 rounded-lg focus:outline-none focus:border-purple-400"));
    //    builder.AppendHtml(helper.Raw($@"<div class=""flex items-center absolute inset-y-0 right-0 mr-3 text-sm leading-5"">
    //            <svg class=""h-4 text-purple-700"" fill=""none"" xmlns=""http://www.w3.org/2000/svg"" viewbox=""0 0 576 512"">
    //                <path fill=""currentColor"" d=""M572.52 241.4C518.29 135.59 410.93 64 288 64S57.68 135.64 3.48 241.41a32.35 32.35 0 0 0 0 29.19C57.71 376.41 165.07 448 288 448s230.32-71.64 284.52-177.41a32.35 32.35 0 0 0 0-29.19zM288 400a144 144 0 1 1 144-144 143.93 143.93 0 0 1-144 144zm0-240a95.31 95.31 0 0 0-25.31 3.79 47.85 47.85 0 0 1-66.9 66.9A95.78 95.78 0 1 0 288 160z"">
    //                </path>
    //            </svg>
    //        </div>"));
    //    builder.AppendHtml(helper.CloseDiv());
    //    return builder;
    //}
}
 