'use client';
import Tag from '@/app/components/tag';
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
            let baseUrl = apiUrlRequest;

            const [path, queryString] = apiUrlRequest.split('?');

            if (queryString) {
                // Tenta achar algo tipo 'companyId=123' ou 'userId=abc';
                const match = queryString.match(/([a-zA-Z]+Id)=[^&]+/);

                if (match) {
                    // Mantém até o primeiro parâmetro que termina com Id;
                    baseUrl = `${path}?${match[0]}`;
                } else {
                    // Se não tem nada com Id, remove tudo após o ?;
                    baseUrl = path;
                }
            }

            setApiUrlBase(baseUrl);
        }

        if (apiUrlRequest) {
            handleGetBaseUrl();
        }
    }, [apiUrlRequest]);

    useEffect(() => {
        function handleCheckSelectedFilters() {
            const data = handleLoopFormData(modalFilterFormData, 'label');

            if (!data.url) {
                setApiUrlRequest(apiUrlBase);
                setFiltersInternal([]);
                return;
            }

            const params = new URLSearchParams(data.url);

            const result = Array.from(params.entries()).map(([key, value]) => ({
                name: decodeURIComponent(key),
                value: decodeURIComponent(value)
            }));

            // console.log('handleCheckSelectedFilters', result);
            setFiltersInternal(result);
        }

        if (apiUrlBase && modalFilterFormData) {
            handleCheckSelectedFilters();
        }
    }, [apiUrlBase, modalFilterFormData, setApiUrlRequest]);

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
                    // console.log(`Removing property '${propertyName}' from filters.`);

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
        const mark = apiUrlBase.includes('?') ? '&' : '?';
        const queryStringNormalized = queryString ? `${mark}${queryString}` : '';
        const url = `${apiUrlBase}${queryStringNormalized}`;
        setApiUrlRequest(url);
    }

    function handleWorkaroundTranslation(content: string): string {
        const dictionary: Record<string, string> = {
            clientId: 'ID do Cliente',
            fullName: 'Nome Completo',
            email: 'E-mail',
            cpf: 'CPF',
            address: 'Endereço',
            addressNumber: 'Número',
            city: 'Cidade',
            state: 'Estado',
            zipCode: 'CEP',
            country: 'País',
            dateOfBirth: 'Data de Nascimento',
            phone: 'Telefone',
            notes: 'Observações',
            companyId: 'ID da Empresa',
            company: 'Empresa',

            companyUserRole: 'Tipo de membro',
            modules: 'Módulos atribuídos'
        };

        if (dictionary[content]) {
            return dictionary[content];
        }

        console.warn(`Nenhuma tradução encontrada para a propriedade "${content}".`);
        return content;
    }

    return (
        <section className={styles.main}>
            {
                filtersInternal?.filter(x => !x.name.includes('Id'))?.map((x: iFilter, i: number) => (
                    <Tag
                        key={i}
                        text={`${handleWorkaroundTranslation(x.name)}: ${x.value}`}
                        handleRemoveFunction={() => handleRemoveFilter(x)}
                        tippyContent='Remover filtro'
                    />
                ))
            }
        </section>
    )
}