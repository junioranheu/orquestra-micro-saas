'use client';
import { Line, LineChart, ResponsiveContainer, Tooltip, XAxis, YAxis } from 'recharts';
import styles from './index.module.scss';

// #region interfaces
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
    series: iChartSerie[];
    height?: number;
    showYAxis?: boolean;
}
// #endregion

// #region functions
function handleFormatDate(d: string) {
    const date = new Date(d);
    return date.toLocaleString('pt-BR');
}

function handleNormalizeSeriePoints(points: iChartPoint[]) {
    return points.map(p => {
        let d: Date | null = null;

        if (p.dateTime instanceof Date) {
            d = p.dateTime;
        }
        else if (typeof p.dateTime === 'string' && p.dateTime.trim() !== '') {
            d = new Date(p.dateTime);
        }

        if (!d || isNaN(d.getTime())) {
            return null;
        }

        return {
            _d: d,
            time: d.toISOString(),
            value: p.value
        };
    }).filter(Boolean);
}

function handleMergeSeries(series: iChartSerie[]) {
    const map = new Map<string, any>();

    for (const s of series) {
        const normalized = handleNormalizeSeriePoints(s.object);

        for (const p of normalized) {
            const t = p!.time;
            if (!map.has(t)) {
                map.set(t, { time: t });
            }
            map.get(t)[s.id] = p!.value;
        }
    }

    return [...map.values()].sort((a, b) => new Date(a.time).getTime() - new Date(b.time).getTime());
}
// #endregion

export default function ChartGeneric({ series, height = 160, showYAxis = true }: iChartGenericProps) {

    const data = handleMergeSeries(series);

    return (
        <div className={styles.card}>

            <div className={styles.header}>
                <span className={styles.badge}>
                    {series[0]?.label?.charAt(0)?.toUpperCase() ?? ''}
                </span>

                <span className={styles.title}>
                    {series?.map(s => s.label.toUpperCase()).join(' / ')}
                </span>
            </div>

            <div className={styles.chartArea} style={{ height }}>
                <ResponsiveContainer>
                    <LineChart data={data} margin={{ top: 0, right: 5, left: 0, bottom: 0 }}>

                        <XAxis
                            dataKey='time'
                            tick={{ fontSize: 10 }}
                            stroke='var(--black)'
                            interval='preserveStartEnd'
                            tickFormatter={handleFormatDate}
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
                            labelFormatter={handleFormatDate}
                            labelStyle={{ fontSize: 11 }}
                            itemStyle={{ fontSize: 11 }}
                        />

                        {
                            series.map(s => (
                                <Line
                                    key={s.id}
                                    type='monotone'
                                    dataKey={s.id}
                                    stroke={s.color || 'var(--main)'}
                                    strokeWidth={2}
                                    dot={{ r: 3 }}
                                    activeDot={{ r: 5 }}
                                />
                            ))
                        }
                    </LineChart>
                </ResponsiveContainer>
            </div>
        </div>
    )
}