import ImgMaestro from '@/app/assets/gif/maestro.gif';
import Tippy from '@/app/components/tool-tip';
import SYSTEM from '@/app/consts/system';
import Image from 'next/image';
import { CSSProperties, ReactElement, ReactNode, useEffect, useState } from 'react';
import styles from './index.module.scss';

interface iProps {
    width?: number;
    isCentralized?: boolean;
    tippyContent?: string | ReactNode;
    tippyPlacement?: 'top' | 'bottom' | 'left' | 'right';
    flip?: boolean;           // Espelha sempre;
    flipPeriodic?: boolean;   // Espelha periodicamente;
    flipInterval?: number;    // Intervalo da inversão periódica;
    absolutePosition?: 'top-right' | 'top-left' | 'bottom-right' | 'bottom-left';
    hasAnimateDelay?: boolean;
    handleFunction?: ((param?: any) => void) | null;
    elementIfShowMascotIsFalse?: ReactElement;
}

export default function Mascot({
    width = 52,
    isCentralized = true,
    tippyContent,
    tippyPlacement = 'top',
    flip = false,
    flipPeriodic = false,
    flipInterval = 3000,
    absolutePosition,
    hasAnimateDelay = true,
    handleFunction,
    elementIfShowMascotIsFalse
}: iProps) {

    const [flipped, setFlipped] = useState<boolean>(flip);
    const [showMascot, setShowMascot] = useState<boolean>(true);

    useEffect(() => {
        const show = localStorage.getItem(SYSTEM.LOCAL_STORAGE_SHOW_MASCOT);

        if (show === null) {
            localStorage.setItem(SYSTEM.LOCAL_STORAGE_SHOW_MASCOT, 'true');
            setShowMascot(true);
        } else {
            setShowMascot(show === 'true');
        }
    }, []);

    // Controle de flip periódico;
    useEffect(() => {
        if (!flipPeriodic) return;

        const interval = setInterval(() => {
            setFlipped(prev => !prev);
        }, flipInterval);

        return () => clearInterval(interval);
    }, [flipPeriodic, flipInterval]);

    const absoluteStyle: CSSProperties = absolutePosition ? {
        position: 'absolute',
        ...(absolutePosition.includes('top') ? { top: '1rem' } : { bottom: '1rem' }),
        ...(absolutePosition.includes('left') ? { left: '1rem' } : { right: '1rem' }),
        zIndex: 9999
    } : {};

    async function handleClick(e: any) {
        if (handleFunction) {
            handleFunction(e);
        }
    }

    if (!showMascot) {
        if (elementIfShowMascotIsFalse) {
            return elementIfShowMascotIsFalse;
        }

        return null;
    }

    return (
        <Tippy content={tippyContent} placement={tippyPlacement} interactive={true}>
            <div
                className={`${styles.mascot} ${hasAnimateDelay && SYSTEM.ANIMATE_DELAY_1s}`}
                style={{
                    margin: isCentralized ? 'auto' : 'none',
                    transform: flipped ? 'scaleX(-1)' : 'scaleX(1)',
                    transition: 'transform 0.5s ease',
                    cursor: (handleFunction ? 'pointer' : ''),
                    ...absoluteStyle
                }}
                onClick={(e) => handleClick(e)}
            >
                <Image src={ImgMaestro} width={width} height={width} alt='' unoptimized={true} />
            </div>
        </Tippy>
    )
}