using System.ComponentModel;

namespace Orquestra.Domain.Enums;

public enum CompanyTypeEnum
{
    [Description("Clínica de Odontologia")]
    ClinicaOdontologia = 1,

    [Description("Consultório Médico")]
    ConsultorioMedico = 2,

    [Description("Clínica de Estética")]
    ClinicaEstetica = 3,

    [Description("Psicólogo")]
    Psicologos = 4,

    [Description("Terapeuta")]
    Terapeuta = 5,

    [Description("Academia")]
    Academia = 6,

    [Description("Pet Shop/Clínica Veterinária")]
    PetShopClinicaVeterinaria = 7,

    [Description("Salão de Beleza")]
    SalaoBeleza = 8,

    [Description("Oficina Mecânica")]
    OficinaMecanica = 9,

    [Description("Estúdio de Fotografia")]
    EstudioFotografia = 10,

    [Description("Consultório de Fisioterapia")]
    ConsultorioFisioterapia = 11
}