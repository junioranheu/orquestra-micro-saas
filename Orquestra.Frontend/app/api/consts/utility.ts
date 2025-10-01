import { BASE } from '@/app/api/fetch';

const controller = 'api/Utility';

export const CONSTS_UTILITY = {
    getBuildVersion: `${BASE}/${controller}/GetBuildVersion`,
    getState: `${BASE}/${controller}/GetState`,
    getCity: `${BASE}/${controller}/GetCity`,
    getModuleEnum: `${BASE}/${controller}/GetModuleEnum`,
    getEnum: `${BASE}/${controller}/GetEnum`,
};

export interface iBuildVersion {
    buildVersion: string;
    assemblyName: string;
    configuration: string;
}