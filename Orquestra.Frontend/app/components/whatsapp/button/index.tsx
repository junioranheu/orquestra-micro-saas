'use client';
import Tippy from '@/app/components/tool-tip';
import { useMemo } from 'react';
import styles from './index.module.scss';

interface iProps {
    phone: string;
    message?: string;
    tippyContent?: string;
    tippyPlacement?: 'left' | 'right' | 'top' | 'bottom';
    position?: 'bottom-left' | 'bottom-right' | 'top-left' | 'top-right';
}

export default function WhatsappButton({ phone, message, tippyContent = 'Suporte via WhatsApp', tippyPlacement = 'left', position = 'bottom-right' }: iProps) {

    const whatsappUrl = useMemo(() => {
        const text = encodeURIComponent(message ?? '');
        return `https://wa.me/${phone}?text=${text}`;
    }, [phone, message]);

    const positionClass = styles[position];

    return (
        <Tippy content={tippyContent} placement={tippyPlacement}>
            <a
                href={whatsappUrl}
                target='_blank'
                rel='noopener noreferrer'
                className={`${styles.whatsappButton} ${positionClass}`}
            >
                <svg
                    xmlns='http://www.w3.org/2000/svg'
                    width='24'
                    height='24'
                    fill='currentColor'
                    viewBox='0 0 24 24'
                >
                    <path d='M12 0C5.373 0 0 5.373 0 12c0 2.118.55 4.093 1.512 5.828L0 24l6.318-1.658A11.945 11.945 0 0012 24c6.627 0 12-5.373 12-12S18.627 0 12 0zm0 22a9.94 9.94 0 01-5.07-1.383l-.36-.214-3.75.984.998-3.645-.235-.374A9.944 9.944 0 012 12C2 6.486 6.486 2 12 2s10 4.486 10 10-4.486 10-10 10zm5.223-7.854c-.279-.14-1.647-.814-1.903-.907-.256-.093-.443-.14-.63.14-.186.279-.721.907-.885 1.094-.163.186-.326.21-.605.07-.279-.14-1.18-.435-2.247-1.386-.83-.74-1.39-1.653-1.553-1.932-.163-.279-.017-.43.123-.57.127-.127.279-.326.419-.489.14-.163.186-.279.279-.465.093-.186.047-.35-.023-.489-.07-.14-.63-1.521-.863-2.084-.226-.543-.457-.469-.63-.477l-.54-.009c-.163 0-.43.062-.657.31-.226.248-.863.843-.863 2.054s.884 2.382 1.006 2.547c.123.163 1.74 2.653 4.217 3.719.59.254 1.05.405 1.409.519.592.189 1.13.162 1.556.098.475-.071 1.647-.674 1.881-1.324.233-.65.233-1.209.163-1.324-.07-.116-.256-.186-.535-.326z' />
                </svg>
            </a>
        </Tippy>
    )
}