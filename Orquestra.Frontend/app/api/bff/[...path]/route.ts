import SYSTEM from '@/app/consts/system';
import { NextRequest, NextResponse } from 'next/server';

/**
 * BFF (Backend For Frontend)
 *
 * Objetivo:
 * Fazer o browser conversar somente com o frontend.
 *
 * Fluxo:
 * Browser -> Next.js -> Backend (.NET)
 *
 * Isso transforma cookies third-party em first-party,
 * evitando problemas com:
 * - Safari ITP
 * - Firefox ETP
 * - Chrome third-party cookie deprecation
 */

const BACKEND_URL = process.env.NEXT_PUBLIC_API_URL_BASE as string;

/**
 * Expiração do refresh token em segundos.
 *
 * 60 segundos
 * 60 minutos
 * 24 horas
 * 14 dias
 */
const REFRESH_TOKEN_EXPIRATION_IN_SECONDS = 60 * 60 * 24 * 14;

/**
 * Header utilizado por browsers
 * para marcar cookies expirados.
 */
const EXPIRED_COOKIE_DATE = 'expires=thu, 01 jan 1970';

/**
 * Header utilizado para invalidar
 * cookies imediatamente.
 */
const EXPIRED_COOKIE_MAX_AGE = 'max-age=0';

/**
 * Headers que não devem ser repassados ao browser.
 *
 * Alguns deles podem quebrar compressão,
 * streaming ou conflito de cookies.
 */
const RESPONSE_HEADERS_TO_SKIP = new Set([
    'set-cookie',
    'transfer-encoding',
    'content-encoding',
    'content-length',
]);

/**
 * Configuração padrão do cookie.
 *
 * Como agora o cookie é same-origin,
 * SameSite=Lax funciona normalmente.
 */
const COOKIE_OPTIONS = {
    httpOnly: true,
    secure: true,
    sameSite: 'lax' as const,
    path: '/'
};

/**
 * Handler principal do proxy.
 *
 * Responsabilidades:
 * - montar URL do backend
 * - preparar request
 * - chamar backend
 * - criar response do Next.js
 * - sincronizar cookies
 */
async function handler(request: NextRequest, { params }: { params: Promise<{ path: string[] }> }) {
    try {
        const { path } = await params;

        const targetUrl = buildTargetUrl(request, path);

        const upstreamRequest = await buildUpstreamRequest(request);

        const upstreamResponse = await fetch(targetUrl, upstreamRequest);

        const nextResponse = buildNextResponse(upstreamResponse);

        syncAuthCookie(upstreamResponse, nextResponse);

        return nextResponse;
    } catch {
        return NextResponse.json(
            { Messages: ['Não foi possível conectar ao servidor. Tente novamente mais tarde.'] },
            { status: 502 }
        );
    }
}

/**
 * Monta URL final da API.
 *
 * Exemplo:
 *
 * Front:
 * /api/bff/api/User/Get
 *
 * Backend:
 * https://api.site.com/api/User/Get
 */
function buildTargetUrl(request: NextRequest, path: string[]) {
    const targetPath = path.join('/');

    const url = new URL(request.url);

    const queryString = url.searchParams.toString();

    return `${BACKEND_URL}/${targetPath}${queryString ? `?${queryString}` : ''}`;
}

/**
 * Monta request que será enviada ao backend.
 */
async function buildUpstreamRequest(request: NextRequest) {
    const headers = buildForwardHeaders(request);

    const body = await buildRequestBody(request);

    return {
        method: request.method,
        headers,
        body
    };
}

/**
 * Define quais headers serão encaminhados.
 *
 * Não repassamos:
 * - host
 * - origin
 * - referer
 *
 * Isso evita problemas de CORS,
 * domínio e comportamento inesperado.
 */
function buildForwardHeaders(request: NextRequest) {
    const headers = new Headers();

    const headersToForward = [
        'accept',
        'content-type',
        'x-internal-key',
    ];

    for (const key of headersToForward) {
        const value = request.headers.get(key);

        if (value) {
            headers.set(key, value);
        }
    }

    /**
     * Repassa cookie de autenticação
     * manualmente para o backend.
     */
    const authCookie = request.cookies.get(SYSTEM.COOKIE_AUTH_BACK)?.value;

    if (authCookie) {
        headers.set('Cookie', `${SYSTEM.COOKIE_AUTH_BACK}=${authCookie}`);
    }

    return headers;
}

/**
 * Encaminha body apenas para métodos
 * que suportam body.
 */
async function buildRequestBody(request: NextRequest) {
    if (request.method === 'GET' || request.method === 'HEAD') {
        return null;
    }

    return await request.arrayBuffer();
}

/**
 * Cria response do Next.js
 * baseada na response do backend.
 */
function buildNextResponse(upstreamResponse: Response) {
    const responseHeaders = new Headers();

    upstreamResponse.headers.forEach((value, key) => {
        if (!RESPONSE_HEADERS_TO_SKIP.has(key.toLowerCase())) {
            responseHeaders.set(key, value);
        }
    });

    return new NextResponse(upstreamResponse.body, {
        status: upstreamResponse.status,
        headers: responseHeaders,
    });
}

/**
 * Intercepta cookies do backend
 * e reaplica como first-party cookies.
 */
function syncAuthCookie(upstreamResponse: Response, nextResponse: NextResponse) {
    const setCookies = upstreamResponse.headers.getSetCookie?.() ?? [];

    for (const setCookie of setCookies) {
        /**
         * Ignora cookies irrelevantes.
         */
        if (!setCookie.includes(SYSTEM.COOKIE_AUTH_BACK)) {
            continue;
        }

        const cookieValue = extractCookieValue(
            setCookie,
            SYSTEM.COOKIE_AUTH_BACK
        );

        if (cookieValue === null) {
            continue;
        }

        /**
         * Detecta remoção de cookie.
         */
        const isDelete = isCookieDeletion(setCookie);

        if (isDelete) {
            nextResponse.cookies.set(SYSTEM.COOKIE_AUTH_BACK, '', {
                ...COOKIE_OPTIONS,
                maxAge: 0,
            });

            continue;
        }

        /**
         * Reaplica cookie autenticado.
         */
        nextResponse.cookies.set(SYSTEM.COOKIE_AUTH_BACK, cookieValue, {
            ...COOKIE_OPTIONS,
            maxAge: REFRESH_TOKEN_EXPIRATION_IN_SECONDS,
        });
    }
}

/**
 * Verifica se backend pediu
 * exclusão do cookie.
 */
function isCookieDeletion(setCookieHeader: string) {
    const header = setCookieHeader.toLowerCase();

    return (
        header.includes(EXPIRED_COOKIE_MAX_AGE) ||
        header.includes(EXPIRED_COOKIE_DATE)
    );
}

/**
 * Extrai valor do cookie do header Set-Cookie.
 */
function extractCookieValue(setCookieHeader: string, cookieName: string): string | null {
    const prefix = `${cookieName}=`;

    const startIndex = setCookieHeader.indexOf(prefix);

    if (startIndex === -1) {
        return null;
    }

    const valueStart = startIndex + prefix.length;

    const endIndex = setCookieHeader.indexOf(';', valueStart);

    return endIndex === -1
        ? setCookieHeader.substring(valueStart)
        : setCookieHeader.substring(valueStart, endIndex);
}

export const GET = handler;
export const POST = handler;
export const PUT = handler;
export const DELETE = handler;
export const PATCH = handler;