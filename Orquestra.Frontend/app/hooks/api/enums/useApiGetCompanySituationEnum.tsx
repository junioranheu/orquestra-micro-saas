import { CONSTS_UTILITY } from '@/app/api/consts/utility';
import { Fetch } from '@/app/api/fetch';
import { iDropdownOption } from '@/app/components/input/drop-down';
import { useEffect, useState } from 'react';

export default function useApiGetCompanySituationEnum(): iDropdownOption[] | undefined {

    const [list, setList] = useState<iDropdownOption[]>();

    useEffect(() => {
        async function handleFetch() {
            const result = await Fetch.get({ url: `${CONSTS_UTILITY.getEnum}?name=CompanyTypeEnum` }) as iDropdownOption[];
            setList(result);
        }

        handleFetch();
    }, []);

    return list;

}