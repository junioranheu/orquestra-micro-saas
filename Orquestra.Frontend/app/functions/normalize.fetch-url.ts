import { iFormDataLoopResult } from './set.formState';

export function handleNormalizeFetchUrl(url: string, data: iFormDataLoopResult, keepInitialIdParams: boolean = true) {
    const [baseUrl, originalQuery] = url.split('?');

    const originalParams = new URLSearchParams(originalQuery ?? '');
    const filterParams = new URLSearchParams(data?.url ?? '');

    // 1️⃣ Trata params iniciais;
    if (keepInitialIdParams) {
        // Mantém SOMENTE os que terminam com Id;
        for (const key of Array.from(originalParams.keys())) {
            if (!key.endsWith('Id')) {
                originalParams.delete(key);
            }
        }
    } else {
        // Remove tudo (comportamento antigo);
        originalParams.forEach((_, key) => {
            originalParams.delete(key);
        });
    }

    // 2️⃣ Limpa filtros vazios;
    for (const [key, value] of Array.from(filterParams.entries())) {
        const isDate = key.toLowerCase().includes('date');

        if (isDate && (!value || value === 'undefined')) {
            filterParams.delete(key);
            continue;
        }

        if (!value || !value.trim()) {
            filterParams.delete(key);
        }
    }

    // 3️⃣ Merge filtros;
    for (const [key, value] of Array.from(filterParams.entries())) {
        if (keepInitialIdParams && key.endsWith('Id') && originalParams.has(key)) {
            continue; // Nunca sobrescreve Id inicial;
        }

        originalParams.set(key, value);
    }

    const finalQuery = originalParams.toString();

    return finalQuery ? `${baseUrl}?${finalQuery}` : baseUrl;
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