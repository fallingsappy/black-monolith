using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;

namespace DDrop.Utility.PythonOperations
{
    public class PythonProvider
    {
        public string RunScript(string scriptToRun, string interpreter, byte[] image, int ksize = 9, int treshold1 = 50, int treshold2 = 5, int size1 = 100, int size2 = 250)
        {
            using (Process process = new Process())
            {
                process.StartInfo.FileName = interpreter;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.Arguments = string.Concat(scriptToRun, " ", image, " ", ksize, " ", treshold1, " ", treshold2, " ", size1, " ", size2);
                process.Start();

                // Open the named pipe.
                var server = new NamedPipeServerStream("NPtest");

                Console.WriteLine("Waiting for connection...");
                server.WaitForConnection();

                Console.WriteLine("Connected.");
                var br = new BinaryReader(server);
                var bw = new BinaryWriter(server);

                while (true)
                {
                    try
                    {
                        //var len = (int)br.ReadUInt32();            // Read string length
                        //var str = new string(br.ReadChars(len));    // Read string

                        //Console.WriteLine("Read: \"{0}\"", str);

                        var str = "gdgdsg";
                        str = new string(str.Reverse().ToArray());  // Just for fun

                        var buf = Encoding.ASCII.GetBytes(str);     // Get ASCII byte array     
                        bw.Write((uint)buf.Length);                // Write string length
                        bw.Write(buf);                              // Write string
                        Console.WriteLine("Wrote: \"{0}\"", str);
                    }
                    catch (EndOfStreamException)
                    {
                        break;                    // When client disconnects
                    }
                }

                Console.WriteLine("Client disconnected.");
                server.Close();
                server.Dispose();

                StreamReader streamReader = process.StandardOutput;
                
                process.Close();

                return streamReader.ReadToEnd();
            }            
        }
    }
}
