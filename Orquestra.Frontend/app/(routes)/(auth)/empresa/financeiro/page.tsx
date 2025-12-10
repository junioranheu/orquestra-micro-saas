'use client';
import { CONSTS_SALES, iSalesOutput, iSalesTableOutput } from '@/app/api/consts/sales';
import SvgSales from '@/app/assets/svg/sales.svg';
import CardSimple from '@/app/components/card/simple';
import TableGeneric, { iTableColumn } from '@/app/components/table/generic';
import TemplatePageHeader from '@/app/components/template/template-page-header';
import ROUTES from '@/app/consts/routes';
import toast from '@/app/functions/toast';
import useApiGetMe from '@/app/hooks/api/useApiGetMe';
import useApiRequestToSetterOnUrlChange from '@/app/hooks/api/useApiRequestToSetterOnUrlChange';
import useTitle from '@/app/hooks/useTitle';
import { useRouter } from 'next/navigation';
import { useEffect, useState } from 'react';
import EmpresaFinanceiroChart from './components/chart';

export default function EmpresaFinanceiro() {

    useTitle('Gestão financeira');

    const me = useApiGetMe({});
    const router = useRouter();

    const [currentPage, setCurrentPage] = useState<number>(1);
    const [sales, setSales] = useState<iSalesOutput | undefined>(undefined);
    const [apiUrlRequest, setApiUrlRequest] = useState<string>(CONSTS_SALES.getChart);
    useApiRequestToSetterOnUrlChange<iSalesOutput>({ apiUrlRequest: apiUrlRequest, setter: setSales, hasPaginationInput: true, index: currentPage });

    useEffect(() => {
        if (me && me?.currentMainCompany?.companyId) {
            setApiUrlRequest(`${CONSTS_SALES.getChart}?companyId=${me?.currentMainCompany?.companyId}`);
        }
    }, [me]);

    const columns = [
        {
            title: 'Tipo',
            dataIndex: 'type',
            key: 'type',
            width: '12rem',
            render: (value: string, record: iSalesTableOutput) => {
                const handleClick = () => {
                    switch (record.type.toLowerCase()) {
                        case 'agenda':
                            router.push(ROUTES.EMPRESA_AGENDAMENTOS);
                            break;
                        case 'estoque':
                            router.push(ROUTES.EMPRESA_ESTOQUE);
                            break;
                        case 'ordem de serviço':
                            router.push(ROUTES.EMPRESA_ORDEM_DE_SERVICO);
                            break;
                        default:
                            toast({ content: 'Navegação não disponível para este tipo de registro.' });
                            break;
                    }
                };

                return (
                    <span
                        onClick={handleClick}
                        style={{
                            cursor: 'pointer',
                            fontWeight: 500,
                            textDecoration: 'underline dashed var(--contrast)',
                            textUnderlineOffset: '4px'
                        }}
                    >
                        {value}
                    </span>
                );
            }
        },
        {
            title: 'Título',
            dataIndex: 'title',
            key: 'title',
            width: '16rem'
        },
        {
            title: 'Valor',
            dataIndex: 'value',
            key: 'value',
            render: (value: number) => `R$ ${value.toFixed(2)}`
        },
        {
            title: 'Data',
            dataIndex: 'date',
            key: 'date',
            render: (value?: string | null) => value ? new Date(value).toLocaleDateString('pt-BR') : '-'
        }
    ] as iTableColumn[];

    return (
        <TemplatePageHeader title='Gestão financeira'>
            <CardSimple
                img={SvgSales}
                title={`Finanças ${me?.currentMainCompany?.name ? `• ${me.currentMainCompany.name}` : ''}`}
                description={`Acompanhe as finanças da sua empresa de forma prática e eficiente.<br/>Os dados financeiros são automaticamente atualizados com informações dos módulos <a href='${ROUTES.EMPRESA_AGENDAMENTOS}'>agenda</a>, <a href='${ROUTES.EMPRESA_ORDEM_DE_SERVICO}'>ordens de serviço</a> e <a href='${ROUTES.EMPRESA_ESTOQUE}'>estoque</a>, proporcionando uma visão completa e integrada do seu fluxo de caixa, custos e vendas.`}
                style={{ marginBottom: '2rem' }}
            />

            <EmpresaFinanceiroChart
                chart={sales?.chart ?? []}
            />

            <TableGeneric
                idPropName='id'
                columns={columns}
                data={sales?.table ?? []}
                currentPage={currentPage}
                setCurrentPage={setCurrentPage}
                totalRowsCount={sales?.tableTotalCount}
            />
        </TemplatePageHeader>
    )
}

