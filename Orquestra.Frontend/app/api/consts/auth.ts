import { BASE } from '@/app/api/fetch';

const controller = 'api/Auth';

export const CONSTS_AUTH = {
    auth: `${BASE}/${controller}`,
    meSimple: `${BASE}/${controller}/Me/Simple`,
    me: `${BASE}/${controller}/Me`,
    logout: `${BASE}/${controller}`
};