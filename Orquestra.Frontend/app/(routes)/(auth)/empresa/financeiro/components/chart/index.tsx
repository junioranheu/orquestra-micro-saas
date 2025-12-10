'use client';
import { iMe } from '@/app/api/consts/auth';
import { CONSTS_SALES, iSalesChartOutput } from '@/app/api/consts/sales';
import ChartGeneric, { iChartSerie } from '@/app/components/chart/generic';
import Mascot from '@/app/components/mascot';
import useApiRequestToSetterOnUrlChange from '@/app/hooks/api/useApiRequestToSetterOnUrlChange';
import { Guid } from 'guid-typescript';
import { useEffect, useState } from 'react';

interface iProps {
    me: iMe | undefined;
}

export default function EmpresaFinanceiroChart({ me }: iProps) {

    const [sales, setSales] = useState<iSalesChartOutput[] | undefined>([]);
    const [apiUrlRequest, setApiUrlRequest] = useState<string>(CONSTS_SALES.getChart);
    useApiRequestToSetterOnUrlChange<iSalesChartOutput[]>({ apiUrlRequest: apiUrlRequest, setter: setSales });

    useEffect(() => {
        if (me && me?.currentMainCompany?.companyId) {
            setApiUrlRequest(`${CONSTS_SALES.getChart}?companyId=${me?.currentMainCompany?.companyId}`);
        }
    }, [me]);

    if (!sales || !sales.length) {
        return <Mascot
            tippyContent='Carregando...'
            isCentralized={false}
            flip={true}
            flipPeriodic={true}
        />;
    }

    const series: iChartSerie[] = sales.map(x => ({
        id: Guid.create().toString(),
        label: x.type,
        color: x.color,
        object: x.items.map(item => ({
            dateTime: item.dateTime,
            value: Number(item.value) || 0
        }))
    }));

    return (
        <div>
            <ChartGeneric mode='line' series={series} height={350} />
        </div>
    )
}