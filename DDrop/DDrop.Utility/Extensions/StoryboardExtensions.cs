using System.Threading.Tasks;
using System.Windows.Media.Animation;

namespace DDrop.Utility.Extensions
{
    public static class StoryboardExtensions
    {
        public static Task BeginAsync(this Storyboard timeline)
        {
            var source = new TaskCompletionSource<object>();
            timeline.Completed += delegate { source.SetResult(null); };
            timeline.Begin();
            return source.Task;
        }
    }
}