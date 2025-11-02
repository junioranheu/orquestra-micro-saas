import { BASE } from '@/app/api/fetch';
import { UserRoleEnum } from '@/app/enums/userRoleEnum';

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
    role: UserRoleEnum;
    status: boolean;
    createdDate: Date;
    refreshTokenExpirationDate: string;
}

export interface iUserPaginated {
    output: iUser[];
    count: number;
}