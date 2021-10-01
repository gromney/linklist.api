using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinkList.api.Configurations;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace LinkList.api
{
    public abstract class MongoRepository
    {
        private readonly IMongoClient client;
        // protected readonly IMongoCollection collection;
        protected  IMongoDatabase database;

        public MongoRepository(IMongoClient client, IOptions<MongoDbOptions> mongoOptions)
        {
            this.client = client;
            database = client.GetDatabase(mongoOptions.Value.DatabaseName);
        }
    }
    public interface IRepository<T> where T : class
    {
        Task Insert(T entity);
        Task<IEnumerable<T>> GetlAll();
        IQueryable<T> Find();
    }
}