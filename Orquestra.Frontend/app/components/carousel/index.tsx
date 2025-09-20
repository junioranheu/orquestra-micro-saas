'use client';
import SYSTEM from '@/app/consts/system';
import handleShuffleArray from '@/app/functions/shuffle.array';
import useWindowSize from '@/app/hooks/useWindowSize';
import Image, { StaticImageData } from 'next/image';
import { useCallback, useEffect, useState } from 'react';
import styles from './index.module.scss';

interface iProps {
    items: iItemProps[];
    autoSlideInterval?: number;
    mustShuffle: boolean;
    mustHideButtonsIfSmallScreen: boolean;
    alignCaptionToRight: boolean;
}

interface iItemProps {
    image: (string | StaticImageData);
    caption?: string;
}

export default function Carousel({ items, autoSlideInterval = 5000, mustShuffle, mustHideButtonsIfSmallScreen, alignCaptionToRight }: iProps) {

    const windowSize = useWindowSize();
    const [current, setCurrent] = useState<number>(0);
    const [mustShowButtons, setMustShowButtons] = useState<boolean>(items?.length > 1);

    const [shuffledItems, setShuffledItems] = useState<null | iItemProps[]>(null);

    useEffect(() => {
        if (mustShuffle && !shuffledItems?.length) {
            setShuffledItems(handleShuffleArray(items));
        } else if (!mustShuffle && !shuffledItems?.length) {
            setShuffledItems(items);
        }
    }, [items, mustShuffle, shuffledItems?.length]);

    useEffect(() => {
        if (mustHideButtonsIfSmallScreen) {
            const isSmall = windowSize.width < 801;
            setMustShowButtons(!isSmall);
        }
    }, [mustHideButtonsIfSmallScreen, windowSize]);

    const nextImage = useCallback(() => {
        setCurrent((prev) => (prev + 1) % items.length);
    }, [items.length]);

    const prevImage = useCallback(() => {
        setCurrent((prev) => (prev - 1 + items.length) % items.length);
    }, [items.length]);

    useEffect(() => {
        const interval = setInterval(nextImage, autoSlideInterval);
        return () => clearInterval(interval);
    }, [autoSlideInterval, nextImage]);

    if (!items || items?.length === 0) {
        return null;
    }

    return (
        <div className={`${styles.carouselContainer} ${SYSTEM.ANIMATE_FADE_IN_RIGHT_FAST}`}>
            {
                mustShowButtons && <button className={`${styles.prev} ${styles.arrow}`} onClick={prevImage}>‹</button>
            }

            <div>
                {
                    shuffledItems?.map((item, index) => (
                        <picture
                            key={index}
                            className={`${styles.carouselImage} ${current === index ? styles.active : ''}`}
                        >
                            <Image src={item.image} alt={`Img ${index + 1}`} priority={true} />

                            {
                                item.caption && item.caption?.length && (
                                    <div className={`${styles.caption} ${(alignCaptionToRight ? styles.right : styles.left)}`}>
                                        {item.caption}
                                    </div>
                                )
                            }
                        </picture>
                    ))
                }
            </div>

            {
                mustShowButtons && <button className={`${styles.next} ${styles.arrow}`} onClick={nextImage}>›</button>
            }
        </div>
    )
}