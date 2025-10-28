'use client';
import { CONSTS_LOG, iLogNotificationOutput, iLogNotificationOutputPaginated } from '@/app/api/consts/log';
import TableGeneric, { iTableColumn } from '@/app/components/table/generic';
import TemplatePageHeader from '@/app/components/template/page-header';
import useApiRequestToSetterOnUrlChange from '@/app/hooks/api/useApiRequestToSetterOnUrlChange';
import useTitle from '@/app/hooks/useTitle';
import { useState } from 'react';

export default function UsuarioNotificacoes() {

    useTitle('Notificações do sistema');

    const [currentPage, setCurrentPage] = useState<number>(1);
    const [notifications, setNotifications] = useState<iLogNotificationOutputPaginated>();
    useApiRequestToSetterOnUrlChange<iLogNotificationOutputPaginated>({ apiUrlRequest: CONSTS_LOG.getNotification, setter: setNotifications, hasPaginationInput: true, index: currentPage, limit: 15 });

    const columns = [
        {
            title: 'Data',
            dataIndex: 'date',
            key: 'date',
            render: (value: string) => new Date(value).toLocaleString('pt-BR')
        },
        {
            title: 'Tipo',
            dataIndex: 'logType',
            key: 'logType',
            render: (value: string, record: iLogNotificationOutput) => (
                <div style={{
                    minWidth: 'fit-content',
                    whiteSpace: 'nowrap',
                    overflow: 'hidden',
                    textOverflow: 'ellipsis'
                }}>
                    {record.emoji} {value}
                </div>
            )
        },
        {
            title: 'Detalhes',
            dataIndex: 'story',
            key: 'story'
        },
        {
            title: '',
            dataIndex: 'description',
            key: 'description'
        },
    ] as iTableColumn[];

    return (
        <TemplatePageHeader title='Notificações do sistema'>
            <TableGeneric
                idPropName='logId'
                columns={columns}
                data={notifications?.output ?? []}
                currentPage={currentPage}
                setCurrentPage={setCurrentPage}
                totalRowsCount={notifications?.count}
            />
        </TemplatePageHeader>
    )
}