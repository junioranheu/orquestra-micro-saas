const nextConfig: import('next').NextConfig = {
    reactStrictMode: true,
    distDir: 'build',
    output: 'standalone',

    eslint: {
        ignoreDuringBuilds: true
    }
}

export default nextConfig;