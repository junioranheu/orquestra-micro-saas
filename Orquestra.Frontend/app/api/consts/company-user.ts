import { BASE } from '@/app/api/fetch';
import { Guid } from 'guid-typescript';
import iCompanyOutput from './company';
import { iUser } from './user';

const controller = 'api/CompanyUser';

export const CONSTS_COMPANY_USER = {
    inviteUser: `${BASE}/${controller}/InviteUser`,
    getAllByCompanyId: `${BASE}/${controller}/GetAllByCompanyId`,
    verify: `${BASE}/${controller}/Verify`,
    updateCurrentMainCompanyUser: `${BASE}/${controller}/UpdateCurrentMainCompanyUser`,
    updateModules: `${BASE}/${controller}/updateModules`
};

export interface iCompanyUser {
    companyUserId: Guid;
    companyId: Guid;
    company: iCompanyOutput;
    userId: Guid;
    user: iUser;
    companyUserRole: string;
    modules: string[] | string;
    isCurrentMainCompanyUser: boolean;
    createdDate: Date;
    inviterUserId: Guid;
    inviterUser: iUser;
    status: boolean;
    isOwner: boolean;
}

export interface iCompanyUserPaginated {
    output: iCompanyUser[];
    count: number;
}