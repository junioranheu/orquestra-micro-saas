import iMe from '@/app/api/consts/auth';

export const MODULES = {
    Scheduling: 'Scheduling',
    Sales: 'Sales',
} as const;

type Module = typeof MODULES[keyof typeof MODULES];

// Verificar se o usuário tem a permissão para visualizar um elemento;
export function handleCheckShowElement(me: iMe | undefined, rolesRequired: Module[]): boolean {
    if (!me || !me?.isAuth || !me?.currentMainCompany) {
        return false;
    }

    if (!rolesRequired || !rolesRequired.length) {
        return true;
    }

    const modules = me?.currentMainCompany?.modules as string[];
    // console.log(modules);

    if (!modules || !modules?.length) {
        return false;
    }

    let isShowElement = false;

    modules?.forEach(x => {
        rolesRequired.forEach(required => {
            if ((x === required) && !isShowElement) {
                isShowElement = true;
            }
        });
    })

    console.log(rolesRequired, isShowElement);
    return isShowElement;
}