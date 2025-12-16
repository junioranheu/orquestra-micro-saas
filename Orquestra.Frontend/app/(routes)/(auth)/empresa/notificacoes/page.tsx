'use client';
import { CONSTS_LOG, iLogNotificationOutput, iLogNotificationOutputPaginated } from '@/app/api/consts/log';
import NotificationJsonVisualize from '@/app/components/notification-json-visualize';
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
                const notification: iLogNotificationOutput = {
                    changedFields: value
                };

                return (
                    <NotificationJsonVisualize notification={notification} theme='inherit' />
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