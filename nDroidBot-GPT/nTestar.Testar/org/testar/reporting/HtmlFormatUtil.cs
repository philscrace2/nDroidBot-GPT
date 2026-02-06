using System;
using System.Collections.Generic;
using org.testar.monkey;

namespace org.testar.reporting
{
    public class HtmlFormatUtil : BaseFormatUtil
    {
        public HtmlFormatUtil(string filePath) : base(filePath, "html")
        {
        }

        public void addContent(string text)
        {
            if (text.Contains("\n", StringComparison.Ordinal))
            {
                content.AddRange(splitStringAtNewline(text));
            }
            else
            {
                content.Add(text);
            }
        }

        public void addContent(string text, string tag)
        {
            if (text.Contains("\n", StringComparison.Ordinal))
            {
                content.Add("<" + tag + ">");
                content.AddRange(splitStringAtNewline(text));
                content.Add("</" + tag + ">");
            }
            else
            {
                content.Add("<" + tag + ">" + text + "</" + tag + ">");
            }
        }

        public void addHeader(string title)
        {
            addHeader(title, string.Empty, string.Empty);
        }

        public void addHeader(string title, string script)
        {
            addHeader(title, script, string.Empty);
        }

        public void addHeader(string title, string script, string style)
        {
            Assert.notNull(title);
            Assert.notNull(script);
            Assert.notNull(style);

            content.Add("<!DOCTYPE html>");
            content.Add("<html lang=\"en\">");
            content.Add("<head>");

            if (!string.IsNullOrEmpty(script))
            {
                addContent(script, "script");
            }

            if (!string.IsNullOrEmpty(style))
            {
                addContent(style, "style");
            }

            content.Add("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">");
            addContent(title, "title");
            content.Add("</head>");
            content.Add("<body>");
        }

        public void addFooter()
        {
            content.Add("</body>");
            content.Add("</html>");
        }

        public void addHeading(int level, string text)
        {
            Assert.isTrue(level >= 1 && level <= 6, "Invalid HTML heading level");
            Assert.notNull(text);
            addContent(text, "h" + level);
        }

        public void addParagraph(string text)
        {
            Assert.notNull(text);
            addContent(text, "p");
        }

        public void addLineBreak()
        {
            content.Add("<br>");
        }

        public void addList(bool ordered, List<string> items)
        {
            content.Add(ordered ? "<ol>" : "<ul>");
            foreach (string item in items)
            {
                addContent(item, "li");
            }
            content.Add(ordered ? "</ol>" : "</ul>");
        }

        public void addButton(string buttonClass, string buttonText)
        {
            content.Add("<button type='button' class='" + buttonClass + "'>" + buttonText + "</button>");
        }
    }
}
