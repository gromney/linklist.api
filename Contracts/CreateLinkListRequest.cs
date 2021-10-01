using System.Collections.Generic;
using LinkList.api.Domain;
using MongoDB.Bson;

namespace LinkList.api.Contracts
{
    public class CreateLinkListRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public List<Link> Links { get; set; }
    }
}