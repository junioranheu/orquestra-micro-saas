'use client';
import Image, { StaticImageData } from 'next/image';
import { useState } from 'react';
import styles from './index.module.scss';

interface iProps {
    images: (string | StaticImageData)[];
}

export default function Carousel({ images }: iProps) {

    const [current, setCurrent] = useState<number>(0);

    const nextImage = () => setCurrent((prev) => (prev + 1) % images.length);
    const prevImage = () => setCurrent((prev) => (prev - 1 + images.length) % images.length);

    if (!images || images.length === 0) {
        return null;
    }

    return (
        <div className={styles.carouselContainer}>
            <button className={styles.prev} onClick={prevImage}>‹</button>

            <picture className={styles.carouselImage} title='Img'>
                <Image src={images[current]} alt={`Img ${current + 1}`} priority={true} />
            </picture>

            <button className={styles.next} onClick={nextImage}>›</button>
        </div>
    )
}