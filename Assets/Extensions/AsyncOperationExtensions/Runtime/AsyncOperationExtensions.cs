using System.Threading.Tasks;
using UnityEngine;

namespace OMG.Extensions.AsyncOperationExtensions.Runtime
{
    public static class ____/*AsyncOperationExtensions*/
    {
        public static async Task GetAwaitable(this AsyncOperation asyncOperation) {
            var tcs = new TaskCompletionSource<bool>();
            asyncOperation.completed += (op) => { tcs.SetResult(true); };
            await tcs.Task;
        }
    }
}