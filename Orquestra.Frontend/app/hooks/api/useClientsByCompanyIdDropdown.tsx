import { CONSTS_CLIENT, iClientPaginated } from '@/app/api/consts/client';
import { Fetch } from '@/app/api/fetch';
import { iDropdownOption } from '@/app/components/input/drop-down';
import { handleTransformArrayToDropdownOptionsGuid } from '@/app/functions/transform.arrayToDropdownOptions';
import { Guid } from 'guid-typescript';
import { useEffect, useState } from 'react';

export function useClientsByCompanyIdDropdown(companyId?: Guid | undefined) {

    const [clientsDropDown, setClientsDropDown] = useState<iDropdownOption[]>([]);
    const [loading, setLoading] = useState(false);

    useEffect(() => {
        if (!companyId) {
            return;
        }

        async function handleFetchClients() {
            setLoading(true);

            try {
                const clients = await Fetch.get({
                    url: `${CONSTS_CLIENT.getAllByCompanyId}?companyId=${companyId}`,
                }) as iClientPaginated;

                if (clients?.count) {
                    const options = handleTransformArrayToDropdownOptionsGuid(
                        clients.output ?? [], 'clientId', ['fullName', 'phone', 'email']);
                    setClientsDropDown(options);
                } else {
                    setClientsDropDown([]);
                }
            } finally {
                setLoading(false);
            }
        }

        handleFetchClients();
    }, [companyId]);

    return { clientsDropDown, loading, };
}