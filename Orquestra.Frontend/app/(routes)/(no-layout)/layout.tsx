import Head from '@/app/(routes)/head';

export default function RootLayout({ children, }: { children: React.ReactNode; }) {
    return (
        <html lang='pt-BR'>
            <head>
                <script src='https://cdn.tailwindcss.com'></script>
            </head>

            <Head />

            <body>
                <main>
                    {children}
                </main>
            </body>
        </html>
    )
}