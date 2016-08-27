using System.IO;

namespace Configuration
{
    public class Configuration
    {
        public string[] Commands;

        public Configuration ParseConfig(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException();

            string[] fileContent = File.ReadAllLines(path);

            for (int i = 0; i < fileContent.Length; i++)
            {
                string line = fileContent[i].Trim(new char[] { ' ', '\t' }).Trim(new char[] { ' ', '\t' });
                //if ()
            }
            return null;
        }
    }
}
