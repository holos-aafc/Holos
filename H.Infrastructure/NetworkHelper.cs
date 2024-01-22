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
                    return true;
            }
            catch
            {
                return false;
            }
        }
    }
}