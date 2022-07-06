#if UNITY_EDITOR_WIN

using System;
using System.Runtime.InteropServices;

// References
// https://docs.microsoft.com/en-us/windows/win32/api/winuser/
// https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setforegroundwindow
// https://stackoverflow.com/questions/12535165/how-do-i-set-the-focus-to-the-desktop-from-within-my-c-sharp-application
internal static class WinUser{

    [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
    static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    [DllImport("user32.dll")]
    private static extern IntPtr GetActiveWindow();

    [DllImport("user32.dll")]
    private static extern IntPtr SetActiveWindow(IntPtr hwnd);

    [DllImport("user32.dll")]
    private static extern IntPtr SetForegroundWindow(IntPtr hwnd);

    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);

    [DllImport("user32.dll", EntryPoint = "SendMessage", SetLastError = true)]
    static extern IntPtr SendMessage(IntPtr hWnd, Int32 Msg, IntPtr wParam, IntPtr lParam);

    const int WM_COMMAND = 0x111;
    const int MIN_ALL = 419;
    const int MIN_ALL_UNDO = 416;

    public static void Refocus(){
        var editorWindow = GetActiveWindow();
        var shellWindow  = FindWindow("Shell_TrayWnd", null);
        SetForegroundWindow(shellWindow);
        // NOTE - waiting a too short delay will not "kick" the editor
        System.Threading.Thread.Sleep(1000);
        SetForegroundWindow(editorWindow);
    }

}

#endif
