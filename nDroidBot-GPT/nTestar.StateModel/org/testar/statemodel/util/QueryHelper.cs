using System.Text.RegularExpressions;

namespace org.testar.statemodel.util
{
    public static class QueryHelper
    {
        public static int ParseCountQueryResponse(string output, string field)
        {
            if (string.IsNullOrWhiteSpace(output) || string.IsNullOrWhiteSpace(field))
            {
                return -1;
            }

            string regex = field + ":\\s*(\\d+)";
            Match match = Regex.Match(output, regex);
            if (match.Success && int.TryParse(match.Groups[1].Value, out int value))
            {
                return value;
            }

            return -1;
        }
    }
}
