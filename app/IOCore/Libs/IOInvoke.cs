using System;
using System.Runtime.InteropServices;

namespace IOCore.Libs
{
    public class IOInvoke
    {
        [DllImport("User32", ExactSpelling = true, EntryPoint = "GetWindowLongW", SetLastError = true)]
        private static extern int GetWindowLong_x86(IntPtr hWnd, int nIndex);

        [DllImport("User32", ExactSpelling = true, EntryPoint = "GetWindowLongPtrW", SetLastError = true)]
        private static extern IntPtr GetWindowLongPtr_x64(IntPtr hWnd, int nIndex);

        public static IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex)
        {
            return IntPtr.Size == 4 ? (IntPtr)GetWindowLong_x86(hWnd, nIndex) : GetWindowLongPtr_x64(hWnd, nIndex);
        }

        //

        [DllImport("User32", ExactSpelling = true, EntryPoint = "SetWindowLongW", SetLastError = true)]
        private static extern int SetWindowLong_x86(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("User32", ExactSpelling = true, EntryPoint = "SetWindowLongPtrW", SetLastError = true)]
        private static extern IntPtr SetWindowLongPtr_x64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        public static IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
        {
            return IntPtr.Size == 4 ? (IntPtr)SetWindowLong_x86(hWnd, nIndex, (int)dwNewLong) : SetWindowLongPtr_x64(hWnd, nIndex, dwNewLong);
        }
    }
}
