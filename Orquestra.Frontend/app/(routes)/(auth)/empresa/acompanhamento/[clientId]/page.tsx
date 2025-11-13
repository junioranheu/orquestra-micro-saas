'use client';
import iClient, { CONSTS_CLIENT } from '@/app/api/consts/client';
import { Fetch } from '@/app/api/fetch';
import TemplatePageHeader from '@/app/components/template/template-page-header';
import { handleGetFirstName } from '@/app/functions/get.formatUserName';
import useTitle from '@/app/hooks/useTitle';
import { useParams, useRouter } from 'next/navigation';
import { useEffect, useState } from 'react';

export default function EmpresaAcompanhamentoCliente() {

    useTitle('Acompanhamento');

    const router = useRouter();
    const params = useParams();
    const query = params.clientId;
    const [client, setClient] = useState<iClient | null>();

    useEffect(() => {
        async function handleFetch() {
            const clientId = query?.toString();
            const client = await Fetch.get({ url: `${CONSTS_CLIENT.get}?clientId=${clientId}` }) as iClient;

            setTimeout(() => {
                setClient(client);
            }, 1000);
        }

        handleFetch();
    }, [query, router]);

    if (!client) {
        return (
            <TemplatePageHeader title='Carregando acompanhamento...' isLoading={true}></TemplatePageHeader>
        )
    }

    return (
        <TemplatePageHeader title={`Acompanhamento • ${handleGetFirstName(client.fullName)}`}>
            <h1>CLIENTE {client.fullName}</h1>
        </TemplatePageHeader>
    )
}