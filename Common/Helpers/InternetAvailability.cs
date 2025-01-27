using Common.Exceptions;
using System.Runtime.InteropServices;

namespace Common.Helpers;
public partial class InternetAvailability
{
    [LibraryImport("wininet.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool InternetGetConnectedState(out int description, int reservedValue);

    public static bool IsInternetAvailable()
    {
        return InternetGetConnectedState(out int _, 0);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="exceptionMessage"></param>
    /// <exception cref="NetworkException"></exception>
    public static void ThrowIfNoInternetConnection(string exceptionMessage)
    {
        if (!IsInternetAvailable())
            throw new NetworkException(exceptionMessage);
    }
}