'use client';
import Head from '@/app/(routes)/head';
import { CookieDefault } from '@/app/components/cookie';
import { HANKEN } from '@/app/fonts/fonts';
import useStandardIntructions from '@/app/hooks/useStandardInstructions';
import feather from 'feather-icons';
import { ReactNode, useEffect } from 'react';

export default function RootLayout({ children }: Readonly<{ children: ReactNode }>) {

    useStandardIntructions();

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