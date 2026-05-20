using nTestar.Desktop.Winforms.mvp;

namespace nTestar.Desktop.Winforms
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var model = MainScreenModel.CreateDefault();
            var form = new MainFormWinForms();
            var presenter = new MainPresenter(form, model);

            presenter.Initialise();

            Application.Run(form);
        }
    }
}