using System;
using System.IO;
using System.Threading;
using JsonStructures;
using System.Runtime.Serialization.Json;

namespace TestStatus
{

    class Program
    {
        static char[] ByteToCharArray(byte[] array)
        {
            char[] output = new char[array.Length];
            for (int i = 0; i < array.Length; i++)
                output[i] = (char)array[i];

            return output;
        }
        static Block timeBlock()
        {
            int hour = DateTime.Now.Hour;
            int minute = DateTime.Now.Minute;
            string text = (hour < 10 ? "0" + hour : hour.ToString()) + ':' +
                (minute < 10 ? "0" + minute : minute.ToString());
            return new Block("clock", "none", text, 9);
        }

        static Block dateBlock()
        {
            DateTime today = DateTime.Today;
            string day = today.Day.ToString();
            if (day.Length < 2)
                day = '0' + day;

            string dayOfWeek = string.Empty;
            switch ((int)today.DayOfWeek)
            {
                case 0:
                    dayOfWeek = "Sun";
                    break;
                case 1:
                    dayOfWeek = "Mon";
                    break;
                case 2:
                    dayOfWeek = "Tue";
                    break;
                case 3:
                    dayOfWeek = "Thu";
                    break;
                case 4:
                    dayOfWeek = "Wed";
                    break;
                case 5:
                    dayOfWeek = "Fri";
                    break;
                case 6:
                    dayOfWeek = "Sat";
                    break;
            }

            string month = string.Empty;
            switch (today.Month)
            {
                case 1:
                    month = "Jan";
                    break;
                case 2:
                    month = "Feb";
                    break;
                case 3:
                    month = "Mar";
                    break;
                case 4:
                    month = "Apr";
                    break;
                case 5:
                    month = "May";
                    break;
                case 6:
                    month = "Jun";
                    break;
                case 7:
                    month = "Jul";
                    break;
                case 8:
                    month = "Aug";
                    break;
                case 9:
                    month = "Sep";
                    break;
                case 10:
                    month = "Oct";
                    break;
                case 11:
                    month = "Nov";
                    break;
                case 12:
                    month = "Dec";
                    break;
            }

            return new Block("calendar", "none", dayOfWeek + ' ' + day + ' ' + month, 9);
        }

        static void Main(string[] args)
        {
            MemoryStream output = new MemoryStream();

            BarInfo test = new BarInfo(1, false);
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(BarInfo));
            serializer.WriteObject(output, test);

            serializer = new DataContractJsonSerializer(typeof(Block[]));
            Console.WriteLine(output.ToArray());
            for (int i = 0; i < 5; i++)
            {
                output = new MemoryStream();
                Block[] blocks = new Block[2];
                blocks[1] = timeBlock();
                blocks[0] = dateBlock();
                serializer.WriteObject(output, blocks);
                Console.Write(i == 0 ? '[' : ',');
                Console.WriteLine(ByteToCharArray(output.ToArray()));
                Thread.Sleep(2000);
            }
            output.Dispose();
        }
    }
}
