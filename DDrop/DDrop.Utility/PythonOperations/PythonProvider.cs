using System.Diagnostics;
using System.IO;

namespace DDrop.Utility.PythonOperations
{
    public class PythonProvider
    {
        public string RunScript(string scriptToRun, string interpreter, byte[] image, int ksize = 9, int treshold1 = 50, int treshold2 = 5, int size1 = 100, int size2 = 250)
        {
            Process process = new Process();
            process.StartInfo.FileName = interpreter;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = false;
            process.StartInfo.Arguments = string.Concat(scriptToRun, " ", image, " ", ksize, " ", treshold1, " ", treshold2, " ", size1, " ", size2);
            process.Start();

            StreamReader streamReader = process.StandardOutput;

            return streamReader.ReadToEnd();
        }
    }
}
