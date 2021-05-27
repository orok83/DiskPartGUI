using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiskPartForm
{
    static class Disks
    {
        internal static List<Disk> getDisks()
        {
            string output = getDiskstable();
            return getDisksList(output);
        }

        private static List<Disk> getDisksList(string output)
        {
            string table = output.Split(new string[] { "DISKPART>" }, StringSplitOptions.None)[1];
            var rows = table.Split(new string[] { "\n" }, StringSplitOptions.None);
            var disks = new List<Disk>();
            for (int i = 0; i <= rows.Length; i++)
            {
                try
                {
                    var status = rows[i].Split(new string[] { " " }, StringSplitOptions.None)[7];
                    var sizeAtBeggingString = rows[i].Remove(0, rows[i].IndexOf(status) + status.Count());
                    disks.Add(new Disk
                    {
                        Index = "Disk " + Int32.Parse(rows[i].Split(new string[] { " " }, StringSplitOptions.None)[3]).ToString(),
                        status = status,
                        Size = $"{sizeAtBeggingString.Trim().Split(new string[] { " " }, StringSplitOptions.None)[0]} {sizeAtBeggingString.Trim().Split(new string[] { " " }, StringSplitOptions.None)[1]}",
                    });
                }
                catch (Exception)
                {
                    //update later
                }
            }
            return disks;
        }

        internal static string cleanDisk(string index, string formatType)
        {
            Process process = runDiskpart();
            process.StandardInput.WriteLine($"Select {index}");
            process.StandardInput.WriteLine("Clean");
            process.StandardInput.WriteLine("Create Partition Primary");
            process.StandardInput.WriteLine($"Format fs={formatType} Quick");
            process.StandardInput.WriteLine("Active");
            return exitDiskpart(process);
        }

        private static string getDiskstable()
        {
            Process process = runDiskpart();
            process.StandardInput.WriteLine("list disk");
            return exitDiskpart(process);
        }

        private static string exitDiskpart(Process process)
        {
            process.StandardInput.WriteLine("exit");
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return output;
        }

        private static Process runDiskpart()
        {
            Process process = new Process();
            process.StartInfo.FileName = "diskpart.exe";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();
            return process;
        }
    }
}
