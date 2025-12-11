'use client';
import { iMe } from '@/app/api/consts/auth';
import { iSalesChartOutput } from '@/app/api/consts/sales';
import ChartGeneric, { iChartSerie } from '@/app/components/chart/generic';

interface iProps {
    me: iMe | undefined;
    chart: iSalesChartOutput[];
}

export default function EmpresaFinanceiroChart({ me, chart }: iProps) {
    if (!chart || !chart.length) {
        return null;
    }

    const series: iChartSerie[] = chart?.map(x => ({
        id: x.type,
        label: x.type,
        color: x.color,
        object: x.items.map(item => ({
            dateTime: item.dateTime,
            value: Number(item.value) || 0
        }))
    }));

    return (
        <div style={{ marginBottom: '2rem' }}>
            <ChartGeneric
                mode='line'
                series={series}
                height={350}
                badgeContent={me?.currentMainCompany?.name ?? ''}
                showRedReferenceLine={true}
                legend={[
                    { label: 'Entrada', color: 'var(--main)' },
                    { label: 'Saída', color: 'var(--contrast)' },
                    { label: 'Linha base', color: 'var(--red)' }
                ]}
            />
        </div>
    )
}