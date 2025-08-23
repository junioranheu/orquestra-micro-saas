import { NextResponse, type NextRequest } from 'next/server';
import ROUTES from './app/consts/routes';

export function middleware(request: NextRequest) {
    const pathname = request.nextUrl.pathname;
    // console.log(`Um ${request.method} foi realizado em ${pathname}`);

    if (pathname == '/') {
        return NextResponse.redirect(new URL(ROUTES.DASHBOARD, request.url));
    }

    return NextResponse.next();
}

export const config = {
    matcher: ['/((?!api|_next/static|_next/image|favicon.ico).*)']
};