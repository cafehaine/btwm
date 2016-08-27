using System.Windows.Forms;
using System.Diagnostics;
using System;

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
            //TODO: read from config bar process and arguments
            string barCommand = "D:\\Documents\\Prog\\btwm\\btwmbar\\bin\\Debug\\btwmbar.exe";

            Process barProcess = new Process();
            ProcessStartInfo startInfo = barProcess.StartInfo;

            startInfo.FileName = barCommand;
            startInfo.Arguments = "pos=top font=\"Fira Code:12\" command=\"D:\\Documents\\Prog\\btwm\\TestStatus\\Bin\\Debug\\TestStatus.exe\"";
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
