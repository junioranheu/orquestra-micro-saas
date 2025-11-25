import { CONSTS_UTILITY } from '@/app/api/consts/utility';
import { Fetch } from '@/app/api/fetch';
import { iDropdownOption } from '@/app/components/input/drop-down';
import { useEffect, useState } from 'react';

interface iProps {
    enumName: 'CompanyInvoiceSituationEnum' | 'CompanySituationEnum' | 'CompanyTypeEnum' |
    'CompanyUserRoleEnum' | 'LogTypeEnum' | 'ModuleEnum' |
    'PaymentTypeEnum' | 'ScheduleStatusEnum' | 'UserRoleEnum' |
    'VerificationTypeEnum' | 'PlanTypeEnum' | 'RecoverPasswordQuestionEnum' |
    'ClientFollowUpStatusEnum' | 'QuoteStatusEnum'; // Sempre que necessário, adicionar os novos enums aqui;
}

export default function useApiGetEnum({ enumName }: iProps): iDropdownOption[] | undefined {

    const [list, setList] = useState<iDropdownOption[]>();

    useEffect(() => {
        async function handleFetch() {
            const result = await Fetch.get({ url: `${CONSTS_UTILITY.getEnum}?name=${enumName}` }) as iDropdownOption[];
            setList(result);
        }

        handleFetch();
    }, [enumName]);

    return list;

}