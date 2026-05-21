const nextConfig: import('next').NextConfig = {
    /** Evita execuções duplicadas do React Strict Mode no ambiente de desenvolvimento. */
    reactStrictMode: false,

    /** Remove o header "x-powered-by" para não expor o uso de Next.js nas responses. */
    poweredByHeader: false
}

export default nextConfig;