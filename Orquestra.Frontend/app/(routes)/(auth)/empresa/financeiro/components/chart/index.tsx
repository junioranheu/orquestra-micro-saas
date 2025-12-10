'use client';
import { iSalesChartOutput } from '@/app/api/consts/sales';
import ChartGeneric, { iChartSerie } from '@/app/components/chart/generic';
import Mascot from '@/app/components/mascot';
import { Guid } from 'guid-typescript';

interface iProps {
    chart: iSalesChartOutput[];
}

export default function EmpresaFinanceiroChart({ chart }: iProps) {

    if (!chart || !chart.length) {
        return <Mascot
            tippyContent='Carregando...'
            isCentralized={false}
            flip={true}
            flipPeriodic={true}
        />;
    }

    const series: iChartSerie[] = chart?.map(x => ({
        id: Guid.create().toString(),
        label: x.type,
        color: x.color,
        object: x.items.map(item => ({
            dateTime: item.dateTime,
            value: Number(item.value) || 0
        }))
    }));

    return (
        <div style={{ marginBottom: '2rem' }}>
            <ChartGeneric mode='line' series={series} height={350} />
        </div>
    )
}