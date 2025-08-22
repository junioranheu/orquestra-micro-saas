import { BASE } from '@/app/api/fetch';

const controller = 'api/Auth';

export const CONSTS_AUTH = {
    auth: `${BASE}/${controller}`
};

export default interface iAuthInput {
    email: string;
    password: string;
}