'use client';
import Head from '@/app/(routes)/head';
import { CookieDefault } from '@/app/components/cookie';
import { HANKEN } from '@/app/fonts/fonts';
import useCheckAzureServer from '@/app/hooks/api/useCheckAzureServer';
import useStandardIntructions from '@/app/hooks/useStandardInstructions';
import feather from 'feather-icons';
import { ReactNode, useEffect } from 'react';
import 'tippy.js/dist/tippy.css';

export default function RootLayout({ children }: Readonly<{ children: ReactNode }>) {

    useStandardIntructions();
    useCheckAzureServer();

    useEffect(() => {
        feather.replace();
    }, [])

    return (
        <html lang='pt-BR'>
            <Head />

            <body className={HANKEN.className}>
                <main>
                    {children}

                    <CookieDefault extenseButtonDescription={false} />
                </main>
            </body>
        </html>
    )
}