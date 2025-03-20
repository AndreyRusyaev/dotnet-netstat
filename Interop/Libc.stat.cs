using System.Runtime.InteropServices;

#pragma warning disable CS8981 // CS8981:The type name only contains lower-cased ascii characters.

internal static partial class Libc
{
    [DllImport("libc", SetLastError = true)]
    public static extern int stat(string pathname, ref statbuf statbuf);

    [DllImport("libc", SetLastError = true)]
    public static extern int lstat(string pathname, ref statbuf statbuf);

    [StructLayout(LayoutKind.Sequential)]
    public struct statbuf
    {
        public long st_dev;		/* Device.  */
        public long st_ino;  /* File serial number. */
        public long st_nlink;		/* Link count.  */
        public int st_mode;		/* File mode.  */        
        public int st_uid;		/* User ID of the file's owner.	*/
        public int st_gid;		/* Group ID of the file's group.*/
        public int __pad0;
        public long st_rdev;		/* Device number, if device.  */        
        public long st_size;  /* Size of file, in bytes. */
        public long st_blksize;	/* Optimal block size for I/O.  */
        public long st_blocks;  /* 512-byte blocks */
        public long st_atime;			/* Time of last access.  */
        public long st_atimensec;	/* Nscecs of last access.  */
        public long st_mtime;			/* Time of last modification.  */
        public long st_mtimensec;	/* Nsecs of last modification.  */
        public long st_ctime;			/* Time of last status change.  */
        public long st_ctimensec;	/* Nsecs of last status change.  */        
        public long __glibc_reserved1;
        public long __glibc_reserved2;
        public long __glibc_reserved3;
    }
}