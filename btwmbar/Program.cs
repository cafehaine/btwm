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
            int barAlpha = 255;
            Color wsForeF = Color.White;
            Color wsForeN = Color.FromArgb(200, 200, 200);
            Color wsBackF = Color.Firebrick;
            Color wsBackN = Color.FromArgb(25, 25, 25);

            string fontName = "Courier New";
            int fontSize = 12;
            string command = "";

            foreach (string arg in args)
            {
                string[] data = arg.Split('=');
                switch (data[0])
                {
                    case "command":
                        command = data[1];
                        break;
                    case "pos":
                        if (data[1] == "top")
                            pos = Bar.Position.top;
                        break;
                    case "barBack":
                        barBack = HexColor.HexToColor(data[1], barBack);
                        barAlpha = barBack.A;
                        barBack = Color.FromArgb(barBack.R, barBack.G, barBack.B);
                        break;
                    case "barFore":
                        barFore = HexColor.HexToColor(data[1], barFore);
                        break;
                    case "wsForeF":
                        wsForeF = HexColor.HexToColor(data[1], wsForeF);
                        break;
                    case "wsForeN":
                        wsForeN = HexColor.HexToColor(data[1], wsForeN);
                        break;
                    case "wsBackF":
                        wsBackF = HexColor.HexToColor(data[1], wsBackF);
                        break;
                    case "wsBackN":
                        wsBackN = HexColor.HexToColor(data[1], wsBackN);
                        break;
                    case "font":
                        string[] fontInfo = data[1].Split(':');
                        fontName = fontInfo[0];
                        fontSize = int.Parse(fontInfo[1]);
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
            Application.Run(new Bar(pos, output, barBack, barAlpha, barFore,
                wsForeF, wsForeN, wsBackF, wsBackN, font, ref statusStream));
            if (!statusLine.HasExited)
                statusLine.Kill();
        }
    }
}
