using Ardalis.SmartEnum;

namespace HangulApi.Entities;

public class VoiceType : SmartEnum<VoiceType>
{
    public static readonly VoiceType Male = new(nameof(Male), 1);
    public static readonly VoiceType Female = new(nameof(Female), 2);
    public static readonly VoiceType Combined = new(nameof(Combined), 3);

    private VoiceType(string name, int value) : base(name, value) { }
}
