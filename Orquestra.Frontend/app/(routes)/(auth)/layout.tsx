'use client';
import Head from '@/app/(routes)/head';
import Loading from '@/app/components/loading';
import Navbar from '@/app/components/navbar/nav';
import UpNav from '@/app/components/navbar/up-nav';
import SYSTEM from '@/app/consts/system';
import { GlobalContextProvider } from '@/app/contexts/global.context';
import { UserProvider } from '@/app/contexts/user.context';
import { HANKEN } from '@/app/fonts/fonts';
import '@/app/styles/globals.scss';
import 'animate.css/animate.min.css';
import feather from 'feather-icons';
import { ReactNode, useEffect } from 'react';
import { Toaster } from 'sonner';
import 'tippy.js/dist/tippy.css';

export default function RootLayout({ children }: Readonly<{ children: ReactNode }>) {

    useEffect(() => {
        feather.replace();
    }, [])

    return (
        <html lang='pt-BR'>
            <UserProvider>
                <GlobalContextProvider>
                    <Head />

                    <body className={HANKEN.className}>
                        <Toaster expand={false} closeButton={false} />

                        <header>
                            <UpNav />
                            <Navbar />
                        </header>

                        <main className={SYSTEM.ANIMATE}>
                            <Loading typeMessage='normal' />
                            {children}
                        </main>
                    </body>
                </GlobalContextProvider>
            </UserProvider>
        </html>
    )
}