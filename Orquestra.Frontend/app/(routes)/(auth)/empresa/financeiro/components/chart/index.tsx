'use client';
import { iMe } from '@/app/api/consts/auth';
import { CONSTS_SALES, iSalesChartOutput } from '@/app/api/consts/sales';
import toast from '@/app/functions/toast';
import useApiRequestToSetterOnUrlChange from '@/app/hooks/api/useApiRequestToSetterOnUrlChange';
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

    useEffect(() => {
        if (sales?.length) {
            sales?.forEach(element => {
                toast({ content: JSON.stringify(element) });
            });
        }
    }, [sales]);

    return (
        <h1>xd</h1>
    )
}