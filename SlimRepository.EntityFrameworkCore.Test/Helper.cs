using System.Runtime.CompilerServices;

namespace SlimRepository.EntityFrameworkCore.Test
{
    internal static class Helper
    {
        /// <summary>
        /// Returns the name of the caller method.
        /// </summary>
        /// <param name="caller">The name of the caller method (is injected by the attribute)</param>
        /// <returns>Returns the name of the caller method.</returns>
        public static string GetCallerName([CallerMemberName] string caller = null)
        {
            return caller;
        }
    }
}
