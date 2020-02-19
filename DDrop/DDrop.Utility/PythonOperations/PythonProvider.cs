using DDrop.BE.Models;
using System;
using System.Diagnostics;
using System.IO;

namespace DDrop.Utility.PythonOperations
{
    public class PythonProvider
    {
        public DropPhoto RunScript(string inputPath, string outputPath, DropPhoto image, string scriptToRun, string interpreter)
        {
            Process process = new Process();
            process.StartInfo.FileName = interpreter;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = false;
            process.StartInfo.Arguments = string.Concat(scriptToRun, " ", inputPath, " ", outputPath);
            process.Start();

            StreamReader streamReader = process.StandardOutput;

            string output = streamReader.ReadToEnd();
            string[] outputAsArray = output.Split(new[] { "\r\n" }, StringSplitOptions.None);
            string diameters = outputAsArray[2].Replace("[", "");
            diameters = diameters.Replace("]", "");
            diameters = diameters.Replace(",", "");
            string[] formatedDiameters = diameters.Split(new[] { " " }, StringSplitOptions.None);

            decimal x = Convert.ToDecimal(formatedDiameters[0]);
            decimal y = Convert.ToDecimal(formatedDiameters[1]);

            image.XDiameterInPixels = Convert.ToInt32(x);
            image.YDiameterInPixels = Convert.ToInt32(y);

            return image;
        }
    }
}
