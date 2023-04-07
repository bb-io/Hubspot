using Apps.Hubspot.Http;
using Apps.Hubspot.Models;
using Blackbird.Applications.Sdk.Common.Authentication;

namespace Apps.Hubspot.Actions
{
    public abstract class BaseActions<TEntity, TCreateOrUpdateEntity> : RestRequestProvider 
        where TEntity : class
        where TCreateOrUpdateEntity : class
    {
        protected Dictionary<string, string> RequestWithBodyHeaders = new Dictionary<string, string>
            {
                { "content-type", "application/json" }
            };

        protected BaseActions(HttpRequestProvider httpRequestProvider) : base(httpRequestProvider)
        {
        }

        protected IEnumerable<TEntity> GetAll(
            string url, 
            Dictionary<string, string>? queryParameters, 
            AuthenticationCredentialsProvider authenticationCredentialsProvider
            )
        {
            var results = GetAll<GetAllResponse<TEntity>>(url, queryParameters, RequestWithBodyHeaders, authenticationCredentialsProvider)?.Results;
            return results ?? Enumerable.Empty<TEntity>();
        }

        protected async Task<IEnumerable<TEntity>> GetAllAsync(
            string url,
            Dictionary<string, string>? queryParameters,
            AuthenticationCredentialsProvider authenticationCredentialsProvider
            )
        {
            var response = await GetAllAsync<GetAllResponse<TEntity>>(url, queryParameters, RequestWithBodyHeaders, authenticationCredentialsProvider);
            return response?.Results ?? Enumerable.Empty<TEntity>();
        }

        protected TEntity? GetOne(
            string url, 
            long id, 
            Dictionary<string, string>? queryParameters, 
            AuthenticationCredentialsProvider authenticationCredentialsProvider
            )
        {
            var requestUrl = $"{url}/{id}";
            return GetOne<TEntity>(requestUrl, queryParameters, RequestWithBodyHeaders, authenticationCredentialsProvider);
        }

        protected TEntity? Create(
            string url,             
            Dictionary<string, string>? queryParameters,
            TCreateOrUpdateEntity entity,
            AuthenticationCredentialsProvider authenticationCredentialsProvider)
        {
            return Create<TCreateOrUpdateEntity, TEntity>(url, queryParameters, RequestWithBodyHeaders, entity, authenticationCredentialsProvider);
        }

        protected TEntity? Update(
            string url,
            long id,
            Dictionary<string, string>? queryParameters,
            TCreateOrUpdateEntity entity,
            AuthenticationCredentialsProvider authenticationCredentialsProvider
            )
        {
            var requestUrl = $"{url}/{id}";
            return Patch<TCreateOrUpdateEntity, TEntity>(requestUrl, queryParameters, RequestWithBodyHeaders, entity, authenticationCredentialsProvider);
        }

        protected void Delete(
            string url,
            long id,
            Dictionary<string, string>? queryParameters,
            AuthenticationCredentialsProvider authenticationCredentialsProvider
            )
        {
            var requestUrl = $"{url}/{id}";
            Delete<object>(requestUrl, queryParameters, RequestWithBodyHeaders, authenticationCredentialsProvider);
        }
    }
}
