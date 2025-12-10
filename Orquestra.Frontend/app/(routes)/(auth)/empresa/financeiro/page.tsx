'use client';
import { CONSTS_SALES, iSalesChartOutput } from '@/app/api/consts/sales';
import useApiGetMe from '@/app/hooks/api/useApiGetMe';
import useApiRequestToSetterOnUrlChange from '@/app/hooks/api/useApiRequestToSetterOnUrlChange';
import useTitle from '@/app/hooks/useTitle';
import { useEffect, useState } from 'react';
import styles from './page.module.scss';

export default function EmpresaFinanceiro() {

    useTitle('Gestão financeira');

    const me = useApiGetMe({});

    const [sales, setSales] = useState<iSalesChartOutput[] | undefined>([]);
    const [apiUrlRequest, setApiUrlRequest] = useState<string>(CONSTS_SALES.getChart);
    useApiRequestToSetterOnUrlChange<iSalesChartOutput[]>({ apiUrlRequest: apiUrlRequest, setter: setSales });

    useEffect(() => {
        if (me && me?.currentMainCompany?.companyId) {
            setApiUrlRequest(`${CONSTS_SALES.getChart}?companyId=${me?.currentMainCompany?.companyId}`);
        }
    }, [me]);

    useEffect(() => {
        console.clear();
        console.log(sales);
    }, [sales]);

    return (
        <section className={styles.main}>
            <h1>Olá... Financeiro</h1>
        </section>
    )
}