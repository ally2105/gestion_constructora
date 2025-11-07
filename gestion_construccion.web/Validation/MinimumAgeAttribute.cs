using System.ComponentModel.DataAnnotations;
using System;

namespace gestion_construccion.web.Validation
{
    // Atributo de validación personalizado para verificar una edad mínima.
    public class MinimumAgeAttribute : ValidationAttribute
    {
        private readonly int _minimumAge;

        // El constructor recibe la edad mínima requerida.
        public MinimumAgeAttribute(int minimumAge)
        {
            _minimumAge = minimumAge;
            // Se puede establecer un mensaje de error por defecto.
            ErrorMessage = $"El usuario debe tener al menos {_minimumAge} años.";
        }

        // El método IsValid es donde se ejecuta la lógica de validación.
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            // Si el valor no es una fecha, no se puede validar aquí (otras validaciones como [DataType] se encargarán).
            if (value is DateTime dateOfBirth)
            {
                // Se calcula la edad.
                var today = DateTime.Today;
                var age = today.Year - dateOfBirth.Year;

                // Si el cumpleaños de este año aún no ha pasado, se resta un año a la edad.
                if (dateOfBirth.Date > today.AddYears(-age))
                {
                    age--;
                }

                // Si la edad calculada es menor que la edad mínima requerida, la validación falla.
                if (age < _minimumAge)
                {
                    // Se devuelve un resultado de validación fallido con el mensaje de error.
                    return new ValidationResult(ErrorMessage);
                }
            }

            // Si la validación es exitosa, se devuelve Success.
            return ValidationResult.Success;
        }
    }
}
