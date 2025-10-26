'use client';
import { useEffect, useState } from 'react';
import styles from './index.module.scss';

interface iProps {
    text: string | undefined;
    delay?: number;
}

export default function ContentLoaderText({ text, delay = 0 }: iProps) {

    const [isLoaded, setIsLoaded] = useState<boolean>(false);

    useEffect(() => {
        if (text) {
            const timer = setTimeout(() => setIsLoaded(true), delay);
            return () => clearTimeout(timer);
        }
    }, [text, delay]);

    if (!text || !isLoaded) {
        return (
            <span className={styles.main}>
                <span className={styles.skeleton}></span>
            </span>
        )
    }

    return (
        <span className={styles.fadeIn} dangerouslySetInnerHTML={{ __html: text }} />
    )
}