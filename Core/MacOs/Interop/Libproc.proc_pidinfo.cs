using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;

internal static partial class Libproc
{
    public const int PROC_PIDLISTFDS = 1;

    public const int PROX_FDTYPE_SOCKET = 2;

    [StructLayout(LayoutKind.Sequential)]
    public struct proc_fdinfo 
    {
        public int proc_fd;
        public int proc_fdtype;
    }

    [DllImport("libproc")]
    public static extern int proc_pidinfo(int pid, int flavor, long arg, IntPtr buffer, int buffersize);

    [DllImport("libproc")]
    public static extern int proc_pidinfo(int pid, int flavor, long arg, [Out][MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 4)] proc_fdinfo[] buffer, int buffersize);
}