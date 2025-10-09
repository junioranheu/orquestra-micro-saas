import Tippy from '@tippyjs/react';
import { CSSProperties } from 'react';
import styles from './index.module.scss';

interface iProps {
    text: string;
    handleRemoveFunction?: ((param?: any) => void) | null;
    fixCapitalizeStr?: boolean;
    style?: CSSProperties;
    tippyContent?: string;
}

export default function Tag({ text, handleRemoveFunction, fixCapitalizeStr = true, style = {}, tippyContent }: iProps) {

    function handleCapitalizeAndSeparateText(input: string, capitalizeAll: boolean = true): string {
        if (!input) {
            return '';
        }

        input = input.replace(/_/g, ' ');

        // Separar em antes e depois do ':';
        const [beforeColon, ...afterColonParts] = input.split(':');

        // Se o beforeColon for todo maiúsculo, não altera;
        if (beforeColon === beforeColon.toUpperCase()) {
            return afterColonParts.length > 0 ? `${beforeColon}: ${afterColonParts.join(':').trim()}` : beforeColon;
        }

        const words = beforeColon.trim().split(/(?=[A-Z])| /);
        let transformedBeforeColon: string;

        if (capitalizeAll) {
            // Capitalizar todas as palavras;
            transformedBeforeColon = words.map(word => word.charAt(0).toUpperCase() + word.slice(1).toLowerCase()).join(' ');
        } else {
            // Capitalizar apenas a primeira palavra;
            transformedBeforeColon = words.map((word, index) => index === 0 ? word.charAt(0).toUpperCase() + word.slice(1).toLowerCase() : word.toLowerCase()).join(' ');
        }

        if (afterColonParts.length > 0) {
            return `${transformedBeforeColon}: ${afterColonParts.join(':').trim()}`;
        }

        return transformedBeforeColon;
    }

    return (
        <div
            className={styles.tag}
            style={style}
        >
            {
                fixCapitalizeStr ?
                    <span dangerouslySetInnerHTML={{ __html: handleCapitalizeAndSeparateText(text, false) }} /> :
                    <span dangerouslySetInnerHTML={{ __html: text }} />
            }

            {
                handleRemoveFunction && (
                    <Tippy content={tippyContent ?? 'Remover'}>
                        <span
                            className={styles.x}
                            onClick={() => handleRemoveFunction && handleRemoveFunction()}
                        >
                            ✕
                        </span>
                    </Tippy>
                )
            }
        </div>
    )
}