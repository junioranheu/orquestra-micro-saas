import CardSimpleWithChildren from '@/app/components/card/simple-with-children';
import { ReactNode } from 'react';
import styles from './index.module.scss';

export type iProps = {
    title: ReactNode;
    subtitle?: ReactNode;
    children: ReactNode;
    hasBorder?: boolean;
}

export default function CardCreamWithChildren({ title, subtitle, children, hasBorder = true }: iProps) {
    return (
        <CardSimpleWithChildren style={{
            backgroundColor: 'var(--cream)',
            padding: '4rem 1rem',
            border: hasBorder ? '1px solid var(--gray-light)' : 'none'
        }}>
            <div className={styles.header}>
                <h2 className={styles.title}>{title}</h2>

                {
                    subtitle && (typeof subtitle === 'string' ? (
                        <p className={styles.subtitle}>{subtitle}</p>
                    ) : (
                        <div className={styles.subtitle}>{subtitle}</div>
                    ))
                }
            </div>

            <div className={styles.content}>
                {children}
            </div>
        </CardSimpleWithChildren>
    )
}