import { iFormDataLoopResult } from './set.formState';

export function handleNormalizeFetchUrl(url: string, data: iFormDataLoopResult) {
    // Separa base da query existente (se houver);
    const [baseUrl] = url.split('?');

    // Pega a string de params que veio do data (pode ser '' ou undefined);
    let dataUrl = data?.url ?? '';

    // Normaliza e limpa params vazios;
    if (dataUrl) {
        const params = new URLSearchParams(dataUrl);

        for (const [key, value] of Array.from(params.entries())) {
            const isDate = key.toLowerCase().includes('date');

            // Se for campo de data;
            if (isDate) {
                // E for um campo de data vazio...
                if (value === 'undefined' || value === '' || value === null) {
                    // Simplesmente remove da URL;
                    params.delete(key);
                    continue;
                }
            }

            // Campos comuns → remove se vazio;
            if (!value || !value.trim()) {
                params.delete(key);
            }
        }

        dataUrl = params.toString(); // '' se nada sobrar;
    }

    // Se sobrou algo válido, anexa com ?, senão retorna apenas a base (sem query);
    return dataUrl ? `${baseUrl}?${dataUrl}` : baseUrl;
}

export function handleRemoveDuplicateQueryParams(url: string): string {
    const [baseUrl, queryString] = url.split('?');

    if (!queryString) {
        return url;
    }

    const params = new URLSearchParams(queryString);

    // Map pra garantir que o último valor de cada chave seja mantido;
    const paramMap = new Map<string, string>();

    // Percorre na ordem e sobrescreve duplicados (ficando com o último);
    for (const [key, value] of params.entries()) {
        paramMap.set(key, value);
    }

    // Recria a query string com os últimos valores;
    const newQuery = Array.from(paramMap.entries()).filter(([, value]) => value?.trim()).map(([key, value]) => `${encodeURIComponent(key)}=${encodeURIComponent(value)}`).join('&');

    return `${baseUrl}?${newQuery}`;
}