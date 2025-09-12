import { NextResponse, type NextRequest } from 'next/server';
import ROUTES from './app/consts/routes';
import SYSTEM from './app/consts/system';

const PUBLIC_PATHS_BLOCKED_WITH_TOKEN = [ROUTES.LOGIN, ROUTES.CRIAR_CONTA, ROUTES.LANDING_PAGE, ROUTES.USUARIO_VERIFICADO];

export async function middleware(request: NextRequest) {
    const token = request.cookies.get(SYSTEM.COOKIE_NAME)?.value;
    const pathname = request.nextUrl.pathname;
    const isPublicPath = PUBLIC_PATHS_BLOCKED_WITH_TOKEN.some(path => pathname.startsWith(path));

    // Sem token: só permite rotas públicas;
    if (!token) {
        return isPublicPath ? NextResponse.next() : NextResponse.redirect(new URL(ROUTES.LOGIN, request.url));
    }

    // Com token: redireciona home ('/') e rotas públicas para dashboard;
    if (pathname === '/' || isPublicPath) {
        return NextResponse.redirect(new URL(ROUTES.DASHBOARD, request.url));
    }

    return NextResponse.next();
}

export const config = {
    matcher: ['/((?!api|_next/static|_next/image|favicon.ico).*)'],
};