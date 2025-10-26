import { BASE } from '@/app/api/fetch';
import { Guid } from 'guid-typescript';
import iCompanyOutput from './company';

const controller = 'api/Client';

export const CONSTS_CLIENT = {
    post: `${BASE}/${controller}`,
    put: `${BASE}/${controller}`,
    updatePlanType: `${BASE}/${controller}/UpdatePlanType`,
    get: `${BASE}/${controller}`,
    getAllByCompanyId: `${BASE}/${controller}/GetAllByCompanyId`,
    disable: `${BASE}/${controller}/Disable`
};

export default interface iClient {
    clientId: Guid;
    fullName: string;
    email: string;
    cpf: string;
    address?: string;
    addressNumber?: string;
    city?: string;
    state?: string;
    zipCode?: string;
    country?: string;
    dateOfBirth?: Date;
    phone?: string;
    notes?: string;
    companyId: Guid;
    company: iCompanyOutput;
    createdDate: Date;
}

export interface iClientPaginated {
    output: iClient[];
    count: number;
}