export enum UserRoleEnum {
    Common = 1,
    Maintainer = 999,
    Administrator = 1000
}

export function getUserRoleDescription(role: UserRoleEnum) {
    switch (role) {
        case UserRoleEnum.Common:
            return 'Usuário do sistema';
        case UserRoleEnum.Maintainer:
            return 'Suporte do sistema';
        case UserRoleEnum.Administrator:
            return 'Administrador do sistema';
        default:
            return 'Desconhecido';
    }
}