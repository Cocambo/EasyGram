using Markdig;
using Microsoft.AspNetCore.Html;

namespace EasyGram.Services
{
    public class MarkdownService : IMarkdownService
    {
        private readonly MarkdownPipeline _pipeline; // объект из библиотеки Markdig для настройки Markdown

        public MarkdownService()
        {
            _pipeline = new MarkdownPipelineBuilder() // создаем _pipeline с расширением .UseAdvancedExtensions(), для поддержки расширенного md
                .UseAdvancedExtensions()
                .Build();
        }

        public HtmlString ToHtml(string markdown)
        {
            if (string.IsNullOrEmpty(markdown))
                return new HtmlString("");

            var html = Markdown.ToHtml(markdown, _pipeline);
            return new HtmlString(html);
        }

        public string ToHtmlString(string markdown)
        {
            if (string.IsNullOrEmpty(markdown))
                return "";

            return Markdown.ToHtml(markdown, _pipeline);
        }
    }
}
