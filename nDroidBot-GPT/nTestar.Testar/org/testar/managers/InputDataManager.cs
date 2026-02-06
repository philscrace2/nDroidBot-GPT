using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using org.testar.monkey.alayer;
using org.testar.monkey.alayer.webdriver.enums;

namespace org.testar.managers
{
    public class InputDataManager
    {
        private static readonly Random Random = new();

        public static string getRandomTextFromCustomInputDataFile(string customDataFile)
        {
            try
            {
                var allLines = File.ReadAllLines(customDataFile);
                if (allLines.Length > 0)
                {
                    return allLines[Random.Next(allLines.Length)];
                }
            }
            catch (IOException)
            {
                Console.Error.WriteLine($"{customDataFile} is empty or file was not found. Returning a random text input data.");
            }

            return getRandomTextInputData();
        }

        public static string getRandomTextInputData(Widget widget)
        {
            var typeValue = widget.get(WdTags.WebType, string.Empty).ToLowerInvariant();
            if (typeValue.Contains("email", StringComparison.Ordinal))
            {
                return getRandomEmailInput();
            }

            if (typeValue.Contains("number", StringComparison.Ordinal))
            {
                return getRandomNumberInput();
            }

            if (typeValue.Contains("url", StringComparison.Ordinal))
            {
                return getRandomUrlInput();
            }

            return getRandomTextInputData();
        }

        public static string getRandomTextInputData()
        {
            return Random.Next(5) switch
            {
                0 => getRandomNumberInput(),
                1 => getRandomAlphabeticInput(10),
                2 => getRandomUrlInput(),
                3 => getRandomDateInput(),
                _ => getRandomEmailInput()
            };
        }

        public static string getRandomNumberInput()
        {
            return Random.Next().ToString();
        }

        public static string getRandomAlphabeticInput(int count)
        {
            if (count < 1)
            {
                Console.Error.WriteLine($"Random Alphabetic Input length {count} cannot be less than 1. Return a random Alphabetic Input of length 10.");
                count = 10;
            }

            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var buffer = new StringBuilder(count);
            for (int i = 0; i < count; i++)
            {
                buffer.Append(chars[Random.Next(chars.Length)]);
            }

            return buffer.ToString();
        }

        public static string getRandomUrlInput()
        {
            var urls = new List<string>
            {
                "www.foo.com",
                "www.boo.com",
                "www.fooboo.com",
                "www.foo.org",
                "www.boo.org",
                "www.fooboo.org",
                "www.testar.org"
            };

            return urls[Random.Next(urls.Count)];
        }

        public static string getRandomDateInput()
        {
            var dates = new List<string>
            {
                "22-03-2017",
                "03-22-2017",
                "2017-03-22",
                "2017-22-03",
                "00-00-1900",
                "12-12-7357",
                "01-01-01",
                "2017",
                "November",
                "31 December",
                "1st January",
                "2001 February"
            };

            return dates[Random.Next(dates.Count)];
        }

        public static string getRandomEmailInput()
        {
            var emails = new List<string>
            {
                "foo@boo.org",
                "boo@foo.org",
                "fooboo@org.com",
                "foo-boo@foo.com"
            };

            return emails[Random.Next(emails.Count)];
        }
    }
}
