'use client';
import Tag from '@/app/components/tag';
import handleNormalizeFetchUrl from '@/app/functions/normalize.fetch-url';
import { handleLoopFormData } from '@/app/functions/set.formState';
import { Dispatch, SetStateAction, useEffect, useState } from 'react';
import styles from './index.module.scss';

interface iProps {
    modalFilterFormData?: any;
    setModalFilterFormData?: Dispatch<SetStateAction<any>>;
    apiUrlRequest: string;
    setApiUrlRequest: Dispatch<SetStateAction<string>>;
}

interface iFilter {
    name: string;
    value: string | number;
}

export default function FiltersSelected({ modalFilterFormData, setModalFilterFormData, apiUrlRequest, setApiUrlRequest }: iProps) {

    const [apiUrlBase, setApiUrlBase] = useState<string>('');
    const [filtersInternal, setFiltersInternal] = useState<iFilter[]>([]);

    useEffect(() => {
        function handleGetBaseUrl() {
            const baseUrl = apiUrlRequest.substring(0, apiUrlRequest.indexOf('?'));
            setApiUrlBase(baseUrl);
        }

        function handleCheckSelectedFilters() {
            console.clear();
            const data = handleLoopFormData(modalFilterFormData, 'label');
            const url = handleNormalizeFetchUrl(apiUrlRequest, data);
            console.log('data', data);
            console.log('url', url);

            const cleanedQueryString = apiUrlRequest.replace(/^\?/, '');
            const cleanedQueryStringAfterQuestionMark = cleanedQueryString.substring(cleanedQueryString.indexOf('?') + 1);
            // console.log('cleanedQueryString', cleanedQueryString);
            // console.log('cleanedQueryStringAfterQuestionMark', cleanedQueryStringAfterQuestionMark);

            if (!cleanedQueryStringAfterQuestionMark) {
                setFiltersInternal([]);
            }

            const keyValuePairs = cleanedQueryStringAfterQuestionMark.split('&');
            const result: iFilter[] = [];

            keyValuePairs.forEach(pair => {
                const [key, value] = pair.split('=');

                if (key && value) {
                    const keyDecoded = decodeURIComponent(key);
                    const valueDeconded = decodeURIComponent(value);

                    result.push({ name: keyDecoded, value: valueDeconded });
                }
            });

            // console.log('handleCheckSelectedFilters', result);
            setFiltersInternal(result);
        }

        if (apiUrlRequest) {
            handleGetBaseUrl();
            handleCheckSelectedFilters();
        }
    }, [apiUrlRequest, modalFilterFormData]);

    function handleRemoveFilter(filterClickedToRemove: iFilter) {
        // Remover filtro visualmente;
        const updatedFilters = filtersInternal.filter(f => f !== filterClickedToRemove);
        setFiltersInternal(updatedFilters);

        // Remover filtro programaticamente;
        if (modalFilterFormData) {
            // console.clear();
            const propertyName = filterClickedToRemove.name;

            // console.log('Clicked:', filterClickedToRemove);
            // console.log('Current filters properties:', Object.keys(modalFilterformData));

            if (modalFilterFormData.hasOwnProperty(propertyName)) {
                if (setModalFilterFormData) {
                    // console.log(`Removing property "${propertyName}" from filters.`);

                    setModalFilterFormData((prevFilters: any) => {
                        if (propertyName in prevFilters) {
                            const value = prevFilters[propertyName];
                            const newValue = Array.isArray(value) ? [] : null;
                            // console.log('newValue', newValue);

                            return { ...prevFilters, [propertyName]: newValue };
                        }

                        return prevFilters;
                    });
                }
            }
        }

        // Recriar query;
        const queryString = updatedFilters.map(f => `${f.name}=${f.value}`).join('&');
        const queryStringNormalized = queryString ? `?${queryString}` : '';
        const url = `${apiUrlBase}${queryStringNormalized}`;
        setApiUrlRequest(url);
    }

    return (
        <section className={styles.main}>
            {
                filtersInternal?.filter(x => !x.name.includes('Id'))?.map((x: iFilter, i: number) => (
                    <Tag
                        key={i}
                        text={`${x.name}: ${x.value}`}
                        handleRemoveFunction={() => handleRemoveFilter(x)}
                        tippyContent='Remover filtro'
                    />
                ))
            }
        </section>
    )
}