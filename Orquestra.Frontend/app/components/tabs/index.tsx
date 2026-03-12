'use client';
import { ReactNode, useState } from 'react';
import styles from './index.module.scss';

const EMPTY_DISABLED_INDEXES: number[] = [];

interface TabsProps {
    tabs: string[];
    contents: ReactNode[];
    activeIndexDefault?: number;
    isBig?: boolean;
    disabledIndexes?: number[];
}

export default function Tabs({ tabs, contents, activeIndexDefault = 0, isBig = false, disabledIndexes = EMPTY_DISABLED_INDEXES }: TabsProps) {

    function handleGetFirstEnabledIndex(): number {
        const firstEnabledIndex = tabs.findIndex((_, index) => !disabledIndexes.includes(index));
        return firstEnabledIndex === -1 ? 0 : firstEnabledIndex;
    }

    function handleGetSafeIndex(index: number): number {
        const isOutOfBounds = index < 0 || index >= tabs.length || index >= contents.length;

        if (isOutOfBounds || disabledIndexes.includes(index)) {
            return handleGetFirstEnabledIndex();
        }

        return index;
    }

    const [activeIndex, setActiveIndex] = useState<number>(() => handleGetSafeIndex(activeIndexDefault));

    useEffect(() => {
        setActiveIndex(handleGetSafeIndex(activeIndexDefault));
    }, [activeIndexDefault, tabs.length, contents.length, disabledIndexes]);

    function handleClick(index: number) {
        if (disabledIndexes.includes(index)) {
            return;
        }

        if (index < 0 || index >= contents.length) {
            return;
        }

        setActiveIndex(index);
    }

    return (
        <div className={styles.container}>
            <div className={`${styles.header} ${isBig && styles.big}`}>
                {
                    tabs?.map((tab, index) => {
                        const isDisabled = disabledIndexes.includes(index);

                        return (
                            <button
                                key={index}
                                disabled={isDisabled}
                                className={`
                                ${styles.tab}
                                ${index === activeIndex ? styles.active : ''}
                                ${isDisabled ? styles.disabled : ''}
                            `}
                                onClick={() => handleClick(index)}
                                dangerouslySetInnerHTML={{ __html: tab }}
                            />
                        );
                    })
                }
            </div>

            <div className={styles.content}>
                {contents[activeIndex]}
            </div>
        </div>
    )
}
