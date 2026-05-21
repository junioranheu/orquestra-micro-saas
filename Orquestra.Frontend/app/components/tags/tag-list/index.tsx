import Tippy from '@/app/components/tool-tip';
import styles from './index.module.scss';

interface iTagItem {
    label: string;
    color?: string;
    title?: string;
    handleFunction?: ((param?: any) => void) | null;
}

interface iProps {
    tags: iTagItem[];
}

export default function TagList({ tags }: iProps) {
    return (
        <div className={styles.tags}>
            {
                tags?.filter(x => typeof x?.label === 'string' && x.label.trim())?.map((x, index) => (
                    x.title ? (
                        <Tippy key={index} content={x.title}>
                            <span
                                className={styles.tag}
                                style={{ backgroundColor: x.color || 'var(--main)', cursor: x.handleFunction ? 'pointer' : 'default' }}
                                onClick={() => x.handleFunction && x.handleFunction()}
                            >
                                {x.label}
                            </span>
                        </Tippy>
                    ) : (
                        <span
                            key={index}
                            className={styles.tag}
                            style={{ backgroundColor: x.color || 'var(--main)', cursor: x.handleFunction ? 'pointer' : 'default' }}
                            onClick={() => x.handleFunction && x.handleFunction()}
                        >
                            {x.label}
                        </span>
                    )
                ))
            }
        </div>

    )
}