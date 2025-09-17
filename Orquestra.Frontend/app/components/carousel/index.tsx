'use client';
import useWindowSize from '@/app/hooks/useWindowSize';
import Image, { StaticImageData } from 'next/image';
import { useEffect, useState } from 'react';
import styles from './index.module.scss';

interface iProps {
    images: (string | StaticImageData)[];
    mustHideButtonsIfSmallScreen: boolean;
}

export default function Carousel({ images, mustHideButtonsIfSmallScreen }: iProps) {

    const windowSize = useWindowSize();
    const [current, setCurrent] = useState<number>(0);
    const [mustShowButtons, setMustShowButtons] = useState<boolean>(images?.length > 1);

    useEffect(() => {
        if (mustHideButtonsIfSmallScreen) {
            const isSmall = windowSize.width < 801;
            setMustShowButtons(!isSmall);
        }
    }, [mustHideButtonsIfSmallScreen, windowSize]);

    const nextImage = () => setCurrent((prev) => (prev + 1) % images.length);
    const prevImage = () => setCurrent((prev) => (prev - 1 + images.length) % images.length);

    if (!images || images.length === 0) {
        return null;
    }

    return (
        <div className={styles.carouselContainer}>
            {
                mustShowButtons && <button className={styles.prev} onClick={prevImage}>‹</button>
            }

            <picture className={styles.carouselImage} title='Img'>
                <Image src={images[current]} alt={`Img ${current + 1}`} priority={true} />
            </picture>

            {
                mustShowButtons && <button className={styles.next} onClick={nextImage}>›</button>
            }
        </div>
    )
}