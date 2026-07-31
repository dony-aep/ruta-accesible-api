namespace Api.Models;

/// <summary>
/// Ciclo de vida de un reporte: lo crea el ciudadano, lo clasifica la IA y
/// la entidad responsable lo verifica y lo atiende.
/// </summary>
public enum EstadoReporte
{
    Registrado,
    Analizado,
    Verificado,
    Atendido
}
