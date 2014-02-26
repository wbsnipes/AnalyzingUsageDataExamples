using System;
using System.IO;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;
using EnvDTE;

namespace Monitor
{
    public static class MonitoredEventFactory
    {

        public static AbstractMonitoredEvent GetMonitoredEvent(Command DTECommandObj)
        {
                object eventObj = new MonitoredCommandEvent(DTECommandObj);
                return (AbstractMonitoredEvent)eventObj;

        }

    }
}
