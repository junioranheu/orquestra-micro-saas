using System.ComponentModel;

namespace Orquestra.Domain.Enums;

public enum RecoverPasswordQuestionEnum
{
    [Description("Qual é o nome do seu primeiro animal de estimação?")]
    PetName,

    [Description("Qual é o nome da sua cidade natal?")]
    BirthCity,

    [Description("Qual é o nome da sua mãe?")]
    MotherName,

    [Description("Qual é o nome do seu melhor amigo de infância?")]
    ChildhoodFriend
}