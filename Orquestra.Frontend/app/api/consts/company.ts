import { BASE } from '@/app/api/fetch';
import { Guid } from 'guid-typescript';
import { iCompanyUser } from './company-user';

const controller = 'api/Company';

export const CONSTS_COMPANY = {
    post: `${BASE}/${controller}`,
    get: `${BASE}/${controller}`,
    put: `${BASE}/${controller}`,
    getAll: `${BASE}/${controller}/GetAll`,
    getAllByUserId: `${BASE}/${controller}/GetAllByUserId`,
    verify: `${BASE}/${controller}/Verify`,
    getModules: `${BASE}/${controller}/Module`,
    updateModules: `${BASE}/${controller}/Module`,
    getModulesInfo: `${BASE}/${controller}/Module/GetInfo`
};

export default interface iCompanyOutput {
    companyId: Guid;

    name: string;
    email: string;
    phone?: string;
    companyType: string;

    address?: string;
    addressNumber?: string;
    city?: string;
    state?: string;
    zipCode?: string;
    country?: string;

    // logo?: number[]; // Entity;
    logoFormFile?: globalThis.File | null; // Input;
    logoBase64?: string; // Output;
    logoContentType?: string;
    color?: string;

    companySituation?: string;
    planStartDate?: Date | string;
    planEndDate?: Date | string;
    modules?: string[];

    // Extras;
    modulesStr?: string[];
    status?: boolean;
    createdDate?: Date;
    companyUsers?: iCompanyUser[];
    amountOfClients?: number;
    companyTypeStr?: string;
    companySituationStr?: string;
    userModules?: string[];
    userModulesStr?: string[];
    isAdm?: boolean;
}

export interface iCalculatePriceModuleCompanyOutput {
    module: string;
    moduleStr: string;
    companyAlreadyHasThisModule: boolean;
    originalPrice: number;
    discountPercentage: number;
    discountedPrice: number;
    proportionalPrice: number;
}