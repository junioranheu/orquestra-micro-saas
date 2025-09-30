import { BASE } from '@/app/api/fetch';
import { Guid } from 'guid-typescript';

const controller = 'api/Company';

export const CONSTS_COMPANY = {
    post: `${BASE}/${controller}`,
    get: `${BASE}/${controller}`,
    getAll: `${BASE}/${controller}/GetAll`,
    getAllByUserId: `${BASE}/${controller}/GetAllByUserId`,
    verify: `${BASE}/${controller}/Verify`,
    getModules: `${BASE}/${controller}/Module`,
    updateModules: `${BASE}/${controller}/Module`,
    getModulesInfo: `${BASE}/${controller}/Module/GetInfo`
};

// Essa interface é uma mistura de CompanyOutput e CompanySimpleOutput do back-end. Boa sorte!
export default interface iCompanySimpleOutput {
    companyId: Guid;
    name: string;
    email: string;
    companyType: string;
    companySituation: string;
    logoUrl?: string;
    color?: string;
    planType: string;
    planStartDate: Date;
    planEndDate: Date;
    modules: string[];

    // Extras;
    modulesStr: string[];
    userModules: string[];
    userModulesStr: string[];
    isAdm: boolean;
    amountOfClients: number;
    companyTypeStr: string;
    companySituationStr: string;
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