namespace GameVault.Source.Domain.Contants
{
    public class Roles
    {
        public const string Administrator = "Administrator";
        public const string Technician = "Technician";
        public const string Employee = "Employee";

        public static readonly string[] All =
        [
            Administrator,
            Technician,
            Employee
        ];
    }
}
