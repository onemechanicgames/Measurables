using System.Threading.Tasks;
using UnityEngine;

namespace OMG.Utilities.TaskUtilities.Runtime
{
    public static class DelayUtility
    {
        public static async Task WaitForFramesRoughly(int frameCount) {
            float targetRate = Application.targetFrameRate;
            if (targetRate <= 0)
                targetRate = 60;
            
            var ratio = frameCount / targetRate;

            await Task.Delay((int)(ratio * 1000));
        }
    }
}