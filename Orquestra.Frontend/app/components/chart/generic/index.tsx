'use client';
import { Fragment, ReactNode } from 'react';
import { Bar, BarChart, Line, LineChart, ResponsiveContainer, Tooltip, XAxis, YAxis } from 'recharts';
import styles from './index.module.scss';

// #region interfaces
interface iChartGenericProps {
    mode: 'line' | 'bar';
    series: iChartSerie[];
    height?: number;
    showYAxis?: boolean;
}

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

export default function ChartGeneric({ mode, series, height = 160, showYAxis = true }: iChartGenericProps) {

    const data = handleMergeSeries(series);

    function handleRenderLineOrBar(mode: 'line' | 'bar', s: iChartSerie) {
        if (mode === 'line') {
            return (
                <Line
                    key={s.id}
                    type='monotone'
                    dataKey={s.id}
                    stroke={s.color || 'var(--main)'}
                    strokeWidth={2}
                    dot={{ r: 3 }}
                    activeDot={{ r: 5 }}
                />
            );
        }

        return (
            <Bar
                key={s.id}
                dataKey={s.id}
                fill={s.color || 'var(--main)'}
                radius={[4, 4, 0, 0]}
            />
        );
    }

    function handleRenderChart(mode: 'line' | 'bar', children: ReactNode, data: any) {
        if (mode === 'line') {
            return <LineChart data={data}>{children}</LineChart>;
        }

        return <BarChart data={data}>{children}</BarChart>;
    }

    return (
        <div className={styles.card}>
            <div className={styles.header}>
                <span className={styles.badge}>
                    {series?.map(x => x.label?.charAt(0)?.toUpperCase()).join(' • ')}
                </span>

                <span className={styles.title}>
                    {series?.map(x => x.label.toUpperCase()).join(', ')}
                </span>
            </div>

            <div className={styles.chartArea} style={{ height }}>
                <ResponsiveContainer>
                    {
                        handleRenderChart(
                            mode,
                            <Fragment>
                                <XAxis
                                    dataKey='time'
                                    tick={{ fontSize: 10 }}
                                    stroke='var(--black)'
                                    interval='preserveStartEnd'
                                    tickFormatter={handleFormatDate}
                                />

                                {showYAxis && (
                                    <YAxis
                                        tick={{ fontSize: 10 }}
                                        stroke='var(--black)'
                                        width={35}
                                    />
                                )}

                                <Tooltip
                                    labelFormatter={handleFormatDate}
                                    labelStyle={{ fontSize: 11 }}
                                    itemStyle={{ fontSize: 11 }}
                                    formatter={(value: any, name: string) => {
                                        const serie = series.find(x => x.id === name);
                                        const label = serie?.label ?? '';
                                        const formatted = `R$ ${Number(value).toFixed(2)}`;
                                        return [formatted, label];
                                    }}
                                />

                                {series.map(s => handleRenderLineOrBar(mode, s))}
                            </Fragment>,
                            data
                        )
                    }
                </ResponsiveContainer>
            </div>
        </div>
    )
}