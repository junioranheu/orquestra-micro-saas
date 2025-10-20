import SYSTEM from '@/app/consts/system';
import { handleGetRandomGreeting } from '@/app/functions/get.greeting';
import useWindowSize from '@/app/hooks/useWindowSize';
import { useEffect, useMemo, useState } from 'react';
import styles from './index.module.scss';

export default function UpNav() {

    const windowSize = useWindowSize();
    const [showContent, setShowContent] = useState<boolean>(true);

    useEffect(() => {
        setShowContent(windowSize.width > 801);
    }, [windowSize]);

    const greeting = useMemo(() => {
        return `Tenha ${handleGetRandomGreeting({ mustIncludeUmUma: true }).toLocaleLowerCase()} ✨`;
    }, []);

    return (
        <nav className={styles.nav}>
            {
                showContent && (
                    <span>Bem-vindo ao {SYSTEM.NAME}! {greeting}</span>
                )
            }
        </nav>
    )
}