using System;
using System.Collections.Generic;

namespace ejemploRecuperacion.Models;

public partial class Usuario
{
    public long Id { get; set; }

    public string Email { get; set; } = null!;

    public DateTime? FechaRegistro { get; set; }

    public string Contraseña { get; set; } = null!;

    public string? TokenRecuperacion { get; set; }
}
