'use client';
import { CONSTS_LOG, iLogPaginated } from '@/app/api/consts/log';
import TableGeneric, { iTableColumn } from '@/app/components/table/generic';
import SYSTEM from '@/app/consts/system';
import useApiRequestToSetterOnUrlChange from '@/app/hooks/useApiRequestToSetterOnUrlChange';
import useTitle from '@/app/hooks/useTitle';
import { useState } from 'react';
import styles from './page.module.scss';

export default function Logs() {

    useTitle('Logs');

    const [currentPage, setCurrentPage] = useState<number>(0);
    const [logs, setLogs] = useState<iLogPaginated>();
    useApiRequestToSetterOnUrlChange<iLogPaginated>({ apiUrlRequest: CONSTS_LOG.get, setter: setLogs });

    const columns = [
        {
            title: 'Data',
            dataIndex: 'createdDate',
            key: 'createdDate',
            render: (value: string) => new Date(value).toLocaleString('pt-BR')
        },
        {
            title: 'Tipo',
            dataIndex: 'logType',
            key: 'logType'
        },
        {
            title: 'Status',
            dataIndex: 'status',
            key: 'status'
        },
        {
            title: 'Endpoint',
            dataIndex: 'endpoint',
            key: 'endpoint'
        },
        {
            title: 'Descrição',
            dataIndex: 'description',
            key: 'description'
        }
    ] as iTableColumn[];

    return (
        <section className={styles.main}>
            <TableGeneric
                idPropName='logId'
                columns={columns}
                data={logs?.output ?? []}
                currentPage={currentPage}
                setCurrentPage={setCurrentPage}
                totalRowsCount={logs?.count}
                title={`Logs do ${SYSTEM.NAME}`}
            />
        </section>
    )
}