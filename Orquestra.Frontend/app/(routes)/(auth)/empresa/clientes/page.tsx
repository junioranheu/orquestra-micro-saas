'use client';
import { CONSTS_CLIENT, iClientPaginated } from '@/app/api/consts/client';
import Icon from '@/app/components/icon';
import TableGeneric, { iTableColumn, iTableManagingOptions } from '@/app/components/table/generic';
import useApiGetMe from '@/app/hooks/api/useApiGetMe';
import useApiRequestToSetterOnUrlChange from '@/app/hooks/useApiRequestToSetterOnUrlChange';
import useTitle from '@/app/hooks/useTitle';
import { Fragment, useEffect, useState } from 'react';
import EmpresaClientesModalFilters, { iClientFormModalFilterData } from './modal/filter';
import styles from './page.module.scss';

export default function EmpresaClientes() {

    useTitle('Clientes');
    const me = useApiGetMe({});

    const [currentPage, setCurrentPage] = useState<number>(1);
    const [clients, setClients] = useState<iClientPaginated>();

    const [apiUrlRequest, setApiUrlRequest] = useState<string>(CONSTS_CLIENT.getAllByCompanyId);
    useApiRequestToSetterOnUrlChange<iClientPaginated>({ apiUrlRequest: apiUrlRequest, setter: setClients, hasPaginationInput: true, index: currentPage, limit: 15 });

    useEffect(() => {
        if (me && me?.currentMainCompany?.companyId) {
            setApiUrlRequest(`${CONSTS_CLIENT.getAllByCompanyId}?companyId=${me?.currentMainCompany?.companyId}`);
        }
    }, [me]);

    const [isModalFilterOpen, setIsModalFilterOpen] = useState<boolean>(false);

    const [modalFilterFormData, setModalFilterFormData] = useState<iClientFormModalFilterData>({
        fullName: null, email: null, CPF: null, address: null, dateOfBirth: null, notes: null, phone: null
    });

    function handleAddNew() {
        alert('xd');
    }

    const columns = [
        {
            title: 'Nome completo',
            dataIndex: 'fullName',
            key: 'fullName'
        },
        {
            title: 'E-mail',
            dataIndex: 'email',
            key: 'email'
        },
        {
            title: 'CPF',
            dataIndex: 'cpf',
            key: 'cpf'
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
            label: 'Cadastrar novo cliente',
            function: () => handleAddNew(),
            icon: <Icon icon='user-plus' />
        }
    ] as iTableManagingOptions[];

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

                    title={`Clientes cadastrados em ${me?.currentMainCompany.name ?? ''}`}
                    managingOptions={managingOptions}
                    btn_filter_label='Filtrar'
                    btn_filter_function={() => setIsModalFilterOpen(true)}

                    modalFilterFormData={modalFilterFormData}
                    setModalFilterFormData={setModalFilterFormData}
                    apiUrlRequest={apiUrlRequest}
                    setApiUrlRequest={setApiUrlRequest}
                />
            </section>

            <EmpresaClientesModalFilters
                isModalFilterOpen={isModalFilterOpen}
                setIsModalFilterOpen={setIsModalFilterOpen}
                modalFilterFormData={modalFilterFormData}
                setModalFilterFormData={setModalFilterFormData}
                apiUrlRequest={apiUrlRequest}
                setApiUrlRequest={setApiUrlRequest}
                setCurrentPage={setCurrentPage}
            />
        </Fragment>
    )
}