using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace btwmbar
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            //TODO: check if argument syntax is valid
            #region Argument parsing
            Bar.Position pos = Bar.Position.bottom;
            Screen output = Screen.PrimaryScreen;

            Color barFore = Color.White;
            Color barBack = Color.Black;
            Color wsForeF = Color.White;
            Color wsForeN = Color.FromArgb(200, 200, 200);
            Color wsBackF = Color.Firebrick;
            Color wsBackN = Color.FromArgb(25, 25, 25);

            string fontName = "Courier New";
            int fontSize = 12;
            string command = "D:\\Documents\\Prog\\btwm\\TestStatus\\Bin\\Debug\\TestStatus.exe";

            foreach (string arg in args)
            {
                string[] data = arg.Split('=');
                switch (data[0])
                {
                    case "command":
                        if (data[1] != "NotSet")
                            command = data[1];
                        break;
                    case "pos":
                        if (data[1] == "top")
                            pos = Bar.Position.top;
                        break;
                    case "colors":
                        string[] colors = data[1].Split(':');
                        barBack = HexColor.HexToColor(colors[0], barBack, false);
                        barFore = HexColor.HexToColor(colors[1], barFore);
                        wsBackN = HexColor.HexToColor(colors[2], wsBackN);
                        wsForeN = HexColor.HexToColor(colors[3], wsForeN);
                        wsBackF = HexColor.HexToColor(colors[4], wsBackF);
                        wsForeF = HexColor.HexToColor(colors[5], wsForeF);
                        break;
                    case "font":
                        string[] fontInfo = data[1].Split(':');
                        fontName = fontInfo[0];
                        fontSize = int.Parse(fontInfo[1]);
                        break;
                    default:
                        MessageBox.Show("Invalid argument: " + data[0]);
                        break;
                }
            }

            Font font = new Font(fontName, fontSize, GraphicsUnit.Pixel);
            #endregion

            Process statusLine = new Process();
            statusLine.StartInfo.FileName = command;
            statusLine.StartInfo.RedirectStandardOutput = true;
            statusLine.StartInfo.UseShellExecute = false;
            statusLine.Start();
            StreamReader statusStream = statusLine.StandardOutput;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Bar(pos, output, barBack, barFore,
                wsForeF, wsForeN, wsBackF, wsBackN, font, ref statusStream));
            if (!statusLine.HasExited)
                statusLine.Kill();
        }
    }
}
