using System;
using System.Xml.Linq;
using EnvDTE;
using System.Xml.Serialization;

namespace Monitor
{

    [XmlRoot(ElementName = "MonitoredEvent", Namespace = "http://Monitor")]
    public class MonitoredCommandEvent : AbstractMonitoredEvent
    {

        /// <summary>
        /// DTE object EventID integer distinguishes events with a shared GUID.
        /// </summary>
        public int EventID { get; set; }

        /// <summary>
        /// GUID of the DTE event from Visual Studio
        /// </summary>
        public String Guid { get; set; }

        /// <summary>
        /// Default constructor to use in serilization
        /// </summary>
        public MonitoredCommandEvent()
        {
        }

        /// <summary>
        /// Create an object from the Command class of the DTE
        /// </summary>
        /// <param name="DTECommandObj">Command class of the DTE</param>
        public MonitoredCommandEvent(Command DTECommandObj)
        {
            if (DTECommandObj != null)
            {
                this.EventName = DTECommandObj.Name;
                this.Classification = EventName.Split('.')[0];  //use the first part of event name
                this.Guid = DTECommandObj.Guid;
                this.EventID = DTECommandObj.ID;
            }
            else
            {
                throw new ArgumentNullException("DTECommandObj");
            }
        }

        #region Event registration, disposal, and hander
        ///<summary>
        ///The event type object holds the event class type for this interceptor for example CommandEvents
        ///the RegisterEvent method registers the event 
        ///</summary>
        private CommandEvents eventTypeObject;

        public override bool RegisterEventForMonitoring(object dte)
        {
            if (!isDisposed && eventTypeObject == null && dte != null)
            {
                eventTypeObject = (dte as DTE).Events.get_CommandEvents(Guid, EventID) as CommandEvents;
            }
            if (eventTypeObject != null)
            {
                eventTypeObject.AfterExecute += new _dispCommandEvents_AfterExecuteEventHandler(OnAfterExecute);
            }
            return (eventTypeObject != null);
        }


        /// <summary>
        /// Remove the event from the handler list
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (eventTypeObject != null)
                eventTypeObject.AfterExecute -= OnAfterExecute;
            this.isDisposed = true;
        }


        /// <summary>
        /// Method receives event after the command completes execution.  Adds the end of 
        /// the command event to the log
        /// </summary>
        /// <param name="Guid">Guid of the command</param>
        /// <param name="ID">numeric id of the command</param>
        /// <param name="CustomIn"></param>
        /// <param name="CustomOut"></param>
        private void OnAfterExecute(string Guid, int ID, object CustomIn, object CustomOut)
        {
            this.ToLog();
        }
        #endregion
    }
}
