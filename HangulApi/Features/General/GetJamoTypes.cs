using HangulApi.Entities;
using MediatR;

namespace HangulApi.Features.General;

public static class GetJamoTypes
{
    public record Query : IRequest<IEnumerable<Response>>;

    public record Response(string Name, string Displayname);

    public class Handler : IRequestHandler<Query, IEnumerable<Response>>
    {
        public Task<IEnumerable<Response>> Handle(Query request, CancellationToken cancellationToken)
        {
            var types = JamoType.List.Select(jt => new Response(jt.Name, jt.DisplayName));
            return Task.FromResult(types);
        }
    }
}
