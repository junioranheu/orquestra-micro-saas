import { Fetch } from '@/app/api/fetch';
import { iDropdownOption } from '@/app/components/input/drop-down';
import styles from '@/app/components/input/drop-down/index.module.scss';
import { CSSProperties, Dispatch, SetStateAction, useState } from 'react';
import { MultiValue, SingleValue } from 'react-select';
import AsyncSelect from 'react-select/async';

interface iParams {
    title?: string;
    multiple?: boolean;
    setSelectedOption: Dispatch<SetStateAction<iDropdownOption | null>> | Dispatch<SetStateAction<iDropdownOption[]>>;
    apiUrl: string;
    isStyleSimple?: boolean;
    style?: CSSProperties;
    showDefaultOption0?: boolean;
    placeholder?: string;
}

export default function DropdownAsync({
    title,
    multiple = false,
    setSelectedOption,
    apiUrl,
    isStyleSimple = false,
    placeholder
}: iParams) {

    const [inputValue, setInputValue] = useState<string>('');

    async function handleLoadOptions(input: string): Promise<iDropdownOption[]> {
        // console.clear();

        if (!input) {
            return [];
        }

        try {
            const response = await Fetch.get({ url: apiUrl }) as iDropdownOption[];
            // console.log(response);

            return response;
        } catch (error: unknown) {
            return [];
        }
    }

    function handleInputChange(newValue: string): string {
        setInputValue(newValue);
        return newValue;
    }

    function handleChange(e: SingleValue<iDropdownOption> | MultiValue<iDropdownOption>) {
        // console.log(e);

        // @ts-ignore;
        setSelectedOption(e);
    }

    const customStyle = {
        control: (base: any) => ({
            ...base,
            boxShadow: 'none',
            border: 'none'
        }),
        dropdownIndicator: (base: any) => ({
            ...base,
            padding: 4
        }),
        clearIndicator: (base: any) => ({
            ...base,
            padding: 4
        }),
        valueContainer: (base: any) => ({
            ...base,
            padding: '0px 6px'
        }),
        input: (base: any) => ({
            ...base,
            margin: 0,
            padding: 0
        })
    };

    return (
        <div className={styles.main}>
            {title && <span className={styles.title}>{title}</span>}

            <AsyncSelect
                defaultOptions
                isClearable={true}
                isSearchable={true}
                cacheOptions={true}
                loadOptions={handleLoadOptions}
                onInputChange={handleInputChange}
                onChange={handleChange}
                isMulti={multiple}
                placeholder={(placeholder ?? 'Selecione')}
                styles={customStyle}
                noOptionsMessage={() => 'Nenhuma opção encontrada'}
                className={`${styles.dropdown} ${isStyleSimple && styles.simple}`}
            />
        </div>
    )
}