'use client';
import { CONSTS_SERVICE_ORDER, iServiceOrder, iServiceOrderPaginated } from '@/app/api/consts/service-order';
import { Fetch } from '@/app/api/fetch';
import Icon from '@/app/components/icon';
import Button from '@/app/components/input/button';
import TableGeneric, { iTableColumn, iTableManagingOptions } from '@/app/components/table/generic';
import TemplatePageHeader from '@/app/components/template/template-page-header';
import swal from '@/app/functions/swal';
import toast from '@/app/functions/toast';
import useApiGetEnum from '@/app/hooks/api/useApiGetEnum';
import useApiGetMe from '@/app/hooks/api/useApiGetMe';
import useApiRequestToSetterOnUrlChange from '@/app/hooks/api/useApiRequestToSetterOnUrlChange';
import { useClientsByCompanyIdDropdown } from '@/app/hooks/api/useClientsByCompanyIdDropdown';
import useTitle from '@/app/hooks/useTitle';
import { Dispatch, Fragment, SetStateAction, useEffect, useState } from 'react';
import EmpresaServiceOrderModalFilters from './modal/filter';
import EmpresaServiceOrderModalView from './modal/view';

export default function EmpresaOrdemDeServico() {

    useTitle('Ordem de serviço');

    const me = useApiGetMe({});
    const serviceOrderStatusEnum = useApiGetEnum({ enumName: 'ServiceOrderStatusEnum' });

    const [currentPage, setCurrentPage] = useState<number>(1);
    const [serviceOrders, setServiceOrders] = useState<iServiceOrderPaginated>();
    const [trigger, setTrigger] = useState<Date>(new Date());
    const [apiUrlRequest, setApiUrlRequest] = useState<string>(CONSTS_SERVICE_ORDER.getAllByCompanyId);
    useApiRequestToSetterOnUrlChange<iServiceOrderPaginated>({ apiUrlRequest: apiUrlRequest, setter: setServiceOrders, hasPaginationInput: true, index: currentPage, limit: 15, trigger: trigger });

    useEffect(() => {
        if (me && me?.currentMainCompany?.companyId) {
            setApiUrlRequest(`${CONSTS_SERVICE_ORDER.getAllByCompanyId}?companyId=${me?.currentMainCompany?.companyId}`);
        }
    }, [me]);

    const { clientsDropDown, } = useClientsByCompanyIdDropdown(me?.currentMainCompany?.companyId);

    const [isModalFilterOpen, setIsModalFilterOpen] = useState<boolean>(false);

    const [modalFilterFormData, setModalFilterFormData] = useState<iServiceOrder>({
        title: null, clientId: null, executionDate: null, serviceOrderStatus: null
    });

    const columns = [
        {
            title: 'Título',
            dataIndex: 'title',
            key: 'title',
            render: (value?: string | null) => value ?? '-'
        },
        {
            title: 'Observações',
            dataIndex: 'observation',
            key: 'observation',
            render: (value?: string | null) => value ?? '-'
        },
        {
            title: 'Data de Execução',
            dataIndex: 'executionDate',
            key: 'executionDate',
            render: (value?: Date | string | null) => value ? new Date(value).toLocaleDateString('pt-BR') : '-'
        },
        {
            title: 'Data de criação',
            dataIndex: 'createdDate',
            key: 'createdDate',
            render: (value?: Date | string | null) => value ? new Date(value).toLocaleDateString('pt-BR') : '-'
        },
        {
            title: 'Status',
            dataIndex: 'serviceOrderStatus',
            key: 'serviceOrderStatus',
            render: (value: number) => {
                const item = serviceOrderStatusEnum?.find(x => x.value === value);
                const text = item?.label ?? '-';

                if (value === 4) {
                    return (
                        <span style={{ color: 'var(--contrast)', fontWeight: 600 }}>
                            {text}
                        </span>
                    );
                }

                return text;
            }
        },
    ] as iTableColumn[];

    const managingOptions = [
        {
            label: 'Editar OS',
            function: (e) => handleOpenModalView(e),
            icon: <Icon icon='edit' />
        },
        ...(me?.isUserAdmOfCurrentMainCompany ? [
            {
                label: 'Desativar OS',
                function: (e: iServiceOrder) => handleDisable(e, setTrigger),
                icon: <Icon icon='x' />
            }
        ] : [])
    ] as iTableManagingOptions[];

    const [isModalViewOpen, setIsModalViewOpen] = useState<boolean>(false);
    const [serviceOrderClicked, setServiceOrderClicked] = useState<iServiceOrder | undefined>(undefined);
    const [typeModal, setTypeModal] = useState<('edit' | 'create')>('create');

    function handleOpenModalView(serviceOrder: iServiceOrder | undefined) {
        setTypeModal(serviceOrder ? 'edit' : 'create');
        setServiceOrderClicked(serviceOrder);
        setIsModalViewOpen(true);
    }

    async function handleDisable(serviceOrder: iServiceOrder, setTrigger: Dispatch<SetStateAction<Date>>) {
        swal({
            content: 'Você tem certeza que deseja desativar esta OS? Este processo é irreversível.',
            confirmBtnText: 'Sim, desejo desativar',
            mustConfirm: true,
            checkboxLabel: 'Sim, confirmo',
            confirmFunction: async () => {
                const output = await Fetch.put({ url: `${CONSTS_SERVICE_ORDER.disable}?serviceOrderId=${serviceOrder.serviceOrderId}` });

                if (output) {
                    toast({ content: 'OS desativada com sucesso.' });
                    setTrigger(new Date());
                    return;
                }

                toast({ content: 'Não foi possível desativar esta OS. Tente novamente mais tarde.' });
            },
            cancelBtnText: 'Voltar',
            icon: 'question'
        });
    }

    return (
        <Fragment>
            <TemplatePageHeader
                title='Ordens de serviço registradas'
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
                            label='Registrar nova OS'
                            handleFunction={() => handleOpenModalView(undefined)}
                            icon_feather={<Icon icon='plus-circle' size='small' />}
                        />
                    )
                ]}
            >
                <TableGeneric
                    idPropName='serviceOrderId'
                    columns={columns}
                    data={serviceOrders?.output ?? []}
                    currentPage={currentPage}
                    setCurrentPage={setCurrentPage}
                    totalRowsCount={serviceOrders?.count}
                    managingOptions={managingOptions}
                    modalFilterFormData={modalFilterFormData}
                    setModalFilterFormData={setModalFilterFormData}
                    apiUrlRequest={apiUrlRequest}
                    setApiUrlRequest={setApiUrlRequest}
                />
            </TemplatePageHeader>

            <EmpresaServiceOrderModalFilters
                isModalOpen={isModalFilterOpen}
                setIsModalOpen={setIsModalFilterOpen}
                modalFilterFormData={modalFilterFormData}
                setModalFilterFormData={setModalFilterFormData}
                apiUrlRequest={apiUrlRequest}
                setApiUrlRequest={setApiUrlRequest}
                setCurrentPage={setCurrentPage}
                clientsDropDown={clientsDropDown ?? []}
            />

            <EmpresaServiceOrderModalView
                isModalOpen={isModalViewOpen}
                setIsModalOpen={setIsModalViewOpen}
                type={typeModal}
                clientsDropDown={clientsDropDown ?? []}
                serviceOrder={serviceOrderClicked}
                companyId={me?.currentMainCompany?.companyId}
                setTrigger={setTrigger}
            />
        </Fragment>
    )
}