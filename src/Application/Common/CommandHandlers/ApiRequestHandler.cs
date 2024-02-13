using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Application.Common.CommandHandlers
{
    public interface IApiRequest<TResponse> : IRequest<ActionResult<TResponse>>
    {
    }

    public interface IApiRequestHandler<in TRequest, TResponse> : IRequestHandler<TRequest, ActionResult<TResponse>>
        where TRequest : IApiRequest<TResponse>
    {
    }

    public interface IApiRequest : IRequest<ActionResult>
    {
    }

    public interface IApiRequestHandler<in TRequest> : IRequestHandler<TRequest, ActionResult>
        where TRequest : IApiRequest
    {
    }
}
