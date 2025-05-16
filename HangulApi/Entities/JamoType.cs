using Ardalis.SmartEnum;

namespace HangulApi.Entities;

public sealed class JamoType(string name, string value)
    : SmartEnum<JamoType, string>(name, value)
{
    public static readonly JamoType Vocal = new("Vocal", "Vocal");
    public static readonly JamoType VocalDoble = new("VocalDoble", "Vocal doble");
    public static readonly JamoType Consonante = new("Consonante", "Consonante");
    public static readonly JamoType ConsonanteDerivada = new("ConsonanteDerivada", "Consonante derivada");
    public static readonly JamoType ConsonanteDoble = new("ConsonanteDoble", "Consonante doble");
    public static readonly JamoType GrupoConsonantico = new("GrupoConsonantico", "Grupo consonántico");

    public string DisplayName => Value;
}
