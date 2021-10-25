using System.Net.Http;

namespace WebStore.WebAPI.Clients.Base
{
    public abstract class BaseClient
    {
        protected HttpClient Http { get; }
        protected string Address { get; }

        public BaseClient(HttpClient client, string address)
        {
            Http = client;
            Address = address;
        }
    }
}
