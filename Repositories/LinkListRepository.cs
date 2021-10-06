using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using link_list.api.Contracts.Responses;
using LinkList.api;
using LinkList.api.Configurations;
using LinkList.api.Contracts;
using LinkList.api.Domain;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace LinkList.api.Repositories
{
    public class LinkListRepository : MongoRepository, ILinkListRepository
    {
        private readonly IMongoCollection<LinkListModel> collection;

        public LinkListRepository(IMongoClient client, IOptions<MongoDbOptions> mongoOptions) : base(client, mongoOptions)
        {
            collection = database.GetCollection<LinkListModel>(mongoOptions.Value.LinkListCollection);
        }

        public async Task<Response<List<LinkListModel>>> GetlAll(string userId)
        {
            var filter = Builders<LinkListModel>.Filter.Eq(x => x.UserId,userId);
            var result = await collection.FindAsync(filter).Result.ToListAsync();

            var response = new Response<List<LinkListModel>>(result);
            return response;
        }

        public async Task<Response<LinkListModel>> GetCollection(string title)
        {
            var filter = Builders<LinkListModel>.Filter.Eq(x => x.Title, title);
            var result = await collection.FindAsync(filter).Result.FirstOrDefaultAsync();

            if (result == null)
            {
                var errors = new List<string> { "Link List not exist" };
                return new Response<LinkListModel>(errors);
            }
            var response = new Response<LinkListModel>(result);
            return response;
        }

        public async Task<Response<bool>> Available(string title)
        {
            var filter = Builders<LinkListModel>.Filter.Eq(x => x.Title, title);
            var list = await collection.FindAsync(filter).Result.FirstOrDefaultAsync();

            if (list != null)
            {
                var error = new List<string> { "Url not Available" };
                return new Response<bool>(error);
            }

            return new Response<bool>(true);
        }

        public async Task<Response<LinkListModel>> Publish(CreateLinkListRequest request)
        {
            var available = await Available(request.Title);

            if (!available.Success)
            {
                return new Response<LinkListModel>(available.Errors);
            }


            var newList = new LinkListModel
            {
                UserId = request.UserId,
                Title = request.Title,
                Description = request.Description,
                Links = request.Links
            };
            try
            {

                await collection.InsertOneAsync(newList);
            }
            catch (System.Exception ex)
            {
                var errors = new List<string> {
                    "Error insertando en MongoDB",
                    ex.Message
                };
                return new Response<LinkListModel>(errors);
            }

            return new Response<LinkListModel>(newList);
        }

        public async Task<Response<LinkListModel>> Update(string title, UpdateLinkListRequest request)
        {

            var filter = Builders<LinkListModel>.Filter.Eq(x => x.Title, title);
            var update = Builders<LinkListModel>.Update
                .Set(rec => rec.Description, request.Description)
                .Set(rec => rec.Links, request.Links);

            var listToUpdate = await GetCollection(title);
            if (!listToUpdate.Success)
            {
                var errors = new List<string> { "Link List not found" };
                return new Response<LinkListModel>(errors);
            }

            var result = await collection.UpdateOneAsync(filter, update);
            if (!result.IsAcknowledged)
            {
                var errors = new List<string> { "Link List cant be updated" };
                return new Response<LinkListModel>(errors);
            }

            var modified = await GetCollection(title);
            return new Response<LinkListModel>(modified.Data);
        }
    }
}

public interface ILinkListRepository
{
    Task<Response<List<LinkListModel>>> GetlAll(string userId);
    Task<Response<LinkListModel>> GetCollection(string title);
    Task<Response<bool>> Available(string title);
    Task<Response<LinkListModel>> Publish(CreateLinkListRequest request);
    Task<Response<LinkListModel>> Update(string title, UpdateLinkListRequest entity);
}