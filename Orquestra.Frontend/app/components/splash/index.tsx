import Mascot from '@/app/components/mascot';
import SYSTEM from '@/app/consts/system';
import { useEffect, useMemo, useState } from 'react';
import styles from './index.module.scss';

interface iProps {
    text: string;
    destroyAfterSeconds?: number;
    isGradient?: boolean;
    showGif?: boolean;
}

export default function Splash({ text, destroyAfterSeconds, isGradient = false, showGif = false }: iProps) {

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
        <section className={`${styles.splash} notSelectable`} style={{ background: isGradient ? 'var(--gradient)' : 'var(--white)' }}>
            <div className={`${styles.loading} ${SYSTEM.ANIMATE}`}>
                {
                    letters.map((letter, i) => (
                        <span key={i} style={{ animationDelay: `${i * 0.1}s` }}>
                            {letter}
                        </span>
                    ))
                }

                {
                    showGif && (
                        <div className={styles.gif}>
                            <Mascot width={96} flip={true} hasAnimateDelay={false} />
                        </div>
                    )
                }
            </div>
        </section>
    )
}