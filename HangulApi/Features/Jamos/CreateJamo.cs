using FluentValidation;
using HangulApi.Data;
using HangulApi.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HangulApi.Features.Jamos;

public static class CreateJamo
{
    public record Command(string Character, JamoType Type, string Pronunciation, string CharacterRomaji,
        string? Name, string? NameRomaji, string? Description)
        : IRequest<Response>;

    public record Response(Guid Id);

    public class CreateJamoValidator : AbstractValidator<Command>
    {
        public CreateJamoValidator(HangulDbContext dbContext)
        {
            RuleFor(j => j.Character)
                .NotEmpty().WithMessage("El caracter es obligatorio.")
                .MustAsync(async (character, cancellationToken) =>
                {
                    return !await dbContext.Jamos.AnyAsync(j => j.Character == character, cancellationToken);
                }).WithMessage("Ya existe un jamo con este caracter.");

            RuleFor(j => j.Type)
                .NotNull().WithMessage("El tipo es obligatorio.");

            RuleFor(x => x.Pronunciation)
                .NotEmpty().WithMessage("La pronunciación es obligatoria.");

            RuleFor(x => x.CharacterRomaji)
                .NotEmpty().WithMessage("El caracter en romaji es obligatorio.");
        }
    }

    public class Handler(HangulDbContext _dbContext
        //, IFileStorageService fileStorageService
        )
        : IRequestHandler<Command, Response>
    {
        public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
        {
            var newJamo = new Jamo
            {
                Character = request.Character,
                Type = request.Type,
                Pronunciation = request.Pronunciation,
                CharacterRomaji = request.CharacterRomaji,
                Name = request.Name,
                NameRomaji = request.NameRomaji,
                Description = request.Description
            };

            _dbContext.Jamos.Add(newJamo);

            try
            {
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
            {
                throw new ValidationException(
                [
                    new FluentValidation.Results.ValidationFailure("Character", "Ya existe un jamo con este carácter.")
                ]);
            }

            //await fileStorageService
            //    .UploadJamoMetadataAsync(newJamo.Id, newJamo, cancellationToken);
            return new Response(newJamo.Id);
        }
    }

    private static bool IsUniqueConstraintViolation(DbUpdateException ex)
    {
        return ex.InnerException?.Message.Contains("UNIQUE", StringComparison.OrdinalIgnoreCase) ?? false;
    }
}
