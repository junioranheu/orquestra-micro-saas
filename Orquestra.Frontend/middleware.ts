import { NextResponse, type NextRequest } from 'next/server';
import { CONSTS_AUTH } from './app/api/consts/auth';
import { BASE } from './app/api/fetch';
import ROUTES from './app/consts/routes';
import SYSTEM from './app/consts/system';
import { MODULE_ENUM } from './app/enums/modulesEnum';

const BACKEND_URL = process.env.NEXT_PUBLIC_API_URL_BASE || '';

const PUBLIC_PATHS = [
    ROUTES.LANDING_PAGE, ROUTES.ETC_AJUDA, ROUTES.ETC_SEGURANCA,
    ROUTES.EMPRESA_VERIFICADA, ROUTES.USUARIO_VERIFICADO
];

const PUBLIC_PATHS_BLOCKED_WITH_TOKEN = [
    ROUTES.LOGIN, ROUTES.CRIAR_CONTA, ROUTES.USUARIO_SENHA_REDEFINIDA
];

export const MODULES_PERMISSIONS: Record<string, string[]> = {
    // [ROUTES.EMPRESA_CLIENTES]: ['*'],
    // [ROUTES.EMPRESA_COLABORADORES]: ['*'],
    [ROUTES.EMPRESA_COLABORADORES]: [MODULE_ENUM.Member.toString()],
    [ROUTES.EMPRESA_CLIENTES]: [MODULE_ENUM.Client.toString()],
    [ROUTES.EMPRESA_AGENDAMENTOS]: [MODULE_ENUM.Scheduling.toString()],
    [ROUTES.EMPRESA_INTEGRACAO_WHATSAPP]: [MODULE_ENUM.IntegrationWhatsApp.toString()],
    [ROUTES.EMPRESA_ACOMPANHAMENTOS]: [MODULE_ENUM.CostumerFollowUp.toString()],
    [ROUTES.EMPRESA_NOTA_FISCAL]: [MODULE_ENUM.Invoice.toString()],
    [ROUTES.EMPRESA_FINANCEIRO]: [MODULE_ENUM.Sales.toString()],
    [ROUTES.EMPRESA_ORCAMENTO]: [MODULE_ENUM.Quote.toString()],
    [ROUTES.EMPRESA_ORDEM_DE_SERVICO]: [MODULE_ENUM.ServiceOrder.toString()],
    [ROUTES.EMPRESA_ESTOQUE]: [MODULE_ENUM.Inventory.toString()]
};

export async function middleware(request: NextRequest) {
    const token = request.cookies.get(SYSTEM.COOKIE_AUTH_FRONT)?.value;
    const pathname = request.nextUrl.pathname;

    const isPublicPath = PUBLIC_PATHS.some(x => pathname.startsWith(x));

    // #1 - Rota pública;
    if (isPublicPath) {
        return NextResponse.next();
    }

    const isPublicPathBlockedWithToken = PUBLIC_PATHS_BLOCKED_WITH_TOKEN.some(x => pathname.startsWith(x));

    // #2 - Rotas públicas que são bloqueadas caso o usuário não tenha um token;
    if (!token) {
        return isPublicPathBlockedWithToken ? NextResponse.next() : NextResponse.redirect(new URL(ROUTES.LOGIN, request.url));
    }

    // #3 - Com token: redireciona home ('/') e rotas públicas para dashboard;
    if (pathname === '/' || isPublicPathBlockedWithToken) {
        return NextResponse.redirect(new URL(ROUTES.DASHBOARD, request.url));
    }

    // #4 - Checar se o usuário tem acesso à rota;
    const hasAccess = await handleCheckUserAccess(token, pathname, request);

    if (!hasAccess) {
        return NextResponse.redirect(new URL(ROUTES.ERRO_403, request.url));
    }

    return NextResponse.next();
}

export const config = {
    matcher: ['/((?!api|_next/static|_next/image|favicon.ico).*)'],
};

async function handleCheckUserAccess(token: string, pathname: string, request: NextRequest): Promise<boolean> {
    try {
        // #1 - Checa quais permissões o módulo precisa;
        const mustModulePermissions = MODULES_PERMISSIONS[pathname];

        if (!mustModulePermissions) {
            return true; // Rota sem restrição explícita;
        }

        // #2 - Validar integridade do cookie antes de confiar no conteúdo;
        let tokenJson: { userId?: string };

        try {
            tokenJson = JSON.parse(token);
        } catch {
            return false; // Cookie corrompido ou forjado;
        }

        // #3 - Validar que userId é um GUID válido (formato: xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx);
        const guidRegex = /^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i;

        if (!tokenJson.userId || !guidRegex.test(tokenJson.userId)) {
            return false; // userId ausente ou inválido;
        }

        // #4 - Encaminhar o COOKIE_AUTH_BACK ao backend; o endpoint agora é autenticado;
        const authBackCookie = request.cookies.get(SYSTEM.COOKIE_AUTH_BACK)?.value;

        if (!authBackCookie) {
            return false; // Sem cookie HttpOnly = não autenticado;
        }

        const headers: Record<string, string> = {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
            'Cookie': `${SYSTEM.COOKIE_AUTH_BACK}=${authBackCookie}`
        };

        // Middleware já roda server-side dentro do Next.js;
        // Então a request vai direto para API para evitar problemas de dupla proxyagem no BFF;
        const urlMeModulesNormalized = `${BACKEND_URL}${CONSTS_AUTH.meModules.replace(BASE, '')}`;
        const res = await fetch(urlMeModulesNormalized, { method: 'GET', headers });
        // console.log(`Resposta request [${urlMeModulesNormalized}]`, res.ok);

        if (!res.ok) {
            return false;
        }

        const userModules = await res.json();

        // #5 - Checagem: verificar se o usuário tem qualquer permissão necessária (especificamente);
        const specificModules = mustModulePermissions.filter(role => role !== '*');
        const hasAccess = specificModules.length === 0 ? true : specificModules.some(role => userModules.includes(Number(role)));

        return hasAccess;
    } catch (err: unknown) {
        console.error(err);
        return false;
    }
}