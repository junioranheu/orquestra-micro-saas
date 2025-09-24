import styles from './index.module.scss';

interface iTagItem {
    label: string;
    color?: string;
}

interface iProps {
    tags: iTagItem[];
}

export default function Tags({ tags }: iProps) {
    return (
        <div className={styles.tags}>
            {
                tags?.filter(x => x && x.label?.trim()).map((x, index) => (
                    <span
                        key={index}
                        className={styles.tag}
                        style={{ backgroundColor: x.color || 'var(--main)' }}
                    >
                        {x.label}
                    </span>
                ))
            }
        </div>
    )
}