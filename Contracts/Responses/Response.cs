using System.Collections.Generic;

namespace link_list.api.Contracts.Responses
{
    public class Response<T>
    {


        public Response(T data)
        {
            this.Data = data;
            this.Success = true;
        }
        public Response(List<string> errors)
        {
            this.Errors = errors;
            this.Success = false;
        }
        public T Data { get; }
        public bool Success { get; }
        public List<string> Errors { get; }
    }
}