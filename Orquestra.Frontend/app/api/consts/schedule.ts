import { BASE } from '@/app/api/fetch';
import { Guid } from 'guid-typescript';
import iClient from './client';
import iCompanySimpleOutput from './company';
import { iUserResponse } from './user';

const controller = 'api/Schedule';

export const CONSTS_SCHEDULE = {
    post: `${BASE}/${controller}`,
    get: `${BASE}/${controller}`,
    getAllByCompanyId: `${BASE}/${controller}/GetAllByCompanyId`
};

export default interface iSchedule {
    scheduleId: Guid;
    date: Date;
    durationMinutes: number;
    paymentType: string;
    scheduleStatus: string;
    clientId: Guid;
    client?: iClient;
    companyId?: Guid;
    company: iCompanySimpleOutput;
    usersIds: Guid[];
    isRestrictForSpecificUsers: boolean;
    customTitle: string;
    customUrl: string;
    observation: string; // Observação no cadastro do schedule;
    amountReceived: number;

    dateEnd: Date;
    observations: string[]; // Avisos do sistema;
    usersOutput: iUserResponse[];
}