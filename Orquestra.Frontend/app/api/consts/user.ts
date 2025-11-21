import { BASE } from '@/app/api/fetch';
import { USER_ROLE_ENUM } from '@/app/enums/userRoleEnum';

const controller = 'api/User';

export const CONSTS_USER = {
    get: `${BASE}/${controller}`,
    create: `${BASE}/${controller}`,
    put: `${BASE}/${controller}`,
    verify: `${BASE}/${controller}`
};

export interface iUserInput {
    fullName?: string;
    email?: string;
    password?: string;
    recoverPasswordQuestion?: string;
    recoverPasswordAnswer?: string;

    // Extras;
    inviteToken?: string;
    newPasswordConfirmation?: string;
}

export interface iUser {
    userId: string;
    fullName: string;
    email: string;
    role: USER_ROLE_ENUM;
    status: boolean;
    createdDate: Date;
    refreshTokenExpirationDate: string;
}

export interface iUserPaginated {
    output: iUser[];
    count: number;
}