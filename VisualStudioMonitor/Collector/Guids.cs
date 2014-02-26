// Guids.cs
// MUST match guids.h
using System;

namespace Microsoft.Collector
{
    static class GuidList
    {
        public const string guidCollectorPkgString = "d7e029aa-21d3-4802-93a0-b91669ade8e6";
        public const string guidCollectorCmdSetString = "01882daa-c3af-4862-bbeb-12e2e59f08c4";

        public static readonly Guid guidCollectorCmdSet = new Guid(guidCollectorCmdSetString);
    };
}