using Microsoft.AspNetCore.Html;

namespace EasyGram.Services
{
    public interface IMarkdownService
    {
        HtmlString ToHtml(string markdown);
        string ToHtmlString(string markdown);
    }
}
