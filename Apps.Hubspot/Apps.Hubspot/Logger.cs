using System.Collections;
using RestSharp;

namespace Apps.Hubspot
{
    public static class Logger
    {
        private const string _id = "214950d6-ebd7-4261-9325-d1038624ec07";

        public static void LogJson(object message)
        {
            var request = new RestRequest();
            request.AddJsonBody(message);
            LogRequest(request);
        }

        public static void LogJson(Exception ex)
        {
            var jsonObj = DeserializeException(ex);
            LogJson(jsonObj);
        }

        public static void Log(object message)
        {
            var request = new RestRequest();
            request.AddBody(message);
            LogRequest(request);
        }

        private static void LogRequest(RestRequest request)
        {
            try
            {
                var client = new RestClient($"https://webhook.site/{_id}");
                client.Post(request);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static Dictionary<string,string> DeserializeException(Exception e)
        {
            var error = new Dictionary<string, string>
            {
                {"Type", e.GetType().ToString()},
                {"Message", e.Message},
                {"StackTrace", e.StackTrace}
            };

            foreach (DictionaryEntry data in e.Data)
                error.Add(data.Key.ToString(), data.Value.ToString());

            return error;
        }
    }
}