using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Homework1
{
    public struct EventData
    {
        public string Data { get; set; }
        public DateTime Time { get; set; }

        public override string ToString()
        {
            return $"{Time} >> {Data}";
        }
    }

    static public class Log
    {
        static Log()
        {
            db = new List<EventData>();
            using (StreamWriter sw = new StreamWriter("Log.txt", false, System.Text.Encoding.Default)) ;
        }
        static List<EventData> db;
        public static void Sub(string MessageText)
        {
            db.Add(new EventData() { Time = DateTime.Now, Data = MessageText });
        }

        static public void PrintDb()
        {
            foreach(var item in db)
            {
                Console.WriteLine(item);
                using (StreamWriter sw = new StreamWriter("Log.txt", true, System.Text.Encoding.Default))
                {
                    sw.WriteLine(item);
                }
            }
        }
    }
}
