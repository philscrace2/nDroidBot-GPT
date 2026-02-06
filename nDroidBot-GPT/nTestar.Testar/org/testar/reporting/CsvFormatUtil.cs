using System.Collections.Generic;
using System.Text;

namespace org.testar.reporting
{
    public class CsvFormatUtil : BaseFormatUtil
    {
        private readonly string delimiter;
        private readonly Dictionary<int, Field> fields;
        private readonly Dictionary<string, int> fieldsIndex;
        private bool fieldsLocked;

        public CsvFormatUtil(string filePath, string delimiter) : base(filePath, "csv")
        {
            this.delimiter = delimiter;
            fields = new Dictionary<int, Field>();
            fieldsIndex = new Dictionary<string, int>();
        }

        public void addNewField(string shortName, string name, string? value)
        {
            if (fieldsLocked)
            {
                return;
            }

            Field newField = new Field(shortName, name, value);
            int index = fieldsIndex.Count;
            fields[index] = newField;
            fieldsIndex[shortName] = index;
        }

        public void addNewField(string shortName, string name)
        {
            addNewField(shortName, name, null);
        }

        public void setFieldValue(string shortName, string newValue)
        {
            if (fieldsIndex.TryGetValue(shortName, out int index))
            {
                fields[index].setValue(newValue);
            }
        }

        public void resetValue(string shortName)
        {
            if (fieldsIndex.TryGetValue(shortName, out int index))
            {
                fields[index].resetValue();
            }
        }

        public void resetValues()
        {
            foreach (Field field in fields.Values)
            {
                field.resetValue();
            }
        }

        public void startNextRow()
        {
            if (!fieldsLocked)
            {
                for (int i = 0; i < fields.Count; i++)
                {
                    content.Add(fields[i].getName());
                }
                fieldsLocked = true;
            }

            var joiner = new StringBuilder();
            for (int i = 0; i < fields.Count; i++)
            {
                if (i > 0)
                {
                    joiner.Append(delimiter);
                }
                joiner.Append(fields[i].getValue());
            }
            content.Add(joiner.ToString());
            resetValues();

            writeToFile();
        }

        private sealed class Field
        {
            private readonly string shortName;
            private readonly string name;
            private string? value;

            public Field(string shortName, string name, string? value)
            {
                this.shortName = shortName;
                this.name = name;
                this.value = value;
            }

            public string getShortName() => shortName;
            public string getName() => name;
            public string getValue() => value ?? string.Empty;
            public void setValue(string newValue) => value = newValue;
            public void resetValue() => value = null;
        }
    }
}
