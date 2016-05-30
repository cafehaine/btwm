using System.Windows.Forms;

// ---------- Future note to self ----------
// Change this to hide console:
// Project -> Properties -> Application -> Windows Application

namespace btwm
{
    class Program
    {
        static void Main(string[] args)
        {
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
