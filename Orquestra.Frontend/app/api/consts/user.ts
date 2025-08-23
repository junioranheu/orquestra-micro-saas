import { BASE } from '@/app/api/fetch';

const controller = 'api/User';

export const CONSTS_USER = {
    get: `${BASE}/${controller}`,
    create: `${BASE}/${controller}`,
    put: `${BASE}/${controller}`
};

export default interface iUsuario {
    userId: string;
    fullName: string;
    email: string;
    token: string;
    isAuth: boolean;
}

export interface iUsuarioResponse {
    linq: iUsuario[];
    count: number;
}