using DDrop.Utility.ImageOperations;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;

namespace DDrop.BL.ImageProcessing.Python
{
    public class PythonProvider : IPythonProvider
    {
        public System.Drawing.Point[] GetDiameters(byte[] inputPhoto, string tempFileName, string scriptToRun, string interpreter, int ksize = 9, int treshold1 = 50, int treshold2 = 5, int size1 = 100, int size2 = 250)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;
            DirectoryInfo di = Directory.CreateDirectory(path + @"\Temp\");

            string fullPath = di.FullName + tempFileName + "." + ImageValidator.GetImageFormat(inputPhoto);

            File.WriteAllBytes(fullPath, inputPhoto);

            System.Drawing.Point[] points = GetDiametersViaSystemIO(scriptToRun, interpreter, fullPath, ksize, treshold1, treshold2, size1, size2);

            File.Delete(fullPath);

            return points;
        }

        private System.Drawing.Point[] GetDiametersViaSystemIO(string scriptToRun, string interpreter, string imagePath, int ksize = 9, int treshold1 = 50, int treshold2 = 5, int size1 = 100, int size2 = 250)
        {
            using (Process process = new Process())
            {
                process.StartInfo.FileName = interpreter;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = false;
                process.StartInfo.Arguments = string.Concat(scriptToRun, " ", imagePath, " ", ksize, " ", treshold1, " ", treshold2, " ", size1, " ", size2);
                process.Start();

                StreamReader streamReader = process.StandardOutput;

                string output = streamReader.ReadToEnd();

                if (string.IsNullOrWhiteSpace(output))
                    throw new InvalidOperationException("Не удалось построить контур");

                string[] outputAsArray = output.Split(new[] { "\r\n\r\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                System.Drawing.Point[] points = new System.Drawing.Point[outputAsArray.Length];

                for (int i = 0; i < outputAsArray.Length; i++)
                {
                    outputAsArray[i] = outputAsArray[i].Replace("[", "");
                    outputAsArray[i] = outputAsArray[i].Replace("]", "");

                    var formatedDiamters = outputAsArray[i].Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                    points[i] = new System.Drawing.Point()
                    {
                        X = Convert.ToInt32(formatedDiamters[0]),
                        Y = Convert.ToInt32(formatedDiamters[1])
                    };
                }

                process.Close();

                return points;
            }
        }

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

