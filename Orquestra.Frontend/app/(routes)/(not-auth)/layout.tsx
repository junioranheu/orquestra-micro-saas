'use client';
import Head from '@/app/(routes)/head';
import '@/app/_styles/globals.scss';
import { HANKEN } from '@/app/fonts/fonts';
import { ReactNode } from 'react';
import { Toaster } from 'sonner';

export default function RootLayout({ children }: Readonly<{ children: ReactNode }>) {
    return (
        <html lang='pt-BR'>
            <Head />

            <body className={HANKEN.className}>
                <Toaster expand={false} closeButton={false} />

                <main>
                    {children}
                </main>
            </body>
        </html>
    )
}