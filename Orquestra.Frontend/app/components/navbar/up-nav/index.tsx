'use client';
import SYSTEM from '@/app/consts/system';
import { handleGetRandomGreeting } from '@/app/functions/get.greeting';
import useWindowSize from '@/app/hooks/useWindowSize';
import { useEffect, useState } from 'react';
import styles from './index.module.scss';

export default function UpNav() {

    const windowSize = useWindowSize();
    const [showContent, setShowContent] = useState<boolean>(false);
    const [greeting, setGreeting] = useState<string>('');

    useEffect(() => {
        setShowContent(windowSize.width > 801);
        setGreeting(`Tenha ${handleGetRandomGreeting({ mustIncludeUmUma: true }).toLocaleLowerCase()} ✨`);
    }, [windowSize]);

    if (!showContent) {
        return null;
    }

    return (
        <nav className={styles.nav}>
            <span>Bem-vindo ao {SYSTEM.NAME}! {greeting}</span>
        </nav>
    )
}