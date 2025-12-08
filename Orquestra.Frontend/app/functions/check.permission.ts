import { iMe } from '@/app/api/consts/auth';
import { Module, MODULE_ENUM } from '@/app/enums/modulesEnum';

interface iProps {
    me: iMe | undefined;
    modulesRequired: Module[];
    mustBeSystemAdmin?: boolean;
}

// Verificar se o usuário tem a permissão para visualizar um elemento específico;
export function handleCheckShowElement({ me, modulesRequired: rolesRequired, mustBeSystemAdmin = false }: iProps): boolean {
    if (mustBeSystemAdmin) {
        return handleCheckIfSysAdm({ me });
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

// Verificar se o usuário é um administrador do sistema;
export function handleCheckIfSysAdm({ me }: { me: iMe | undefined }): boolean {
    const systemAdmin = 1000; // Back-end;

    if (me && me.roles?.map(x => x.toString()).includes(systemAdmin.toString())) {
        return true;
    }

    return false;
}

// Verificar se o usuário tem acesso a pelo menos 1 dos módulos (se não passar nenhum param, verifica todos);
export function hasAccessToAnyModule({ me, modulesToCheck }: { me: iMe | undefined, modulesToCheck?: MODULE_ENUM[] }): boolean {
    if (!me || !me.isAuth || !me.currentMainCompany) {
        return false;
    }

    const userModules = me.currentMainCompany.userModules as string[] | undefined;

    if (!userModules || !userModules.length) {
        return false;
    }

    // Se não passar modulesToCheck, pega todos do enum;
    const modulesToCheckList = modulesToCheck && modulesToCheck.length > 0 ? modulesToCheck : Object.values(MODULE_ENUM).filter(v => typeof v === 'number') as MODULE_ENUM[];

    const userModulesStr = userModules.map(x => x.toString());
    const modulesToCheckStr = modulesToCheckList.map(x => x.toString());

    return modulesToCheckStr.some(module => userModulesStr.includes(module));
}