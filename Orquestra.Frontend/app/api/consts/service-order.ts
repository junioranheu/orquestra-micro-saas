import { BASE } from '@/app/api/fetch';
import { iClient } from './client';
import { iCompanyOutput } from './company';
import { iQuote } from './quote';

const controller = 'api/ServiceOrder';

export const CONSTS_SERVICE_ORDER = {
    post: `${BASE}/${controller}`,
    put: `${BASE}/${controller}`,
    get: `${BASE}/${controller}`,
    getAllByCompanyId: `${BASE}/${controller}/GetAllByCompanyId`,
    disable: `${BASE}/${controller}/Disable`
};

export interface iServiceOrder {
    serviceOrderId?: string;
    companyId?: string;
    company?: iCompanyOutput | null;

    quoteId?: string | null;
    quote?: iQuote | null;

    clientId?: string;
    client?: iClient | null;

    title?: string | null;
    observation?: string | null;

    executionDate?: Date | string | null;

    serviceOrderStatus?: string | null;

    createdDate?: Date | null;
}

export interface iServiceOrderPaginated {
    output: iServiceOrder[];
    count: number;
}