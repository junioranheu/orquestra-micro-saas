'use client';
import Head from '@/app/(routes)/head';
import UpNav from '@/app/components/navbar/up-nav';
import { GlobalContextProvider } from '@/app/contexts/global.context';
import { UserProvider } from '@/app/contexts/user.context';
import { HANKEN } from '@/app/fonts/fonts';
import useCheckAzureServer from '@/app/hooks/api/useCheckAzureServer';
import useShowNProgressOnPageLoad from '@/app/hooks/useShowNProgressOnPageLoad';
import useStandardIntructions from '@/app/hooks/useStandardInstructions';
import '@/app/styles/globals.scss';
import 'animate.css/animate.min.css';
import 'nprogress/nprogress.css';
import { ReactNode } from 'react';
import { Toaster } from 'sonner';
import 'tippy.js/dist/tippy.css';

export default function RootLayout({ children }: Readonly<{ children: ReactNode }>) {

    useStandardIntructions();
    useCheckAzureServer();
    useShowNProgressOnPageLoad();

    return (
        <html lang='pt-BR'>
            <UserProvider>
                <GlobalContextProvider>
                    <Head />

                    <body className={`body ${HANKEN.className}`}>
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