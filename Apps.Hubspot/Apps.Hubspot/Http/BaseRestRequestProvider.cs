using Blackbird.Applications.Sdk.Common.Authentication;

namespace Apps.Hubspot.Http
{
    public abstract class RestRequestProvider
    {
        private readonly HttpRequestProvider _httpRequestProvider;

        protected RestRequestProvider(HttpRequestProvider httpRequestProvider)
        {
            _httpRequestProvider = httpRequestProvider;
        }

        protected TResponse? GetAll<TResponse>(
            string url, 
            Dictionary<string, string>? queryParameters,
            Dictionary<string, string> headers,
            AuthenticationCredentialsProvider authenticationCredentialsProvider
            )
            where TResponse : class
        {
            return _httpRequestProvider.Get<TResponse>(url, queryParameters, headers, authenticationCredentialsProvider);
        }

        protected async Task<TResponse?> GetAllAsync<TResponse>(
            string url,
            Dictionary<string, string>? queryParameters,
            Dictionary<string, string> headers,
            AuthenticationCredentialsProvider authenticationCredentialsProvider
            )
            where TResponse : class
        {
            return await _httpRequestProvider.GetAsync<TResponse>(url, queryParameters, headers, authenticationCredentialsProvider);
        }

        public TEntity? GetOne<TEntity>(
            string url,
            Dictionary<string, string>? queryParameters,
            Dictionary<string, string> headers,
            AuthenticationCredentialsProvider authenticationCredentialsProvider
            )
            where TEntity : class
        {
            return _httpRequestProvider.Get<TEntity>(url, queryParameters, headers, authenticationCredentialsProvider);
        }

        public TResponse? Create<TEntity, TResponse>(
            string url,
            Dictionary<string, string>? queryParameters,
            Dictionary<string, string> headers,
            TEntity entity,
            AuthenticationCredentialsProvider authenticationCredentialsProvider)
            where TEntity : class
        {
            return _httpRequestProvider.Post<TEntity, TResponse>(url, queryParameters, headers, entity, authenticationCredentialsProvider);
        }

        public Task<TResponse?> CreateAsync<TEntity, TResponse>(
            string url,
            Dictionary<string, string>? queryParameters,
            Dictionary<string, string> headers,
            TEntity entity,
            AuthenticationCredentialsProvider authenticationCredentialsProvider)
            where TEntity : class
        {
            return _httpRequestProvider.PostAsync<TEntity, TResponse>(url, queryParameters, headers, entity, authenticationCredentialsProvider);
        }

        public TResponse? Patch<TEntity, TResponse>(
            string url,
            Dictionary<string, string>? queryParameters,
            Dictionary<string, string> headers,
            TEntity entity,
            AuthenticationCredentialsProvider authenticationCredentialsProvider)
            where TEntity : class
        {
            {
                return _httpRequestProvider.Patch<TEntity, TResponse>(url, queryParameters, headers, entity, authenticationCredentialsProvider);
            }
        }

        public TResponse? Put<TEntity, TResponse>(
            string url,
            Dictionary<string, string>? queryParameters,
            Dictionary<string, string> headers,
            TEntity entity,
            AuthenticationCredentialsProvider authenticationCredentialsProvider)
            where TEntity : class
        {
            {
                return _httpRequestProvider.Put<TEntity, TResponse>(url, queryParameters, headers, entity, authenticationCredentialsProvider);
            }
        }

        public Task<TResponse?> PutAsync<TEntity, TResponse>(
            string url,
            Dictionary<string, string>? queryParameters,
            Dictionary<string, string> headers,
            TEntity entity,
            AuthenticationCredentialsProvider authenticationCredentialsProvider)
            where TEntity : class
        {
            {
                return _httpRequestProvider.PutAsync<TEntity, TResponse>(url, queryParameters, headers, entity, authenticationCredentialsProvider);
            }
        }

        public TResponse? Delete<TResponse>(
            string url,
            Dictionary<string, string>? queryParameters,
            Dictionary<string, string> headers,
            AuthenticationCredentialsProvider authenticationCredentialsProvider
            )
        {
            return _httpRequestProvider.Delete<TResponse>(url, queryParameters, headers, authenticationCredentialsProvider);
        }

        public async Task<TResponse?> DeleteAsync<TResponse>(
            string url,
            Dictionary<string, string>? queryParameters,
            Dictionary<string, string> headers,
            AuthenticationCredentialsProvider authenticationCredentialsProvider
            )
        {
            return await _httpRequestProvider.DeleteAsync<TResponse>(url, queryParameters, headers, authenticationCredentialsProvider);
        }
    }
}
