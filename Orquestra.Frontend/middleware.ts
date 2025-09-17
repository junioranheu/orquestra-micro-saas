import { NextResponse, type NextRequest } from 'next/server';
import { CONSTS_AUTH } from './app/api/consts/auth';
import ROUTES from './app/consts/routes';
import SYSTEM from './app/consts/system';
import { MODULES } from './app/functions/check.permission';

const PUBLIC_PATHS_BLOCKED_WITH_TOKEN = [ROUTES.LOGIN, ROUTES.CRIAR_CONTA, ROUTES.LANDING_PAGE, ROUTES.USUARIO_VERIFICADO];

export const MODULES_PERMISSIONS: Record<string, string[]> = {
    [ROUTES.EMPRESA_CLIENTES]: ['*'],
    [ROUTES.EMPRESA_MEMBROS]: ['*'],
    [ROUTES.EMPRESA_AGENDAMENTOS]: [MODULES.Scheduling],
    [ROUTES.EMPRESA_FINANCEIRO]: [MODULES.Sales]
};

export async function middleware(request: NextRequest) {
    const token = request.cookies.get(SYSTEM.COOKIE_NAME)?.value;
    const pathname = request.nextUrl.pathname;
    const isPublicPath = PUBLIC_PATHS_BLOCKED_WITH_TOKEN.some(path => pathname.startsWith(path));

    // #1 - Sem token: só permite rotas públicas;
    if (!token) {
        return isPublicPath ? NextResponse.next() : NextResponse.redirect(new URL(ROUTES.LOGIN, request.url));
    }

    // #2 - Com token: redireciona home ('/') e rotas públicas para dashboard;
    if (pathname === '/' || isPublicPath) {
        return NextResponse.redirect(new URL(ROUTES.DASHBOARD, request.url));
    }

    // #3 - Checar se o usuário tem acesso à rota;
    const hasAccess = await handleCheckUserAccess(token, pathname);
    // console.log('hasAccess', hasAccess);

    if (!hasAccess) {
        return NextResponse.redirect(new URL(ROUTES.ERRO_403, request.url));
    }

    return NextResponse.next();
}

export const config = {
    matcher: ['/((?!api|_next/static|_next/image|favicon.ico).*)'],
};

async function handleCheckUserAccess(token: string, pathname: string): Promise<boolean> {
    try {
        // #1 - Checa quais permissões o módulo precisa;
        const mustModulePermissions = MODULES_PERMISSIONS[pathname];
        // console.log('mustModulePermissions', mustModulePermissions);

        if (!mustModulePermissions) {
            return true; // Rota sem restrição explícita;
        }

        // #2 - Requisição para descobrir os módulos do usuário;
        const tokenJson = JSON.parse(token);

        const headers: Record<string, string> = {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        };

        const res = await fetch(`${CONSTS_AUTH.meModules}?userId=${tokenJson.userId}`, { method: 'GET', headers });
        // console.log(res);

        if (!res.ok) {
            return false;
        }

        const userModules = await res.json();
        console.log('userModules', userModules, userModules?.length);

        // #3 - Checagem: se precisar de QUALQEUER modulo mas o usuário não tem nenhum: false;
        if (mustModulePermissions.includes('*') && (!userModules || !userModules.length)) {
            return false;
        }

        // #4 - Checagem: verificar se o usuário tem qualquer permissão necessária (especificamente);
        const specificModules = mustModulePermissions.filter(role => role !== '*');
        const hasAccess = specificModules.length === 0 ? true : specificModules.some(role => userModules.includes(role));
        // console.log('hasAccess', hasAccess);

        return hasAccess;
    } catch (err: unknown) {
        console.error(err);
        return false;
    }
}