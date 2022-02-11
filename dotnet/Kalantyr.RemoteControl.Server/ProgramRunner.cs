using System;
using System.IO;

namespace Kalantyr.RemoteControl.Server
{
    public static class ProgramRunner
    {
        public static void LogIfException(Action action)
        {
            try
            {
                action();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                WriteToLogFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "error.log"), e);
                throw;
            }
        }

        private static void WriteToLogFile(string logFileName, Exception e)
        {
            using var file = new FileStream(logFileName, FileMode.Create, FileAccess.Write, FileShare.None);
            using var writer = new StreamWriter(file);
            writer.WriteLine($"--------- ERROR -----------");
            writer.WriteLine($"{DateTime.Now} {e.GetType().Name} {e.Message}");
            writer.WriteLine(e.GetBaseException().ToString());
            writer.Flush();
        }
    }
}
