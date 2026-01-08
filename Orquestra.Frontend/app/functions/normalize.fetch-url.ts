import { iFormDataLoopResult } from './set.formState';

export function handleNormalizeFetchUrl(url: string, data: iFormDataLoopResult, keepInitialIdParams: boolean = true) {
    const [baseUrl, originalQuery] = url.split('?');

    const originalParams = new URLSearchParams(originalQuery ?? '');
    const filterParams = new URLSearchParams(data?.url ?? '');

    // 🔒 Identifica SOMENTE o PRIMEIRO parâmetro da query;
    const firstParamEntry = Array.from(originalParams.entries())[0];
    const immutableFirstIdKey = keepInitialIdParams && firstParamEntry && firstParamEntry[0].endsWith('Id') ? firstParamEntry[0] : null;

    // 1️⃣ Remove tudo que não for Id (comportamento atual);
    if (keepInitialIdParams) {
        for (const key of Array.from(originalParams.keys())) {
            if (!key.endsWith('Id')) {
                originalParams.delete(key);
            }
        }
    } else {
        originalParams.forEach((_, key) => originalParams.delete(key));
    }

    // 2️⃣ Limpa filtros vazios;
    for (const [key, value] of Array.from(filterParams.entries())) {
        const isDate = key.toLowerCase().includes('date');
        // console.log('value', value);

        if (isDate && (!value || value === 'undefined' || value === 'null' || value === undefined || value === null)) {
            filterParams.delete(key);
            continue;
        }

        if (!value || !value.trim()) {
            filterParams.delete(key);
        }
    }

    // 3️⃣ Merge FINAL (regra correta);
    for (const [key, value] of Array.from(filterParams.entries())) {
        // 🚫 Só bloqueia o PRIMEIRO param se terminar com Id;
        if (immutableFirstIdKey && key === immutableFirstIdKey) {
            continue;
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