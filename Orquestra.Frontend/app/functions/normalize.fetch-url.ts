import { iFormDataLoopResult } from './set.formState';

export default function handleNormalizeFetchUrl(url: string, data: iFormDataLoopResult) {
    const mark = url.includes('?') ? '&' : '?';
    const normalizedUrl = `${url}${(data?.url ? `${mark}${data?.url}` : '')}`;
    // console.log(normalizedUrl);

    return normalizedUrl;
}