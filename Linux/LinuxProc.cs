internal sealed class LinuxProc
{
    public static IEnumerable<ProcInfo> GetProcInfos()
    {
        var result = new List<ProcInfo>();

        var rootPath = Environment.GetEnvironmentVariable("PROC_ROOT") ?? "/proc";
        foreach (var procEntryPath in Directory.GetFileSystemEntries(rootPath))
        {
            var procEntryName = Path.GetFileName(procEntryPath);

            if (!int.TryParse(procEntryName, out var pid))
            {
                continue;
            }

            var procInfo = new ProcInfo(pid, procEntryPath);

            if (Libc.TryReadLink(Path.Combine(procEntryPath, "exe"), out string exePath))
            {
                procInfo.ExePath = exePath;
            }

            try
            {
                foreach (var fdEntryPath in Directory.GetFileSystemEntries(Path.Combine(procEntryPath, "fd")))
                {
                    var fdEntryName = Path.GetFileName(fdEntryPath);

                    if (!int.TryParse(fdEntryName, out var fd))
                    {
                        continue;
                    }

                    if (!Libc.TryReadLink(fdEntryPath, out string fdTargetPath))
                    {
                        continue;
                    }                   

                    procInfo.AddFileDescriptor(
                        new FileDescriptorInfo(fd, fdEntryPath, fdTargetPath)
                    );
                }
            }
            catch (UnauthorizedAccessException)
            {
                // skip unauthorized items
            }

            result.Add(procInfo);
        }

        return result;
    }    

    public sealed class ProcInfo
    {
        private List<FileDescriptorInfo> fileDescriptorInfos = new List<FileDescriptorInfo>();

        public ProcInfo(int id, string procEntryPath, string? exePath = null)
        {
            Id = id;
            ProcEntryPath = procEntryPath;
            ExePath = exePath;
        }

        public int Id { get; private set; }

        public string ProcEntryPath { get; private set; }

        public string? ExePath { get; set; }

        public IReadOnlyCollection<FileDescriptorInfo> Fds => fileDescriptorInfos.AsReadOnly();

        public void AddFileDescriptor(FileDescriptorInfo fileDescriptorInfo)
        {
            fileDescriptorInfos.Add(fileDescriptorInfo);
        }
    }

    public sealed class FileDescriptorInfo
    {
        public FileDescriptorInfo(int id, string procEntryPath, string? targetPath)
        {
            Id = id;
            ProcEntryPath = procEntryPath;
            TargetPath = targetPath;

            Libc.statbuf fdStat = new Libc.statbuf();
            if (Libc.lstat(procEntryPath, ref fdStat) != -1)
            {
                EntryINode = fdStat.st_ino;
                EntryCreated = DateTimeOffset.FromUnixTimeSeconds(fdStat.st_ctime);
                EntryModified = DateTimeOffset.FromUnixTimeSeconds(fdStat.st_ctime);
            }

            if (!string.IsNullOrEmpty(targetPath) && TryParseSocketIno(targetPath, out int socketIno))
            {
                EntryINode = socketIno;
            }
        }

        public int Id { get; private set; }

        public string? ProcEntryPath { get; private set; }

        public string? TargetPath { get; private set; }

        public long EntryINode { get; private set; }

        public DateTimeOffset EntryCreated { get; private set; }

        public DateTimeOffset EntryModified { get; private set; }

        private static bool TryParseSocketIno(string path, out int socketIno)
        {
            socketIno = 0;

            var pattern = "socket:[";
            var patternStart = path.IndexOf(pattern);
            if (patternStart == -1)
            {
                return false;
            }

            var inoStartIndex = patternStart + pattern.Length;

            var inoEndIndex = path.IndexOf("]", inoStartIndex);
            if (inoEndIndex == -1)
            {
                return false;
            }

            var socketInoString = path.Substring(inoStartIndex, inoEndIndex - inoStartIndex);
            if (!int.TryParse(socketInoString, out socketIno))
            {
                return false;
            }

            return true;
        }
    }
}