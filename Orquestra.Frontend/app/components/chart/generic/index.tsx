'use client';
import { Line, LineChart, ResponsiveContainer, Tooltip, XAxis, YAxis } from 'recharts';
import styles from './index.module.scss';

export interface iChartPoint {
    dateTime: Date | string | null | undefined;
    value: number;
}

export interface iChartSerie {
    id: string;
    label: string;
    color?: string;
    object: iChartPoint[];
}

interface iChartGenericProps {
    serie: iChartSerie;
    height?: number;
    showYAxis?: boolean;
}

function handleFormatFullDate(d: Date) {
    const day = String(d.getDate()).padStart(2, '0');
    const month = String(d.getMonth() + 1).padStart(2, '0');
    const year = d.getFullYear();
    const hh = String(d.getHours()).padStart(2, '0');
    const mm = String(d.getMinutes()).padStart(2, '0');
    const ss = String(d.getSeconds()).padStart(2, '0');

    return `${day}/${month}/${year} ${hh}:${mm}:${ss}`;
}

function handleNormalizeData(points: iChartPoint[]) {
    return points.map(p => {
        let d: Date | null = null;

        // Se já é Date;
        if (p.dateTime instanceof Date) {
            d = p.dateTime;
        }

        // Se é string válida;
        else if (typeof p.dateTime === 'string' && p.dateTime.trim() !== '') {
            d = new Date(p.dateTime);
        }

        // Se não virou uma data válida, ignora o ponto;
        if (!d || isNaN(d.getTime())) {
            return null;
        }

        return {
            time: handleFormatFullDate(d),
            value: p.value,
            _d: d
        };
    }).
        filter(Boolean).
        sort((a, b) => a!._d.getTime() - b!._d.getTime());
}

export default function ChartGeneric({ serie, height = 160, showYAxis = true }: iChartGenericProps) {

    const data = handleNormalizeData(serie.object);
    const color = serie.color || 'var(--main-light)';

    return (
        <div className={styles.card}>
            <div className={styles.header}>
                <span className={styles.badge}>I</span>
                <span className={styles.title}>{serie.label.toUpperCase()}</span>
            </div>

            <div className={styles.chartArea} style={{ height }}>
                <ResponsiveContainer>
                    <LineChart data={data} margin={{ top: 4, right: 10, left: -10, bottom: 0 }}>
                        <XAxis
                            dataKey='time'
                            tick={{ fontSize: 10 }}
                            stroke='var(--black)'
                            interval='preserveStartEnd'
                        />

                        {
                            showYAxis && (
                                <YAxis
                                    tick={{ fontSize: 10 }}
                                    stroke='var(--black)'
                                    width={35}
                                />
                            )
                        }

                        <Tooltip
                            labelStyle={{ fontSize: 11 }}
                            itemStyle={{ fontSize: 11 }}
                            formatter={(v: any) => [v, 'Valor']}
                        />

                        <Line
                            type='monotone'
                            dataKey='value'
                            stroke={color}
                            strokeWidth={2}
                            dot={false}
                            activeDot={{ r: 4 }}
                        />
                    </LineChart>
                </ResponsiveContainer>
            </div>
        </div>
    )
}