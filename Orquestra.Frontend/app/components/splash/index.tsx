import SYSTEM from '@/app/consts/system';
import useDisableScroll from '@/app/hooks/useDisableScroll';
import { useMemo } from 'react';
import Styles from './index.module.scss';

interface iProps {
    text: string;
}

export default function Splash({ text }: iProps) {

    useDisableScroll();
    const letters = useMemo(() => Array.from(text), [text]);

    return (
        <section className={Styles.splash}>
            <div className={`${Styles.loading} ${SYSTEM.ANIMATE}`}>
                {
                    letters.map((letter, i) => (
                        <span
                            key={i}
                            style={{ animationDelay: `${i * 0.1}s` }}
                        >
                            {letter}
                        </span>
                    ))
                }
            </div>
        </section>
    )
}