'use client';
import { ReactNode, useState } from 'react';
import styles from './index.module.scss';

interface TabsProps {
    tabs: string[];
    contents: ReactNode[];
    activeIndexDefault?: number;
    isBig?: boolean;
}

export default function Tabs({ tabs, contents, activeIndexDefault = 0, isBig = false }: TabsProps) {

    const [activeIndex, setActiveIndex] = useState<number>(activeIndexDefault);

    return (
        <div className={styles.container}>
            <div className={`${styles.header} ${isBig && styles.big}`}>
                {
                    tabs.map((tab, index) => (
                        <button
                            key={index}
                            className={`${styles.tab} ${index === activeIndex ? styles.active : ''}`}
                            onClick={() => setActiveIndex(index)}
                            dangerouslySetInnerHTML={{ __html: tab }}
                        />
                    ))
                }
            </div>

            <div className={styles.content}>
                {contents[activeIndex]}
            </div>
        </div>
    )
}