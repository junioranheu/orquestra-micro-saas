import Image, { StaticImageData } from 'next/image';
import { CSSProperties, ChangeEvent, Dispatch, FocusEventHandler, KeyboardEventHandler, ReactNode, SetStateAction, cloneElement, useEffect, useState, } from 'react';
import { IMaskInput } from 'react-imask';
import styles from './index.module.scss';

interface iProps<T> {
    fieldName: keyof T;
    formData: T;
    setFormData?: Dispatch<SetStateAction<T>>;

    title?: string;
    type?: 'text' | 'password' | 'email' | 'number' | 'datetime-local' | 'date' | 'time';
    classes?: string;
    style?: CSSProperties;
    placeholder?: string;
    isDisabled?: boolean;
    minChar?: number;
    mask?: string;
    showIcon?: boolean;
    isObligatory?: boolean;
    svg_component?: ReactNode;
    svg_staticImageData?: StaticImageData | null;

    handleExtraValidation?: () => boolean | null;
    handleKeyDown?: KeyboardEventHandler<HTMLInputElement>;
    handleBlur?: FocusEventHandler<HTMLInputElement>;
    handleOnChange?: KeyboardEventHandler<HTMLInputElement>;
}

export default function InputMask<T>({
    fieldName,
    formData,
    setFormData,

    title = '',
    type = 'text',
    classes = '',
    style = {},
    placeholder,
    isDisabled = false,
    minChar = 0,
    mask = '',
    showIcon = false,
    isObligatory = false,
    svg_component = null,
    svg_staticImageData = null,
    handleExtraValidation = () => null,
    handleKeyDown = () => null,
    handleBlur = () => null,
    handleOnChange = () => null
}: iProps<T>) {

    const value_formData = formData?.[fieldName] ?? '';

    const [showErrorIcon, setShowErrorIcon] = useState<boolean>(true);
    const svgDefaultProps = { width: 20 };

    const defaultHandleChange = (e: ChangeEvent<HTMLInputElement>) => {
        if (setFormData) {
            setFormData((prev: T) => ({
                ...prev,
                [fieldName]: e.target.value as T[keyof T]
            }));
        }
    };

    useEffect(() => {
        function handleCheckErrorIcon() {
            if (!value_formData) {
                setShowErrorIcon(true);
                return false;
            }

            if (value_formData?.toString()?.length >= (minChar ?? 0)) {
                setShowErrorIcon(false);
                return;
            }

            setShowErrorIcon(true);
        }

        if (handleExtraValidation) {
            const isExtraValidationOk = handleExtraValidation();
            setShowErrorIcon(!isExtraValidationOk);
            return;
        }

        handleCheckErrorIcon();
    }, [value_formData, minChar, handleExtraValidation]);

    return (
        <div className={styles.main}>
            {
                (title || showIcon) && (
                    <div className={styles.wrapperTop}>
                        {title && (
                            <span className={styles.title}>
                                {title} {isObligatory && <span className={styles.obligatory}>*</span>}
                            </span>
                        )}

                        {
                            showIcon && (showErrorIcon ? (
                                <span className={styles.errorIcon}>✕</span>
                            ) : (
                                <span className={`${styles.successIcon} animate__animated animate__headShake`}>✔</span>
                            ))
                        }
                    </div>
                )
            }

            <div className={`${styles.wrapper} ${(svg_component || svg_staticImageData) && styles.wrapSvg}`}>
                {/* @ts-expect-error: svg_component é dinâmico e pode não ter props compatíveis; */}
                {svg_component && cloneElement(svg_component, svgDefaultProps)}
                {svg_staticImageData && <Image src={svg_staticImageData} alt='' />}

                <IMaskInput
                    key={type}
                    mask={mask}
                    type={type}
                    className={classes}
                    style={style}
                    placeholder={placeholder}
                    name={String(fieldName)}
                    readOnly={isDisabled}
                    disabled={isDisabled}
                    value={value_formData?.toString() ?? ''}
                    onKeyDown={handleKeyDown}
                    onBlur={handleBlur}
                    onChange={handleOnChange}
                    inputRef={(input) => {
                        if (!input) {
                            return;
                        }

                        input.oninput = (e: Event) => {
                            const target = e.target as HTMLInputElement;
                            defaultHandleChange({ target } as ChangeEvent<HTMLInputElement>);
                        };
                    }}
                />
            </div>
        </div>
    )
}