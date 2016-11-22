using System.Windows.Forms;
using System.Diagnostics;
using System;
using System.Reflection;
using System.IO;

//! ---------- Future note to self ----------
//! Change this to hide console:
//! Project -> Properties -> Application -> Windows Application

namespace btwm
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("BTWM is starting...");

            // Base this on this executable's path
            string barCommand = Path.GetDirectoryName(Application.ExecutablePath) + "\\btwmbar.exe";

            Config.Configuration config = new Config.Configuration();

            string arguments = config.Bar.ToCommand();

            Process barProcess = new Process();
            ProcessStartInfo startInfo = barProcess.StartInfo;

            startInfo.FileName = barCommand;
            startInfo.Arguments = arguments;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardInput = true; // send info to bar through stdin
            startInfo.RedirectStandardOutput = true; // receive info from bar through stdout

            barProcess.StartInfo = startInfo;

            barProcess.Start();
            barProcess.WaitForInputIdle();

            Handler mainHandler = new Handler(barProcess.StandardInput, barProcess.StandardOutput);

            while (mainHandler.Running)
            {
                //! This lets Windows know that the application is running and
                //! responding. (Without this it is impossible to receive any
                //! kind of native hook).
                Application.DoEvents();
            }

            if (!barProcess.HasExited)
                barProcess.Kill();
        }
    }
}
