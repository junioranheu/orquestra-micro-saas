'use client';
import { CONSTS_CLIENT, iClient, iClientPaginated } from '@/app/api/consts/client';
import { CONSTS_QUOTE, iQuote, iQuoteItem, iQuotePaginated } from '@/app/api/consts/quote';
import Icon from '@/app/components/icon';
import Button from '@/app/components/input/button';
import TableGeneric, { iTableColumn, iTableManagingOptions } from '@/app/components/table/generic';
import TemplatePageHeader from '@/app/components/template/template-page-header';
import ROUTES from '@/app/consts/routes';
import useApiGetMe from '@/app/hooks/api/useApiGetMe';
import useApiRequestToSetterOnUrlChange from '@/app/hooks/api/useApiRequestToSetterOnUrlChange';
import useTitle from '@/app/hooks/useTitle';
import { useRouter } from 'next/navigation';
import { Fragment, useEffect, useState } from 'react';
import EmpresaQuotesModalView from './modal/view';

export default function EmpresaOrcamento() {

    useTitle('Orçamentos');

    const me = useApiGetMe({});
    const router = useRouter();

    const [currentPage, setCurrentPage] = useState<number>(1);
    const [quotes, setQuotes] = useState<iQuotePaginated>();
    const [trigger, setTrigger] = useState<Date>(new Date());
    const [apiUrlRequest, setApiUrlRequest] = useState<string>(CONSTS_QUOTE.getAllByCompanyId);
    useApiRequestToSetterOnUrlChange<iQuotePaginated>({ apiUrlRequest: apiUrlRequest, setter: setQuotes, hasPaginationInput: true, index: currentPage, limit: 15, trigger: trigger });

    const [clients, setClients] = useState<iClientPaginated>();
    const [apiUrlRequestClients, setApiUrlRequestClients] = useState<string>(CONSTS_CLIENT.getAllByCompanyId);
    useApiRequestToSetterOnUrlChange<iClientPaginated>({ apiUrlRequest: apiUrlRequestClients, setter: setClients, isSelectAll: true });

    useEffect(() => {
        if (me && me?.currentMainCompany?.companyId) {
            setApiUrlRequest(`${CONSTS_QUOTE.getAllByCompanyId}?companyId=${me?.currentMainCompany?.companyId}`);
            setApiUrlRequestClients(`${CONSTS_CLIENT.getAllByCompanyId}?companyId=${me?.currentMainCompany?.companyId}`);
        }
    }, [me]);

    const [isModalFilterOpen, setIsModalFilterOpen] = useState<boolean>(false);

    // const [modalFilterFormData, setModalFilterFormData] = useState<iClientFormDataModalFilter>({
    //     fullName: null, email: null, cpf: null, address: null, addressNumber: null, city: null, state: null, zipCode: null, country: null, dateOfBirth: null, notes: null, phone: null
    // });

    const columns = [
        {
            title: 'Título',
            dataIndex: 'title',
            key: 'title'
        },
        {
            title: 'Observações',
            dataIndex: 'observation',
            key: 'observation'
        },
        {
            title: 'Validade',
            dataIndex: 'validUntil',
            key: 'validUntil',
            render: (value?: Date) =>
                value ? new Date(value).toLocaleDateString('pt-BR') : '-'
        },
        {
            title: 'Status',
            dataIndex: 'quoteStatus',
            key: 'quoteStatus'
        },
        {
            title: 'Qtd. de itens',
            dataIndex: 'items',
            key: 'items',
            render: (items?: iQuoteItem[]) => items?.length ?? 0
        }
    ] as iTableColumn[];

    const managingOptions = [
        {
            label: 'Visualizar detalhes',
            function: (e) => router.push(`${ROUTES.EMPRESA_CLIENTES}/${e.clientId}`),
            icon: <Icon icon='search' />
        },
        {
            label: 'Editar orçamento',
            function: (e) => handleOpenModalView(e),
            icon: <Icon icon='edit' />
        }
    ] as iTableManagingOptions[];

    const [isModalViewOpen, setIsModalViewOpen] = useState<boolean>(false);
    const [quoteClicked, setQuoteClicked] = useState<iQuote | undefined>(undefined);
    const [typeModal, setTypeModal] = useState<('edit' | 'create')>('create');

    function handleOpenModalView(quote: iQuote | undefined) {
        setTypeModal(quote ? 'edit' : 'create');
        setQuoteClicked(quote);
        setIsModalViewOpen(true);
    }

    return (
        <Fragment>
            <TemplatePageHeader
                title='Orçamentos registrados'
                actions={[
                    <Button
                        key='search'
                        label='Filtrar'
                        isStyleSimple={true}
                        handleFunction={() => setIsModalFilterOpen(true)}
                        icon_feather={<Icon icon='search' size='small' />}
                    />,
                    me?.isUserAdmOfCurrentMainCompany && (
                        <Button
                            key='add'
                            label='Registrar novo orçamento'
                            handleFunction={() => handleOpenModalView(undefined)}
                            icon_feather={<Icon icon='plus-circle' size='small' />}
                        />
                    )
                ]}
            >
                <TableGeneric
                    idPropName='quoteId'
                    columns={columns}
                    data={quotes?.output ?? []}
                    currentPage={currentPage}
                    setCurrentPage={setCurrentPage}
                    totalRowsCount={quotes?.count}
                    managingOptions={managingOptions}
                    // modalFilterFormData={modalFilterFormData}
                    // setModalFilterFormData={setModalFilterFormData}
                    apiUrlRequest={apiUrlRequest}
                    setApiUrlRequest={setApiUrlRequest}
                />
            </TemplatePageHeader>

            {/* <EmpresaClientesModalFilters
                isModalOpen={isModalFilterOpen}
                setIsModalOpen={setIsModalFilterOpen}
                modalFilterFormData={modalFilterFormData}
                setModalFilterFormData={setModalFilterFormData}
                apiUrlRequest={apiUrlRequest}
                setApiUrlRequest={setApiUrlRequest}
                setCurrentPage={setCurrentPage}
            /> */}

            <EmpresaQuotesModalView
                isModalOpen={isModalViewOpen}
                setIsModalOpen={setIsModalViewOpen}
                type={typeModal}
                clients={clients?.output as iClient[] ?? []}
                quote={quoteClicked}
                companyId={me?.currentMainCompany?.companyId}
                setTrigger={setTrigger}
            />
        </Fragment>
    )
}