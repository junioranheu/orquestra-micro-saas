import Button from '@/app/components/input/button';
import Image, { StaticImageData } from 'next/image';
import styles from './index.module.scss';

export type iProps = {
    img?: StaticImageData;
    isImgInsideOfCard?: boolean;
    title?: string;
    description?: string;
    buttonLabel?: string;
    buttonDisabled?: boolean;
    buttonFunction?: () => void;
    className?: string;
}

export default function CardSimple({
    img,
    isImgInsideOfCard = true,
    title,
    description,
    buttonLabel,
    buttonDisabled = false,
    buttonFunction,
    className
}: iProps) {
    return (
        <div className={styles.wrapper}>
            {
                !isImgInsideOfCard && (
                    <div className={styles.imgOutside}>
                        {
                            img && (
                                <Image src={img} alt='' priority={true} />
                            )
                        }
                    </div>
                )
            }

            <article className={styles.card}>
                <div className={styles.inner}>
                    {
                        isImgInsideOfCard && (
                            <div className={styles.left}>
                                {
                                    img && (
                                        <Image src={img} alt='' priority={true} />
                                    )
                                }
                            </div>
                        )
                    }

                    <div className={`${styles.right} ${className}`}>
                        <h3 className={styles.title}>{title}</h3>
                        <p className={styles.description}>{description}</p>

                        {
                            buttonLabel && buttonFunction && (
                                <div className={styles.actions}>
                                    <Button
                                        label={buttonLabel}
                                        handleFunction={() => !buttonDisabled && buttonFunction()}
                                        isDisabled={buttonDisabled}
                                    />
                                </div>
                            )
                        }
                    </div>
                </div>
            </article>
        </div>
    )
}