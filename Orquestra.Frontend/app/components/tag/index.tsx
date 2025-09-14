import handleProperlyCapitalizeStr from '@/app/functions/fix.properlyCapitalizeText';
import { CSSProperties } from 'react';
import styles from './index.module.scss';

interface iParams {
    text: string;
    handleClick?: any | null;
    fixCapitalizeStr?: boolean;
    style?: CSSProperties;
}

export default function Tag({ text, handleClick, fixCapitalizeStr = true, style = {} }: iParams) {
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