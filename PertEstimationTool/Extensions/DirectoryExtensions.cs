using System;
using System.IO;

namespace PertEstimationTool.Extensions
{
    public static class DirectoryExtensions
    {
        public static string CreateDirectory(this string directory) => !Directory.Exists(directory) ? Directory.CreateDirectory(directory).ToString() : directory;

        public static string CheckPath(this string path, string fileName = null, string fileExtension = null)
        {
            var inPathFileName = string.Empty;
            var separator = string.Empty;

            if (!Path.EndsInDirectorySeparator(path))
            {
                inPathFileName = Path.GetFileName(path);
                separator = $@"{Path.DirectorySeparatorChar}";
            }

            if (!string.IsNullOrEmpty(inPathFileName))
            {
                inPathFileName = inPathFileName.GetFileName(fileExtension);
                var checkPath = Path.GetDirectoryName(path);

                if (!Directory.Exists(checkPath))
                    checkPath.CreateDirectory();

                if (!Directory.Exists(checkPath))
                    throw new DirectoryNotFoundException($"{checkPath} directory not found!");

                return Path.Combine(checkPath, $@"{Path.DirectorySeparatorChar}", inPathFileName);
            }
            else
            {
                fileName = fileName.GetFileName(fileExtension);

                if (!Directory.Exists(path))
                    path.CreateDirectory();

                if (!Directory.Exists(path))
                    throw new DirectoryNotFoundException($"{path} directory not found!");

                return Path.Combine(path, separator, fileName);
            }
        }

        public static string GetFileName(this string fileName, string fileExtension = null)
        {
            var isFileNameHasExtension = !string.IsNullOrEmpty(Path.GetExtension(fileName));

            if (isFileNameHasExtension)
                return fileName;

            if (string.IsNullOrEmpty(fileExtension))
                throw new Exception($"Extension in the file name not found {fileName}");
            else
                return fileName + fileExtension;
        }
    }
}
