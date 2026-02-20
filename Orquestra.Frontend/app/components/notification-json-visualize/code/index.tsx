import { iLogNotificationOutput } from '@/app/api/consts/log';
import React from 'react';
import styles from './index.module.scss';

interface iProps {
    notification: iLogNotificationOutput;
    theme: 'dark' | 'inherit';
}

export default function NotificationJsonVisualizeCode({ notification, theme }: iProps) {

    const content = typeof notification.changedFields === 'object'
        ? JSON.stringify(notification.changedFields, null, 2)
        : (notification.changedFields ?? '');

    // Tokeniza a string e converte pra React nodes usando classes do module.scss;
    function handleJsonToReactNodes(json: string) {
        if (!json) {
            return null;
        }

        const tokenRegex = /(\x22(?:\\.|[^\x22\\])*\x22)(?=\s*:)|(\x22(?:\\.|[^\x22\\])*\x22)|(\btrue\b|\bfalse\b|\bnull\b)|(-?\d+(\.\d+)?([eE][+-]?\d+)?)/g;

        const nodes: React.ReactNode[] = [];
        let lastIndex = 0;
        let match: RegExpExecArray | null;
        let keyCounter = 0;

        while ((match = tokenRegex.exec(json)) !== null) {
            const [fullMatch] = match;
            const index = match.index;

            if (index > lastIndex) {
                nodes.push(json.slice(lastIndex, index));
            }

            if (match[1]) {
                nodes.push(
                    <span key={`k-${keyCounter++}`} className={
                        theme === 'dark' ? styles.jsonKeyDark : styles.jsonKeyInherit
                    }>
                        {match[1]}
                    </span>
                );
            } else if (match[2]) {
                nodes.push(
                    <span key={`s-${keyCounter++}`} className={
                        theme === 'dark' ? styles.jsonStringDark : styles.jsonStringInherit
                    }>
                        {match[2]}
                    </span>
                );
            } else if (match[3]) {
                const isNull = match[3] === 'null';

                nodes.push(
                    <span key={`b-${keyCounter++}`}
                        className={
                            isNull
                                ? (theme === 'dark' ? styles.jsonNullDark : styles.jsonNullInherit)
                                : (theme === 'dark' ? styles.jsonBooleanDark : styles.jsonBooleanInherit)
                        }>
                        {match[3]}
                    </span>
                );
            } else if (match[4]) {
                nodes.push(
                    <span key={`n-${keyCounter++}`} className={
                        theme === 'dark' ? styles.jsonNumberDark : styles.jsonNumberInherit
                    }>
                        {match[4]}
                    </span>
                );
            }

            lastIndex = index + fullMatch.length;
        }

        if (lastIndex < json.length) {
            nodes.push(json.slice(lastIndex));
        }

        return nodes;
    }

    const nodes = handleJsonToReactNodes(content);

    return (
        <div className={theme === 'dark' ? styles.jsonBlockDark : styles.jsonBlockInherit}>
            <pre>
                <code>
                    {nodes}
                </code>
            </pre>
        </div>
    )
}