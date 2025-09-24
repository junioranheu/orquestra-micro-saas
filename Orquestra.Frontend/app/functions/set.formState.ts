import { iDropdownOption } from '@/app/components/input/drop-down';
import { ChangeEvent, Dispatch, SetStateAction } from 'react';

// Helpers para os inputs;
export function handleInputFormStateChange(e: ChangeEvent<HTMLInputElement>, setForm: Dispatch<SetStateAction<any>>, isGetWithoutMask: boolean = false): void {
    const { name, value, type, checked } = e.target;
    setForm((prevState: any) => ({ ...prevState, [name]: type === 'checkbox' ? checked : (isGetWithoutMask ? handleGetValueWithoutMask(e) : value) }));
}

export function handleSelectFormStateChange(e: ChangeEvent<HTMLSelectElement>, setForm: Dispatch<SetStateAction<any>>): void {
    const { name, value } = e.target;
    setForm((prevState: any) => ({ ...prevState, [name]: parseInt(value) }));
}

export function handleGetValueWithoutMask(e: ChangeEvent<HTMLInputElement>): string {
    return e.target.value.replace(/[^\d]/g, '');
}

function handleNormalizeDropdownUpdateFormField<T, K extends keyof T>(setForm: Dispatch<SetStateAction<T>>, field: K, value: T[K]): void {
    setForm(prevState => ({ ...prevState, [field]: value }));
}

export function handleSetDropdownOption<T>(formData: T, setForm: Dispatch<SetStateAction<T>>, field: keyof T | string): Dispatch<SetStateAction<any>> {
    return (value: SetStateAction<any>) => {
        handleNormalizeDropdownUpdateFormField(setForm, field as keyof T, typeof value === 'function' ? (value as Function)(formData[field as keyof T] as any) : value);
    }
}

// Helpers extras;
export function handleClearFormData<T>(setForm: Dispatch<SetStateAction<T>>) {
    // @ts-ignore
    setForm((prevState) => ({ ...Object.keys(prevState).reduce((acc, key) => ({ ...acc, [key]: null }), {}) }));
}

export interface iFormDataLoopResult {
    json: any;
    url: string;
}

export function handleLoopFormData(formData: any, dropDownWhichValue: 'value' | 'label' = 'value', log: boolean = false, hideNull: boolean = true): iFormDataLoopResult {
    let jsonObject: any = {};
    let urlString = '';

    if (typeof formData === 'object' && formData !== null) {
        if (Array.isArray(formData)) {
            for (const item of formData) {
                const { json, url } = handleLoopFormData(item, dropDownWhichValue, log, hideNull);
                jsonObject.push(json);
                urlString += url;
            }
        } else {
            for (const key in formData) {
                if (!hideNull || formData[key] !== null) {
                    // const item = `${key}: ${formData[key]}`;
                    // log && console.log(item);

                    let data = formData[key];

                    if (isDropdownOption(formData[key])) {
                        data = dropDownWhichValue === 'value' ? formData[key].value : formData[key].label;
                    }

                    jsonObject[key] = data;
                    urlString += `${key}=${encodeURIComponent(data)}&`;
                }
            }
        }
    } else {
        if (!hideNull) {
            console.log(formData);
        }
    }

    // Remover o último '&';
    if (urlString.length > 0) {
        urlString = urlString.slice(0, -1);
    }

    return { json: jsonObject, url: urlString };

    function isDropdownOption(obj: any): obj is iDropdownOption {
        return typeof obj === 'object' && obj !== null && 'value' in obj;
    }
}

export function handleParseQueryString(queryString: string): Record<string, string> {
    const result: Record<string, string> = {};

    if (!queryString) {
        return result;
    }

    const cleanedQueryString = queryString.startsWith('?') ? queryString.slice(1) : queryString;
    const pairs = cleanedQueryString.split('&');

    pairs.forEach(pair => {
        const [key, value] = pair.split('=');
        result[decodeURIComponent(key)] = decodeURIComponent(value);
    });

    return result;
}

/**
 * Encontra uma opção de dropdown por label ou value
 * @param options Array de opções do dropdown
 * @param key O valor que você quer buscar (pode ser label ou value)
 * @param by 'label' ou 'value' (opcional, default: 'value')
 * @returns A opção encontrada ou undefined
 */
export function handleFindDropdownOption<T>(options: iDropdownOption<T>[], key: string | T, by: 'label' | 'value' = 'value'): iDropdownOption<T> | undefined {
    return options.find(option => {
        if (by === 'label') {
            return option.label === key;
        }

        return option.value === key;
    })
}