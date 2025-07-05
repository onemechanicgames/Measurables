using System;
using Unity.Profiling.LowLevel;
using Unity.Profiling.LowLevel.Unsafe;

namespace OMG.Utilities.ProfilingUtilities.Runtime
{
    public class ProfilingUtility
    {
        private IntPtr _markerPointer;

        public void BeginSample(string markerName) {
            _markerPointer = ProfilerUnsafeUtility.CreateMarker(markerName, ProfilerUnsafeUtility.CategoryScripts,
                MarkerFlags.Script | MarkerFlags.AvailabilityNonDevelopment, 0);
            ProfilerUnsafeUtility.BeginSample(_markerPointer);
        }

        public void EndSample() {
            ProfilerUnsafeUtility.EndSample(_markerPointer);
        }
    }
}