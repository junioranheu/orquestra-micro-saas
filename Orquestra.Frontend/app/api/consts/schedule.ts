import { BASE } from '@/app/api/fetch';
import { Guid } from 'guid-typescript';
import { iClient } from './client';
import { iClientFollowUp } from './client-follow-up';
import { iCompanyOutput } from './company';
import { iUserPaginated } from './user';

const controller = 'api/Schedule';

export const CONSTS_SCHEDULE = {
    post: `${BASE}/${controller}`,
    put: `${BASE}/${controller}`,
    disable: `${BASE}/${controller}/Disable`,
    get: `${BASE}/${controller}`,
    getAllByCompanyId: `${BASE}/${controller}/GetAllByCompanyId`,
    getAllByClientId: `${BASE}/${controller}/GetAllByClientId`
};

export interface iSchedule {
    scheduleId: Guid;
    dateStart: Date | string;
    timeStart: string;
    dateEnd: Date | string;
    timeEnd: string;
    paymentType: string;
    scheduleStatus: string | number;
    scheduleType: string | number;
    clientId: Guid;
    client?: iClient;
    companyId?: Guid;
    company?: iCompanyOutput;
    usersIds: Guid[];
    isRestrictForSpecificUsers: boolean;
    customTitle: string;
    customUrl: string;
    observation: string; // Observação no cadastro do schedule;
    amountReceived: number;

    // Extras;
    observations: string[]; // Avisos do sistema;
    usersOutput?: iUserPaginated[];
    messageIntegrationWhatsapp?: string;
    clientFollowUps?: iClientFollowUp[];
}