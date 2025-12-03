'use client';
import { CONSTS_CLIENT_FOLLOW_UP, iClientFollowUp, iClientFollowUpPaginated } from '@/app/api/consts/client-follow-up';
import SvgFollowUp from '@/app/assets/svg/follow-up.svg';
import CardSimple from '@/app/components/card/simple';
import Icon from '@/app/components/icon';
import TableGeneric, { iTableColumn, iTableManagingOptions } from '@/app/components/table/generic';
import TemplatePageHeader from '@/app/components/template/template-page-header';
import ROUTES from '@/app/consts/routes';
import useApiGetEnum from '@/app/hooks/api/useApiGetEnum';
import useApiGetMe from '@/app/hooks/api/useApiGetMe';
import useApiRequestToSetterOnUrlChange from '@/app/hooks/api/useApiRequestToSetterOnUrlChange';
import useTitle from '@/app/hooks/useTitle';
import Tippy from '@tippyjs/react';
import { useRouter } from 'next/navigation';
import { useEffect, useState } from 'react';

export default function EmpresaAcompanhamento() {

    useTitle('Acompanhamentos');

    const me = useApiGetMe({});
    const router = useRouter();
    const clientFollowUpStatusEnum = useApiGetEnum({ enumName: 'ClientFollowUpStatusEnum' });

    const [currentPage, setCurrentPage] = useState<number>(1);
    const [clientsFollowUps, setClientsFollowUps] = useState<iClientFollowUpPaginated>();

    const [apiUrlRequest, setApiUrlRequest] = useState<string>(CONSTS_CLIENT_FOLLOW_UP.getAllByCompanyId);
    useApiRequestToSetterOnUrlChange<iClientFollowUpPaginated>({ apiUrlRequest: apiUrlRequest, setter: setClientsFollowUps, hasPaginationInput: true, index: currentPage, limit: 15 });

    useEffect(() => {
        if (me && me?.currentMainCompany?.companyId) {
            setApiUrlRequest(`${CONSTS_CLIENT_FOLLOW_UP.getAllByCompanyId}?companyId=${me?.currentMainCompany?.companyId}`);
        }
    }, [me]);

    const columns = [
        {
            title: 'Cliente',
            dataIndex: 'client.fullName',
            key: 'client.fullName',
            width: '12rem',
            render: (value: string, record: iClientFollowUp) => (
                <Tippy content={`Visualizar detalhes do cliente ${record.client?.fullName}`} placement='right'>
                    <span
                        onClick={() => router.push(`${ROUTES.EMPRESA_CLIENTES}/${record.clientId}`)}
                        style={{
                            cursor: 'pointer',
                            fontWeight: 500,
                            textDecoration: 'underline dashed var(--contrast)',
                            textUnderlineOffset: '4px'
                        }}
                    >
                        {value}
                    </span>
                </Tippy>
            )
        },
        {
            title: 'Observação',
            dataIndex: 'observation',
            key: 'observation',
            width: '32rem'
        },
        {
            title: 'Status',
            dataIndex: 'clientFollowUpStatus',
            key: 'clientFollowUpStatus',
            render: (value: number) => clientFollowUpStatusEnum?.find(x => x.value === value)?.label ?? '-'
        },
        {
            title: 'Criado em',
            dataIndex: 'createdDate',
            key: 'createdDate',
            render: (value?: Date) => value ? new Date(value).toLocaleDateString('pt-BR') : '-'
        }
    ] as iTableColumn[];

    const managingOptions = [
        {
            label: 'Visualizar acompanhamentos',
            function: (e) => router.push(`${ROUTES.EMPRESA_CLIENTES}/${e.clientId}`),
            icon: <Icon icon='search' />
        }
    ] as iTableManagingOptions[];

    return (
        <TemplatePageHeader title='Acompanhamentos registrados'>
            <CardSimple
                img={SvgFollowUp}
                title='Acompanhamentos'
                description='Crie e gerencie acompanhamentos personalizados para cada cliente.<br/>Acesse a tela de clientes e selecione um deles para visualizar o detalhamento e cadastrar novos acompanhamentos.'
                buttonLabel='Ir para clientes'
                buttonFunction={() => router.push(ROUTES.EMPRESA_CLIENTES)}
                style={{ marginBottom: '2rem' }}
            />

            <TableGeneric
                idPropName='clientId'
                columns={columns}
                data={clientsFollowUps?.output ?? []}
                currentPage={currentPage}
                setCurrentPage={setCurrentPage}
                totalRowsCount={clientsFollowUps?.count}
                managingOptions={managingOptions}
            />
        </TemplatePageHeader>
    )
}
