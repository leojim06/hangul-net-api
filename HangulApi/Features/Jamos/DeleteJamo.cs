using HangulApi.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HangulApi.Features.Jamos;

public static class DeleteJamo
{
    public record Command(Guid Id) : IRequest<bool?>;
    public class Handler(HangulDbContext _dbContext) : IRequestHandler<Command, bool?>
    {
        public async Task<bool?> Handle(Command request, CancellationToken cancellationToken)
        {
            var jamo = await _dbContext.Jamos.FirstOrDefaultAsync(jamo => jamo.Id == request.Id, cancellationToken);
            if (jamo == null) return null;

            _dbContext.Jamos.Remove(jamo);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
