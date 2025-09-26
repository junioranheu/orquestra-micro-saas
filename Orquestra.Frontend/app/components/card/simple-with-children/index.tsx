import { CSSProperties, ReactNode } from 'react';
import styles from './index.module.scss';

export type iProps = {
    children: ReactNode;
    style?: CSSProperties;
}

export default function CardSimpleWithChildren({ children, style }: iProps) {
    return (
        <article className={styles.card} style={style}>
            {children}
        </article>
    )
}