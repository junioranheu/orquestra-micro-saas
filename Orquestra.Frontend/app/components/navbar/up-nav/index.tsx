'use client';
import ContentLoaderText from '@/app/components/content-loader/text';
import SYSTEM from '@/app/consts/system';
import { handleGetRandomGreeting } from '@/app/functions/get.greeting';
import { useEffect, useState } from 'react';
import styles from './index.module.scss';

export default function UpNav() {

    const [greeting, setGreeting] = useState<string>('');

    useEffect(() => {
        setGreeting(`Bem-vindo ao ${SYSTEM.NAME}! Tenha ${handleGetRandomGreeting({ mustIncludeUmUma: true }).toLocaleLowerCase()} ✨`);
    }, []);

    return (
        <nav className={styles.nav}>
            <ContentLoaderText text={greeting} />
        </nav>
    )
}