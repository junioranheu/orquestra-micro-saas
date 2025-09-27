import { CONSTS_AUTH, iMe } from '@/app/api/consts/auth';
import { Fetch } from '@/app/api/fetch';
import { useEffect, useState } from 'react';

export default function useApiGetMe(isFetch: boolean = true): iMe | undefined {

    const [me, setMe] = useState<iMe>();

    useEffect(() => {
        async function handleFetch() {
            const result = await Fetch.get({ url: CONSTS_AUTH.me }) as iMe;
            setMe(result);
        }

        if (isFetch) {
            handleFetch();
        }
    }, [isFetch]);

    return me;

}