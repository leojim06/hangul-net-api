using FluentValidation;
using HangulApi.Data;
using HangulApi.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HangulApi.Features.Jamos;

public static class UpdateJamo
{
    public record Command(Guid Id, string Character, JamoType Type, string Pronunciation, string CharacterRomaji,
        string? Name, string? NameRomaji, string? Description)
        : IRequest<Guid?>;

    public class UpdateJamoValidator : AbstractValidator<Command>
    {
        public UpdateJamoValidator(HangulDbContext dbContext)
        {
            RuleFor(j => j.Id)
                .NotEmpty().WithMessage("El ID es obligatorio.");

            RuleFor(j => j.Character)
                .NotEmpty().WithMessage("El caracter es obligatorio.")
                .MustAsync(async (command, character, cancellationToken) =>
                {
                    return !await dbContext.Jamos
                        .AnyAsync(j => j.Character == character && j.Id != command.Id, cancellationToken);
                }).WithMessage("Ya existe un jamo con este caracter.");

            RuleFor(j => j.Type)
                .NotNull().WithMessage("El tipo es obligatorio.");

            RuleFor(x => x.Pronunciation)
                .NotEmpty().WithMessage("La pronunciación es obligatoria.");

            RuleFor(x => x.CharacterRomaji)
                .NotEmpty().WithMessage("El caracter en romaji es obligatorio.");
        }
    }

    public class Handler(HangulDbContext dbContext) : IRequestHandler<Command, Guid?>
    {
        public async Task<Guid?> Handle(Command request, CancellationToken cancellationToken)
        {
            var jamo = await dbContext.Jamos.FirstOrDefaultAsync(jamo => jamo.Id == request.Id, cancellationToken);
            if (jamo == null) return null;

            jamo.Character = request.Character;
            jamo.Type = request.Type;
            jamo.Pronunciation = request.Pronunciation;
            jamo.CharacterRomaji = request.CharacterRomaji;
            jamo.Name = request.Name ?? request.Name;
            jamo.NameRomaji = request.NameRomaji ?? request.NameRomaji;
            jamo.Description = request.Description ?? request.Description;

            dbContext.Update(jamo);
            await dbContext.SaveChangesAsync(cancellationToken);

            return jamo.Id;
        }
    }


}
