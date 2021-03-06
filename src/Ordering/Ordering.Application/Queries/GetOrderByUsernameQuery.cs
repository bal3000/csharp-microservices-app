using System.Collections.Generic;
using MediatR;
using Ordering.Application.Responses;

namespace Ordering.Application.Queries
{
    public class GetOrderByUsernameQuery : IRequest<IEnumerable<OrderResponse>>
    {
        public string Username { get; set; }

        public GetOrderByUsernameQuery(string username)
        {
            Username = username;
        }
    }
}