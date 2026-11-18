namespace GameVault.Source.Application.Exceptions
{
    /// <summary>Atajos para los errores que más se repiten en las features de usuario.</summary>
    public static class ApiErrors
    {
        public static ApiException NotFound(string message) =>
            new(message, StatusCodes.Status404NotFound);

        /// <summary>Existe, pero no es tuyo (editar la reseña o la lista de otro).</summary>
        public static ApiException Forbidden(string message) =>
            new(message, StatusCodes.Status403Forbidden);

        /// <summary>Choca con algo que ya existe (segunda reseña del mismo juego...).</summary>
        public static ApiException Conflict(string message) =>
            new(message, StatusCodes.Status409Conflict);

        public static ApiException BadRequest(string message) =>
            new(message, StatusCodes.Status400BadRequest);
    }
}
