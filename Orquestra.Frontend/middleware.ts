import { NextResponse, type NextRequest } from 'next/server';
import ROUTES from './app/consts/routes';

export function middleware(request: NextRequest) {
    const pathname = request.nextUrl.pathname;
    // console.log(`Um ${request.method} foi realizado em ${pathname} às ${handleGetHour()}`);

    if (pathname === ROUTES.INDEX) {
        return NextResponse.redirect(new URL(ROUTES.LOGIN, request.url))
    }

    return NextResponse.next();
}

function handleGetHour() {
    const date = new Date();
    return `${date.getHours()}:${date.getMinutes()}:${date.getSeconds()}`;
}

export const config = {
    matcher: ['/((?!api|_next/static|_next/image|favicon.ico).*)']
};