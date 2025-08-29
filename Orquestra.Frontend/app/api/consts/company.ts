import { Guid } from 'guid-typescript';

export default interface iCompany {
    companyId: Guid;
    name: string;
    email: string;
    companyType: any; // TO DO;
    companySituation: any; // TO DO;
    planType: any; // TO DO;
    planStartDate: Date;
    planEndDate: Date;
}