export enum USER_ROLE_ENUM {
    Common = 1,
    Maintainer = 999,
    Administrator = 1000
}

export function getUserRoleDescription(role: USER_ROLE_ENUM) {
    switch (role) {
        case USER_ROLE_ENUM.Common:
            return 'Usuário do sistema';
        case USER_ROLE_ENUM.Maintainer:
            return 'Suporte do sistema';
        case USER_ROLE_ENUM.Administrator:
            return 'Administrador do sistema';
        default:
            return 'Desconhecido';
    }
}