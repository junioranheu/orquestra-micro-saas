import CardSimpleWithChildren from '@/app/components/card/simple-with-children';
import { ReactNode } from 'react';
import styles from './index.module.scss';

export type iProps = {
    title: ReactNode;
    subtitle?: ReactNode;
    children: ReactNode;
}

export default function CardCreamWithChildren({ title, subtitle, children }: iProps) {
    return (
        <CardSimpleWithChildren style={{ backgroundColor: 'var(--cream)', padding: '4rem 1rem', border: 'none' }}>
            <div className={styles.header}>
                <h2 className={styles.title}>{title}</h2>
                {subtitle && <p className={styles.subtitle}>{subtitle}</p>}
            </div>

            <div className={styles.content}>
                {children}
            </div>
        </CardSimpleWithChildren>
    )
}