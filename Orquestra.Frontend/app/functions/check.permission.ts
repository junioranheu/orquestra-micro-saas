import { iMe } from '@/app/api/consts/auth';
import { Module } from '@/app/consts/modules';

interface iProps {
    me: iMe | undefined;
    rolesRequired: Module[];
    mustBeSystemAdmin?: boolean;
}

// Verificar se o usuário tem a permissão para visualizar um elemento;
export function handleCheckShowElement({ me, rolesRequired, mustBeSystemAdmin = false }: iProps): boolean {
    if (mustBeSystemAdmin) {
        return handleCheckIfSysAdm(me);
    }

    if (!me || !me?.isAuth || !me?.currentMainCompany) {
        return false;
    }

    if (!rolesRequired || !rolesRequired.length) {
        return true;
    }

    const userModules = me?.currentMainCompany?.userModules as string[];
    // console.log('user', userModules);
    // console.log('rolesRequired', rolesRequired);

    if (!userModules || !userModules?.length) {
        return false;
    }

    let isShowElement = false;

    userModules?.forEach(x => {
        rolesRequired.forEach(required => {
            if ((x.toString() === required.toString()) && !isShowElement) {
                isShowElement = true;
            }
        });
    })

    // console.log(rolesRequired, isShowElement);
    return isShowElement;
}

export function handleCheckIfSysAdm(me: iMe | undefined) {
    const systemAdmin = 1000; // Back-end;

    if (me && me.roles?.map(x => x.toString()).includes(systemAdmin.toString())) {
        return true;
    }

    return false;
}