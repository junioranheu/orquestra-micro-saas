import Button from '@/app/components/input/button';
import Image, { StaticImageData } from 'next/image';
import styles from './index.module.scss';

export type iProps = {
    img: StaticImageData;
    title?: string;
    description?: string;
    buttonLabel?: string;
    buttonFunction?: () => void;
}

export default function CardSimple({ img, title, description, buttonLabel, buttonFunction }: iProps) {
    return (
        <article className={styles.card}>
            <div className={styles.inner}>
                <div className={styles.left} aria-hidden>
                    {
                        img && (
                            <Image src={img} alt='' />
                        )
                    }
                </div>

                <div className={styles.right}>
                    <h3 className={styles.title}>{title}</h3>
                    <p className={styles.description}>{description}</p>

                    {
                        buttonLabel && buttonFunction && (
                            <div className={styles.actions}>
                                <Button label={buttonLabel} handleFunction={() => buttonFunction()} />
                            </div>
                        )
                    }
                </div>
            </div>
        </article>
    )
}