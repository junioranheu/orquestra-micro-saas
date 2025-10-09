'use client';
import { CONSTS_CLIENT, iClientPaginated } from '@/app/api/consts/client';
import TableGeneric, { iTableColumn } from '@/app/components/table/generic';
import useApiGetMe from '@/app/hooks/api/useApiGetMe';
import useApiRequestToSetterOnUrlChange from '@/app/hooks/useApiRequestToSetterOnUrlChange';
import useTitle from '@/app/hooks/useTitle';
import { Guid } from 'guid-typescript';
import { useState } from 'react';
import styles from './page.module.scss';

export default function EmpresaClientes() {

    useTitle('Clientes');
    const me = useApiGetMe({});

    const [currentPage, setCurrentPage] = useState<number>(1);
    const [clients, setClients] = useState<iClientPaginated>();
    useApiRequestToSetterOnUrlChange<iClientPaginated>({ apiUrlRequest: `${CONSTS_CLIENT.getAllByCompanyId}?companyId=${me?.currentMainCompany?.companyId ?? Guid.EMPTY}`, setter: setClients, hasPaginationInput: true, index: currentPage, limit: 10 });

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

    return (
        <section className={styles.main}>
            <TableGeneric
                idPropName='clientId'
                columns={columns}
                data={clients?.output ?? []}
                currentPage={currentPage}
                setCurrentPage={setCurrentPage}
                totalRowsCount={clients?.count}
                title='Clientes'
            />
        </section>
    )
}