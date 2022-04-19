using System;

namespace PicSort
{
    class Program
    {
        static void Main(string[] args)
        {
            sort("", "");
        }
        static void sort(string dstPath, string srcPath)
        {
            for (int i = 0; i <= 100; i++)
            {
                Console.Write("\r{0}%   ", i);
            }
        }
    }
}
