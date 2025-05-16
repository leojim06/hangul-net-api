using FluentValidation;
using HangulApi.Data;
using HangulApi.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HangulApi.Features.Audios;

public static class UploadAudio
{
    public record Command(Guid JamoId, IFormFile File, VoiceType VoiceType, Guid? CombinedWith)
        : IRequest<Response>;

    public record Response(Guid Id, string Url);

    public class UploadAudioValidator : AbstractValidator<Command>
    {
        private static readonly string[] AllowedExtensions = [".mp3", ".wav", ".ogg"];
        private const long MaxFileSize = 5 * 1024 * 1024; // 5 MB

        public UploadAudioValidator(HangulDbContext dbContext)
        {
            RuleFor(x => x.JamoId)
                .NotEmpty().WithMessage("El ID del jamo es obligatorio.")
                .MustAsync(async (id, _) => await dbContext.Jamos.AnyAsync(j => j.Id == id))
                .WithMessage("El jamo no existe.");

            RuleFor(x => x.VoiceType)
                .IsInEnum().WithMessage("El tipo de voz no es válido.");

            RuleFor(x => x.CombinedWith)
                .MustAsync(async (id, _) => id == null || await dbContext.Jamos.AnyAsync(j => j.Id == id))
                .WithMessage("El jamo con el cual se va a combinar no existe.");

            RuleFor(x => x.File)
                .NotNull()
                .WithMessage("Se debe proporcionar un archivo de audio.")
                .Must(BeAValidExtension)
                .WithMessage("Solo se permiten archivos de tipo .mp3, .wav o .ogg.")
                .Must(f => f.Length <= MaxFileSize)
                .WithMessage("El tamaño máximo permitido es de 5 MB.");
        }

        private static bool BeAValidExtension(IFormFile file)
        {
            var extension = Path.GetExtension(file.FileName);
            return AllowedExtensions.Contains(extension.ToLower());
        }
    }

    public class Handler(HangulDbContext dbContext
        //, IFileStorageService fileStorageService
        )
        : IRequestHandler<Command, Response>
    {
        public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
        {
            //var url = await fileStorageService.UploadAudioAsync(request.JamoId, request.File, cancellationToken);

            var audio = new Audio
            {
                Url = "",
                JamoId = request.JamoId,
                VoiceType = request.VoiceType,
                CombinedWith = request.CombinedWith,
            };

            dbContext.Audios.Add(audio);
            await dbContext.SaveChangesAsync(cancellationToken);

            return new Response(audio.Id, "");
        }
    }
}
