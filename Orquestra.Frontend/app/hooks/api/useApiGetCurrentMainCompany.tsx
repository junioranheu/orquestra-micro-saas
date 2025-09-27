import { CONSTS_AUTH } from '@/app/api/consts/auth';
import iCompanySimpleOutput from '@/app/api/consts/company';
import { Fetch } from '@/app/api/fetch';
import { useEffect, useState } from 'react';

interface iProps {
    isFetch?: boolean;
    trigger?: Date | undefined;
}

export default function useApiGetCurrentMainCompany({ isFetch = true, trigger = undefined }: iProps): iCompanySimpleOutput | undefined {

    const [currenMainCompany, setCurrenMainCompany] = useState<iCompanySimpleOutput>();

    useEffect(() => {
        async function handleFetch() {
            const result = await Fetch.get({ url: CONSTS_AUTH.meCurrentMainCompany }) as iCompanySimpleOutput;
            setCurrenMainCompany(result);
        }

        if (isFetch) {
            handleFetch();
        }
    }, [isFetch, trigger]);

    return currenMainCompany;

}