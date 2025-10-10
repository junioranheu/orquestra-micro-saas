import { iFormDataLoopResult } from './set.formState';

export function handleNormalizeFetchUrl(url: string, data: iFormDataLoopResult) {
    const mark = url.includes('?') ? '&' : '?';
    const normalizedUrl = `${url}${(data?.url ? `${mark}${data?.url}` : '')}`;
    // console.log(normalizedUrl);

    return normalizedUrl;
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
    const newQuery = Array.from(paramMap.entries()).map(([key, value]) => `${encodeURIComponent(key)}=${encodeURIComponent(value)}`).join('&');

    return `${baseUrl}?${newQuery}`;
}