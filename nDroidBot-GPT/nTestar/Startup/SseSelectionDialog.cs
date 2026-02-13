using System.Collections.Generic;
using System.Windows.Forms;

namespace Startup
{
    internal static class SseSelectionDialog
    {
        public static string? SelectSse(IReadOnlyList<string> options)
        {
            if (options.Count == 0)
            {
                return null;
            }

            using Form form = new Form();
            using ComboBox comboBox = new ComboBox { DataSource = options, Dock = DockStyle.Fill };
            using Button okButton = new Button { Text = "OK", Dock = DockStyle.Bottom };

            okButton.Click += (_, _) => form.Close();

            form.Controls.Add(comboBox);
            form.Controls.Add(okButton);
            form.Text = "TESTAR settings";
            form.StartPosition = FormStartPosition.CenterScreen;
            form.TopMost = true;
            form.ShowDialog();

            return comboBox.SelectedItem?.ToString();
        }
    }
}
