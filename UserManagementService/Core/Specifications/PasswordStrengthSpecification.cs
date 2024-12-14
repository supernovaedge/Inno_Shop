namespace UserManagementService.Core.Specifications
{
    public class PasswordStrengthSpecification
    {
        public bool IsSatisfiedBy(string password)
        {
           
            return password.Length >= 8 &&
                   password.Any(char.IsUpper) &&
                   password.Any(char.IsLower) &&
                   password.Any(char.IsDigit);
        }
    }
}