using HangulApi.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HangulApi.Features.Jamos;

public static class GetAllJamos
{
    public record Query : IRequest<IEnumerable<Response>>;

    public record Response(Guid Id, string Character, string Type, string Pronunciation, string CharacterRomaji,
        string? Name, string? NameRomaji, string? Description, string? ImageUrl);

    public class Handler(HangulDbContext _dbContext) : IRequestHandler<Query, IEnumerable<Response>>
    {
        public async Task<IEnumerable<Response>> Handle(Query request, CancellationToken cancellationToken)
        {
            var jamos = await _dbContext.Jamos.ToListAsync(cancellationToken);
            return jamos.Select(jamo => new Response(jamo.Id, jamo.Character, jamo.Type.DisplayName, jamo.Pronunciation, jamo.CharacterRomaji,
                 jamo.Name, jamo.NameRomaji, jamo.Description, jamo.ImageUrl));
        }
    }
}
