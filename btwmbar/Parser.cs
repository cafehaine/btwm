using System.Drawing;
using System.IO;
using System.Runtime.Serialization.Json;

namespace btwmbar
{
    class Parser
    {
        public struct AdvancedText
        {
            public Color ForeColor;
            public Color BackColor;
            public int ForeAlpha;
            public int BackAlpha;
            public bool Bold;
            public bool Striqued;
            public bool Italic;

        }

        public AdvancedText[] ParsePango(string text)
        {
            return new AdvancedText[0];
        }

        private static MemoryStream stringToMemoryStream(string data)
        {
            MemoryStream output = new MemoryStream(data.Length);
            foreach (char chr in data)
                output.WriteByte((byte)chr);
            output.Position = 0;
            return output;
        }

        private static ColoredText[] parseText(string text, string markup, Color def)
        {
            if (markup == "none")
                return new ColoredText[] { new ColoredText(def, text) };
            ColoredText[] output = new ColoredText[0];
            return output;
        }

        public static Block[] ParseLine(string line, Color defaultColor)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(JsonStructures.Block[]));
            JsonStructures.Block[] blocks = (JsonStructures.Block[])serializer.ReadObject(stringToMemoryStream(line));

            Block[] output = new Block[blocks.Length];

            for (int i = 0; i < blocks.Length; i++)
            {
                JsonStructures.Block block = blocks[i];
                ColoredText[] content = parseText(block.full_text, block.markup, defaultColor);
                output[i] = new Block(content, block.name, block.separator_block_width, '|');
            }

            return output;
        }
    }
}
