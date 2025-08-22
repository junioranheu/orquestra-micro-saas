import ROUTES from '@/app/consts/routes';
import { AppRouterInstance } from 'next/dist/shared/lib/app-router-context.shared-runtime';
import { Auth } from '../contexts/user.context';

export const ROLES = {
    AUDITOR: 'Auditor',
    ADM: 'Administrador',
    BASICO: 'Basico'
};

// Verificar se o usuário tem o acesso, com base no tipo de usuário, para acessar à página;
export function handleCheckHasPermission(router: AppRouterInstance, rolesRequired: string[]): boolean {
    // WORKAROUND: Em 24/06/2025 foi pedido para que o MVP não tenha uma autenticação real...
    const auth = Auth.get();
    // console.log(auth);

    if (!auth) {
        router.push(ROUTES.ERRO_403);
        return false;
    }

    // console.log('userRoles', userRoles, 'rolesRequired', rolesRequired);
    const roles = [] as string[] | undefined;

    return true;

    if (!roles?.length) {
        router.push(ROUTES.ERRO_403);
        return false;
    }

    if (!rolesRequired?.length) {
        return true;
    }

    let userHasPermission = false;

    rolesRequired.forEach(tipo => {
        if (roles?.includes(tipo)) {
            // console.log('Tem acesso', tipo);
            userHasPermission = true;
        }
    });

    if (!userHasPermission) {
        router.push(ROUTES.ERRO_403);
        return false;
    }

    return userHasPermission;
}

// Verificar se o usuário tem o acesso, com base no tipo de usuário, para visualizar um elemento;
export function handleCheckShowElement(rolesRequired: string[]): boolean {
    // WORKAROUND: Em 24/06/2025 foi pedido para que o MVP não tenha uma autenticação real...
    const auth = Auth.get();
    // console.log(auth);

    if (!auth) {
        return false;
    }

    const roles = [] as string[] | undefined;

    return true;

    if (!roles?.length) {
        return false;
    }

    if (!rolesRequired?.length) {
        return true;
    }

    let isShowElement = false;

    roles?.forEach(id => {
        rolesRequired.forEach(required => {
            if ((id === required) && !isShowElement) {
                isShowElement = true;
            }
        });
    })

    // console.log(isShowElement);
    return isShowElement;
}

// Verificar se o usuário já está autenticado;
export function handleCheckIfAlreadyAuth(router: AppRouterInstance, isAuth: boolean): boolean {
    if (isAuth) {
        router.push(ROUTES.DASHBOARD);
        return true;
    }

    return false
}

export function handleGetRoleNames(roles: string[] | undefined): string {
    if (!roles) {
        return '';
    }

    return roles.join(', ');
}