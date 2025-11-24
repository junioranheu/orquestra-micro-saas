import { BASE } from '@/app/api/fetch';
import { iClient } from './client';
import { iCompanyOutput } from './company';

const controller = 'api/Quote';

export const CONSTS_QUOTE = {
    post: `${BASE}/${controller}`,
    put: `${BASE}/${controller}`,
    get: `${BASE}/${controller}`,
    getAllByCompanyId: `${BASE}/${controller}/GetAllByCompanyId`
};

export interface iQuote {
    quoteId: string;
    companyId: string;
    company?: iCompanyOutput | null;
    clientId: string;
    client?: iClient | null;
    title?: string | null;
    observation?: string | null;
    validUntil?: Date | null;
    quoteStatus: string;
    items: iQuoteItem[];
}

export interface iQuoteItem {
    quoteItemId: string;
    quoteId: string;
    quote?: iQuote | null;
    title: string;
    description?: string | null;
    quantity: number;
    unitPrice: number;
    totalPrice: number;
    totalValue?: number | null;
}

export interface iQuotePaginated {
    output: iQuote[];
    count: number;
}