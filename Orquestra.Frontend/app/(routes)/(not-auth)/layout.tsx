'use client';
import Head from '@/app/(routes)/head';
import UpNav from '@/app/components/navbar/up-nav';
import { GlobalContextProvider } from '@/app/contexts/global.context';
import { UserProvider } from '@/app/contexts/user.context';
import { HANKEN } from '@/app/fonts/fonts';
import useCheckAzureServer from '@/app/hooks/useCheckAzureServer';
import '@/app/styles/globals.scss';
import 'animate.css/animate.min.css';
import { ReactNode } from 'react';
import { Toaster } from 'sonner';

export default function RootLayout({ children }: Readonly<{ children: ReactNode }>) {

    useCheckAzureServer();

    return (
        <html lang='pt-BR'>
            <UserProvider>
                <GlobalContextProvider>
                    <Head />

                    <body className={HANKEN.className}>
                        <Toaster expand={false} closeButton={false} />
                        <UpNav />

                        <main>
                            {children}
                        </main>
                    </body>
                </GlobalContextProvider>
            </UserProvider>
        </html>
    )
}