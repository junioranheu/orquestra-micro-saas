import { BASE } from '@/app/api/fetch';
import { Guid } from 'guid-typescript';
import { iClient } from './client';
import { iSchedule } from './schedule';

const controller = 'api/ClientFollowUp';

export const CONSTS_CLIENT_FOLLOW_UP = {
    post: `${BASE}/${controller}`,
    put: `${BASE}/${controller}`,
    disable: `${BASE}/${controller}/Disable`,
    get: `${BASE}/${controller}`,
    getAllByCompanyId: `${BASE}/${controller}/GetAllByCompanyId`
};

export interface iClientFollowUp {
    clientFollowUpId?: Guid;
    clientId?: Guid;
    client?: iClient;
    companyId?: Guid;
    scheduleId?: Guid | null;
    schedule?: iSchedule;
    observation?: string;
    clientFollowUpStatus?: string;
    imagesFormFile?: globalThis.File[] | null | []; // Input;
    imagesBase64?: string[]; // Output;
    createdDate?: Date;
}

export interface iClientFollowUpPaginated {
    output: iClientFollowUp[];
    count: number;
}