import { Dispatch, SetStateAction, useEffect, useState } from 'react';
import Select, { MultiValue, SingleValue } from 'react-select';
import styles from './drop-down.module.scss';

export interface iDropdownOption {
    value: number;
    label: string;
}

interface iParams {
    title?: string;
    options: iDropdownOption[];
    multiple?: boolean;
    selectedOption: iDropdownOption | iDropdownOption[] | null;
    setSelectedOption: Dispatch<SetStateAction<iDropdownOption | null>> | Dispatch<SetStateAction<iDropdownOption[]>>;
    className?: string;
    isStyleSimple?: boolean;
    showDefaultOption0?: boolean;
    placeholder?: string;
    isDisabled?: boolean;
    isSearchable?: boolean;
    isClearable?: boolean;
}

export default function Dropdown({
    title,
    options,
    multiple = false,
    selectedOption,
    setSelectedOption,
    className = '',
    isStyleSimple = false,
    placeholder,
    isDisabled = false,
    isSearchable = true,
    isClearable = true
}: iParams) {

    const [uniqueOptions, setUniqueOptions] = useState<iDropdownOption[]>([]);

    useEffect(() => {
        const uniqueOptionsArray = Array.from(new Map(options?.map(option => [option.value, option])).values()) as iDropdownOption[];
        setUniqueOptions(uniqueOptionsArray);
        // console.log(options, uniqueOptionsArray);
    }, [options]);

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
        <div className={`${styles.main} ${isDisabled && styles.disabled}`}>
            {title && <span className={styles.title}>{title}</span>}

            <Select
                isClearable={isClearable}
                isSearchable={isSearchable}
                options={uniqueOptions}
                value={selectedOption}
                onChange={(e) => handleChange(e)}
                isMulti={multiple}
                placeholder={(placeholder ?? 'Selecione')}
                styles={customStyle}
                noOptionsMessage={() => 'Nenhuma opção encontrada'}
                className={`${styles.dropdown} ${isStyleSimple && styles.simple} ${className && className}`}
                isDisabled={isDisabled}
            />
        </div>
    )
}