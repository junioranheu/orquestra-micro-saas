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

export default interface iCompanySimpleOutput {
    companyId: Guid;
    name: string;
    email: string;
    companyType: any; // TO DO;
    companySituation: any; // TO DO;
    planType: any; // TO DO;
    planStartDate: Date;
    planEndDate: Date;
    modules: any; // TO DO;
    modulesStr: string[];
    userModules: any; // TO DO;
    userModulesStr: string[];
}

export interface iCalculatePriceModuleCompanyOutput {
    module: any; // TO DO;
    moduleStr: string;
    companyAlreadyHasThisModule: boolean;
    originalPrice: number;
    discountPercentage: number;
    discountedPrice: number;
    proportionalPrice: number;
}