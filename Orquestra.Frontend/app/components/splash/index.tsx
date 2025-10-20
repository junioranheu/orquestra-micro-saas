import SYSTEM from '@/app/consts/system';
import useDisableScroll from '@/app/hooks/useDisableScroll';
import { useEffect, useMemo, useState } from 'react';
import Styles from './index.module.scss';

interface iProps {
    text: string;
    destroyAfterSeconds?: number;
}

export default function Splash({ text, destroyAfterSeconds }: iProps) {

    useDisableScroll();

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
        <section className={Styles.splash}>
            <div className={`${Styles.loading} ${SYSTEM.ANIMATE}`}>
                {
                    letters.map((letter, i) => (
                        <span key={i} style={{ animationDelay: `${i * 0.1}s` }}>
                            {letter}
                        </span>
                    ))
                }
            </div>
        </section>
    )
}