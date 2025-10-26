import ImgLoading from '@/app/assets/gif/loading-cat.gif';
import Tippy from '@tippyjs/react';
import Image from 'next/image';
import { ReactNode, useEffect, useState } from 'react';
import styles from './index.module.scss';

interface iProps {
    width?: number;
    isCentralized?: boolean;
    tippyContent?: string | ReactNode;
    tippyPlacement?: 'top' | 'bottom' | 'left' | 'right';
    flip?: boolean;           // Espelha sempre;
    flipPeriodic?: boolean;   // Espelha periodicamente;
    flipInterval?: number;    // Intervalo da inversão periódica;
}

export default function LoadingGif({
    width = 64,
    isCentralized = true,
    tippyContent,
    tippyPlacement = 'top',
    flip = false,
    flipPeriodic = false,
    flipInterval = 3000
}: iProps) {
    const [flipped, setFlipped] = useState(flip);

    // Controle de flip periódico;
    useEffect(() => {
        if (!flipPeriodic) return;

        const interval = setInterval(() => {
            setFlipped(prev => !prev);
        }, flipInterval);

        return () => clearInterval(interval);
    }, [flipPeriodic, flipInterval]);

    return (
        <Tippy content={tippyContent} placement={tippyPlacement} interactive={true}>
            <div
                className={styles.loader}
                style={{
                    margin: isCentralized ? 'auto' : 'none',
                    transform: flipped ? 'scaleX(-1)' : 'scaleX(1)',
                    transition: 'transform 0.5s ease'
                }}
            >
                <Image src={ImgLoading} width={width} height={width} alt='' />
            </div>
        </Tippy>
    )
}