'use client';
import { iMe } from '@/app/api/consts/auth';
import { CONSTS_COMPANY_INVOICE, iCompanyInvoice, iCompanyInvoicePaginated } from '@/app/api/consts/company-invoice';
import Icon from '@/app/components/icon';
import TableGeneric, { iTableColumn, iTableExtraItems, iTableManagingOptions } from '@/app/components/table/generic';
import useApiRequestToSetterOnUrlChange from '@/app/hooks/api/useApiRequestToSetterOnUrlChange';
import { useState } from 'react';
import styles from './index.module.scss';

interface iProps {
    me: iMe;
}

export default function EmpresaUsoEPlanoTabFaturas({ me }: iProps) {

    const [currentPage, setCurrentPage] = useState<number>(1);
    const [invoice, setInvoices] = useState<iCompanyInvoicePaginated>();
    useApiRequestToSetterOnUrlChange<iCompanyInvoicePaginated>({ apiUrlRequest: `${CONSTS_COMPANY_INVOICE.get}?companyId=${me?.currentMainCompany?.companyId}`, setter: setInvoices, hasPaginationInput: true, index: currentPage, limit: 15 });

    const columns = [
        {
            title: 'Data',
            dataIndex: 'createdDate',
            key: 'createdDate',
            render: (value: string) => value ? new Date(value).toLocaleString('pt-BR') : '-'
        },
        {
            title: 'Nº da fatura',
            dataIndex: 'invoiceNumber',
            key: 'invoiceNumber'
        },
        {
            title: 'Tipo de plano',
            dataIndex: 'planType',
            key: 'planType',
            render: (value: number) => {
                const map: Record<number, string> = {
                    1: 'Grátis',
                    2: 'Básico',
                    3: 'Premium'
                };
                return map[value] ?? '-';
            }
        },
        {
            title: 'Valor',
            dataIndex: 'amount',
            key: 'amount',
            render: (value: number) => value?.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' })
        },
        {
            title: 'Situação',
            dataIndex: 'companyInvoiceSituation',
            key: 'companyInvoiceSituation',
            render: (value: number) => {
                const map: Record<number, string> = {
                    1: 'Pendente',
                    2: 'Aprovado',
                    999: 'Cancelado'
                };

                return map[value] ?? '-';
            }
        },
        {
            title: 'Descrição',
            dataIndex: 'description',
            key: 'description'
        }
    ] as iTableColumn[];

    const managingOptions = [
        {
            label: 'Pagar',
            function: (e) => handlePay(e),
            icon: <Icon icon='dollar-sign' />
        }
    ] as iTableManagingOptions[];

    const tableExtraItems = [
        { title: 'Situação da empresa', label: me?.currentMainCompany?.companySituationStr ?? '-' }
    ] as iTableExtraItems[];

    function handlePay(e: iCompanyInvoice) {
        alert(e.companyInvoiceSituation);
    }

    return (
        <section className={styles.main}>
            <TableGeneric
                idPropName='companyInvoiceId'
                columns={columns}
                data={invoice?.output ?? []}
                currentPage={currentPage}
                setCurrentPage={setCurrentPage}
                totalRowsCount={invoice?.count}

                title={`Faturas do ${me.currentMainCompany.name}`}
                managingOptions={managingOptions}
                extraItems={tableExtraItems}
            />
        </section>
    )
}