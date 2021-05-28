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
        internal static List<Disk> GetDisks()
        {
            string output = GetDiskstable();
            return GetDisksList(output);
        }

        private static List<Disk> GetDisksList(string output)
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
            //remove disk 0 (hard drive)
            disks = disks.Where(d => d.Index != "Disk 0").ToList();
            return disks;
        }

        internal static string CleanDisk(string index, string formatType)
        {
            Process process = RunDiskpart();
            process.StandardInput.WriteLine($"Select {index}");
            process.StandardInput.WriteLine("Clean");
            process.StandardInput.WriteLine("Clean");
            process.StandardInput.WriteLine("Create Partition Primary");
            process.StandardInput.WriteLine($"Format fs={formatType} Quick");
            process.StandardInput.WriteLine("Active");
            var cleaningResult = ExitDiskpart(process);
            cleaningResult = cleaningResult.Replace("DiskPart has encountered an error: Access is denied.", "");
            cleaningResult = cleaningResult.Replace("See the System Event Log for more information.", "");
            return cleaningResult;
        }

        private static string GetDiskstable()
        {
            Process process = RunDiskpart();
            process.StandardInput.WriteLine("list disk");
            return ExitDiskpart(process);
        }

        private static string ExitDiskpart(Process process)
        {
            process.StandardInput.WriteLine("exit");
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return output;
        }

        private static Process RunDiskpart()
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
