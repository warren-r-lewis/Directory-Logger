using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DirectoryObserver
{
    class Program
    {
        static void Main(string[] args)
        {
            RunProgram();
            Console.ReadLine();
        }

        //Method to consolidate other methods
        static public void RunProgram()
        {
            string originFile = FileToAnalyze();
            bool AccessLevel = IncludeSubDirectories();
            if (AccessLevel)
            {
                CsvOutPutAll(originFile);
            }
            else
            {
                CsvOutPutTop(originFile);
            }

        }

        //method to include sub directories
        static bool IncludeSubDirectories()
        {
            bool flag = true;
            string dirFlagInput;

            do
            {
                Console.Write("Do you want to include subdirectories? (Y/N): ");
                dirFlagInput = Console.ReadLine();
            }
            while ((dirFlagInput != "Y") && (dirFlagInput != "N"));

            if (dirFlagInput == "Y")
            {
                flag = true;
            }
            else if (dirFlagInput == "N")
            {
                flag = false;
            }
            else
            {
                Console.WriteLine("Invalid input. Default to top directory only");
            }
            return flag;
        }


        //method to check if entered directory exists
        static string DirectoryExists(string directory)
        {
            bool directroyExists = Directory.Exists(directory);
            while (!directroyExists)
            {
                Console.Write("Directory does not exist. Enter an existing directory: ");
                directory = Console.ReadLine();
            }

            return directory;
        }
        //Method asks for file to analyze
        static string FileToAnalyze()
        {
            Console.Write("Enter the directory you wish to analyze: ");
            string originFile = Console.ReadLine();
            DirectoryExists(originFile);
            return originFile;
        }


        //Method to get the type, path, and hash of origin files, and then adds to csv, when including sub folders
        static void CsvOutPutAll(string originFile)
        {

            string[] files = Directory.GetFiles(originFile, "*", SearchOption.AllDirectories);
            Console.Write("Enter destination file name: ");
            string destinationFile = Console.ReadLine();
            foreach (string file in files)
            {
                String Signature = BitConverter.ToString(File.ReadAllBytes(file));
                string fileType = "NULL";
                if (Signature.Substring(0, 5) == "FF-D8")
                {
                    fileType = "JPEG";
                }
                else if (Signature.Substring(0, 11) == "25-50-44-46")
                {
                    fileType = "PDF";
                }
                string type = fileType;

                var pathInfo = new FileInfo(file);
                string path = pathInfo.FullName;
                int hash = pathInfo.GetHashCode();
                PictureFile NewEntry = new PictureFile(type, path, hash);
                NewEntry.AddToCSV(destinationFile);


            }

        }

        // Above, without subfolders
        static void CsvOutPutTop(string originFile)
        {
            string originFileDestination = FileToAnalyze();

            string[] files = Directory.GetFiles(originFile, "*", SearchOption.TopDirectoryOnly);
            Console.Write("Enter destination file name: ");
            string destinationFile = Console.ReadLine();

            foreach (string file in files)
            {
                String Signature = BitConverter.ToString(File.ReadAllBytes(file));
                string fileType = "NULL";
                if (Signature.Substring(0, 5) == "FF-D8")
                {
                    fileType = "JPEG";
                }
                else if (Signature.Substring(0, 11) == "25-50-44-46")
                {
                    fileType = "PDF";
                }
                string type = fileType;

                var pathInfo = new FileInfo(file);
                string path = pathInfo.FullName;
                int hash = pathInfo.GetHashCode();
                PictureFile NewEntry = new PictureFile(type, path, hash);
                NewEntry.AddToCSV(destinationFile);


            }

        }

        //class to create a data type for each file  
        public class PictureFile
        {
            public string type;
            public string path;
            public int hash;

            public PictureFile(string aType, string aPath, int aHash)
            {
                type = aType;
                path = aPath;
                hash = aHash;
            }
            public void AddToCSV(string destinationFile)
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(@destinationFile, true))
                {
                    file.WriteLine(type + "," + path + "," + hash);
                }

            }
        }
    }
}
