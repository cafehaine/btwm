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
        static void Main(string[] args)
        {
            bool debug = false;
            if (Array.FindIndex(args, str => str == "debug") != -1)
                debug = true;

            Handler mainHandler = new Handler();

            //? Support of debug mode might be removed later.
            if (debug)
                Console.WriteLine("Debug mode, enter 'help' for help.");

            while (mainHandler.Running)
            {
                if (debug)
                {
                    Console.ResetColor();
                    switch (Console.ReadLine())
                    {
                        case "help":
                            Console.WriteLine("Help Mode: Command List:\n" +
                                "\tvert\t- switch to vertical split\n" +
                                "\thoriz\t- switch to horizontal split\n" +
                                "\thide\t- hide the layout\n" +
                                "\tshow\t- show the layout\n" +
                                "\tspawn\t- spawn a new notepad instance");
                            break;
                        case "spawn":
                            Process.Start("notepad.exe");
                            break;
                        case "vert":
                            mainHandler.CommandExecutor("splitv");
                            break;
                        case "horiz":
                            mainHandler.CommandExecutor("splith");
                            break;
                        case "": // Force update
                            break;
                        default:
                            Console.WriteLine("Unknown command.");
                            break;
                    }
                }
                //! This lets Windows know that the application is running and
                //! responding. (Without this it is impossible to receive shell
                //! hooks).
                Application.DoEvents();
            }
        }
    }
}
