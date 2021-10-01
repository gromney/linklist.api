using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace LinkList.api.Domain
{
    public class LinkListModel
    {
        public LinkListModel()
        {
            Links = new List<Link>();
        }
        [BsonId(IdGenerator = typeof(ObjectIdGenerator))]
        public ObjectId Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<Link> Links { get; set; }
    }
}