'use client';
import Head from '@/app/(routes)/head';
import Loading from '@/app/components/loading';
import Navbar from '@/app/components/navbar/nav';
import UpNav from '@/app/components/navbar/up-nav';
import Sidebar from '@/app/components/sidebar';
import { GlobalContextProvider } from '@/app/contexts/global.context';
import { UserProvider } from '@/app/contexts/user.context';
import { HANKEN } from '@/app/fonts/fonts';
import useCheckAzureServer from '@/app/hooks/useCheckAzureServer';
import '@/app/styles/globals.scss';
import 'animate.css/animate.min.css';
import feather from 'feather-icons';
import { ReactNode, useEffect } from 'react';
import { Toaster } from 'sonner';
import 'tippy.js/dist/tippy.css';

export default function RootLayout({ children }: Readonly<{ children: ReactNode }>) {

    useCheckAzureServer();

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
                        <Loading typeMessage='normal' />
                        <UpNav />

                        <div className='main-wrapper'>
                            <Sidebar />

                            <main>
                                <header>
                                    <Navbar />
                                </header>

                                <div className='children'>
                                    {children}
                                </div>
                            </main>
                        </div>
                    </body>
                </GlobalContextProvider>
            </UserProvider>
        </html>
    )
}