import { BASE } from '@/app/api/fetch';
import { UserRoleEnum } from '@/app/enums/userRoleEnum';
import iCompanyOutput from './company';

const controller = 'api/Auth';

export const CONSTS_AUTH = {
    auth: `${BASE}/${controller}`,
    meSimple: `${BASE}/${controller}/Me/Simple`,
    me: `${BASE}/${controller}/Me`,
    meCurrentMainCompany: `${BASE}/${controller}/Me/CurrentMainCompany`,
    meModules: `${BASE}/${controller}/Me/Modules`,
    logout: `${BASE}/${controller}`,
    sendRecoverPassword: `${BASE}/${controller}/Send/RecoverPassword`,
    verifyRecoverPassword: `${BASE}/${controller}/Verify/RecoverPassword`
};

export interface iMeSimple {
    isAuth: boolean;
    userId: string;
    userName: string;
    email: string;
    roles: string[];
    rolesStr: UserRoleEnum[];
}

export interface iMe extends iMeSimple {
    currentMainCompany: iCompanyOutput;
    tokenExpirationDate: Date;
    refreshTokenExpirationDate: Date;
    isUserAdmOfCurrentMainCompany: boolean;
}