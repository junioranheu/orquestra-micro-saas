'use client';
import Head from '@/app/(routes)/head';
import { CookieDefault } from '@/app/components/cookie';
import NavbarNotAuth from '@/app/components/navbar/nav-not-auth';
import UpNav from '@/app/components/navbar/up-nav';
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
            <Head />

            <body className={HANKEN.className}>
                <Toaster expand={false} closeButton={false} />
                <UpNav />

                <main className='no-layout'>
                    <header>
                        <NavbarNotAuth />
                    </header>

                    <div className='children'>
                        {children}
                    </div>

                    <CookieDefault extenseButtonDescription={false} />
                </main>
            </body>
        </html>
    )
}