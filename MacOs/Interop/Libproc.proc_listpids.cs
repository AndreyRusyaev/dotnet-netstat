using System.Runtime.InteropServices;

internal static partial class Libproc
{
    public const int PROC_ALL_PIDS = 1;

    [DllImport("libproc")]
    public static extern int proc_listpids(int type, int typeinfo, IntPtr buffer, int buffersize);
}