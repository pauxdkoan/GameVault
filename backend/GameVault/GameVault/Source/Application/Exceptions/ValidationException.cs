namespace GameVault.Source.Application.Exceptions
{
    public sealed class ValidationException: ApiException
    {
        public ValidationException(Dictionary<string, string[]> errors)
            :base(
                 "Uno o más errores de validación ocurrieron.",
                  StatusCodes.Status400BadRequest,
                  errors
                 )
        { }
    }
}
