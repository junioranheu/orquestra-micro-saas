import { Guid } from 'guid-typescript';
import { Dispatch, SetStateAction, useEffect, useState } from 'react';
import Select, { MultiValue, SingleValue } from 'react-select';
import styles from './index.module.scss';

export interface iDropdownOption<T = Guid | number> {
    value: T;
    label: string;
}

interface iProps {
    title?: string;
    options: iDropdownOption[];
    multiple?: boolean;
    selectedOption: iDropdownOption | iDropdownOption[] | null | undefined;
    setSelectedOption: Dispatch<SetStateAction<iDropdownOption | null>> | Dispatch<SetStateAction<iDropdownOption[]>>;
    className?: string;
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
    placeholder,
    isDisabled = false,
    isSearchable = true,
    isClearable = true
}: iProps) {

    const [uniqueOptions, setUniqueOptions] = useState<iDropdownOption[]>([]);

    useEffect(() => {
        const uniqueOptionsArray = Array.from(new Map(options?.map(option => [option.value, option])).values()) as iDropdownOption[];
        setUniqueOptions(uniqueOptionsArray);
        // console.log(options, uniqueOptionsArray);
    }, [options]);

    // function handleChange(e: SingleValue<iDropdownOption> | MultiValue<iDropdownOption>) {
    //     if (Array.isArray(e)) {
    //         console.log('handleChange/multiple', e);

    //         // @ts-ignore;
    //         setSelectedOption(e as iDropdownOption[]);
    //     } else {
    //         console.log('handleChange/single', e);

    //         // @ts-ignore;
    //         setSelectedOption(e as iDropdownOption | null);
    //     }
    // }

    function handleChange(e: SingleValue<iDropdownOption> | MultiValue<iDropdownOption>) {
        if (Array.isArray(e)) {
            const fixed = e.map(opt => ({
                ...opt,
                value: (opt.value as any).value ?? opt.value
            }));

            console.log('handleChange/multiple', e, 'fixed', fixed);

            // @ts-ignore;
            setSelectedOption(fixed as iDropdownOption[]);
        } else {
            console.log('handleChange/single', e);

            // @ts-ignore;
            setSelectedOption(e as iDropdownOption | null);
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
                className={`${styles.dropdown} ${className && className}`}
                isDisabled={isDisabled}
            />
        </div>
    )
}