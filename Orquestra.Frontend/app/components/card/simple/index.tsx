import Button from '@/app/components/input/button';
import Image, { StaticImageData } from 'next/image';
import { CSSProperties } from 'react';
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
    style?: CSSProperties;
    hasCardAltStyle?: boolean;
}

export default function CardSimple({
    img,
    isImgInsideOfCard = true,
    title,
    description,
    buttonLabel,
    buttonDisabled = false,
    buttonFunction,
    className,
    style,
    hasCardAltStyle = false
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

            <article className={`${styles.card} ${(hasCardAltStyle && styles.cardAltStyle)}`} style={style}>
                <div className={styles.inner}>
                    {
                        isImgInsideOfCard && (
                            <div className={styles.left}>
                                {
                                    img && (
                                        <Image src={img} alt='' priority={true} width={125} />
                                    )
                                }
                            </div>
                        )
                    }

                    <div className={`${styles.right} ${className}`}>
                        <h3 className={styles.title} dangerouslySetInnerHTML={{ __html: title ?? '' }} />
                        <p className={styles.description} dangerouslySetInnerHTML={{ __html: description ?? '' }} />

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