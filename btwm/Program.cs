using System.Windows.Forms;
using System.Diagnostics;

// ---------- Future note to self ----------
// Change this to hide console:
// Project -> Properties -> Application -> Windows Application

namespace btwm
{
    class Program
    {
        static void Main(string[] args)
        {
            // Kill all instances of explorer.exe
            // (should not have to happen since it is a replacement)
            Process[] explorerInstances = Process.GetProcessesByName(
                "explorer.exe");

            foreach(Process prcss in explorerInstances)
            {
                prcss.Kill();
            }


            Handler mainHandler = new Handler();

            while (mainHandler.running)
            {
                // This lets Windows know that the application is running and
                // responding. (Without this it is impossible to receive shell
                // hooks).
                Application.DoEvents();
            }
        }
    }
}
