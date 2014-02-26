using System;

namespace Monitor
{
    public static class DataRecorder
    {
        public static void Start()
        {
            logDirectoryPath = System.IO.Path.GetTempPath();
            logFileName = System.IO.Path.Combine(logDirectoryPath, "collector " + DateTime.Now.ToString("yyyy-MM-dd HH.mm.ss") + ".log");
            try
            {
                using (System.IO.StreamWriter streamWriter = new System.IO.StreamWriter(
                    new System.IO.FileStream(logFileName, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write, System.IO.FileShare.ReadWrite)
                    ))
                {
                    streamWriter.WriteLine("Collector Started");
                }
            }
            catch (System.IO.IOException ioexception)
            {
                Console.WriteLine("Error creating log file " + ioexception);
            }
            myEvents = new MonitoredEventCollection();
            myEvents.RegisterEventInventoryForEventMonitoring();
        }
        
        public static void Stop()
        {
            myEvents.DeRegisterEventMonitoringForInventory();
            WriteLog("Collector Stopped");
        }

        public static void WriteLog(string logToWrite)
        {
            try
            {
                using (System.IO.StreamWriter streamWriter = new System.IO.StreamWriter(
                    new System.IO.FileStream(logFileName, System.IO.FileMode.Append, System.IO.FileAccess.Write, System.IO.FileShare.ReadWrite)
                    ))
                {
                    streamWriter.WriteLine(logToWrite);
                }
            }
            catch (System.IO.IOException ioexception)
            {
                Console.WriteLine("Error writing to log file " + ioexception);
            }
        }

        static MonitoredEventCollection myEvents;
        private static string logFileName;
        private static string logDirectoryPath;
    }
}
