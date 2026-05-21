'use client';
import Tippy from '@/app/components/tool-tip';
import { useRouter } from 'next/navigation';

interface iProps {
    size?: number;
    color?: string;
    className?: string;
    href: string;
    tippyContent: string;
    tippyPlacement?: 'top' | 'bottom' | 'left' | 'right';
}

export default function ArrowUpRight({ size = 18, color = 'var(--main)', className, href, tippyContent, tippyPlacement = 'top' }: iProps) {

    const router = useRouter();

    function handleClick() {
        if (href) {
            router.push(href);
        }
    }

    return (
        <Tippy content={tippyContent} placement={tippyPlacement}>
            <svg
                className={className}
                width={size}
                height={size}
                viewBox='0 0 24 24'
                fill='none'
                stroke={color}
                strokeWidth={2}
                strokeLinecap='round'
                strokeLinejoin='round'
                style={{ cursor: href ? 'pointer' : 'default', outline: 'none', userSelect: 'none' }}
                onClick={handleClick}
            >
                <line x1='7' y1='17' x2='17' y2='7' />
                <polyline points='7 7 17 7 17 17' />
            </svg>
        </Tippy>
    )
}