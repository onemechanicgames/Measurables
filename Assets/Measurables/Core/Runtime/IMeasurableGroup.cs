using System.Threading.Tasks;

namespace OMG.Measurables.Core.Runtime
{
    public interface IMeasurableGroup
    {
        string Title { get; }
        string Description { get; }
        Task Execute(TaskCompletionSource<Task> accessToken, int[] iterations);
    }
}