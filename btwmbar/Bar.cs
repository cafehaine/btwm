using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Threading;

namespace btwmbar
{
    class Bar : Form
    {
        public enum Position : int
        {
            top = 1,
            bottom = 0
        }

        #region Variables
        private SolidBrush barBack;
        private SolidBrush barFore;
        private SolidBrush wsForeF;
        private SolidBrush wsForeN;
        private SolidBrush wsBackF;
        private SolidBrush wsBackN;
        private Pen separatorPen;

        private Block[] blocks;
        private Screen output;
        private Position pos;
        private StreamReader statusStream;
        private Thread inputWorker;
        private string[] workspaces;
        private int selectedWorkspace;

        private int barHeight;

        #endregion

        public void UpdateBlocks(Block[] Blocks)
        {
            blocks = Blocks;
            Invalidate();
        }

        private async void streamWorker()
        {
            inputWorker.Priority = ThreadPriority.BelowNormal;
            while (true)
            {
                string line = await statusStream.ReadLineAsync();
                if (line != null)
                {
                    if (line != string.Empty)
                    {
                        switch (line[0])
                        {
                            case '{': // status information
                                break;
                            case '[': // line
                            case ',':
                                UpdateBlocks(Parser.ParseLine(line.Substring(1), barFore.Color));
                                break;
                            default:
                                break; // Unknown data
                        }
                    }
                }
                else
                {
                    break; // status closed
                }
            }
        }

        public Bar(Position Pos, Screen Output, Color BarBack, Color BarFore,
            Color WsForeF, Color WsForeN, Color WsBackF, Color WsBackN,
            Font Font, ref StreamReader StatusOutput)
        {
            Text = "BTWM-EXCLUDED Bar";
            BackColor = BarBack;
            ForeColor = BarFore;
            this.Font = Font;
            output = Output;
            pos = Pos;
            blocks = new Block[0];
            statusStream = StatusOutput;

            DoubleBuffered = true; // Reduce flickering on redraw
            TopMost = true; // Draw on top of everything (until another form is declared as topmost)

            barBack = new SolidBrush(BarBack);
            barFore = new SolidBrush(BarFore);
            wsBackF = new SolidBrush(WsBackF);
            wsBackN = new SolidBrush(WsBackN);
            wsForeF = new SolidBrush(WsForeF);
            wsForeN = new SolidBrush(WsForeN);
            separatorPen = new Pen(barFore);

            // define a protocol to exchange data between btwm and btwmbar
            workspaces = new string[4];
            workspaces[0] = "0";
            workspaces[1] = "1";
            workspaces[2] = "2";
            workspaces[3] = "3";

            selectedWorkspace = 2;

            FormBorderStyle = FormBorderStyle.None;

            inputWorker = new Thread(new ThreadStart(streamWorker));
            inputWorker.Start();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            int x, y, width, height;

            width = output.Bounds.Width;
            height = Font.Height + 2;

            barHeight = height;

            x = output.Bounds.X;

            switch (pos)
            {
                case Position.top:
                    y = 0;
                    break;
                default:
                    y = output.Bounds.Y + output.Bounds.Height - height;
                    break;
            }

            SetBoundsCore(x, y, width, height, BoundsSpecified.All);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            g.FillRectangle(barBack, ClientRectangle);

            #region Workspaces
            float LeftMost = 0;
            for (int i = 0; i < workspaces.Length; i++)
            {
                SizeF size = g.MeasureString(workspaces[i], Font);
                g.FillRectangle(i == selectedWorkspace ? wsBackF : wsBackN,
                    LeftMost, 0, size.Width + 4, barHeight);
                g.DrawString(workspaces[i], Font,
                    i == selectedWorkspace ? wsForeF : wsForeN,
                    new PointF(LeftMost + 2, 1));
                LeftMost += size.Width + 5;
            }
            #endregion

            #region Blocks
            float RightMost = ClientRectangle.Width;
            for (int i = blocks.Length - 1; i >= 0; i--)
            {
                Block block = blocks[i];
                for (int j = block.Content.Length - 1; j >= 0; j--)
                {
                    ColoredText content = block.Content[j];
                    SizeF size = g.MeasureString(content.Text, Font);
                    g.DrawString(content.Text, Font, content.Brush, new PointF(RightMost - size.Width, 1));
                    RightMost -= size.Width;
                }
                if (i != 0)
                {
                    g.DrawLine(separatorPen, RightMost - block.SepWidth / 2, 0, RightMost - block.SepWidth / 2, Height);
                    RightMost -= block.SepWidth;
                }
            }
            #endregion
        }

        ~Bar()
        {
            inputWorker.Abort();
            statusStream.Dispose();
        }
    }
}
