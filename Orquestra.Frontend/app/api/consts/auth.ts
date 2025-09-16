import { BASE } from '@/app/api/fetch';
import { UserRoleEnum } from '@/app/enums/userRoleEnum';
import iCompanySimpleOutput from './company';

const controller = 'api/Auth';

export const CONSTS_AUTH = {
    auth: `${BASE}/${controller}`,
    meSimple: `${BASE}/${controller}/Me/Simple`,
    me: `${BASE}/${controller}/Me`,
    logout: `${BASE}/${controller}`
};

export default interface iMe {
    isAuth: boolean;
    userId: string;
    userName: string;
    email: string;
    roles: string[];
    rolesStr: UserRoleEnum[];
    currentMainCompany: iCompanySimpleOutput;
    tokenExpirationDate: Date;
    refreshTokenExpirationDate: Date;
    isUserAdmOfCurrentMainCompany: boolean;
}