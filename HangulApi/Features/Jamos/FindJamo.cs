using HangulApi.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HangulApi.Features.Jamos;

public static class FindJamo
{
    public record Query(Guid Id) : IRequest<Response?>;

    public record Response(Guid Id, string Character, string Type, string Pronunciation, string CharacterRomaji,
         string? Name, string? NameRomaji, string? Description, string? ImageUrl);

    public class Handler(HangulDbContext _dbContext) : IRequestHandler<Query, Response?>
    {
        public async Task<Response?> Handle(Query request, CancellationToken cancellationToken)
        {
            var jamo = await _dbContext.Jamos.FirstOrDefaultAsync(jamo => jamo.Id == request.Id, cancellationToken);
            if (jamo == null) return null;

            return new Response(jamo.Id, jamo.Character, jamo.Type.DisplayName, jamo.Pronunciation, jamo.CharacterRomaji,
                 jamo.Name, jamo.NameRomaji, jamo.Description, jamo.ImageUrl);
        }
    }
}
