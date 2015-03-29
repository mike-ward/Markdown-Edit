using System;
using System.IO;

namespace MarkdownEdit.Models
{
    internal static class FileExtensions
    {
        public static string GetRelativePathFrom(this FileSystemInfo to, FileSystemInfo from)
        {
            return from.GetRelativePathTo(to);
        }

        public static string GetRelativePathTo(this FileSystemInfo from, FileSystemInfo to)
        {
            Func<FileSystemInfo, string> getPath = fsi =>
            {
                var d = fsi as DirectoryInfo;
                return d == null ? fsi.FullName : d.FullName.TrimEnd('\\') + "\\";
            };

            var fromPath = getPath(from);
            var toPath = getPath(to);

            var fromUri = new Uri(fromPath);
            var toUri = new Uri(toPath);

            var relativeUri = fromUri.MakeRelativeUri(toUri);
            var relativePath = Uri.UnescapeDataString(relativeUri.ToString());

            return relativePath.Replace('/', Path.DirectorySeparatorChar);
        }

        public static FileSystemWatcher WatchFile(this string file, Action onChange)
        {
            var fileWatcher = new FileSystemWatcher
            {
                Path = Path.GetDirectoryName(file),
                Filter = Path.GetFileName(file),
                NotifyFilter = NotifyFilters.LastWrite
            };
            fileWatcher.Changed += (sender, args) =>
            {
                // Suggested method to "debounce" multiple notifications
                try
                {
                    fileWatcher.EnableRaisingEvents = false;
                    onChange();
                }
                finally
                {
                    fileWatcher.EnableRaisingEvents = true;
                }
            };
            fileWatcher.EnableRaisingEvents = true;
            return fileWatcher;
        }
    }
}