import { BASE } from '@/app/api/fetch';
import { Guid } from 'guid-typescript';

const controller = 'api/Sales';

export const CONSTS_SALES = {
    getChart: `${BASE}/${controller}/GetChart`
};

export interface iSalesOutput {
    table: iSalesTableOutput[];
    chart: iSalesChartOutput[];
}

export interface iSalesTableOutput {
    id: Guid;
    type: string;
    title: string;
    description: string;
    value: number;
    date?: string | null;
}

export interface iSalesChartOutput {
    type: string;
    color: string;
    items: {
        dateTime: string;
        value: number;
    }[];
}