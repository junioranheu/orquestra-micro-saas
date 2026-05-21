'use client';
import Head from '@/app/(routes)/head';
import { CookieDefault } from '@/app/components/cookie';
import { INTER } from '@/app/fonts/fonts';
import useCheckAzureServer from '@/app/hooks/api/useCheckAzureServer';
import useStandardIntructions from '@/app/hooks/useStandardInstructions';
import feather from 'feather-icons';
import { Fragment, ReactNode, useEffect } from 'react';

export default function LayoutClient({ children }: Readonly<{ children: ReactNode }>) {

    useStandardIntructions();
    useCheckAzureServer();

    useEffect(() => {
        feather.replace();
    }, [])

    return (
        <Fragment>
            <Head />

            <body className={INTER.className}>
                <main>
                    {children}

                    <CookieDefault extenseButtonDescription={false} />
                </main>
            </body>
        </Fragment>
    )
}