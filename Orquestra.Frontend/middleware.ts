import { NextResponse, type NextRequest } from 'next/server';
import ROUTES from './app/consts/routes';
import SYSTEM from './app/consts/system';

const PUBLIC_PATHS = [ROUTES.ENTRAR, ROUTES.CRIAR_CONTA, '/public'];

export async function middleware(request: NextRequest) {
    const token = request.cookies.get(SYSTEM.COOKIE_NAME)?.value;
    const pathname = request.nextUrl.pathname;
    const isPublicPath = PUBLIC_PATHS.some(path => pathname.startsWith(path));

    // Sem token: só permite rotas públicas;
    if (!token) {
        return isPublicPath ? NextResponse.next() : NextResponse.redirect(new URL(ROUTES.ENTRAR, request.url));
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