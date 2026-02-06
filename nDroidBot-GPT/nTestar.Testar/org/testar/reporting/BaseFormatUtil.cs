using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace org.testar.reporting
{
    public abstract class BaseFormatUtil
    {
        private FileInfo file;
        protected readonly List<string> content = new List<string>();
        private readonly string fileSuffix;

        public FileInfo getFile()
        {
            return file;
        }

        protected BaseFormatUtil(string fileString, string fileSuffix)
        {
            this.fileSuffix = fileSuffix.StartsWith(".") ? fileSuffix.ToLowerInvariant() : "." + fileSuffix.ToLowerInvariant();
            file = new FileInfo(enforceFileSuffix(fileString));
            createFile();
        }

        private string enforceFileSuffix(string fileName)
        {
            return fileName.ToLowerInvariant().EndsWith(fileSuffix) ? fileName : fileName + fileSuffix;
        }

        protected string[] splitStringAtNewline(string longString)
        {
            return longString.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.None);
        }

        private void createFile()
        {
            if (!file.Exists)
            {
                if (file.Directory != null && !file.Directory.Exists)
                {
                    try
                    {
                        file.Directory.Create();
                    }
                    catch
                    {
                        Console.WriteLine("Failed to create the directory structure.");
                    }
                }

                try
                {
                    using var _ = file.Create();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error creating the file: " + e.Message);
                }
            }
        }

        public void appendToFileName(string appendToName)
        {
            string oldPath = file.FullName;
            string? directory = file.DirectoryName;
            string newName = enforceFileSuffix(file.Name.Replace(getFileExtension(file.Name), appendToName));
            string newPath = directory == null ? newName : Path.Combine(directory, newName);
            File.Move(oldPath, newPath, true);
            file = new FileInfo(newPath);
        }

        private string getFileExtension(string fileName)
        {
            int lastDotIndex = fileName.LastIndexOf(".", StringComparison.Ordinal);
            return lastDotIndex == -1 ? ".html" : fileName.Substring(lastDotIndex);
        }

        public void renameFile(string newName)
        {
            string? directory = file.DirectoryName;
            string newPath = directory == null ? enforceFileSuffix(newName) : Path.Combine(directory, enforceFileSuffix(newName));
            File.Move(file.FullName, newPath, true);
            file = new FileInfo(newPath);
        }

        public void moveFile(string newDirectory)
        {
            string newPath = Path.Combine(newDirectory, file.Name);
            File.Move(file.FullName, newPath, true);
            file = new FileInfo(newPath);
        }

        public void duplicateFile(string destinationPath)
        {
            try
            {
                File.Copy(file.FullName, destinationPath, true);
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine("Error copying the file: " + e.Message);
            }
            catch (IOException e)
            {
                Console.WriteLine(e);
            }
        }

        public void writeToFile()
        {
            if (content.Count == 0)
            {
                return;
            }

            try
            {
                using var stream = new FileStream(file.FullName, FileMode.Append, FileAccess.Write, FileShare.Read);
                using var writer = new StreamWriter(stream, new UTF8Encoding(false));
                foreach (string line in content)
                {
                    writer.WriteLine(line);
                }
            }
            catch (IOException e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                content.Clear();
            }
        }
    }
}
