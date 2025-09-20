'use client';
import Head from '@/app/(routes)/head';
import { CookieDefault } from '@/app/components/cookie';
import Icon from '@/app/components/icon';
import UpNav from '@/app/components/navbar/up-nav';
import ROUTES from '@/app/consts/routes';
import { HANKEN } from '@/app/fonts/fonts';
import '@/app/styles/globals.scss';
import Tippy from '@tippyjs/react';
import 'animate.css/animate.min.css';
import feather from 'feather-icons';
import { useRouter } from 'next/navigation';
import { useEffect, useState } from 'react';
import { Toaster } from 'sonner';
import 'tippy.js/dist/tippy.css';

export default function RootLayout({ children, }: { children: React.ReactNode; }) {

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
                    <FloatBackButton />

                    {children}

                    <CookieDefault extenseButtonDescription={false} />
                </main>
            </body>
        </html>
    )
}

export function FloatBackButton() {

    const router = useRouter();
    const [pos, setPos] = useState<{ x: number; y: number }>({ x: 20, y: 20 });

    function handleMouseDown(e: React.MouseEvent) {
        e.preventDefault();

        const shiftX = e.clientX - pos.x;
        const shiftY = e.clientY - pos.y;

        const moveAt = (ev: MouseEvent) => {
            setPos({ x: ev.clientX - shiftX, y: ev.clientY - shiftY });
        };

        const stopMoving = () => {
            document.removeEventListener('mousemove', moveAt);
            document.removeEventListener('mouseup', stopMoving);
        };

        document.addEventListener('mousemove', moveAt);
        document.addEventListener('mouseup', stopMoving);
    }

    return (
        <Tippy content='Clique duas vezes para voltar ao início' placement='right'>
            <span
                className='float-back-button'
                style={{ position: 'fixed', top: pos.y, left: pos.x, }}
                onDoubleClick={() => router.push(ROUTES.DASHBOARD)}
                onMouseDown={handleMouseDown}
            >
                <Icon icon='home' weight='bold' />
            </span>
        </Tippy>
    )
}