'use client';
import SYSTEM from '@/app/consts/system';
import handleCheckIsProd from '@/app/functions/check.isProd';
import Link from 'next/link';
import { useRef, useState } from 'react';
import styles from './index.module.scss';

interface iProps {
    hideIfProd: boolean;
}

export default function TestEnvironmentBanner({ hideIfProd }: iProps) {

    const [visible, setVisible] = useState<boolean>(true);
    const bannerRef = useRef<HTMLDivElement>(null);
    const posRef = useRef({ x: 0, y: 0, offsetX: 0, offsetY: 0 });

    if (!visible || (hideIfProd && handleCheckIsProd())) {
        return null;
    }

    function handleMouseDown(e: React.MouseEvent<HTMLDivElement>) {
        const rect = bannerRef.current!.getBoundingClientRect();
        posRef.current.offsetX = e.clientX - rect.left;
        posRef.current.offsetY = e.clientY - rect.top;

        window.addEventListener('mousemove', handleMouseMove);
        window.addEventListener('mouseup', handleMouseUp);
    }

    function handleMouseMove(e: MouseEvent) {
        const x = e.clientX - posRef.current.offsetX;
        const y = e.clientY - posRef.current.offsetY;

        if (bannerRef.current) {
            bannerRef.current.style.left = `${x}px`;
            bannerRef.current.style.top = `${y}px`;
        }
    }

    function handleMouseUp() {
        const rect = bannerRef.current!.getBoundingClientRect();

        if (rect.top < 0 || rect.left < 0 || rect.bottom > window.innerHeight || rect.right > window.innerWidth) {
            setVisible(false);
        }

        window.removeEventListener('mousemove', handleMouseMove);
        window.removeEventListener('mouseup', handleMouseUp);
    }

    return (
        <div
            ref={bannerRef}
            className={styles.banner}
            style={{ right: '1rem', top: '1rem', position: 'absolute', cursor: 'grab' }}
            onMouseDown={handleMouseDown}
        >
            <p className={styles.text}>
                Ambiente de teste • Desenvolvido por
                <Link href={SYSTEM.URL_LINKEDIN} target='_blank' rel='noopener noreferrer'>
                    <span>{SYSTEM.AUTHOR}</span>
                </Link>
            </p>
        </div>
    )
}