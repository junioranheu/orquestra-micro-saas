const nextConfig: import('next').NextConfig = {
    reactStrictMode: true,

    eslint: {
        ignoreDuringBuilds: true
    },

    async rewrites() {
        return [
            {
                source: '/api/:path*',
                destination: 'https://orquestra-api.azurewebsites.net/:path*'
            }
        ];
    }
}

export default nextConfig;