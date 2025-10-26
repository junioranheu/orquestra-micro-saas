import ImgLoading from '@/app/assets/gif/loading-cat.gif';
import Tippy from '@tippyjs/react';
import Image from 'next/image';
import { ReactNode } from 'react';
import styles from './index.module.scss';

interface iProps {
    width?: number;
    isCentralized?: boolean;
    tippyContent?: string | ReactNode;
    tippyPlacement?: 'top' | 'bottom' | 'left' | 'right';
}

export default function LoadingGif({ width = 64, isCentralized = true, tippyContent, tippyPlacement = 'top' }: iProps) {
    return (
        <Tippy content={tippyContent} placement={tippyPlacement} interactive={true}>
            <div
                className={styles.loader}
                style={{ margin: (isCentralized ? 'auto' : 'none') }}
            >
                <Image src={ImgLoading} width={width} height={width} alt='' />
            </div>
        </Tippy>
    )
}