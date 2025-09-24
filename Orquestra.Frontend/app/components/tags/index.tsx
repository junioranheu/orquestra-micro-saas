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
                tags?.map((tag, index) => (
                    <span
                        key={index}
                        className={styles.tag}
                        style={{ backgroundColor: tag.color || 'var(--main)' }}
                    >
                        {tag.label}
                    </span>
                ))
            }
        </div>
    )
}