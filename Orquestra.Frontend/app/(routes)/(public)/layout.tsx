'use client';
import Head from '@/app/(routes)/head';
import { CookieDefault } from '@/app/components/cookie';
import NavbarNotAuth from '@/app/components/navbar/nav-not-auth';
import UpNav from '@/app/components/navbar/up-nav';
import { HANKEN } from '@/app/fonts/fonts';
import useShowNProgressOnPageLoad from '@/app/hooks/useShowNProgressOnPageLoad';
import useStandardIntructions from '@/app/hooks/useStandardInstructions';
import '@/app/styles/globals.scss';
import 'animate.css/animate.min.css';
import feather from 'feather-icons';
import { usePathname } from 'next/navigation';
import 'nprogress/nprogress.css';
import { Fragment, ReactNode, useEffect } from 'react';
import { Toaster } from 'sonner';
import 'tippy.js/dist/tippy.css';

export default function RootLayout({ children }: Readonly<{ children: ReactNode }>) {

    useStandardIntructions();
    useShowNProgressOnPageLoad();

    const pathname = usePathname();
    const hideHeader = pathname?.includes('/orquestra');

    useEffect(() => {
        feather.replace();
    }, [])

    return (
        <html lang='pt-BR'>
            <Head />

            <body className={HANKEN.className}>
                <Toaster expand={false} closeButton={false} />

                {
                    !hideHeader && <UpNav />
                }

                <main className='no-layout'>
                    {
                        !hideHeader ? (
                            <Fragment>
                                <header>
                                    <NavbarNotAuth />
                                </header>

                                <div className='children'>
                                    {children}
                                </div>
                            </Fragment>
                        ) : (
                            children
                        )
                    }

                    <CookieDefault extenseButtonDescription={false} />
                </main>
            </body>
        </html>
    )
}