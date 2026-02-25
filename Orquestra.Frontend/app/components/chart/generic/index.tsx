'use client';
import { Fragment, ReactNode, useState } from 'react';
import { Bar, BarChart, Line, LineChart, ReferenceLine, ResponsiveContainer, Tooltip, XAxis, YAxis } from 'recharts';
import styles from './index.module.scss';
 
// #region interfaces
type ChartMode = | 'line' | 'bar';
 
interface iChartGenericProps {
    mode: ChartMode;
    series: iChartSerie[];
    height?: number;
    showYAxis?: boolean;
    badgeContent?: string;
    showRedReferenceLine?: boolean;
    legend?: { label: string; color: string }[];
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
 
    // Subtrai 3 horas (3 * 60 minutos * 60 segundos * 1000 milissegundos);
    const offsetMillis = 3 * 60 * 60 * 1000;
 
    // Cria uma nova data compensada;
    const compensatedDate = new Date(date.getTime() - offsetMillis);
 
    // Formata usando as configurações locais (pt-BR);
    const dateBr = compensatedDate.toLocaleString('pt-BR');
 
    return dateBr;
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
 
    // return [...map.values()].sort((a, b) => new Date(a.time).getTime() - new Date(b.time).getTime());
 
    const result = [...map.values()].
        sort((a, b) => new Date(a.time).getTime() - new Date(b.time).getTime()).
        map(row => {
            for (const s of series) {
                if (!(s.id in row)) {
                    row[s.id] = null;
                }
            }
            return row;
        });
 
    return result;
}
// #endregion
 
export default function ChartGeneric({ mode, series, height = 160, showYAxis = true, badgeContent = '', showRedReferenceLine = false, legend }: iChartGenericProps) {
 
    const data = handleMergeSeries(series);
 
    function handleRenderSerie(mode: ChartMode, s: iChartSerie) {
        switch (mode) {
            case 'line':
                return (
                    <Line
                        key={s.id}
                        type='linear'
                        dataKey={s.id}
                        stroke={s.color || 'var(--main)'}
                        strokeWidth={2}
                        dot={{ r: 1.5 }}
                        activeDot={{ r: 5 }}
                        connectNulls={true}
                    />
                );
 
            case 'bar':
                return (
                    <Bar
                        key={s.id}
                        dataKey={s.id}
                        fill={s.color || 'var(--main)'}
                        radius={[4, 4, 0, 0]}
                    />
                );
        }
    }
 
    function handleRenderChart(mode: ChartMode, children: ReactNode, data: any) {
        switch (mode) {
            case 'line':
                return <LineChart data={data}>{children}</LineChart>;
 
            case 'bar':
                return <BarChart data={data}>{children}</BarChart>;
        }
    }
 
    const legendItems = legend?.length ?
        legend.map((l, i) => ({
            id: series[i]?.id,
            label: l.label,
            color: l.color
        })) : series.map(s => ({
            id: s.id,
            label: s.label,
            color: s.color || 'var(--main)'
        }));
 
    const [activeSeries, setActiveSeries] = useState<string[]>(series.map(s => s.id));
 
    function handleToggleSerie(id: string) {
        setActiveSeries(prev => prev.includes(id) ? prev.filter(x => x !== id) : [...prev, id]);
    }
 
    return (
        <div className={styles.card}>
            <div className={styles.header}>
                <span className={styles.badge}>
                    {badgeContent || series?.map(x => x.label?.charAt(0)?.toUpperCase()).join(' • ')}
                </span>
 
                {/* <span className={styles.title}>
                    {series?.map(x => x.label.toUpperCase()).join(', ')}
                </span> */}
            </div>
 
            {
                legendItems && legendItems?.length > 0 && (
                    <ChartLegend items={legendItems} activeSeries={activeSeries} onToggle={handleToggleSerie} />
                )
            }
 
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
 
                                {
                                    showYAxis && (
                                        <YAxis
                                            tick={{ fontSize: 10 }}
                                            stroke='var(--black)'
                                            width={35}
                                        />
                                    )
                                }
 
                                {
                                    showRedReferenceLine && (
                                        <ReferenceLine
                                            y={0}
                                            stroke='var(--red)'
                                            strokeWidth={1}
                                        />
                                    )
                                }
 
                                <Tooltip
                                    labelFormatter={handleFormatDate}
                                    labelStyle={{ fontSize: 11 }}
                                    itemStyle={{ fontSize: 11 }}
                                    formatter={(value: any, name: string) => {
                                        const serie = series.find(x => x.id === name);
                                        const label = serie?.label ?? '';
 
                                        return [value, label];
                                    }}
                                />
 
                                {
                                    series?.map(s => {
                                        if (!activeSeries.includes(s.id)) {
                                            return null;
                                        }
 
                                        return handleRenderSerie(mode, s);
                                    })
                                }
                            </Fragment>,
 
                            data
                        )
                    }
                </ResponsiveContainer>
            </div>
        </div>
    )
}
 
function ChartLegend({
    items, activeSeries, onToggle
}: {
    items: { id: string; label: string; color: string }[];
    activeSeries: string[];
    onToggle: (id: string) => void;
}) {
    return (
        <div className={styles.legend}>
            {
                items?.map(item => {
                    const isActive = activeSeries.includes(item.id);
 
                    return (
                        <div
                            key={item.id}
                            className={styles.legendItem}
                            onClick={() => onToggle(item.id)}
                            style={{
                                opacity: isActive ? 1 : 0.35,
                                cursor: 'pointer',
                                textDecoration: isActive ? 'none' : 'line-through'
                            }}
                        >
                            <div
                                className={styles.legendColor}
                                style={{ background: item.color }}
                            />
                            <span>{item.label}</span>
                        </div>
                    );
                })
            }
        </div>
    )
}