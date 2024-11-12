using System.ComponentModel.DataAnnotations;
using User_Service.Src.Common.Constants;

namespace User_Service.Src.DataAnnotations
{
    // https://gist.github.com/donpandix/045f865c3bf800893036
    public class RutAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            var rut = value?.ToString() ?? string.Empty;
            if (rut.Length == 11 || rut.Length == 12)
            {
                return CheckRut(rut);
            }
            return false;
        }

        public static bool CheckRut(string rut)
        {
            rut = rut.Replace(".", "").ToUpper();
            if (!RegularExpressions.RutRegex().IsMatch(rut))
                return false;

            string dv = rut.Substring(rut.Length - 1, 1);
            char[] dash = { '-' };
            string[] splittedRut = rut.Split(dash);
            if (dv != CalculateDigit(int.Parse(splittedRut[0])))
                return false;
            return true;
        }

        public static string CalculateDigit(int rut)
        {
            int sum = 0;
            int multiplier = 1;
            while (rut != 0)
            {
                multiplier++;
                if (multiplier == 8)
                    multiplier = 2;
                sum += rut % 10 * multiplier;
                rut /= 10;
            }
            sum = 11 - (sum % 11);

            return sum switch
            {
                11 => "0",
                10 => "K",
                _ => sum.ToString(),
            };
        }
    }
}