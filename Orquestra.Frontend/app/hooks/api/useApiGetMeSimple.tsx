import { CONSTS_AUTH, iMeSimple } from '@/app/api/consts/auth';
import { Fetch } from '@/app/api/fetch';
import { useEffect, useState } from 'react';

export default function useApiGetMeSimple(isFetch: boolean = true): iMeSimple | undefined {

    const [meSimple, setMeSimple] = useState<iMeSimple>();

    useEffect(() => {
        async function handleFetch() {
            const result = await Fetch.get({ url: CONSTS_AUTH.meSimple }) as iMeSimple;
            setMeSimple(result);
        }

        if (isFetch) {
            handleFetch();
        }
    }, [isFetch]);

    return meSimple;

}