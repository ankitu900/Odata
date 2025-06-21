using Microsoft.Extensions.Configuration;
using Microsoft.OData.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Default;

namespace Business.Repositories
{
    public  class ODataContainer : IUnitOfWork
    {
        private readonly Uri _serviceUrl;
        private readonly DataServiceContext _container;
        //private readonly ITokenProvider tokenProvider;
        //private readonly IIdentityContext identityContext;
        private Hashtable _repositories;

        public DataServiceContext Container => _container;

        public ODataContainer(IConfiguration configuration/* ITokenProvider tokenProvider, IIdentityContext identityContext*/)
        {
            //this.tokenProvider = tokenProvider;
            //this.identityContext = identityContext;
            string appEntityUrl = configuration.GetSection("OData")["ApiUrl"];
            _serviceUrl = new Uri(appEntityUrl);
            _container = new Container(_serviceUrl);
            //_container.BuildingRequest += (sender, e) => OnBuildingRequest(sender, e);
            //_container.ReceivingResponse += (receiver, r) => LogResponse(receiver, r);
            _container.SendingRequest2 += _container_SendingRequest2;
            _container.IgnoreResourceNotFoundException = true;
        }

        private void _container_SendingRequest2(object sender, SendingRequest2EventArgs e)
        {
        }

        public IRepository<T> Repository<T>() where T : class
        {
            if (_repositories == null)
                _repositories = new Hashtable();

            var type = typeof(T).Name;

            if (!_repositories.ContainsKey(type))
            {
                var repositoryType = typeof(ODataRepository<>);
                var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(T)), _container);
                _repositories.Add(type, repositoryInstance);
            }

            return (IRepository<T>)_repositories[type];
        }

        public void SaveChanges()
        {
            var response = _container.SaveChanges();
            ExecuteResponse(response);
        }

        public async Task<int> SaveChangesAsync()
        {
            var response = await _container.SaveChangesAsync();
            ExecuteResponse(response);
            return 1;
        }

        public async Task<int> SaveChangesInBatchAsync()
        {
            var response = await _container.SaveChangesAsync(SaveChangesOptions.BatchWithSingleChangeset);
            ExecuteResponse(response);
            return 1;
        }

        private void ExecuteResponse(DataServiceResponse response)
        {
            var error = response.Where(a => a.Error != null).Select(a => a.Error).FirstOrDefault();
            if (error != null)
                throw error;
        }

        //private void OnBuildingRequest(object sender, BuildingRequestEventArgs e)
        //{
        //    string accessToken = tokenProvider.GetToken();
        //    e.Headers.Add("Authorization", "Bearer " + accessToken);
        //    e.Headers.Add("X-Forwarded-For", identityContext.RequestIP);
        //}

        private void LogResponse(object receiver, ReceivingResponseEventArgs r)
        {
            if (r != null && r.ResponseMessage != null && r.ResponseMessage.StatusCode == (int)System.Net.HttpStatusCode.Unauthorized)
            {
                throw new UnauthorizedAccessException("Unauthorized");
            }
        }

        //public void SetTokenProvider(TokenProviderType provider)
        //{
        //    this.tokenProvider.SetTokenProvider(provider);
        //}

        public void Dispose()
        {
            if (_repositories != null)
                _repositories = null;
        }
    }
}
