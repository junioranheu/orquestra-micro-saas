'use client';
import { CSSProperties } from 'react';
import styles from './index.module.scss';

export interface iKpi {
    title: string;
    description: string;
}

interface iProps {
    kpis: iKpi[];
    style?: CSSProperties;
}

export default function CardKpi({ kpis, style }: iProps) {
    return (
        <div className={styles.kpiContainer} style={style}>
            {
                kpis?.map((kpi, index) => (
                    <div key={index} className={styles.kpiCard}>
                        <h3 className={styles.kpiTitle}>{kpi.title}</h3>
                        <p className={styles.kpiDescription} dangerouslySetInnerHTML={{ __html: kpi.description }} />
                    </div>
                ))
            }
        </div>
    )
} 