import styles from './index.module.scss';

interface iProps {
    text: string;
}

export default function Divider({ text }: iProps) {
    return (
        <div className={styles.divider}>
            <span className={styles.line}></span>
            <span className={styles.text}>{text}</span>
            <span className={styles.line}></span>
        </div>
    )
}