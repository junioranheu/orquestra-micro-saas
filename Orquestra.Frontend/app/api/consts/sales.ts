import { BASE } from '@/app/api/fetch';

const controller = 'api/Sales';

export const CONSTS_SALES = {
    getChart: `${BASE}/${controller}/GetChart`
};

export interface iSalesChartOutput {
    type: string;
    title: string;
    description: string;
    value: number;
    date?: Date | null;
}