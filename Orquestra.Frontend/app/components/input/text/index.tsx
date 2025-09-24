import Image, { StaticImageData } from 'next/image';
import { CSSProperties, ChangeEvent, FocusEventHandler, KeyboardEventHandler, ReactNode, cloneElement, useEffect, useState } from 'react';
import { IMaskInput } from 'react-imask';
import styles from './index.module.scss';

interface iProps {
    objectFormData: [any, string];
    title?: string;
    type?: 'text' | 'password' | 'email' | 'number' | 'datetime-local' | 'date' | 'time';
    classes?: string;
    style?: CSSProperties;
    placeholder?: string;
    isDisabled?: boolean;
    minChar?: number;
    mask?: string;
    showIcon?: boolean;
    svg_component?: ReactNode;
    svg_staticImageData?: StaticImageData | null;

    handleChange?: (e: ChangeEvent<HTMLInputElement>) => void;
    handleExtraValidation?: () => boolean | null;
    handleKeyDown?: KeyboardEventHandler<HTMLInputElement> | undefined;
    handleBlur?: FocusEventHandler<HTMLInputElement> | undefined;
}

export default function InputMaskCustom({
    objectFormData, title = '', type = 'text', classes = '', style = {}, placeholder,
    isDisabled = false, minChar = 0, mask = '', showIcon = false, svg_component = null,
    svg_staticImageData = null,
    handleChange, handleExtraValidation = () => null, handleKeyDown = () => null, handleBlur = () => null
}: iProps) {

    const form_formData = objectFormData[0];
    const prop_formData = objectFormData[1];
    const value_formData = form_formData[prop_formData] ?? '';

    const [showErrorIcon, setshowErrorIcon] = useState<boolean>(true);
    const svgDefaultProps = { width: 20 };

    useEffect(() => {
        function handleCheckErrorIcon() {
            if (!value_formData) {
                setshowErrorIcon(true);
                return false;
            }

            // console.log(controleInterno, controleInterno?.length, minChar);
            if (value_formData?.toString()?.length >= (minChar ?? 0)) {
                setshowErrorIcon(false);
                return;
            }

            setshowErrorIcon(true);
        }

        // Caso existe uma validação extra a ser feita, essa é a hora;
        if (handleExtraValidation) {
            const isExtraValidationOk = handleExtraValidation();

            // Exibir o ícone de sucesso ou não, com base, também, na verificação extra;
            setshowErrorIcon(!isExtraValidationOk);
            return;
        }

        // Exibir o ícone de sucesso sem tomar como base a verificação extra;
        handleCheckErrorIcon();
    }, [value_formData, minChar, handleExtraValidation]);

    return (
        <div className={styles.main}>
            {
                (title || showIcon) && (
                    <div className={styles.wrapperTop}>
                        {
                            title && <span className={styles.title}>{title}</span>
                        }

                        {
                            showIcon && (
                                showErrorIcon ? (
                                    <span className={styles.errorIcon}>✕</span>
                                ) : (
                                    <span className={`${styles.successIcon} animate__animated animate__headShake`}>✔</span>
                                )
                            )
                        }
                    </div>
                )
            }

            <div className={`${styles.wrapper} ${((svg_component || svg_staticImageData) && styles.wrapSvg)}`}>
                {/* @ts-ignore */}
                {svg_component && cloneElement(svg_component, svgDefaultProps)}
                {svg_staticImageData && <Image src={svg_staticImageData} alt='' />}

                <IMaskInput
                    mask={mask}
                    type={type}
                    className={classes}
                    style={style}
                    placeholder={placeholder}
                    name={prop_formData}
                    readOnly={isDisabled}
                    disabled={isDisabled}
                    value={value_formData}
                    onKeyDown={handleKeyDown}
                    onBlur={handleBlur}
                    inputRef={(input) => {
                        if (!input) {
                            return;
                        }

                        input.oninput = (e: Event) => {
                            const target = e.target as HTMLInputElement;
                            // console.log('onChange real', target.value);

                            if (handleChange) {
                                handleChange({ target } as ChangeEvent<HTMLInputElement>);
                            }
                        };
                    }}
                />
            </div>
        </div>
    )
}