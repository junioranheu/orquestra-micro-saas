import { Fetch } from '@/app/api/fetch';
import { TABLE_PAGINATION_DEFAULT_LIMIT } from '@/app/components/table/generic';
import { Dispatch, SetStateAction, useEffect } from 'react';

interface iProps<T> {
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

export default function useApiRequestToSetterOnUrlChange<T>(options: iProps<T>): void {

    const {
        apiUrlRequest,
        setter,
        index = 1,
        limit = TABLE_PAGINATION_DEFAULT_LIMIT,
        isSelectAll = false,
        allowRequestNull = true,
        trigger = undefined,
        hasPaginationInput = false,
        setIsRequestLoading
    } = options;

    useEffect(() => {
        async function handleApiRequest() {
            if (apiUrlRequest || allowRequestNull) {
                if (setIsRequestLoading) {
                    setIsRequestLoading(true);
                }

                const separator = apiUrlRequest.includes('?') ? '&' : '?';
                const paginationInput = ((hasPaginationInput || isSelectAll) ? `${separator}index=${(index - 1)}&limit=${limit}&isSelectAll=${isSelectAll}` : '')
                const url = `${apiUrlRequest}${paginationInput}`;

                const blobExportName = ''; // TO DO (?);
                const result = await Fetch.get({ url: url, blobExportName: blobExportName }) as T;

                // console.clear();
                // console.log(`hasPaginationInput: ${hasPaginationInput}`, url, result);

                if (!result) {
                    if (setIsRequestLoading) {
                        setIsRequestLoading(false);
                    }

                    setter(undefined);
                    return;
                }

                if (setIsRequestLoading) {
                    setIsRequestLoading(false);
                }

                setter(result);
            }
        }

        handleApiRequest();
    }, [apiUrlRequest, setter, index, limit, isSelectAll, allowRequestNull, trigger, hasPaginationInput, setIsRequestLoading]);

}