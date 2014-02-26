using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System.Xml.Serialization; //12.0

namespace Monitor
{
    public class MonitoredEventCollection
    {
        /// <summary>
        /// Object to store all the MonitoredEvents we have on file
        /// </summary>
        private List<AbstractMonitoredEvent> IDEEventListenerRegistry;

        /// <summary>
        /// Constructor that reads events from a file or queries Visual Studio for the command events
        /// if the file does not exist. Then saves the events to the file for next time.
        /// </summary>
        public MonitoredEventCollection()
        {
            String EventInventoryFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "CommandGUIDs.xml");
            MonitoredEventCollectionInitialize(EventInventoryFilePath);
        }

        private void MonitoredEventCollectionInitialize(String EventInventoryFilePath) {
            if (File.Exists(EventInventoryFilePath)) {
                IDEEventListenerRegistry = LoadEventsFromFile(EventInventoryFilePath);
            }
            else {
                IDEEventListenerRegistry = QueryVSForAllDTECommands();

            }
            if (IDEEventListenerRegistry != null) {
                saveEventInterestTable(IDEEventListenerRegistry, EventInventoryFilePath);
            }
        }

        private List<AbstractMonitoredEvent> LoadEventsFromFile(string filepath)
        {
            try
            {
                List<AbstractMonitoredEvent> eventList = new List<AbstractMonitoredEvent>();
                var serializer = new System.Xml.Serialization.XmlSerializer(typeof(List<AbstractMonitoredEvent>));
                using (Stream file = new FileStream(filepath, FileMode.Open, FileAccess.Read))
                {
                    eventList = (List<AbstractMonitoredEvent>)serializer.Deserialize(file);
                }
                return eventList;
            }
            catch (System.IO.IOException)
            {
                Console.WriteLine("Error opening file with event inventory" + filepath);
                return null;
            }
        }

        private void saveEventInterestTable(List<AbstractMonitoredEvent> eventList, string filepath)
        {
            try
            {
                var serializer = new System.Xml.Serialization.XmlSerializer(typeof(List<AbstractMonitoredEvent>));
                using (Stream file = new FileStream(filepath, FileMode.Create, FileAccess.Write))
                {
                    serializer.Serialize(file, eventList);
                    file.Flush();
                }
            }
            catch (System.IO.IOException)
            {
                Console.WriteLine("Error creating file for storing monitored events with file path:" + filepath);
            }

        }
        /// <summary>
        /// Query the DTE Commands object for all events it provides.  Could be useful to determine whether new commands from
        /// Add-Ins or Extensions appeared since we built the inventory.  Returns a collection of Events with Immediate type
        /// </summary>
        /// <returns>List of AbstractMonitoredEvents in the DTE object</returns>
        private List<AbstractMonitoredEvent> QueryVSForAllDTECommands()
        {
            List<AbstractMonitoredEvent> EventList = new List<AbstractMonitoredEvent>();
            DTE dteobj = tryGetDTEObject();
            if (dteobj != null)
            {
                
                try
                {
                    foreach (Command DTE_CommandEventObj in dteobj.Commands)
                    {
                        AbstractMonitoredEvent NewEvent = MonitoredEventFactory.GetMonitoredEvent(DTE_CommandEventObj);
                        if (NewEvent != null)
                        {
                            EventList.Add(NewEvent);
                        }
                    }
                }
                //This exception happens during dispose/finalize when VS exits, just return null
                catch (System.Runtime.InteropServices.InvalidComObjectException)
                {
                    return null;
                }
            }
            return EventList;
        }


        /// <summary>
        /// Gets a DTE object for the currently running Visual Studio instance.  Requires references
        /// to EnvDTE, Microsoft.VisualStudio.Shell.12.0, and Microsoft.VisualStudio.OLE.Interop.
        /// </summary>
        /// <returns></returns>
		private static DTE tryGetDTEObject()
		{
			DTE dteobj=null;
            try
            {
                dteobj = ((EnvDTE.DTE)ServiceProvider.GlobalProvider.GetService(typeof(EnvDTE.DTE).GUID)).DTE;
            }
            catch (NullReferenceException)
            { }
            catch (System.Runtime.InteropServices.InvalidComObjectException)
            { }
            catch (System.Runtime.InteropServices.COMException)
            { }
			return dteobj;
		}

        public bool RegisterEventInventoryForEventMonitoring()
        {

            DTE dteobj = tryGetDTEObject();
            bool somethingRegistered = false;
            if (dteobj != null && IDEEventListenerRegistry != null && IDEEventListenerRegistry.Count > 0)
            {

                foreach (AbstractMonitoredEvent command in IDEEventListenerRegistry)
                {
                    if (command.RegisterEventForMonitoring(dteobj))
                    {
                        somethingRegistered = true;
                    }
                }


            }

            return somethingRegistered;
        }

        public void DeRegisterEventMonitoringForInventory()
        {

            foreach (AbstractMonitoredEvent monitoredEvent in IDEEventListenerRegistry)
            {
                monitoredEvent.Dispose();
            }

        }

    }
}
