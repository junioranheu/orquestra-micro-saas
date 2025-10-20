'use client';
import Head from '@/app/(routes)/head';
import Loading from '@/app/components/loading';
import Navbar from '@/app/components/navbar/nav';
import UpNav from '@/app/components/navbar/up-nav';
import Sidebar from '@/app/components/sidebar';
import Splash from '@/app/components/splash';
import SYSTEM from '@/app/consts/system';
import { GlobalContextProvider } from '@/app/contexts/global.context';
import { UserProvider } from '@/app/contexts/user.context';
import { HANKEN } from '@/app/fonts/fonts';
import handleGetRandomNumber from '@/app/functions/get.randomNumber';
import useCheckAzureServer from '@/app/hooks/useCheckAzureServer';
import useShowNProgressOnPageLoad from '@/app/hooks/useShowNProgressOnPageLoad';
import useStandardIntructions from '@/app/hooks/useStandardInstructions';
import '@/app/styles/globals.scss';
import 'animate.css/animate.min.css';
import feather from 'feather-icons';
import 'nprogress/nprogress.css';
import { ReactNode, useEffect } from 'react';
import { Toaster } from 'sonner';
import 'tippy.js/dist/tippy.css';

export default function RootLayout({ children }: Readonly<{ children: ReactNode }>) {

    useStandardIntructions();
    useCheckAzureServer();
    useShowNProgressOnPageLoad();

    useEffect(() => {
        feather.replace();
    }, [])

    return (
        <html lang='pt-BR'>
            <UserProvider>
                <GlobalContextProvider>
                    <Head />

                    <body className={`body ${HANKEN.className}`}>
                        <Toaster expand={false} closeButton={false} />
                        <Loading typeMessage='normal' />
                        <UpNav />

                        <div className='auth'>
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

                        {
                            process.env.NODE_ENV !== 'development' && (
                                <Splash text={SYSTEM.NAME} isGradient={true} destroyAfterSeconds={handleGetRandomNumber(1, 1.25)} />
                            )
                        }
                    </body>
                </GlobalContextProvider>
            </UserProvider>
        </html>
    )
}