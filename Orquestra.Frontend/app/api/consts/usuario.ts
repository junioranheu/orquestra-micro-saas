import { BASE } from '@/app/api/fetch';

const controller = 'api/User';

export const CONSTS_USUARIO = {
    getAll: `${BASE}/${controller}/GetAll`,
    auth: `${BASE}/${controller}/Auth`,
};

export default interface iUsuario {
    userId: string;
    fullName: string;
    email: string;
    isAuth: boolean;
}

export interface iUsuarioResponse {
    linq: iUsuario[];
    count: number;
}