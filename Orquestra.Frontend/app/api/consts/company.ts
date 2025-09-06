import { Guid } from 'guid-typescript';

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