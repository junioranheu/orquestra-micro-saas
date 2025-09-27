import { CONSTS_AUTH, iMe } from '@/app/api/consts/auth';
import { Fetch } from '@/app/api/fetch';
import { useEffect, useState } from 'react';

interface iProps {
    isFetch?: boolean;
    trigger?: Date | undefined;
}

export default function useApiGetMe({ isFetch = true, trigger = undefined }: iProps): iMe | undefined {

    const [me, setMe] = useState<iMe>();

    useEffect(() => {
        async function handleFetch() {
            const result = await Fetch.get({ url: CONSTS_AUTH.me }) as iMe;
            setMe(result);
        }

        if (isFetch) {
            handleFetch();
        }
    }, [isFetch, trigger]);

    return me;

}