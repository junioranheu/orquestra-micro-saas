import { Guid } from 'guid-typescript';
import { Dispatch, JSX, KeyboardEventHandler, ReactNode, SetStateAction, useEffect, useState } from 'react';
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
    setSelectedOption?: Dispatch<SetStateAction<iDropdownOption | null>> | Dispatch<SetStateAction<iDropdownOption>> | Dispatch<SetStateAction<iDropdownOption[] | null>> | Dispatch<SetStateAction<iDropdownOption[]>>;
    className?: string;
    showDefaultOption0?: boolean;
    placeholder?: string;
    isDisabled?: boolean;
    isSearchable?: boolean;
    isClearable?: boolean;
    isObligatory?: boolean;
    showAction?: boolean;
    actionLabel?: string;
    actionLabelIcon?: JSX.Element | null;
    onActionClick?: () => void;
    handleKeyDown?: KeyboardEventHandler<HTMLInputElement>;
    formatOptionLabel?: (option: iDropdownOption) => ReactNode;
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
    isObligatory = false,
    showAction = false,
    actionLabel = 'Ver mais',
    actionLabelIcon = null,
    onActionClick,
    handleKeyDown,
    formatOptionLabel,
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
            (setSelectedOption as Dispatch<SetStateAction<any>> | undefined)?.(e as iDropdownOption[]);
        } else {
            // console.log('handleChange/single', e);
            (setSelectedOption as Dispatch<SetStateAction<any>> | undefined)?.((e ?? null) as iDropdownOption | null);
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
        menuPortal: (base: any) => ({
            ...base,
            zIndex: 9999,
        }),
        option: (base: any, { isFocused, isSelected }: any) => ({
            ...base,
            fontSize: '0.875rem',
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
            {
                title && (
                    <span className={styles.title}>
                        <span dangerouslySetInnerHTML={{ __html: title }}></span> {isObligatory && <span className={styles.obligatory}>*</span>}

                        {
                            showAction && (
                                <a onClick={onActionClick} className={styles.action}>
                                    {
                                        actionLabelIcon && actionLabelIcon
                                    }

                                    {actionLabel}
                                </a>
                            )
                        }
                    </span>
                )
            }

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
                onKeyDown={handleKeyDown}
                formatOptionLabel={formatOptionLabel ? (option) => formatOptionLabel(option as iDropdownOption) : undefined}
            />
        </div>
    )
}