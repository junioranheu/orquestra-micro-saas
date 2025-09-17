'use client';
import useWindowSize from '@/app/hooks/useWindowSize';
import Image, { StaticImageData } from 'next/image';
import { useEffect, useState } from 'react';
import styles from './index.module.scss';

interface iProps {
    images: (string | StaticImageData)[];
    autoSlideInterval?: number;
    mustHideButtonsIfSmallScreen: boolean;
}

export default function Carousel({ images, autoSlideInterval = 5000, mustHideButtonsIfSmallScreen }: iProps) {

    const windowSize = useWindowSize();
    const [current, setCurrent] = useState<number>(0);
    const [mustShowButtons, setMustShowButtons] = useState<boolean>(images?.length > 1);

    useEffect(() => {
        if (mustHideButtonsIfSmallScreen) {
            const isSmall = windowSize.width < 801;
            setMustShowButtons(!isSmall);
        }
    }, [mustHideButtonsIfSmallScreen, windowSize]);

    useEffect(() => {
        const interval = setInterval(nextImage, autoSlideInterval);
        return () => clearInterval(interval);
    }, [images, autoSlideInterval]);

    const nextImage = () => setCurrent((prev) => (prev + 1) % images.length);
    const prevImage = () => setCurrent((prev) => (prev - 1 + images.length) % images.length);

    if (!images || images.length === 0) {
        return null;
    }

    return (
        <div className={styles.carouselContainer}>
            {
                mustShowButtons && <button className={`${styles.prev} ${styles.arrow}`} onClick={prevImage}>‹</button>
            }

            <picture
                key={current}
                className={`${styles.carouselImage} ${styles.fade}`}
                title='Img'
            >
                <Image src={images[current]} alt={`Img ${current + 1}`} priority={true} />
            </picture>

            {
                mustShowButtons && <button className={`${styles.next} ${styles.arrow}`} onClick={nextImage}>›</button>
            }
        </div>
    )
}