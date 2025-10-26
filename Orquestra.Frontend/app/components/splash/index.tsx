import LoadingGif from '@/app/components/loading/gif';
import SYSTEM from '@/app/consts/system';
import { PACIFICO } from '@/app/fonts/fonts';
import { useEffect, useMemo, useState } from 'react';
import styles from './index.module.scss';

interface iProps {
    text: string;
    destroyAfterSeconds?: number;
    isGradient?: boolean;
}

export default function Splash({ text, destroyAfterSeconds, isGradient = false }: iProps) {

    const letters = useMemo(() => Array.from(text), [text]);
    const [visible, setVisible] = useState<boolean>(true);

    useEffect(() => {
        if (destroyAfterSeconds) {
            const timer = setTimeout(() => setVisible(false), destroyAfterSeconds * 1000);
            return () => clearTimeout(timer);
        }
    }, [destroyAfterSeconds]);

    if (!visible) {
        return null;
    }

    return (
        <section className={`${styles.splash} ${PACIFICO.className} notSelectable`} style={{ background: isGradient ? 'var(--gradient)' : 'var(--white)' }}>
            <div className={`${styles.loading} ${SYSTEM.ANIMATE}`}>
                {
                    letters.map((letter, i) => (
                        <span key={i} style={{ animationDelay: `${i * 0.1}s` }}>
                            {letter}
                        </span>
                    ))
                }

                <div className={`${styles.gif}`}>
                    <LoadingGif width={96} />
                </div>
            </div>
        </section>
    )
}