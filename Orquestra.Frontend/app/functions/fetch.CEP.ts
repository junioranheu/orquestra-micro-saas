import swal from './swal';

export interface iCEPData {
    address: string;
    city: string;
    state: string;
    country: string;
}

export async function handleFetchCEP(cep: string): Promise<iCEPData | null> {
    const cleaned = cep.replace(/\D/g, '');

    if (!/^\d{8}$/.test(cleaned)) {
        swal({ content: `O CEP "${cep}" é inválido. Use o formato 00000-000.`, icon: 'warning' });
        return null;
    }

    try {
        const res = await fetch(`https://viacep.com.br/ws/${cleaned}/json/`);

        if (!res.ok) {
            swal({ content: `Erro HTTP ${res.status} ao buscar o CEP ${cep}.`, icon: 'error' });
            return null;
        }

        const data = await res.json();

        if (data.erro) {
            swal({ content: `O CEP ${cep} não foi encontrado.`, icon: 'warning' });
            return null;
        }

        return {
            address: data.logradouro ?? '',
            city: data.localidade ?? '',
            state: data.uf ?? '',
            country: 'Brasil'
        };
    } catch (error: unknown) {
        swal({ content: `Erro ao buscar o CEP ${cep}: ${(error as Error).message}`, icon: 'error' });
        return null;
    }
}