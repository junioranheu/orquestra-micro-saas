import { BASE } from '@/app/api/fetch';
import { Guid } from 'guid-typescript';
import iCompanySimpleOutput from './company';

const controller = 'api/Client';

export const CONSTS_SCHEDULE = {
    post: `${BASE}/${controller}`,
    get: `${BASE}/${controller}`,
    getAllByCompanyId: `${BASE}/${controller}/GetAllByCompanyId`
};

export default interface iClient {
    clientId: Guid;
    fullName: string;
    email: string;
    CPF: string;
    address: Guid;
    dateOfBirth?: Date;
    notes?: string;
    companyId: Guid;
    company: iCompanySimpleOutput;
}