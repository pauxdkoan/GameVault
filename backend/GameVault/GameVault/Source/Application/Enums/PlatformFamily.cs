namespace GameVault.Source.Application.Enums
{
    /// <summary>
    /// Familia a la que pertenece una plataforma. Agrupa generaciones
    /// (PS4, PS5...) para pintar un solo icono por familia en el frontend.
    /// Se envía como texto en el JSON ("PlayStation", "Xbox"...).
    /// </summary>
    public enum PlatformFamily
    {
        PC,
        PlayStation,
        Xbox,
        Nintendo,
        Mobile,
        VR,
        Other
    }
}
