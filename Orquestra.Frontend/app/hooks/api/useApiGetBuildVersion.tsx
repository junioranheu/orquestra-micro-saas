import { CONSTS_UTILITY, iBuildVersion } from '@/app/api/consts/utility';
import { Fetch } from '@/app/api/fetch';
import { useEffect, useState } from 'react';

export default function useApiGetBuildVersion(isFetch: boolean = true): iBuildVersion | undefined {

    const [buildVersion, setBuildVersion] = useState<iBuildVersion>();

    useEffect(() => {
        async function handleFetch() {
            const result = await Fetch.get({ url: CONSTS_UTILITY.getBuildVersion }) as iBuildVersion;
            setBuildVersion(result);
        }

        if (isFetch) {
            handleFetch();
        }
    }, [isFetch]);

    return buildVersion;

}