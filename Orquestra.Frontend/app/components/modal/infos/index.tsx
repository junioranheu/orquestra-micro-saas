import { CSSProperties } from 'react';
import styles from './index.module.scss';

interface iParams {
    title: string;
    items: {
        title: string;
        content: string | number | undefined;
    }[];
    style?: CSSProperties;
    showHr?: boolean;
}

export default function ModalInfos({ title, items, style = {}, showHr = true }: iParams) {
    return (
        <section className={styles.main} style={style}>
            <span className={styles.title}>{title}</span>

            <div className={styles.items}>
                {
                    items?.map((x, i) => (
                        <div className={styles.item} key={i}>
                            <span className={styles.subtitle}>{x.title}</span>
                            <span className={styles.content}>{x.content ? x.content.toString() : '-'}</span>
                        </div>
                    ))
                }
            </div>

            {
                showHr && <hr className={styles.marginTop} />
            }
        </section>
    )
}