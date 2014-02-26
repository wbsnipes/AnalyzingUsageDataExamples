using System;
using System.IO;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Monitor
{
    
    [XmlInclude(typeof(MonitoredCommandEvent))]
    [XmlRoot(ElementName = "MonitoredEvent", Namespace = "http://Monitor")]
    public abstract class AbstractMonitoredEvent
    {
        /// <summary>
        /// Default constructor to use in serialization
        /// </summary>
        protected AbstractMonitoredEvent()
        {
        }
        
        // User friendly event name used for recording in logs
        public String EventName { get; set; }

        // Configured classification for the log
        public String Classification { get; set; }

        // Stores information related to artifacts such as window titles active during the event
        public String ArtifactReference { get; set; }

        public void ToLog()
        {
            DataRecorder.WriteLog(String.Join(",", System.DateTime.UtcNow.ToString("u"), this.EventName, this.Classification));
        }

        #region event handler registration and disposal

        public virtual bool RegisterEventForMonitoring(object dte)
        {
            return false;
        }


        public void Dispose()
        {

            this.Dispose(true);

            //GC.SuppressFinalize(this);

        }

        protected virtual void Dispose(bool disposing)
        {
            this.isDisposed = true;
        }

        protected bool isDisposed;

        #endregion
    }
}
