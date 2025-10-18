import { BASE } from '@/app/api/fetch';

const controller = 'api/Utility';

export const CONSTS_UTILITY = {
    getBuildVersion: `${BASE}/${controller}/GetBuildVersion`,
    getState: `${BASE}/${controller}/GetState`,
    getCity: `${BASE}/${controller}/GetCity`,
    getCountry: `${BASE}/${controller}/GetCountry`,
    getEnum: `${BASE}/${controller}/GetEnum`,
    getPlanType: `${BASE}/${controller}/GetPlanType`
};

export interface iBuildVersion {
    buildVersion: string;
    assemblyName: string;
    configuration: string;
}

export interface iPlanType {
    planDurationDays: number;
    planDurationDaysFree: number;
    plans: {
        planType: number;
        planTypeName: string;
        planTypeDescription: string;
        price: number;
        schedulingLimit: number;
        description: string;
        perks: string[];
        durationDays: number;
    }[];
}