'use client';
import iClient, { CONSTS_CLIENT, iClientPaginated } from '@/app/api/consts/client';
import { Fetch } from '@/app/api/fetch';
import Icon from '@/app/components/icon';
import Button from '@/app/components/input/button';
import TableGeneric, { iTableColumn, iTableManagingOptions } from '@/app/components/table/generic';
import TemplatePageHeader from '@/app/components/template/page-header';
import ROUTES from '@/app/consts/routes';
import swal from '@/app/functions/swal';
import toast from '@/app/functions/toast';
import useApiGetMe from '@/app/hooks/api/useApiGetMe';
import useApiRequestToSetterOnUrlChange from '@/app/hooks/api/useApiRequestToSetterOnUrlChange';
import useTitle from '@/app/hooks/useTitle';
import { useRouter } from 'next/navigation';
import { Dispatch, Fragment, SetStateAction, useEffect, useState } from 'react';
import EmpresaClientesModalFilters, { iClientFormDataModalFilter } from './modal/filter';
import EmpresaClientesModalView from './modal/view';

export default function EmpresaClientes() {

    useTitle('Clientes');

    const me = useApiGetMe({});
    const router = useRouter();

    const [currentPage, setCurrentPage] = useState<number>(1);
    const [clients, setClients] = useState<iClientPaginated>();

    const [trigger, setTrigger] = useState<Date>(new Date());
    const [apiUrlRequest, setApiUrlRequest] = useState<string>(CONSTS_CLIENT.getAllByCompanyId);
    useApiRequestToSetterOnUrlChange<iClientPaginated>({ apiUrlRequest: apiUrlRequest, setter: setClients, hasPaginationInput: true, index: currentPage, limit: 15, trigger: trigger });

    useEffect(() => {
        if (me && me?.currentMainCompany?.companyId) {
            setApiUrlRequest(`${CONSTS_CLIENT.getAllByCompanyId}?companyId=${me?.currentMainCompany?.companyId}`);
        }
    }, [me]);

    const [isModalFilterOpen, setIsModalFilterOpen] = useState<boolean>(false);

    const [modalFilterFormData, setModalFilterFormData] = useState<iClientFormDataModalFilter>({
        fullName: null, email: null, cpf: null, address: null, addressNumber: null, city: null, state: null, zipCode: null, country: null, dateOfBirth: null, notes: null, phone: null
    });

    const columns = [
        {
            title: 'Nome completo',
            dataIndex: 'fullName',
            key: 'fullName'
        },
        {
            title: 'CPF',
            dataIndex: 'cpf',
            key: 'cpf'
        },
        {
            title: 'E-mail',
            dataIndex: 'email',
            key: 'email'
        },
        {
            title: 'CEP',
            dataIndex: 'zipCode',
            key: 'zipCode'
        },
        {
            title: 'Endereço',
            dataIndex: 'address',
            key: 'address'
        },
        {
            title: 'Data de nascimento',
            dataIndex: 'dateOfBirth',
            key: 'dateOfBirth',
            render: (value?: Date) => value ? new Date(value).toLocaleDateString('pt-BR') : '-'
        },
        {
            title: 'Notas',
            dataIndex: 'notes',
            key: 'notes'
        }
    ] as iTableColumn[];

    const managingOptions = [
        {
            label: 'Visualizar detalhes',
            function: (e) => router.push(`${ROUTES.EMPRESA_CLIENTES}/${e.clientId}`),
            icon: <Icon icon='search' />
        },
        {
            label: 'Editar cliente',
            function: (e) => handleOpenModalView(e),
            icon: <Icon icon='edit' />
        },
        {
            label: 'Remover cliente',
            function: (e) => handleDisableClient(e, setTrigger),
            icon: <Icon icon='user-x' />
        }
    ] as iTableManagingOptions[];

    const [isModalViewOpen, setIsModalViewOpen] = useState<boolean>(false);
    const [clientClicked, setClientClicked] = useState<iClient | undefined>(undefined);
    const [typeModal, setTypeModal] = useState<('edit' | 'create')>('create');

    function handleOpenModalView(client: iClient | undefined) {
        setTypeModal(client ? 'edit' : 'create');
        setClientClicked(client);
        setIsModalViewOpen(true);
    }

    return (
        <Fragment>
            <TemplatePageHeader
                title='Clientes cadastrados'
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
                            label='Cadastrar novo cliente'
                            handleFunction={() => handleOpenModalView(undefined)}
                            icon_feather={<Icon icon='plus-circle' size='small' />}
                        />
                    )
                ]}
            >
                <TableGeneric
                    idPropName='clientId'
                    columns={columns}
                    data={clients?.output ?? []}
                    currentPage={currentPage}
                    setCurrentPage={setCurrentPage}
                    totalRowsCount={clients?.count}
                    managingOptions={managingOptions}
                    modalFilterFormData={modalFilterFormData}
                    setModalFilterFormData={setModalFilterFormData}
                    apiUrlRequest={apiUrlRequest}
                    setApiUrlRequest={setApiUrlRequest}
                />
            </TemplatePageHeader>

            <EmpresaClientesModalFilters
                isModalOpen={isModalFilterOpen}
                setIsModalOpen={setIsModalFilterOpen}
                modalFilterFormData={modalFilterFormData}
                setModalFilterFormData={setModalFilterFormData}
                apiUrlRequest={apiUrlRequest}
                setApiUrlRequest={setApiUrlRequest}
                setCurrentPage={setCurrentPage}
            />

            <EmpresaClientesModalView
                isModalOpen={isModalViewOpen}
                setIsModalOpen={setIsModalViewOpen}
                type={typeModal}
                client={clientClicked}
                companyId={me?.currentMainCompany?.companyId}
                setTrigger={setTrigger}
            />
        </Fragment>
    )
}

export async function handleDisableClient(member: iClient, setTrigger: Dispatch<SetStateAction<Date>>) {
    swal({
        content: 'Você tem certeza que deseja remover este cliente?',
        confirmBtnText: 'Sim, desejo remover',
        confirmFunction: async () => {
            const input = { clientId: member.clientId };
            const schedule = await Fetch.put({ url: CONSTS_CLIENT.disable, body: input });

            if (schedule) {
                toast({ content: 'Cliente removido com sucesso.' });
                setTrigger(new Date());
                return;
            }

            toast({ content: 'Não foi possível remover este cliente. Tente novamente mais tarde.' });
        },
        cancelBtnText: 'Voltar',
        icon: 'question'
    });
}