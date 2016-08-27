using System.Drawing;

namespace btwmbar
{
    struct ColoredText
    {
        public SolidBrush Brush;
        public Color Color;
        public string Text;

        public ColoredText(Color Color, string Text)
        {
            this.Color = Color;
            this.Text = Text;
            Brush = new SolidBrush(Color);
        }
    }

    struct Block
    {
        public ColoredText[] Content;
        public string Name;
        public int SepWidth;
        public char SepChar;

        public Block(ColoredText[] Content,string Name, int SepWidth, char SepChar)
        {
            this.Name = Name;
            this.Content = Content;
            this.SepWidth = SepWidth;
            this.SepChar = SepChar;
        }
    }
}
