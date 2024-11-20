using System;
using System.Collections.Generic;
using System.IO;
using MetadataExtractor;
using System.Linq;
using System.Globalization;

namespace PicSort
{
    class Program
    {
        static void Main(string[] args)
        {
            if(args.Length < 2)
            {
                Console.WriteLine("Invalid use. Please provide absolute paths to src and dst folders like such: srcPath dstPath");
                return;
            }
            var srcPath = args[0];
            var dstPath = args[1];
            SortPictures(srcPath, dstPath);
            return;
        }
        
        static void SortPictures(string srcPath, string dstPath)
        {
            try
            {
                DirectoryInfo srcDir = new(srcPath);
                DirectoryInfo dstDir = new(dstPath);

                List<FileInfo> srcFiles = [];
                GetAllFilesRecursively(srcDir, ref srcFiles);

                int numberOfFiles = srcFiles.Count;
                int numberOfTransfers = 0;

                foreach(var file in srcFiles)
                {
                    int percentage = (numberOfTransfers * 100) / numberOfFiles;
                    Console.Write($"\r    Copying files... {numberOfTransfers}/{numberOfFiles} ({percentage}%)                                          ");

                    if (Path.GetExtension(file.FullName).ToLower().EndsWith("aae")) continue;

                    IEnumerable<MetadataExtractor.Directory> metadataDirs = ImageMetadataReader.ReadMetadata(file.FullName);
                    var tags = metadataDirs
                        .SelectMany(x => x.Tags
                            .Select(y => new
                                {
                                    dir = x.Name,
                                    tag = y.Name,
                                    value = y.Description
                            })
                        );

                    var dateTakenTags = tags.Where(x =>
                        x.dir == "QuickTime Track Header" && x.tag == "Created"
                        || x.dir == "Exif IFD0" && x.tag == "Date/Time"
                        || x.dir == "Exif SubIFD" && x.tag == "Date/Time Original")
                        .ToList();

                    DateTime? dateTaken = DateTime.MinValue;
                    if (dateTakenTags.Count == 0)
                    {
                        dateTaken = null;
                    }
                    else
                    {
                        var dateTakenString = dateTakenTags.First().value.ToString();

                        string videoFormat = "ddd MMM dd HH:mm:ss yyyy";
                        string imageFormat = "yyyy:MM:dd HH:mm:ss";
                        if (DateTime.TryParseExact(dateTakenString, videoFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime videDateTaken))
                        {
                            dateTaken = videDateTaken;
                        }
                        else if (DateTime.TryParseExact(dateTakenString, imageFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime imageDateTaken))
                        {
                            dateTaken = imageDateTaken;
                        }

                    }

                    var dstFolderPath = Path.Combine(dstDir.FullName, "unsorted");
                    if(dateTaken != null )
                    {
                        var year = dateTaken.Value.Year.ToString();
                        string month = GetDestinationFolderNameFromMonth(dateTaken.Value.Month);
                        dstFolderPath =  Path.Combine(dstDir.FullName, year, month);
                    }

                    System.IO.Directory.CreateDirectory(dstFolderPath);

                    string dstFilePath = Path.Combine(dstFolderPath, file.Name);

                    file.CopyTo(dstFilePath, true);
                    numberOfTransfers++;
                }
                Console.Write($"\r    Done! - {numberOfTransfers}/{numberOfFiles} (100%)                                          ");
            }
            catch (Exception e)
            {
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine($"We've encountered an error - {e.Message}.\n Exiting...\n");
            }
        }

        static void GetAllFilesRecursively(DirectoryInfo dir, ref List<FileInfo> files)
        {
            foreach (var file in dir.GetFiles())
                files.Add(file);

            foreach(var subdir in dir.GetDirectories())
                GetAllFilesRecursively(subdir, ref files);
        }

        static string GetDestinationFolderNameFromMonth(int month)
        {
            return month switch
            {
                1 => "1Januar",
                2 => "2Februar",
                3 => "3Mart",
                4 => "4April",
                5 => "5Maj",
                6 => "6Jun",
                7 => "7Jul",
                8 => "8Avgust",
                9 => "9Septembar",
                10 => "10Oktobar",
                11 => "11Novembar",
                12 => "12Decembar",
                _ => "Other",
            };
        }
    }
}
