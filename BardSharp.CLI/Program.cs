using CockyGrabber.Grabbers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BardSharp.CLI
{
    internal class Program
    {

        /// <summary>
        /// The path to the "Main.py" file, for test purposes its in the "bin\Debug" folder
        /// </summary>
        public static string PythonScriptPath = @"main.py";

        static void Main(string[] args)
        {
            while (1 == 1)
            {
                Console.WriteLine("Hello, please enter something to ask to Google Bard :");
                string prompt = Console.ReadLine();
                Console.WriteLine(AskBard(prompt));
            }

        }

        /// <summary>
        /// Use https://github.com/dsdanielpark/Bard-API
        /// </summary>
        /// <param name="prompt"></param>
        /// <param name="cookies"></param>
        private static string AskBard(string prompt)
        {

            string cookies = UpdateCookies();

            string PythonBackup = File.ReadAllText(PythonScriptPath);

            string pythonSrc = @"
import sys
from bardapi import Bard
from bardapi import BardCookies

if len(sys.argv) != 2:
    print(""Usage: python main.py 'your_argument'"")
    sys.exit(1)

argument = sys.argv[1]

cookie_dict = {
    %here%
}

bard = BardCookies(cookie_dict=cookie_dict)
print(bard.get_answer(argument)['content'])
";

            string pythonSrcNew = pythonSrc.Replace("%here%", cookies);
            File.WriteAllText(PythonScriptPath, pythonSrcNew);

            // Create a process to run the Python script
            Process process = new Process();
            process.StartInfo.FileName = "python"; // Use the "python" command
            process.StartInfo.Arguments = $"{PythonScriptPath} \"{prompt}\"";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.CreateNoWindow = true;

            // Start the process
            process.Start();

            // Read the output and error streams
            string output = process.StandardOutput.ReadToEnd();

            //process the error if you need
            //string error = process.StandardError.ReadToEnd();

            // Wait for the process to exit
            process.WaitForExit();

            // Close the process
            process.Close();

            // Re write the original file value
            File.WriteAllText(PythonScriptPath, PythonBackup);

            return output;
        }

        /// <summary>
        /// Use chromeGrabber to extract specific Google cookies from current Webbrowser session 
        /// </summary>
        /// <returns></returns>
        private static string UpdateCookies()
        {
            ChromeGrabber grabber = new ChromeGrabber(); // Create Grabber
            var cookies = grabber.GetCookies(); // Collect all Cookies with GetCookies()

            List<string> GoogleCookies = new List<string>();
            string cookieChunk = "";
            // Print the Hostname, Name, and Value of every cookie:
            foreach (var cookie in cookies)
            {
                if (cookie.Name.Contains("__Secure-1P"))
                {
                    GoogleCookies.Add($"\"{cookie.Name}\" : \"{cookie.DecryptedValue}\",");
                    cookieChunk = cookieChunk + Environment.NewLine + $"\"{cookie.Name}\" : \"{cookie.DecryptedValue}\",";
                }
            }

            return cookieChunk.Remove(cookieChunk.Length - 1);
        }
    }
}
