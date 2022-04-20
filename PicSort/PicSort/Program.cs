using System;
using System.Collections.Generic;
using System.IO;

namespace PicSort
{
    class Program
    {
        static void Main(string[] args)
        {
            sort("D:\\slike\\Dzoni's iPhone 11\\temp2", "D:\\slike\\Dzoni's iPhone 11");
        }
        static void sort(string srcPath, string dstPath)
        {
            try
            {
                DirectoryInfo srcDir = new DirectoryInfo(srcPath);
                DirectoryInfo dstDir = new DirectoryInfo(dstPath);

                List<FileInfo> srcFiles = new List<FileInfo>();
                fillList(srcDir, ref srcFiles);

                int numberOfFiles = srcFiles.Count;
                int numberOfTransfers = 0;

                foreach(var file in srcFiles)
                {
                    int percentage = (numberOfTransfers * 100) / numberOfFiles;
                    Console.Write($"\r    Copying files... {numberOfTransfers}/{numberOfFiles} ({percentage}%)                                          ");

                    var date = file.LastWriteTime;
                    int year = date.Year;
                    string month = parseMonth(date.Month);

                    string dstFolderPath = $"{dstDir.FullName}\\{year}\\{month}";
                    Directory.CreateDirectory(dstFolderPath);

                    string dstFilePath = dstFolderPath + $"\\{file.Name}";

                    file.CopyTo(dstFilePath, true);
                    numberOfTransfers++;
                }
                Console.Write($"\r    Done! - {numberOfTransfers}/{numberOfFiles} (100%)                                          ");
            }
            catch (Exception e)
            {
                Console.WriteLine($"We've encountered an error - {e.Message}.\n Exiting...\n");
            }
        }
        static void fillList(DirectoryInfo dir, ref List<FileInfo> files)
        {
            foreach (var file in dir.GetFiles())
                files.Add(file);

            foreach(var subdir in dir.GetDirectories())
                fillList(subdir, ref files);
        }

        static string parseMonth(int month)
        {
            switch (month)
            {
                case 1:
                    return "1Januar";
                case 2:
                    return "2Februar";
                case 3:
                    return "3Mart";
                case 4:
                    return "4April";
                case 5:
                    return "5Maj";
                case 6:
                    return "6Jun";
                case 7:
                    return "7Jul";
                case 8:
                    return "8Avgust";
                case 9:
                    return "9Septembar";
                case 10:
                    return "10Oktobar";
                case 11:
                    return "11Novembar";
                case 12:
                    return "12Decembar";
                default:
                    return "Other";
            }
        }
    }
}
