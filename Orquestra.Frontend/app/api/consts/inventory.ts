import { BASE } from '@/app/api/fetch';
import { Guid } from 'guid-typescript';
import { iCompanyOutput } from './company';

const controller = 'api/Inventory';

export const CONSTS_INVENTORY = {
    post: `${BASE}/${controller}`,
    put: `${BASE}/${controller}`,
    getAllByCompanyId: `${BASE}/${controller}/GetAllByCompanyId`,
    disable: `${BASE}/${controller}/disable`
};

export interface iInventory {
    inventoryId?: Guid;
    companyId?: Guid;
    company?: iCompanyOutput | null;

    name?: string | null;
    description?: string | null;

    quantity?: number;
    unitPrice?: number | null;

    image?: number[] | null; // Entity;
    imageFormFile?: globalThis.File | null; // Input;
    imageBase64?: string; // Output;
    imageContentType?: string | null;

    totalValue?: number | null;
}

export interface iInventoryPaginated {
    output: iInventory[];
    count: number;
}