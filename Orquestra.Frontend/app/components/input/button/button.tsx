'use client';
import Image, { StaticImageData } from 'next/image';
import { useRouter } from 'next/navigation';
import { CSSProperties, ReactNode, RefObject, cloneElement, useState } from 'react';
import styles from './button.module.scss';

interface iParams {
    label: string;
    url?: string;
    isNewTab?: boolean;
    handleFunction?: any | null;
    svg_component?: ReactNode;
    svg_staticImageData?: StaticImageData | null;
    refBtn?: RefObject<HTMLButtonElement | null>;
    isDisabled?: boolean;
    isStyleSimple?: boolean;
    classes?: string;
    style?: CSSProperties;
}

export default function Button({
    label,
    url,
    isNewTab = true,
    handleFunction: handleFuncao,
    svg_component = null,
    svg_staticImageData = null,
    refBtn,
    isDisabled = false,
    isStyleSimple = false,
    classes,
    style = {}
}: iParams) {

    const router = useRouter();
    const [isDisabledInternal, setIsDisabledInternal] = useState<boolean>(false);
    const svgDefaultProps = { width: 20 };

    async function handleClick(e: any) {
        if (!isDisabled) {
            setIsDisabledInternal(true);
        }

        if (!url) {
            if (handleFuncao) {
                try {
                    await handleFuncao(e);
                } catch {
                    setIsDisabledInternal(false);
                }
            }

            if (!isDisabled) {
                setIsDisabledInternal(false);
            }

            return;
        }

        if (isNewTab) {
            window.open(url, '_blank');
        } else {
            router.push(url);
        }

        if (!isDisabled) {
            setIsDisabledInternal(false);
        }
    }

    return (
        <button
            className={`${styles.button} ${classes} ${isStyleSimple && styles.btnSimple}`}
            style={style}
            onClick={(e) => handleClick(e)}
            ref={refBtn}
            disabled={isDisabledInternal || isDisabled}
        >
            {/* @ts-ignore */}
            {svg_component && cloneElement(svg_component, svgDefaultProps)}
            {svg_staticImageData && <Image src={svg_staticImageData} width={20} alt='' />}
            {label}
        </button>
    )
}