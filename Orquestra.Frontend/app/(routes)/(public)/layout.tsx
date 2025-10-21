'use client';
import Head from '@/app/(routes)/head';
import { CookieDefault } from '@/app/components/cookie';
import NavbarNotAuth from '@/app/components/navbar/nav-not-auth';
import UpNav from '@/app/components/navbar/up-nav';
import WhatsappButton from '@/app/components/whatsapp/button';
import ROUTES from '@/app/consts/routes';
import SYSTEM from '@/app/consts/system';
import { HANKEN } from '@/app/fonts/fonts';
import useShowNProgressOnPageLoad from '@/app/hooks/useShowNProgressOnPageLoad';
import useStandardIntructions from '@/app/hooks/useStandardInstructions';
import '@/app/styles/globals.scss';
import 'animate.css/animate.min.css';
import feather from 'feather-icons';
import { usePathname } from 'next/navigation';
import 'nprogress/nprogress.css';
import { ReactNode, useEffect } from 'react';
import { Toaster } from 'sonner';
import 'tippy.js/dist/tippy.css';

export default function RootLayout({ children }: Readonly<{ children: ReactNode }>) {

    useStandardIntructions();
    useShowNProgressOnPageLoad();

    const pathname = usePathname();
    const hideHeader = pathname?.includes(ROUTES.LANDING_PAGE);

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

                <header>
                    <NavbarNotAuth />
                </header>

                <main className='no-layout'>
                    {
                        !hideHeader ? (
                            <div className='children'>
                                {children}

                                <WhatsappButton phone={SYSTEM.PHONE_SUPPORT} />
                            </div>
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