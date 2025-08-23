import { BASE } from '@/app/api/fetch';

const controller = 'api/Auth';

export const CONSTS_AUTH = {
    auth: `${BASE}/${controller}`,
    isAuth: `${BASE}/${controller}/Me`,
    logout: `${BASE}/${controller}/Logout`
};