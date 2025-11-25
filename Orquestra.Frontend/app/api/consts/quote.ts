import { BASE } from '@/app/api/fetch';
import { Guid } from 'guid-typescript';
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
    quoteId?: Guid;
    companyId?: Guid;
    company?: iCompanyOutput | null;
    clientId?: Guid;
    client?: iClient | null;
    title?: string | null;
    observation?: string | null;
    validUntil?: Date | null;
    quoteStatus?: string;
    items?: iQuoteItem[];
}

export interface iQuoteItem {
    quoteItemId?: Guid;
    quoteId?: Guid;
    quote?: iQuote | null;
    title?: string;
    quantity?: number;
    unitPrice?: number;
    totalPrice?: number;
    totalValue?: number | null;
}

export interface iQuotePaginated {
    output: iQuote[];
    count: number;
}