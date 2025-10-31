import { BASE } from '@/app/api/fetch';
import { Guid } from 'guid-typescript';
import iCompanyOutput from './company';

const controller = 'api/IntegrationWhatsApp';

export const CONSTS_INTEGRATION_WHATSAPP = {
    post: `${BASE}/${controller}`,
    get: `${BASE}/${controller}`
};

export interface iIntegrationWhatsapp {
    integrationWhatsAppId: Guid;
    messageReminderBeforeSchedule: string;
    messageOnScheduleConfirmed?: string;
    messageOnScheduleCanceled?: string;
    messageBeforeScheduleAlert?: string;
    company?: iCompanyOutput;
}