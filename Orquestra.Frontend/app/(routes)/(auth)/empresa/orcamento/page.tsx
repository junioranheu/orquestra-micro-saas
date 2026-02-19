'use client';
import { CONSTS_QUOTE, iQuote, iQuoteItem, iQuotePaginated } from '@/app/api/consts/quote';
import { Fetch } from '@/app/api/fetch';
import Icon from '@/app/components/icon';
import Button from '@/app/components/input/button';
import TableGeneric, { iTableColumn, iTableManagingOptions } from '@/app/components/table/generic';
import TemplatePageHeader from '@/app/components/template/template-page-header';
import { handleToBrazilDate } from '@/app/functions/get.date.brazil';
import swal from '@/app/functions/swal';
import toast from '@/app/functions/toast';
import useApiGetEnum from '@/app/hooks/api/useApiGetEnum';
import useApiGetMe from '@/app/hooks/api/useApiGetMe';
import useApiRequestToSetterOnUrlChange from '@/app/hooks/api/useApiRequestToSetterOnUrlChange';
import { useClientsByCompanyIdDropdown } from '@/app/hooks/api/useClientsByCompanyIdDropdown';
import useTitle from '@/app/hooks/useTitle';
import { Dispatch, Fragment, SetStateAction, useEffect, useState } from 'react';
import EmpresaQuotesModalFilters from './modal/filter';
import EmpresaQuotesModalView from './modal/view';

