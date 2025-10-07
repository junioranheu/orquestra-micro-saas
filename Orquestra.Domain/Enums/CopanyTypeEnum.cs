using System.ComponentModel;

namespace Orquestra.Domain.Enums;

public enum CompanyTypeEnum
{
    [Description("Clínica de odontologia")]
    ClinicaOdontologia = 1,

    [Description("Consultório médico")]
    ConsultorioMedico = 2,

    [Description("Clínica de estética")]
    ClinicaEstetica = 3,

    [Description("Psicólogo")]
    Psicologos = 4,

    [Description("Terapeuta")]
    Terapeuta = 5,

    [Description("Academia")]
    Academia = 6,

    [Description("Pet shop")]
    PetShop = 7,

    [Description("Clínica veterinária")]
    ClinicaVeterinaria = 8,

    [Description("Salão de beleza")]
    SalaoBeleza = 9,

    [Description("Barbearia")]
    Barbearia = 10,

    [Description("Oficina mecânica")]
    OficinaMecanica = 11,

    [Description("Estúdio de fotografia")]
    EstudioFotografia = 12,

    [Description("Consultório de fisioterapia")]
    ConsultorioFisioterapia = 13,

    [Description("Nutricionista")]
    Nutricionista = 14,

    [Description("Fisioterapeuta esportivo")]
    FisioterapeutaEsportivo = 15,

    [Description("Coach")]
    Coach = 16,

    [Description("Mentor")]
    Mentor = 17,

    [Description("Consultor")]
    Consultor = 18,

    [Description("Massagista")]
    Massagista = 19,

    [Description("Advogado")]
    Advogado = 20,

    [Description("Contador")]
    Contador = 21,

    [Description("Instrutor de idiomas")]
    InstrutorIdiomas = 22,

    [Description("Professor particular")]
    ProfessorParticular = 23,

    [Description("Artista")]
    Artista = 24,

    [Description("Designer")]
    Designer = 25,

    [Description("Fotógrafo")]
    Fotografo = 26,

    [Description("Freelancer")]
    Freelancer = 27,

    [Description("Outro")]
    Outro = 99
}