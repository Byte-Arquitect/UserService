namespace User_Service.Src.Common.Constants
{
    public static class ErrorMessages
    {
        public const string EntityNotFound = "No se encontro el usuario o el estudiante";

        public const string InvalidCredentials = "Credenciales de acceso invalidas";

        public const string EntityNotDeleted = "La entidad no puso ser eliminado o borrada";

        public const string InternalServerError = "Sucedio un error interno del servidor - Por favor comuniquese con el administrador";

        public const string DuplicateUser = "Usuario duplicado";

        public const string DisabledUser = "El usuario esta deshabilitado - Por favor comuniquese con el administrador";

        public const string UnauthorizedAccess = "Acceso no autorizado";
        
        public const string EntityDuplicated = "Entidad duplicada";
    }
}