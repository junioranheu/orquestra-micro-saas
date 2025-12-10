'use client';
import SvgSales from '@/app/assets/svg/sales.svg';
import CardSimple from '@/app/components/card/simple';
import TemplatePageHeader from '@/app/components/template/template-page-header';
import ROUTES from '@/app/consts/routes';
import useApiGetMe from '@/app/hooks/api/useApiGetMe';
import useTitle from '@/app/hooks/useTitle';
import EmpresaFinanceiroChart from './components/chart';

export default function EmpresaFinanceiro() {

    useTitle('Gestão financeira');

    const me = useApiGetMe({});

    return (
        <TemplatePageHeader title='Gestão financeira'>
            <CardSimple
                img={SvgSales}
                title={`Finanças ${me?.currentMainCompany?.name ? `• ${me.currentMainCompany.name}` : ''}`}
                description={`Acompanhe as finanças da sua empresa de forma prática e eficiente.<br/>Os dados financeiros são automaticamente atualizados com informações dos módulos <a href='${ROUTES.EMPRESA_AGENDAMENTOS}'>agenda</a>, <a href='${ROUTES.EMPRESA_ORDEM_DE_SERVICO}'>ordens de serviço</a> e <a href='${ROUTES.EMPRESA_ESTOQUE}'>estoque</a>, proporcionando uma visão completa e integrada do seu fluxo de caixa, custos e vendas.`}
                style={{ marginBottom: '2rem' }}
            />

            <EmpresaFinanceiroChart me={me} />
        </TemplatePageHeader>
    )
}

