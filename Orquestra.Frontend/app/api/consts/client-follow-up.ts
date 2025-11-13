import { BASE } from '@/app/api/fetch';
import { Guid } from 'guid-typescript';

const controller = 'api/ClientFollowUp';

export const CONSTS_CLIENT_FOLLOW_UP = {
    post: `${BASE}/${controller}`,
    put: `${BASE}/${controller}`,
    disable: `${BASE}/${controller}/Disable`,
    get: `${BASE}/${controller}`
};

export default interface iClientFollowUp {
    clientFollowUpId: Guid;
    clientId: Guid;
    observation: string;
    clientFollowUpStatus: string;
    imagesBase64: string;
    imagesContentType: string;
}