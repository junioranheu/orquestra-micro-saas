import { NextResponse, type NextRequest } from 'next/server';
import ROUTES from './app/consts/routes';

const PUBLIC_PATHS = [ROUTES.ENTRAR, ROUTES.CRIAR_CONTA, '/public'];

export function middleware(request: NextRequest) {
    const pathname = request.nextUrl.pathname;
    const token = request.cookies.get('auth')?.value;
    console.log('request.cookies.get', request.cookies.get('auth'));
    console.log(`cookies.value ${token}`);

    // Redireciona '/' para dashboard;
    if (pathname === '/') {
        return NextResponse.redirect(new URL(ROUTES.DASHBOARD, request.url));
    }

    // Se tiver token e tentar acessar rota de "não auth", redireciona pro dashboard;
    if (token && PUBLIC_PATHS.some(path => pathname.startsWith(path))) {
        return NextResponse.redirect(new URL(ROUTES.DASHBOARD, request.url));
    }

    // Se não tiver token e tentar acessar rota protegida, redireciona pro login;
    if (!token && !PUBLIC_PATHS.some(path => pathname.startsWith(path))) {
        return NextResponse.redirect(new URL(ROUTES.ENTRAR, request.url));
    }

    // Se tiver token e rota protegida, ou rota pública sem token → deixa passar;
    return NextResponse.next();
}

export const config = {
    matcher: ['/((?!api|_next/static|_next/image|favicon.ico).*)'],
};