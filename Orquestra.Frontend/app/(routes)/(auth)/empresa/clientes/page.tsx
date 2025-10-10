'use client';
import { CONSTS_CLIENT, iClientPaginated } from '@/app/api/consts/client';
import TableGeneric, { iTableColumn } from '@/app/components/table/generic';
import useApiGetMe from '@/app/hooks/api/useApiGetMe';
import useApiRequestToSetterOnUrlChange from '@/app/hooks/useApiRequestToSetterOnUrlChange';
import useTitle from '@/app/hooks/useTitle';
import { Fragment, useEffect, useState } from 'react';
import EmpresaClientesModalAdd from './modal/add';
import EmpresaClientesModalFilters, { iClientFormDataModalFilter } from './modal/filter';
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
    const [isModalAddOpen, setIsModalAddOpen] = useState<boolean>(false);

    const [modalFilterFormData, setModalFilterFormData] = useState<iClientFormDataModalFilter>({
        fullName: null, email: null, CPF: null, address: null, dateOfBirth: null, notes: null, phone: null
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
                    btn_add_label='Cadastrar novo'
                    btn_add_function={() => setIsModalAddOpen(true)}
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

            <EmpresaClientesModalAdd
                isModalOpen={isModalAddOpen}
                setIsModalOpen={setIsModalAddOpen}
            />
        </Fragment>
    )
}