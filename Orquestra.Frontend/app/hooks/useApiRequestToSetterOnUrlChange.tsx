import { Fetch } from '@/app/api/fetch';
import { Dispatch, SetStateAction, useEffect } from 'react';

interface iParams<T> {
    apiUrlRequest: string;
    setter: Dispatch<SetStateAction<T | undefined>>;
    index?: number;
    limit?: number;
    isSelectAll?: boolean;
    allowRequestNull?: boolean;
    trigger?: Date | undefined;
    hasPaginationInput?: boolean;
    setIsRequestLoading?: Dispatch<boolean>;
}

export default function useApiRequestToSetterOnUrlChange<T>(options: iParams<T>): void {

    const {
        apiUrlRequest,
        setter,
        index = 1,
        limit = 100,
        isSelectAll = false,
        allowRequestNull = true,
        trigger = undefined,
        hasPaginationInput = true,
        setIsRequestLoading
    } = options;

    useEffect(() => {
        async function handleApiRequest() {
            if (apiUrlRequest || allowRequestNull) {
                setIsRequestLoading && setIsRequestLoading(true);

                const separator = apiUrlRequest.includes('?') ? '&' : '?';
                const paginationInput = (hasPaginationInput ? `${separator}index=${(index - 1)}&limit=${limit}&isSelectAll=${isSelectAll}` : '')
                const url = `${apiUrlRequest}${paginationInput}`;

                const customToken = '';
                const blobExportName = '';

                const result = await Fetch.get(url, customToken, blobExportName) as T;
                // console.clear();
                // console.log(url, result);

                if (!result) {
                    setIsRequestLoading && setIsRequestLoading(false);
                    setter(undefined);
                    return;
                }

                setIsRequestLoading && setIsRequestLoading(false);
                setter(result);
            }
        }

        handleApiRequest();
    }, [apiUrlRequest, setter, index, limit, isSelectAll, allowRequestNull, trigger, hasPaginationInput, setIsRequestLoading]);

}