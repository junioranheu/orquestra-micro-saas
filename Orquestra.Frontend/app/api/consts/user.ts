import { BASE } from '@/app/api/fetch';

const controller = 'api/User';

export const CONSTS_USER = {
    get: `${BASE}/${controller}`,
    create: `${BASE}/${controller}`,
    put: `${BASE}/${controller}`
};

export default interface iUser {
    userId: string;
    fullName: string;
    email: string;
    isAuth: boolean;
}

export interface iUserResponse {
    linq: iUser[];
    count: number;
}