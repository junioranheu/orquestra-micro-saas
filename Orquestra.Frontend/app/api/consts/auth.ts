import { BASE } from '@/app/api/fetch';

const controller = 'api/Auth';

export const CONSTS_AUTH = {
    auth: `${BASE}/${controller}`,
    meSimple: `${BASE}/${controller}/me/simple`,
    me: `${BASE}/${controller}/me`,
    logout: `${BASE}/${controller}`
};