import handleProperlyCapitalizeStr from '@/app/functions/fix.properlyCapitalizeText';
import { CSSProperties } from 'react';
import styles from './index.module.scss';

interface iProps {
    text: string;
    handleClick?: any | null;
    fixCapitalizeStr?: boolean;
    style?: CSSProperties;
}

export default function Tag({ text, handleClick, fixCapitalizeStr = true, style = {} }: iProps) {
    return (
        <div
            className={styles.tag}
            style={style}
        >
            {
                fixCapitalizeStr ?
                    <span dangerouslySetInnerHTML={{ __html: handleProperlyCapitalizeStr(text) }} /> :
                    <span dangerouslySetInnerHTML={{ __html: text }} />
            }

            {
                handleClick && (
                    <span
                        className={styles.x}
                        onClick={() => handleClick && handleClick()}
                    >
                        ✕
                    </span>
                )
            }
        </div>
    )
}