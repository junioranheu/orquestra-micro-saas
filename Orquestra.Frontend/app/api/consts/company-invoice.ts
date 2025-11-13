import { BASE } from '@/app/api/fetch';
import { Guid } from 'guid-typescript';
import { iCompanyOutput } from './company';

const controller = 'api/CompanyInvoice';

export const CONSTS_COMPANY_INVOICE = {
    get: `${BASE}/${controller}`,
    pay: `${BASE}/${controller}/Pay`
};

export interface iCompanyInvoice {
    companyInvoiceId: Guid;
    invoiceNumber: number;
    companyId: Guid;
    company?: iCompanyOutput;
    planType: string;
    amount: number;
    description?: string;
    companyInvoiceSituation: string;
    createdDate?: string;
    updatedDate?: string;
    status?: boolean;
}

export interface iCompanyInvoicePaginated {
    output: iCompanyInvoice[];
    count: number;
}