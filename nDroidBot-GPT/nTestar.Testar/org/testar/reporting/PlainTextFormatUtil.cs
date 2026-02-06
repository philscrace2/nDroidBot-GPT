using System;
using System.Collections.Generic;
using org.testar.monkey;

namespace org.testar.reporting
{
    public class PlainTextFormatUtil : BaseFormatUtil
    {
        public PlainTextFormatUtil(string filePath) : base(filePath, "txt")
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

        public void addHeading(int level, string text)
        {
            Assert.isTrue(level >= 1 && level <= 6, "Invalid heading level: must be between 1 and 6");
            Assert.notNull(text);
            content.Add(new string('#', level) + text);
        }

        public void addParagraph(string text)
        {
            Assert.notNull(text);
            addEmptyLine();
            content.AddRange(splitStringAtNewline(text));
            addEmptyLine();
        }

        public void addEmptyLine()
        {
            content.Add(string.Empty);
        }

        public void addHorizontalLine()
        {
            addHorizontalLine(20);
        }

        public void addHorizontalLine(int length)
        {
            Assert.isTrue(length >= 3 && length <= 100, "Invalid horizontal line length: must be between 3 and 100");
            content.Add(new string('=', length));
        }

        public void addList(bool ordered, List<string> items)
        {
            addEmptyLine();
            for (int i = 0; i < items.Count; i++)
            {
                if (ordered)
                {
                    addContent((i + 1) + items[i]);
                }
                else
                {
                    addContent("- " + items[i]);
                }
            }
            addEmptyLine();
        }
    }
}
