import { BASE } from '@/app/api/fetch';

const controller = 'api/Utility';

export const CONSTS_UTILITY = {
    getBuildVersion: `${BASE}/${controller}/GetBuildVersion`,
    getControllers: `${BASE}/${controller}/GetControllers`,
    getState: `${BASE}/${controller}/GetState`,
    getCity: `${BASE}/${controller}/GetCity`,
    getCountry: `${BASE}/${controller}/GetCountry`,
    getEnum: `${BASE}/${controller}/GetEnum`,
    getPlanType: `${BASE}/${controller}/GetPlanType`,
    getTestServer: `${BASE}/${controller}/GetTestServer`,
};

export interface iBuildVersion {
    buildVersion: string;
    assemblyName: string;
    configuration: string;
}

export interface iPlanTypeOutput {
    planDurationDays: number;
    planDurationDaysFree: number;
    plans: iPlanType[];
}

export interface iPlanType {
    planType: number;
    planTypeName: string;
    planTypeDescription: string;
    price: number;
    schedulingLimit: number;
    description: string;
    perks: string[];
    durationDays: number;
}

export interface iControllerInfo {
    controller: string;
    actions: string[];
}