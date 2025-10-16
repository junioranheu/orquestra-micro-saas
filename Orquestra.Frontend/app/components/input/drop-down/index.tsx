import { Guid } from 'guid-typescript';
import { Dispatch, SetStateAction, useEffect, useState } from 'react';
import Select, { MultiValue, SingleValue } from 'react-select';
import styles from './index.module.scss';

export interface iDropdownOption<T = string | Guid | number> {
    value: T;
    label: string;
}

interface iProps {
    title?: string;
    options: iDropdownOption[];
    isMultiple?: boolean;
    selectedOption: iDropdownOption | iDropdownOption[] | null | undefined;
    setSelectedOption?: Dispatch<SetStateAction<iDropdownOption | null>> | Dispatch<SetStateAction<iDropdownOption[]>>;
    className?: string;
    showDefaultOption0?: boolean;
    placeholder?: string;
    isDisabled?: boolean;
    isSearchable?: boolean;
    isClearable?: boolean;
    isObligatory?: boolean;
}

export default function Dropdown({
    title,
    options,
    isMultiple = false,
    selectedOption,
    setSelectedOption,
    className = '',
    placeholder,
    isDisabled = false,
    isSearchable = true,
    isClearable = true,
    isObligatory = false
}: iProps) {

    const [uniqueOptions, setUniqueOptions] = useState<iDropdownOption[]>([]);

    useEffect(() => {
        const uniqueOptionsArray = Array.from(new Map(options?.map(option => [option.value, option])).values()) as iDropdownOption[];
        setUniqueOptions(uniqueOptionsArray);
        // console.log(options, uniqueOptionsArray);
    }, [options]);

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
        }),
        option: (base: any, { isFocused, isSelected }: any) => ({
            ...base,
            backgroundColor: isFocused ? 'var(--contrast-light)' : isSelected ? 'var(--contrast)' : 'transparent',
            color: isFocused ? 'var(--black)' : isSelected ? 'var(--white)' : 'var(--black)',
            ':active': {
                ...base[':active'],
                backgroundColor: isSelected ? 'var(--contrast)' : 'var(--contrast-light)'
            }
        })
    };

    return (
        <div className={`${styles.main} ${isDisabled && styles.disabled}`}>
            {title && <span className={styles.title}>{title} {isObligatory && <span className={styles.obligatory}>*</span>}</span>}

            <Select
                isClearable={isClearable}
                isSearchable={isSearchable}
                options={uniqueOptions}
                value={selectedOption}
                onChange={(e) => handleChange(e)}
                isMulti={isMultiple}
                placeholder={(placeholder ?? 'Selecione')}
                styles={customStyle}
                menuPortalTarget={document.body}
                noOptionsMessage={() => 'Nenhuma opção encontrada'}
                className={`${styles.dropdown} ${className && className}`}
                isDisabled={isDisabled}
            />
        </div>
    )
}