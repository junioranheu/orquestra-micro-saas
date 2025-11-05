'use client';
import { CONSTS_LOG, iLog, iLogPaginated } from '@/app/api/consts/log';
import Icon from '@/app/components/icon';
import TableGeneric, { iTableColumn, iTableExtraItems, iTableManagingOptions } from '@/app/components/table/generic';
import TemplatePageHeader from '@/app/components/template/template-page-header';
import SYSTEM from '@/app/consts/system';
import handleCopyToClipboard from '@/app/functions/clipboard.copy';
import { DATE_STYLE, handleFormatDate } from '@/app/functions/format.date';
import { handleGetDateBrazil } from '@/app/functions/get.date.brazil';
import toast from '@/app/functions/toast';
import useApiRequestToSetterOnUrlChange from '@/app/hooks/api/useApiRequestToSetterOnUrlChange';
import useTitle from '@/app/hooks/useTitle';
import { useState } from 'react';

export default function Logs() {

    useTitle('Logs');

    const [currentPage, setCurrentPage] = useState<number>(1);
    const [logs, setLogs] = useState<iLogPaginated>();
    useApiRequestToSetterOnUrlChange<iLogPaginated>({ apiUrlRequest: CONSTS_LOG.get, setter: setLogs, hasPaginationInput: true, index: currentPage, limit: 15 });

    const [selectedIds, setSelectedIds] = useState<string[]>([]);

    const columns = [
        {
            title: 'Data',
            dataIndex: 'createdDate',
            key: 'createdDate',
            render: (value: string) => new Date(value).toLocaleString('pt-BR')
        },
        {
            title: 'Tipo de log',
            dataIndex: 'logType',
            key: 'logType',
            render: (value: number) => {
                const map: Record<number, string> = {
                    1: 'Exceção',
                    2: 'Requisição',
                    3: 'Job'
                };

                return map[value] ?? '-';
            }
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

    const managingOptions = [
        {
            label: 'Copiar dados',
            function: (e) => handleCopy(e),
            icon: <Icon icon='copy' />
        }
    ] as iTableManagingOptions[];

    const tableExtraItems = [
        { title: 'Última atualização', label: `Hoje às ${handleFormatDate(handleGetDateBrazil(), DATE_STYLE.HORA_MINUTO_SEGUNDO)}` }
    ] as iTableExtraItems[];

    function handleCopy(e: iLog) {
        const jsonString = JSON.stringify(e, null, 2);
        handleCopyToClipboard(jsonString);
        toast({ content: 'Dados copiados com sucesso.' });
    }

    function handleTeste() {
        alert(`handleTeste: ${selectedIds.length ? selectedIds.join(', ') : 'Nenhum selecionado.'}`);
    }

    return (
        <TemplatePageHeader title={`Logs do ${SYSTEM.NAME}`}>
            <TableGeneric
                idPropName='logId'
                columns={columns}
                data={logs?.output ?? []}
                currentPage={currentPage}
                setCurrentPage={setCurrentPage}
                totalRowsCount={logs?.count}
                managingOptions={managingOptions}
                extraItems={tableExtraItems}
                enableRowSelection={true}
                onSelectionChange={setSelectedIds}
                selectionAction={{
                    label: 'Listar IDs (teste #2)',
                    function: (ids) => toast({ content: `IDs selecionados: ${ids.join(', ')}` }),
                    icon: <Icon icon='activity' size='small' />
                }}
                actionButtons={[
                    { label: 'Listar IDs (teste #1)', onClick: handleTeste, icon: <Icon icon='activity' size='small' />, isSimple: true },
                ]}
            />
        </TemplatePageHeader>
    )
}