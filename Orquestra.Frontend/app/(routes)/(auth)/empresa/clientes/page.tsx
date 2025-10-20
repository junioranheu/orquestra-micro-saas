'use client';
import iClient, { CONSTS_CLIENT, iClientPaginated } from '@/app/api/consts/client';
import { Fetch } from '@/app/api/fetch';
import Icon from '@/app/components/icon';
import TableGeneric, { iTableColumn, iTableManagingOptions } from '@/app/components/table/generic';
import swal from '@/app/functions/swal';
import toast from '@/app/functions/toast';
import useApiGetMe from '@/app/hooks/api/useApiGetMe';
import useApiRequestToSetterOnUrlChange from '@/app/hooks/api/useApiRequestToSetterOnUrlChange';
import useTitle from '@/app/hooks/useTitle';
import { Fragment, useEffect, useState } from 'react';
import EmpresaClientesModalFilters, { iClientFormDataModalFilter } from './modal/filter';
import EmpresaClientesModalView from './modal/view';
import styles from './page.module.scss';

export default function EmpresaClientes() {

    useTitle('Clientes');
    const me = useApiGetMe({});

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
            label: 'Editar cliente',
            function: (e) => handleOpenModalView(e),
            icon: <Icon icon='edit' />
        },
        {
            label: 'Remover membro',
            function: (e) => handleDisable(e),
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


    async function handleDisable(member: iClient) {
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

    return (
        <Fragment>
            <section className={styles.main}>
                <TableGeneric
                    idPropName='clientId'
                    columns={columns}
                    data={clients?.output ?? []}
                    currentPage={currentPage}
                    setCurrentPage={setCurrentPage}
                    totalRowsCount={clients?.count}

                    title={`Clientes cadastrados em ${me?.currentMainCompany?.name ?? ''}`}
                    managingOptions={managingOptions}
                    btn_add_label='Cadastrar novo'
                    btn_add_function={() => handleOpenModalView(undefined)}
                    btn_filter_label='Filtrar'
                    btn_filter_function={() => setIsModalFilterOpen(true)}

                    modalFilterFormData={modalFilterFormData}
                    setModalFilterFormData={setModalFilterFormData}
                    apiUrlRequest={apiUrlRequest}
                    setApiUrlRequest={setApiUrlRequest}
                />
            </section>

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