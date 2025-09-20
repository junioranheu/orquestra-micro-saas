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
import { useEffect, useRef, useState } from 'react';
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
    const buttonRef = useRef<HTMLSpanElement>(null);

    const localStorageItemName = 'floatBackButtonPos';
    const [pos, setPos] = useState({ x: 20, y: 20 }); // Valor inicial seguro;

    useEffect(() => {
        if (typeof window !== 'undefined') {
            const saved = localStorage.getItem(localStorageItemName);

            if (saved) {
                setPos(JSON.parse(saved)); // Atualiza a posição do botão;
            }
        }
    }, []);

    useEffect(() => {
        if (buttonRef.current) {
            buttonRef.current.style.left = pos.x + 'px';
            buttonRef.current.style.top = pos.y + 'px';
        }
    }, [pos]);

    function handleMouseDown(e: React.MouseEvent<HTMLSpanElement, MouseEvent>) {
        e.preventDefault();

        const shiftX = e.clientX - pos.x;
        const shiftY = e.clientY - pos.y;

        const moveAt = (ev: MouseEvent) => {
            if (buttonRef.current) {
                buttonRef.current.style.left = ev.clientX - shiftX + 'px';
                buttonRef.current.style.top = ev.clientY - shiftY + 'px';
            }
        };

        const stopMoving = (ev: MouseEvent) => {
            const newPos = { x: ev.clientX - shiftX, y: ev.clientY - shiftY };
            setPos(newPos); // Atualiza o state;
            localStorage.setItem(localStorageItemName, JSON.stringify(newPos)); // Salva no localStorage;

            document.removeEventListener('mousemove', moveAt);
            document.removeEventListener('mouseup', stopMoving);
        };

        document.addEventListener('mousemove', moveAt);
        document.addEventListener('mouseup', stopMoving);
    }

    return (
        <Tippy content='Clique duas vezes para voltar ao início' placement='right'>
            <span
                ref={buttonRef}
                className='float-back-button'
                style={{ position: 'fixed', top: pos.y, left: pos.x }}
                onMouseDown={handleMouseDown}
                onDoubleClick={() => router.push(ROUTES.DASHBOARD)}
            >
                <Icon icon='home' weight='bold' />
            </span>
        </Tippy>
    )
}