using System;
using System.IO;

namespace NCS.DSS.AzureSearchUtility.Helpers
{
    public static class FileHelper
    {
        public static string GenerateSwaggerFileName(string environmentName)
        {
            return string.Format("{0}{1}{2}{3}{4:yyyyMMdd-hhmmss}{5}", "dss-", environmentName, "-searchutility", "-fa_swagger-def_", DateTime.Now, ".json");
        }

        public static string GenerateFilePath(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentNullException(nameof(fileName));

            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
        }

        public static void GenerateFileOnServer(string path, string content)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));

            if (string.IsNullOrEmpty(content))
                throw new ArgumentNullException(nameof(content));

            File.WriteAllText(path, content);
        }
    }
}
