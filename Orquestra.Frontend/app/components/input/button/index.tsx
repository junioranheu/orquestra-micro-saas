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
    icon_feather?: JSX.Element | null;
    refBtn?: RefObject<HTMLButtonElement | null>;
    isDisabled?: boolean;
    isBig?: boolean;
    classes?: string;
    style?: CSSProperties;
    styleType?: 'transparent' | 'contrast';
}

export default function Button({
    label,
    url,
    isNewTab = true,
    handleFunction,
    svg_component = null,
    svg_staticImageData = null,
    icon_feather,
    refBtn,
    isDisabled = false,
    isBig = false,
    classes,
    style = {},
    styleType = undefined
}: iProps) {

    const router = useRouter();
    const [isDisabledInternal, setIsDisabledInternal] = useState<boolean>(false);
    const svgDefaultProps = { width: 20 };

    async function handleClick(e: any) {
        if (!isDisabled) {
            setIsDisabledInternal(true);
        }

        if (!url) {
            if (handleFunction) {
                try {
                    handleFunction(e);
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
            className={`${styles.button} ${classes} ${styleType && styles[styleType]} ${isBig && styles.big}`}
            style={style}
            onClick={(e) => handleClick(e)}
            ref={refBtn}
            disabled={isDisabledInternal || isDisabled}
            type='button'
        >
            {icon_feather && cloneElement(icon_feather)}
            {/* @ts-expect-error: svg_component pode não ter props compatíveis; */}
            {svg_component && cloneElement(svg_component, svgDefaultProps)}
            {svg_staticImageData && <Image src={svg_staticImageData} width={20} alt='' />}
            {label}
        </button>
    )
}