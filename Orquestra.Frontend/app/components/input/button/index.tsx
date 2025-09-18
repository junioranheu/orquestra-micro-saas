'use client';
import ROUTES from '@/app/consts/routes';
import Image, { StaticImageData } from 'next/image';
import { useRouter } from 'next/navigation';
import { CSSProperties, JSX, ReactNode, RefObject, cloneElement, useState } from 'react';
import styles from './index.module.scss';

interface iProps {
    label: string;
    url?: (typeof ROUTES)[keyof typeof ROUTES];
    isNewTab?: boolean;
    handleFunction?: ((param?: any) => void) | null;
    svg_component?: ReactNode;
    svg_staticImageData?: StaticImageData | null;
    icone_feather?: JSX.Element;
    refBtn?: RefObject<HTMLButtonElement | null>;
    isDisabled?: boolean;
    isStyleSimple?: boolean;
    isBig?: boolean;
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
    icone_feather,
    refBtn,
    isDisabled = false,
    isStyleSimple = false,
    isBig = false,
    classes,
    style = {}
}: iProps) {

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
                    handleFuncao(e);
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
            className={`${styles.button} ${classes} ${isStyleSimple && styles.btnSimple} ${isBig && styles.big}`}
            style={style}
            onClick={(e) => handleClick(e)}
            ref={refBtn}
            disabled={isDisabledInternal || isDisabled}
        >
            {icone_feather && cloneElement(icone_feather)}
            {/* @ts-ignore */}
            {svg_component && cloneElement(svg_component, svgDefaultProps)}
            {svg_staticImageData && <Image src={svg_staticImageData} width={20} alt='' />}
            {label}
        </button>
    )
}