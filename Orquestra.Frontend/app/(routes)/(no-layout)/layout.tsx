'use client';
import Head from '@/app/(routes)/head';
import { CookieDefault } from '@/app/components/cookie';
import UpNav from '@/app/components/navbar/up-nav';
import { HANKEN } from '@/app/fonts/fonts';
import '@/app/styles/globals.scss';
import feather from 'feather-icons';
import { useEffect } from 'react';
import { Toaster } from 'sonner';

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
                <Toaster expand={false} closeButton={false} />
                <UpNav />

                <main className={HANKEN.className}>
                    {children}

                    <CookieDefault extenseButtonDescription={false} />
                </main>
            </body>
        </html>
    )
}