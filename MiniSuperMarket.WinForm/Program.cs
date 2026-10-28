namespace MiniSuperMarket.WinForm
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            // Form khởi chạy đầu tiên là FormLogin
            Application.Run(new FormLogin());
        }
    }
}
