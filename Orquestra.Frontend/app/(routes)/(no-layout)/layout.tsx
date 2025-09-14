'use client';
import Head from '@/app/(routes)/head';
import feather from 'feather-icons';
import { useEffect } from 'react';

export default function RootLayout({ children, }: { children: React.ReactNode; }) {

    useEffect(() => {
        feather.replace();
    }, [])

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