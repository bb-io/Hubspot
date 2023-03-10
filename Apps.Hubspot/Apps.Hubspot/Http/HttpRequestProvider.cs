using Blackbird.Applications.Sdk.Common.Authentication;
using RestSharp;

namespace Apps.Hubspot.Http
{
    public class HttpRequestProvider
    {
        public TResponse? Get<TResponse>(
            string url,
            Dictionary<string, string>? queryParameters,
            Dictionary<string, string> headers,
            AuthenticationCredentialsProvider authenticationCredentialsProvider
            )
        {
            using var client = new RestClient(url);
            var request = CreateRequest(queryParameters, headers, authenticationCredentialsProvider);
            return client.Get<TResponse>(request);
        }

        public TResponse? Post<TRequest, TResponse>(
            string url,
            Dictionary<string, string>? queryParameters,
            Dictionary<string, string> headers,
            TRequest body,
            AuthenticationCredentialsProvider authenticationCredentialsProvider)
            where TRequest : class
        {
            var client = new RestClient(url);
            var request = CreateRequest(queryParameters, headers, authenticationCredentialsProvider);
            request.AddBody(body, "application/json");
            return client.Post<TResponse>(request);
        }

        public TResponse? Patch<TRequest, TResponse>(
            string url,
            Dictionary<string, string>? queryParameters,
            Dictionary<string, string> headers,
            TRequest body,
            AuthenticationCredentialsProvider authenticationCredentialsProvider)
            where TRequest : class
        {
            var client = new RestClient(url);
            var request = CreateRequest(queryParameters, headers, authenticationCredentialsProvider);
            request.AddBody(body, "application/json");
            return client.Patch<TResponse>(request);
        }

        public TResponse? Delete<TResponse>(
            string url,
            Dictionary<string, string>? queryParameters,
            Dictionary<string, string> headers,
            AuthenticationCredentialsProvider authenticationCredentialsProvider)
        {
            var client = new RestClient(url);
            var request = CreateRequest(queryParameters, headers, authenticationCredentialsProvider);
            return client.Delete<TResponse>(request);
        }

        private RestRequest CreateRequest(
            Dictionary<string, string>? queryParameters,
            Dictionary<string, string> headers,
            AuthenticationCredentialsProvider authenticationCredentialsProvider
            )
        {
            var request = new RestRequest();
            HandleAuthentication(request, authenticationCredentialsProvider);
            AddParameters(request, queryParameters);
            AddHeaders(request, headers);
            return request;
        }

        private void AddParameters(RestRequest request, Dictionary<string, string>? queryParameters)
        {
            if (queryParameters == null || !queryParameters.Any())
            {
                return;
            }
            foreach (var queryParameter in queryParameters)
            {
                request.AddParameter(queryParameter.Key, queryParameter.Value);
            }
        }

        private void AddHeaders(RestRequest request, Dictionary<string, string> headers)
        {
            if (headers == null || !headers.Any())
            {
                return;
            }
            foreach (var header in headers)
            {
                request.AddHeader(header.Key, header.Value);
            }
        }

        private void HandleAuthentication(RestRequest request, AuthenticationCredentialsProvider authenticationCredentialsProvider)
        {
            switch (authenticationCredentialsProvider.CredentialsRequestLocation)
            {
                case AuthenticationCredentialsRequestLocation.None:
                    return;
                case AuthenticationCredentialsRequestLocation.QueryString:
                    request.AddQueryParameter(authenticationCredentialsProvider.KeyName, authenticationCredentialsProvider.Value);
                    break;
                case AuthenticationCredentialsRequestLocation.Header:
                    request.AddHeader(authenticationCredentialsProvider.KeyName, authenticationCredentialsProvider.Value);
                    break;
                case AuthenticationCredentialsRequestLocation.Body:
                default:
                    throw new NotImplementedException();
            }
        }

        private void AddRequestBody(RestRequest request, object body)
        {
            request.AddBody(body, "application/json");
        }
    }
}