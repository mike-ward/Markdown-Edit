using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace MarkdownEdit.Models
{
    public static class FileExtensions
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
                catch (ObjectDisposedException)
                {
                }
                finally
                {
                    try
                    {
                        fileWatcher.EnableRaisingEvents = true;
                    }
                    catch (ObjectDisposedException)
                    {
                    }
                }
            };
            fileWatcher.EnableRaisingEvents = true;
            return fileWatcher;
        }

        public static string ReadAllTextRetry(this string file)
        {
            var retries = 3;
            while (retries > 0)
            {
                try
                {
                    retries -= 1;
                    return File.ReadAllText(file);
                }
                catch (IOException ex)
                {
                    const int sharingViolation = 32;
                    if (retries == 0 || (ex.HResult & 0xFFFF) != sharingViolation) throw;
                    Thread.Sleep(50);
                }
            }
            throw new IOException();
        }

        public static string MakeRelativePath(string fromPath, string toPath)
        {
            if (string.IsNullOrEmpty(fromPath)) throw new ArgumentNullException(nameof(fromPath));
            if (string.IsNullOrEmpty(toPath)) throw new ArgumentNullException(nameof(toPath));

            var fromUri = new Uri(fromPath);
            var toUri = new Uri(toPath);
            if (fromUri.Scheme != toUri.Scheme) return toPath; // path can't be made relative.
            var relativeUri = fromUri.MakeRelativeUri(toUri);
            var relativePath = relativeUri.ToString();
            return relativePath;
        }

        public static string GetShortPathName(string longPath)
        {
            var shortPath = new StringBuilder(longPath.Length + 1);
            return GetShortPathName(longPath, shortPath, shortPath.Capacity) == 0 ? longPath : shortPath.ToString();
        }

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int GetShortPathName(String path, StringBuilder shortPath, int shortPathLength);
    }
}