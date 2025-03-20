using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;

internal static partial class Libc
{
    private const int ENOENT = 2;

    private const int EACCES = 13;

    private const int EINVAL = 22;

    [DllImport("libc", SetLastError = true)]
    public static extern int readlink(string path, StringBuilder buf, int bufSize);

    public static bool TryReadLink(string path, out string linkPath)
    {
        linkPath = string.Empty;

        StringBuilder buffer = new StringBuilder(256);

        var bufferSize = readlink(path, buffer, buffer.Capacity);
        if (bufferSize != -1)
        {
            buffer.Length = bufferSize;
            linkPath = buffer.ToString();
            return true;
        }

        var errorCode = Marshal.GetLastSystemError();
        switch (errorCode)
        {
            case ENOENT: // ENOENT - path does not exist
            case EACCES: // EINVAL - permission denied
            case EINVAL: // EINVAL - path is not link
                break;
            default:
                throw new Win32Exception(errorCode);
        }

        return false;
    }
}