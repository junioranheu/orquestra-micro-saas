'use client';
import { CONSTS_LOG, iLog, iLogPaginated } from '@/app/api/consts/log';
import TableGeneric, { iTableColumn } from '@/app/components/table/generic';
import SYSTEM from '@/app/consts/system';
import useApiGetCompanySituationEnum from '@/app/hooks/api/enums/useApiGetCompanySituationEnum';
import useApiRequestToSetterOnUrlChange from '@/app/hooks/useApiRequestToSetterOnUrlChange';
import useTitle from '@/app/hooks/useTitle';
import { useEffect, useState } from 'react';
import styles from './page.module.scss';

export default function Logs() {

    useTitle('Logs');

    const logTypeEnum = useApiGetCompanySituationEnum({ enumName: 'LogTypeEnum' });

    const [currentPage, setCurrentPage] = useState<number>(1);
    const [logs, setLogs] = useState<iLogPaginated>();
    const [logsNormalized, setLogsNormalized] = useState<iLog[]>([]);
    useApiRequestToSetterOnUrlChange<iLogPaginated>({ apiUrlRequest: CONSTS_LOG.get, setter: setLogs, hasPaginationInput: true, index: currentPage, limit: 10 });

    useEffect(() => {
        if (logTypeEnum && logs) {
            const normalized = logs?.output?.map(log => ({
                ...log,
                logType: logTypeEnum?.find(x => x.value.toString() === log.logType.toString())?.label ?? log.logType
            })) ?? [];

            setLogsNormalized(normalized);
        }
    }, [logTypeEnum, logs]);

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
            title: 'Parameters',
            dataIndex: 'parameters',
            key: 'parameters'
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
                data={logsNormalized ?? []}
                currentPage={currentPage}
                setCurrentPage={setCurrentPage}
                totalRowsCount={logs?.count}
                title={`Logs do ${SYSTEM.NAME}`}
            />
        </section>
    )
}