export default function EmpresaOrcamento() {

    useTitle('Orçamentos');

    const me = useApiGetMe({});
    const quoteStatusEnum = useApiGetEnum({ enumName: 'QuoteStatusEnum' });

    const [currentPage, setCurrentPage] = useState<number>(1);
    const [quotes, setQuotes] = useState<iQuotePaginated>();
    const [trigger, setTrigger] = useState<Date>(new Date());
    const [apiUrlRequest, setApiUrlRequest] = useState<string>(CONSTS_QUOTE.getAllByCompanyId);
    useApiRequestToSetterOnUrlChange<iQuotePaginated>({ apiUrlRequest: apiUrlRequest, setter: setQuotes, hasPaginationInput: true, index: currentPage, limit: 15, trigger: trigger });

    useEffect(() => {
        if (me && me?.currentMainCompany?.companyId) {
            setApiUrlRequest(`${CONSTS_QUOTE.getAllByCompanyId}?companyId=${me?.currentMainCompany?.companyId}`);
        }
    }, [me]);

    const { clientsDropDown, } = useClientsByCompanyIdDropdown(me?.currentMainCompany?.companyId);

    const [isModalFilterOpen, setIsModalFilterOpen] = useState<boolean>(false);

    const [modalFilterFormData, setModalFilterFormData] = useState<iQuote>({
        title: null, validUntil: null
    });

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
            title: 'Cliente',
            dataIndex: 'client.fullName',
            key: 'client.fullName'
        },
        {
            title: 'Validade',
            dataIndex: 'validUntil',
            key: 'validUntil',
            render: (value?: Date) => value ? new Date(handleToBrazilDate(value)).toLocaleDateString('pt-BR') : '-'
        },
        {
            title: 'Data de criação',
            dataIndex: 'createdDate',
            key: 'createdDate',
            render: (value?: Date) => value ? new Date(handleToBrazilDate(value)).toLocaleDateString('pt-BR') : '-'
        },
        {
            title: 'Status',
            dataIndex: 'quoteStatus',
            key: 'quoteStatus',
            render: (value: number) => {
                const item = quoteStatusEnum?.find(x => x.value === value);
                const text = item?.label ?? '-';

                if (value === 5) {
                    return (
                        <span style={{ color: 'var(--contrast)', fontWeight: 600 }}>
                            {text}
                        </span>
                    );
                }

                return text;
            }
        },
        {
            title: 'Qtd. de itens',
            dataIndex: 'items',
            key: 'items',
            render: (items?: iQuoteItem[]) => items?.reduce((acc, item) => acc + (item.quantity ?? 0), 0) ?? 0
        }
    ] as iTableColumn[];

    const managingOptions = [
        {
            label: 'Editar orçamento',
            function: (e) => handleOpenModalView(e),
            icon: <Icon icon='edit' />
        },
        {
            label: 'Baixar orçamento em PDF',
            function: (e: iQuote) => handleDownloadPDF(e),
            icon: <Icon icon='file-text' />
        },
        ...(me?.isUserAdmOfCurrentMainCompany ? [
            {
                label: 'Desativar orçamento',
                function: (e: iQuote) => handleDisable(e, setTrigger),
                icon: <Icon icon='x' />
            }
        ] : [])
    ] as iTableManagingOptions[];

    const [isModalViewOpen, setIsModalViewOpen] = useState<boolean>(false);
    const [quoteClicked, setQuoteClicked] = useState<iQuote | undefined>(undefined);
    const [typeModal, setTypeModal] = useState<('edit' | 'create')>('create');

    function handleOpenModalView(quote: iQuote | undefined) {
        setTypeModal(quote ? 'edit' : 'create');
        setQuoteClicked(quote);
        setIsModalViewOpen(true);
    }

    async function handleDisable(quote: iQuote, setTrigger: Dispatch<SetStateAction<Date>>) {
        swal({
            content: 'Você tem certeza que deseja desativar este orçamento? Este processo é irreversível.',
            confirmBtnText: 'Sim, desejo desativar',
            mustConfirm: true,
            checkboxLabel: 'Sim, confirmo',
            confirmFunction: async () => {
                const output = await Fetch.put({ url: `${CONSTS_QUOTE.disable}?quoteId=${quote.quoteId}` });

                if (output) {
                    toast({ content: 'Orçamento desativado com sucesso.' });
                    setTrigger(new Date());
                    return;
                }

                toast({ content: 'Não foi possível desativar este orçamento. Tente novamente mais tarde.' });
            },
            cancelBtnText: 'Voltar',
            icon: 'question'
        });
    }

    async function handleDownloadPDF(quote: iQuote) {
        swal({
            content: 'Deseja gerar e baixar um PDF deste orçamento?',
            confirmBtnText: 'Sim',
            confirmFunction: async () => {
                const output = await Fetch.get({ url: `${CONSTS_QUOTE.getPDF}?quoteId=${quote.quoteId}`, blobExportName: `${quote.company?.name} • Orçamento • ${quote.client?.fullName}.pdf` });

                if (output) {
                    toast({ content: 'PDF gerado com sucesso.' });
                    return;
                }

                toast({ content: 'Não foi possível gerar um PDF para este orçamento. Tente novamente mais tarde.' });
            },
            cancelBtnText: 'Voltar',
            icon: 'question'
        });
    }

    return (
        <Fragment>
            <TemplatePageHeader
                title='Orçamentos registrados'
                actions={[
                    <Button
                        key='search'
                        label='Filtrar'
                        styleType='transparent'
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
                    modalFilterFormData={modalFilterFormData}
                    setModalFilterFormData={setModalFilterFormData}
                    apiUrlRequest={apiUrlRequest}
                    setApiUrlRequest={setApiUrlRequest}
                />
            </TemplatePageHeader>

            <EmpresaQuotesModalFilters
                isModalOpen={isModalFilterOpen}
                setIsModalOpen={setIsModalFilterOpen}
                modalFilterFormData={modalFilterFormData}
                setModalFilterFormData={setModalFilterFormData}
                apiUrlRequest={apiUrlRequest}
                setApiUrlRequest={setApiUrlRequest}
                setCurrentPage={setCurrentPage}
                clientsDropDown={clientsDropDown ?? []}
            />

            <EmpresaQuotesModalView
                isModalOpen={isModalViewOpen}
                setIsModalOpen={setIsModalViewOpen}
                type={typeModal}
                clientsDropDown={clientsDropDown ?? []}
                quote={quoteClicked}
                companyId={me?.currentMainCompany?.companyId}
                setTrigger={setTrigger}
            />
        </Fragment>
    )
}