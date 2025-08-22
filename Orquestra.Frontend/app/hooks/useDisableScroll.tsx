import { useEffect } from 'react';

export default function useDisableScroll(disable: boolean = true): void {

    useEffect(() => {
        handleDisableScroll(disable);

        return () => {
            handleDisableScroll(false);
        };
    }, [disable]);

}

export function handleDisableScroll(disable: boolean = true) {
    const html = document.documentElement;

    if (disable) {
        html.style.overflow = 'hidden';
        document.body.style.overflow = 'hidden';
        return;
    }

    html.style.overflow = '';
    document.body.style.overflow = '';
}