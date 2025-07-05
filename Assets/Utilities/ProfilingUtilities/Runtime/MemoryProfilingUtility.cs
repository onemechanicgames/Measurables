#define SKIP_EDITOR_SNAPSHOT

using System;
using System.IO;
using System.Threading.Tasks;
using Unity.Profiling.Memory;
using UnityEngine;

namespace OMG.Utilities.ProfilingUtilities.Runtime
{
    public class MemoryProfilingUtility
    {
        private const CaptureFlags kCaptureFlags =
            CaptureFlags.ManagedObjects | CaptureFlags.NativeObjects | CaptureFlags.NativeAllocations;

        public async System.Threading.Tasks.Task TakeMemorySnapshot(params object[] snapshotNameTokens) {
            if (snapshotNameTokens.Length == 0) {
                Debug.LogException(new ArgumentException("Snapshot name cannot be null or empty."));
                return;
            }

            var snapshotIdentifier = string.Join('_', snapshotNameTokens);
            var snapshotDirectoryPath = Resources.Load<TextAsset>("MemoryProfilerSnapshotPath").text.Trim();
            var snapshotFilePath = await TakeSnapshot(snapshotIdentifier);

            if (string.IsNullOrEmpty(snapshotFilePath)) {
                Debug.LogWarning("Failed to capture memory snapshot");
                return;
            }

            var finalSnapshot = Path.ChangeExtension(snapshotFilePath, ".snap");
            finalSnapshot = Path.Combine(snapshotDirectoryPath, finalSnapshot);

            if (File.Exists(finalSnapshot))
                File.Delete(finalSnapshot);

            File.Move(snapshotFilePath, finalSnapshot);
        }

        private async Task<string> TakeSnapshot(string snapshotIdentifier) {
#if UNITY_EDITOR && SKIP_EDITOR_SNAPSHOT
            await Task.Delay(1000);
            Debug.LogWarning("Editor snapshots disabled!");
            return null;
#endif

            var tcs = new TaskCompletionSource<string>();

            MemoryProfiler.TakeSnapshot(snapshotIdentifier, OnSnapshotCaptured, kCaptureFlags);

            var taskResult = await tcs.Task;

            return taskResult;

            void OnSnapshotCaptured(string snapshotFilePath, bool success) {
                tcs.SetResult(success ? snapshotFilePath : null);
            }
        }
    }
}