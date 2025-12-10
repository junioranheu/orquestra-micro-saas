'use client';
import { CONSTS_LOG, iLogNotificationOutputPaginated } from '@/app/api/consts/log';
import TableGeneric, { iTableColumn } from '@/app/components/table/generic';
import TemplatePageHeader from '@/app/components/template/template-page-header';
import useApiGetCurrentMainCompany from '@/app/hooks/api/useApiGetCurrentMainCompany';
import useApiRequestToSetterOnUrlChange from '@/app/hooks/api/useApiRequestToSetterOnUrlChange';
import { useFakeLoading } from '@/app/hooks/useFakeLoader';
import useTitle from '@/app/hooks/useTitle';
import { useState } from 'react';

export default function UsuarioNotificacoes() {

    useTitle('Notificações');

    const currentMainCompany = useApiGetCurrentMainCompany({});
    const isLoading = useFakeLoading();

    const [currentPage, setCurrentPage] = useState<number>(1);
    const [notifications, setNotifications] = useState<iLogNotificationOutputPaginated>();
    useApiRequestToSetterOnUrlChange<iLogNotificationOutputPaginated>({ apiUrlRequest: CONSTS_LOG.getNotification, setter: setNotifications, hasPaginationInput: true, index: currentPage, limit: 15 });

    const columns = [
        {
            title: 'Data',
            dataIndex: 'date',
            key: 'date',
            render: (value: string) => new Date(value).toLocaleString('pt-BR'),
            width: '12rem'
        },
        // {
        //     title: 'Tipo',
        //     dataIndex: 'logType',
        //     key: 'logType',
        //     render: (value: string, record: iLogNotificationOutput) => (
        //         <div style={{
        //             minWidth: 'fit-content',
        //             whiteSpace: 'nowrap',
        //             overflow: 'hidden',
        //             textOverflow: 'ellipsis'
        //         }}>
        //             {record.emoji} {value}
        //         </div>
        //     )
        // },
        {
            title: 'Notificação',
            dataIndex: 'story',
            key: 'story',
            width: '15rem'
        },
        {
            title: 'Modificações',
            dataIndex: 'changedFields',
            key: 'changedFields',
            render: (value: string) => {
                let parsed: any;

                try {
                    parsed = JSON.parse(value);
                } catch {
                    parsed = value;
                }

                return (
                    <pre style={{
                        whiteSpace: 'pre-wrap',
                        wordWrap: 'break-word',
                        margin: 0
                    }}>
                        {typeof parsed === 'object' ? JSON.stringify(parsed, null, 2) : parsed}
                    </pre>
                );
            }
        }
    ] as iTableColumn[];

    if (isLoading) {
        return (
            <TemplatePageHeader title='Notificações' isLoading={isLoading}>
            </TemplatePageHeader>
        )
    }

    return (
        <TemplatePageHeader title={`Notificações • ${currentMainCompany?.name ?? ''}`} >
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