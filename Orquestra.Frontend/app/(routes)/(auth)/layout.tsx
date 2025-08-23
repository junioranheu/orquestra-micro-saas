'use client';
import Head from '@/app/(routes)/head';
import Navbar from '@/app/components/(main)/navbar/nav/navbar';
import { GlobalContextProvider } from '@/app/contexts/global.context';
import { UserProvider } from '@/app/contexts/user.context';
import { HANKEN } from '@/app/fonts/fonts';
import useCheckHasPermission from '@/app/hooks/permission/useCheckHasPermission';
import '@/app/styles/globals.scss';
import 'animate.css/animate.min.css';
import { ReactNode } from 'react';
import { Toaster } from 'sonner';

export default function RootLayout({ children }: Readonly<{ children: ReactNode }>) {

    useCheckHasPermission([]);

    return (
        <html lang='pt-BR'>
            <UserProvider>
                <GlobalContextProvider>
                    <Head />

                    <body className={HANKEN.className}>
                        <Toaster expand={false} closeButton={false} />

                        <header>
                            <Navbar />
                        </header>

                        <main>
                            {children}
                        </main>
                    </body>
                </GlobalContextProvider>
            </UserProvider>
        </html>
    )
}