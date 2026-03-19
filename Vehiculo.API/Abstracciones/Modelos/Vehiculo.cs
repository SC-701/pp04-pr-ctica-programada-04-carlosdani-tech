using System.ComponentModel.DataAnnotations;

namespace Abstracciones.Modelos
{
    public class VehiculoBase
    {
        [Required(ErrorMessage = "La propiedad placa es requerida")]
        [RegularExpression(@"[A-Za-z]{3}-[0-9]{3}", ErrorMessage = "El formato de la placa debe ser ABC-###")]
        public string Placa { get; set; }
        [Required(ErrorMessage = "El color del vehiculo es requerido")]
        [StringLength(40, ErrorMessage = "El color del vehiculo no puede exceder los 40 caracteres y menos de 4 caracteres", MinimumLength = 4)]
        public string Color { get; set; }
        [Required(ErrorMessage = "El año del vehiculo es requerido")]
        [RegularExpression(@"^(19|20)\d\d", ErrorMessage = "El año del vehiculo tiene un formato no valido")]
        public int Anio { get; set; }
        [Required(ErrorMessage = "El precio del vehiculo es requerido")]
        public Decimal Precio { get; set; }
        [Required(ErrorMessage = "El correo del propietario es requerido")]
        [EmailAddress]
        public string CorreoPropietario { get; set; }
        [Required(ErrorMessage = "El telefono del propietario es requerido")]
        [Phone]
        public string TelefonoPropietario { get; set; }
    }

    public class VehiculoRequest:VehiculoBase
    {
        public Guid IdModelo { get; set; }
    }
    public class VehiculoResponse:VehiculoBase
    {
        public Guid Id { get; set; }
        public string Modelo { get; set; }
        public string Marca { get; set; }
    }

    public class VehiculoDetalle: VehiculoResponse
    {
        public bool RevisionValida { get; set; }
        public bool RegistroValido { get; set; }
    }
}
