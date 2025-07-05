using System;
using System.Diagnostics;
using System.Threading.Tasks;
using OMG.Utilities.ProfilingUtilities.Runtime;
using OMG.Utilities.TaskUtilities.Runtime;
using UnityEngine.Profiling;

namespace OMG.Measurables.Core.Runtime
{
    public abstract class BaseMeasurableGroup : IMeasurableGroup
    {
        public abstract string Title { get; }
        public abstract string Description { get; }

        protected internal ProfilingUtility ProfilingUtility => _profilingUtility ??= new ProfilingUtility();
        private ProfilingUtility _profilingUtility;

        protected internal MemoryProfilingUtility MemoryProfilingUtility =>
            _memoryProfilingUtility ??= new MemoryProfilingUtility();

        private MemoryProfilingUtility _memoryProfilingUtility;

        private Stopwatch _stopwatch = new();
        
        public int Iterations { get; set; }

        private TaskCompletionSource<Task> _accessToken;
        private int[] _iterations;
        private int _currentIndex;

        protected TaskCompletionSource<Task> _iterationToken;

        public async Task Execute(TaskCompletionSource<Task> accessToken, int[] iterations) {
            _accessToken = accessToken;
            _iterations = iterations;
            _iterations ??= new int[] { 1 };

            if (_iterations.Length == 0)
                _iterations = new int[] { 1 };

            _currentIndex = 0;
            Iterations = _iterations[_currentIndex];

            await Setup();

            _ = ExecuteRecursive();
        }


        protected virtual async Task Setup() { }

        protected void StartStopwatch() {
            _stopwatch.Restart();
        }

        protected Stopwatch StopStopwatch() {
            _stopwatch.Stop();
            return _stopwatch;
        }

        private async Task ExecuteRecursive() {
            Profiler.enabled = true;

            await DelayUtility.WaitForFramesRoughly(2);

            var iterationsCount = _iterations.Length;

            if (_currentIndex >= iterationsCount) {
                _ = FinishRunning();
                return;
            }

            Iterations = _iterations[_currentIndex];

            await RunMeasurables();

            _iterationToken = new TaskCompletionSource<Task>();
            _ = OnRunComplete();
            await _iterationToken.Task;

            _currentIndex++;

            _ = ExecuteRecursive();
        }


        protected abstract Task RunMeasurables();


        protected virtual async Task OnRunComplete() {
            _ = RunNext();
        }


        protected virtual async Task RunNext() {
            await DelayUtility.WaitForFramesRoughly(1);

            _iterationToken.SetResult(Task.CompletedTask);
        }


        protected virtual async Task FinishRunning() {
            _ = ReleaseAccess();
        }


        protected async Task ReleaseAccess() {
            try {
                Profiler.enabled = false;
                this._accessToken.SetResult(Task.CompletedTask);
            }
            catch (Exception e) {
                UnityEngine.Debug.LogError(e);
                throw;
            }
        }
    }
}