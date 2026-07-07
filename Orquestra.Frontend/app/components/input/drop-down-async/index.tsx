import { Fetch } from '@/app/api/fetch';
import { iDropdownOption } from '@/app/components/input/drop-down';
import styles from '@/app/components/input/drop-down/index.module.scss';
import { Dispatch, SetStateAction } from 'react';
import { MultiValue, SingleValue } from 'react-select';
import AsyncSelect from 'react-select/async';

interface iProps {
    title?: string;
    isMultiple?: boolean;
    setSelectedOption: Dispatch<SetStateAction<iDropdownOption | null>> | Dispatch<SetStateAction<iDropdownOption[]>>;
    apiUrl: string;
    placeholder?: string;
    isStyleSimple?: boolean;
    isObligatory?: boolean;
}

export default function DropdownAsync({
    title,
    isMultiple = false,
    setSelectedOption,
    apiUrl,
    placeholder,
    isStyleSimple = false,
    isObligatory = false
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

            // @ts-expect-error: o tipo genérico de setSelectedOption não aceita array aqui;
            setSelectedOption(ids);
        } else {
            // console.log('handleChange/single', e);

            // @ts-expect-error: o tipo genérico de setSelectedOption não aceita valor único aqui;
            setSelectedOption(e);
        }
    }
    const customStyle = {
        control: (base: any) => ({
            ...base,
            boxShadow: 'none',
            border: 'none',
            minHeight: 44,
            height: 'auto',
            flexWrap: 'wrap'
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
            padding: '0px 4px',
            flexWrap: 'wrap',
            gap: '0.125rem',
            overflow: 'hidden',
            maxHeight: '9rem'
        }),
        input: (base: any) => ({
            ...base,
            margin: 0,
            padding: 0
        }),
        multiValue: (base: any) => ({
            ...base,
            maxWidth: '100%',
            flexWrap: 'wrap',
            margin: '0 2px 2px 0'
        }),
        multiValueLabel: (base: any) => ({
            ...base,
            whiteSpace: 'normal',
            overflowWrap: 'anywhere'
        }),
        multiValueRemove: (base: any) => ({
            ...base,
            alignSelf: 'center'
        }),
        menu: (base: any) => ({
            ...base,
            zIndex: 9999
        }),
        menuPortal: (base: any) => ({
            ...base,
            zIndex: 9999,
        })
    };

    return (
        <div className={styles.main}>
            {title && <span className={styles.title}>{title} {isObligatory && <span className={styles.obligatory}>*</span>}</span>}

            <AsyncSelect
                defaultOptions
                isClearable={true}
                isSearchable={true}
                cacheOptions={true}
                loadOptions={handleLoadOptions}
                onInputChange={handleInputChange}
                onChange={handleChange}
                isMulti={isMultiple}
                placeholder={(placeholder ?? 'Selecione')}
                styles={customStyle}
                noOptionsMessage={() => 'Nenhuma opção encontrada'}
                className={`${styles.dropdown} ${isStyleSimple && styles.simple}`}
            />
        </div>
    )
}