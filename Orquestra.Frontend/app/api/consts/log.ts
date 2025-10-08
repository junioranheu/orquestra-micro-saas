import { BASE } from '@/app/api/fetch';
import { Guid } from 'guid-typescript';
import { iUserPaginated } from './user';

const controller = 'api/Log';

export const CONSTS_LOG = {
    get: `${BASE}/${controller}`,
};

export interface iLog {
    logId: Guid;
    logType: string;
    requestType?: string;
    endpoint?: string;
    parameters?: string;
    exception?: string;
    description?: string;
    status: number;
    userId?: Guid;
    user?: iUserPaginated;
    createdDate: Date;
}

export interface iLogPaginated {
    output: iLog[];
    count: number;
}