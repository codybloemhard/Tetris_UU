using System;
using System.Runtime.InteropServices;

namespace Tetris
{
    public static class Program
    {
        /*
        Pressing the [X] button on the monogame window
        does not always close the process, but if you close the
        console the process is killed too so this is needed.
        Get console to show up:
        source: https://stackoverflow.com/questions/4362111/how-do-i-show-a-console-output-window-in-a-forms-application
        */
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();
        
        [STAThread]
        static void Main()
        {
            AllocConsole();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Close this console to close the game!");
            Tetris game = new Tetris();
            game.Run();
        }
    }
}