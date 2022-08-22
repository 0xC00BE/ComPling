using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComPling
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string ipList = Environment.CurrentDirectory.ToString() + @"\goog-prefixes.txt";

            string filter = "dst net";

            foreach (string line in System.IO.File.ReadLines(@ipList))
            {
                filter += line + " or dst net";
            }

            filter = filter.Remove(filter.Length - 11);

            var tcpDumpProc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = @"C:\Program Files\Wireshark\tshark.exe",
                    Arguments = "-D",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };
            tcpDumpProc.Start();
            while (!tcpDumpProc.StandardOutput.EndOfStream)
            {
                string line = tcpDumpProc.StandardOutput.ReadLine();
                Console.WriteLine(line);
            }

            Console.Write("Select interface: ");
            string nic = Console.ReadLine();

            StartProcess(nic, filter);


           
        }

        static void StartProcess(string nic,string filter)
        {
            using (var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = @"C:\Program Files\Wireshark\tshark.exe",
                    Arguments = "-i " + nic + " -f \"(" + filter + ") and (dst port 80 or dst port 443)\" -l -T fields -e frame.len",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                }
            })
            {
                process.OutputDataReceived += (sender, args) => Display(args.Data);
                process.ErrorDataReceived += (sender,args) => Display(args.Data);
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit(); 
            }
        }
        private static void Display(string data)
        {
            Console.WriteLine(data);
            if(Int32.TryParse(data,out int freq)){ 
            Console.Beep(freq, freq);
            }
        }
    }
}
