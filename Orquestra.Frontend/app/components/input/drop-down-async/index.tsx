import { Fetch } from '@/app/api/fetch';
import { iDropdownOption } from '@/app/components/input/drop-down';
import styles from '@/app/components/input/drop-down/index.module.scss';
import { Dispatch, SetStateAction } from 'react';
import { MultiValue, SingleValue } from 'react-select';
import AsyncSelect from 'react-select/async';

interface iProps {
    title?: string;
    multiple?: boolean;
    setSelectedOption: Dispatch<SetStateAction<iDropdownOption | null>> | Dispatch<SetStateAction<iDropdownOption[]>>;
    apiUrl: string;
    isStyleSimple?: boolean;
    placeholder?: string;
}

export default function DropdownAsync({
    title,
    multiple = false,
    setSelectedOption,
    apiUrl,
    isStyleSimple = false,
    placeholder
}: iProps) {

    async function handleLoadOptions(input: string): Promise<iDropdownOption[]> {
        // console.clear();

        if (!input) {
            return [];
        }

        try {
            const response = await Fetch.get({ url: apiUrl }) as iDropdownOption[];
            // console.log(response);

            return response;
        } catch {
            return [];
        }
    }

    function handleInputChange(newValue: string): string {
        return newValue;
    }

    function handleChange(e: SingleValue<iDropdownOption> | MultiValue<iDropdownOption>) {
        if (Array.isArray(e)) {
            // console.log('handleChange/multiple', e);
            const ids = e.map(x => x.value);

            // @ts-ignore;
            setSelectedOption(ids);
        } else {
            // console.log('handleChange/single', e);

            // @ts-ignore;
            setSelectedOption(e);
        }
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