import { iFormDataLoopResult } from './set.formState';

export default function handleNormalizeFetchUrl(url: string, data: iFormDataLoopResult) {
    const normalizedUrl = `${url}${(data?.url ? `?${data?.url}` : '')}`;
    // console.log(normalizedUrl);

    return normalizedUrl;
}