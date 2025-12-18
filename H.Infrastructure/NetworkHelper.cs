using System;
using System.Diagnostics;
using System.Net;

namespace H.Infrastructure
{
    public class NetworkHelper
    {
        public static bool IsConnectedToInternet()
        {
            try
            {
                using (var client = new WebClient())
                using (client.OpenRead("https://google.com/generate_204"))
                {
                    Trace.TraceInformation(
                        $"{nameof(NetworkHelper)}.{nameof(IsConnectedToInternet)} : Successfully connected to the internet");
                    return true;
                }
            }
            catch (Exception e)
            {
                Trace.TraceError("Exception thrown.");
                Trace.TraceError(
                    $"{nameof(NetworkHelper)}.{nameof(IsConnectedToInternet)} : Could not connect to the internet.");
                Trace.TraceError($"Inner Exception message: {e.InnerException}");
                return false;
            }
        }
    }
}