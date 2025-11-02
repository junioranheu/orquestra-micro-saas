'use client';
import ContentLoaderText from '@/app/components/content-loader/text';
import SYSTEM from '@/app/consts/system';
import { handleGetRandomGreeting } from '@/app/functions/get.greeting';
import useWindowSize from '@/app/hooks/useWindowSize';
import { useEffect, useState } from 'react';
import styles from './index.module.scss';

export default function UpNav() {

    const [greeting, setGreeting] = useState<string>('');
    const windowSize = useWindowSize();

    useEffect(() => {
        setGreeting(`Bem-vindo ao ${SYSTEM.NAME}! Tenha ${handleGetRandomGreeting({ mustIncludeUmUma: true }).toLocaleLowerCase()} ✨`);
    }, []);

    if (windowSize.width > 0 && windowSize.width < 801) {
        return null;
    }

    return (
        <nav className={styles.nav}>
            <ContentLoaderText text={greeting} />
        </nav>
    )
}