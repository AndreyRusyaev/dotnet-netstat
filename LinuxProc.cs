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

            var procInfo = new ProcInfo();
            procInfo.Id = pid;
            procInfo.EntryPath = procEntryPath;

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

                    var fdInfo = new FdInfo();
                    fdInfo.Id = fd;
                    fdInfo.EntryPath = fdEntryPath;
                    fdInfo.TargetPath = fdTargetPath;

                    Libc.statbuf fdStat = new Libc.statbuf();
                    if (Libc.lstat(fdEntryPath, ref fdStat) != -1)
                    {
                        fdInfo.EntryINode = fdStat.st_ino;
                        fdInfo.EntryCreated = DateTimeOffset.FromUnixTimeSeconds(fdStat.st_ctime);
                        fdInfo.EntryModified = DateTimeOffset.FromUnixTimeSeconds(fdStat.st_ctime);
                    }

                    if (TryParseSocketIno(fdTargetPath, out int socketIno))
                    {
                        fdInfo.EntryINode = socketIno;
                    }

                    procInfo.Fds.Add(fdInfo);
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

    public sealed class ProcInfo
    {
        public int Id;

        public string EntryPath;

        public string ExePath;

        public List<FdInfo> Fds { get; } = new List<FdInfo>();
    }

    public sealed class FdInfo
    {
        public int Id;

        public string? EntryPath;

        public string? TargetPath;

        public long EntryINode;

        public DateTimeOffset EntryCreated;

        public DateTimeOffset EntryModified;
    }
}