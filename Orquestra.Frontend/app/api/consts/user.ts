import { BASE } from '@/app/api/fetch';
import { UserRoleEnum } from '@/app/enums/userRoleEnum';

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
    role: UserRoleEnum;
    status: boolean;
    createdDate: Date;
}

export interface iUserResponse {
    linq: iUser[];
    count: number;
}