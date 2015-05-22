using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace AsyncTransactions
{
    public static class SlideExtensions
    {
        public static TaskAwaiter GetAwaiter(this Slide slide)
        {
            return Task.WhenAll(slide.Select(t => t()).ToArray()).GetAwaiter();
        }
    }
}