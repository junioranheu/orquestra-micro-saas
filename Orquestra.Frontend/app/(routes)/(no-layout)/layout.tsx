'use client';
import Head from '@/app/(routes)/head';
import { CookieDefault } from '@/app/components/cookie';
import { HANKEN } from '@/app/fonts/fonts';
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
                <main className={HANKEN.className}>
                    {children}

                    <CookieDefault extenseButtonDescription={false} />
                </main>
            </body>
        </html>
    )
}