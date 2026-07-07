import { Guid } from 'guid-typescript';
import { Dispatch, JSX, KeyboardEventHandler, ReactNode, SetStateAction, useMemo } from 'react';
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
    selectedOption: unknown | iDropdownOption | iDropdownOption[] | null | undefined;
    setSelectedOption?: Dispatch<SetStateAction<any>>;
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
    formatOptionLabel
}: iProps) {

    const uniqueOptions = useMemo(() => {
        const uniqueOptionsArray = Array.from(new Map(options?.map(option => [getComparableValue(option.value), option])).values()) as iDropdownOption[];
        return uniqueOptionsArray;
    }, [options]);

    function handleChange(e: SingleValue<iDropdownOption> | MultiValue<iDropdownOption> | null) {
        if (Array.isArray(e)) {
            const values = e.map(option => normalizeStoredValue(option?.value ?? null));
            (setSelectedOption as Dispatch<SetStateAction<any>> | undefined)?.(values as any);
            return;
        }

        const singleValue = e as SingleValue<iDropdownOption> | null;
        const value = normalizeStoredValue(singleValue?.value ?? null);
        (setSelectedOption as Dispatch<SetStateAction<any>> | undefined)?.(value as any);
    }

    function normalizeStoredValue(value: unknown): unknown {
        if (value === null || value === undefined) {
            return null;
        }

        if (Array.isArray(value)) {
            return value.map(item => normalizeStoredValue(item));
        }

        if (typeof value === 'object') {
            const record = value as Record<string, unknown>;
            if (record.value !== undefined && record.value !== null) {
                return normalizeStoredValue(record.value);
            }

            if (record.id !== undefined && record.id !== null) {
                return normalizeStoredValue(record.id);
            }
        }

        return value;
    }

    function getComparableValue(value: unknown): string {
        if (value === null || value === undefined) {
            return '';
        }

        if (typeof value === 'object') {
            if (Array.isArray(value)) {
                return value.map(item => getComparableValue(item)).join('|');
            }

            const recordValue = (value as Record<string, unknown>).value;

            if (recordValue !== undefined && recordValue !== null) {
                return getComparableValue(recordValue);
            }

            const recordId = (value as Record<string, unknown>).id;

            if (recordId !== undefined && recordId !== null) {
                return getComparableValue(recordId);
            }

            return String(value);
        }

        return String(value);
    }

    function getMappedOption(value: unknown): iDropdownOption | undefined {
        if (value === null || value === undefined) {
            return undefined;
        }

        const comparableValue = getComparableValue(value);

        if (typeof value === 'object' && value !== null && 'value' in value && 'label' in value) {
            const option = value as iDropdownOption;
            const matchedOption = uniqueOptions.find(candidate => getComparableValue(candidate.value) === comparableValue);

            return matchedOption ?? option;
        }

        return uniqueOptions.find(candidate => getComparableValue(candidate.value) === comparableValue);
    }

    function normalizeSelectedValue(value: iProps['selectedOption']): iDropdownOption | iDropdownOption[] | null {
        if (isMultiple) {
            if (!Array.isArray(value)) {
                return [];
            }

            return value.map(item => {
                if (item === null || item === undefined) {
                    return null;
                }

                const mappedOption = getMappedOption(item);
                return mappedOption ?? { value: item as any, label: String(item) };
            }).filter((item): item is iDropdownOption => Boolean(item));
        }

        if (Array.isArray(value)) {
            return normalizeSelectedValue(value[0]) as iDropdownOption | null;
        }

        const mappedOption = getMappedOption(value);

        return mappedOption ?? (value === null || value === undefined ? null : { value: value as any, label: String(value) });
    }

    const selectValue = normalizeSelectedValue(selectedOption);

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
                value={selectValue}
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