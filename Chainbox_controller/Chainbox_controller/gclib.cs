using System;
using System.Runtime.InteropServices;
using System.Text;

namespace gclib
{
    public class gclib
    {
        private IntPtr handle;

        [DllImport("gclib.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "GOpen")]
        private static extern int GOpenNative(string address, out IntPtr g);

        [DllImport("gclib.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "GClose")]
        private static extern int GCloseNative(IntPtr g);

        [DllImport("gclib.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "GCommand")]
        private static extern int GCommandNative(
            IntPtr g,
            string command,
            StringBuilder buffer,
            int buffer_len,
            IntPtr bytes_returned
        );

        [DllImport("gclib.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "GVersion")]
        private static extern IntPtr GVersionNative();

        [DllImport("gclib.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "GInfo")]
        private static extern IntPtr GInfoNative(IntPtr g);

        public void GOpen(string address)
        {
            int rc = GOpenNative(address, out handle);
            if (rc != 0)
                throw new Exception("GOpen failed: " + rc);
        }

        public void GClose()
        {
            GCloseNative(handle);
        }

        public string GInfo()
        {
            return Marshal.PtrToStringAnsi(GInfoNative(handle));
        }

        public string GVersion()
        {
            return Marshal.PtrToStringAnsi(GVersionNative());
        }

        public string GCommand(string command)
        {
            StringBuilder buffer = new StringBuilder(1024);

            GCommandNative(
                handle,
                command,
                buffer,
                buffer.Capacity,
                IntPtr.Zero
            );

            return buffer.ToString();
        }
    }
}