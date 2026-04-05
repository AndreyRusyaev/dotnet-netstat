using System.Runtime.InteropServices;
using System.Text;

internal static partial class Libproc
{
    [DllImport("libproc")]
    public static extern int proc_pidpath(int pid, StringBuilder buffer, int buffersize);
}