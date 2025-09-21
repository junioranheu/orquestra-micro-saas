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
    const children = document.querySelector<HTMLElement>('.children');

    if (disable) {
        html.style.overflow = 'hidden';
        document.body.style.overflow = 'hidden';

        if (children) {
            children.style.overflow = 'hidden';
        }

        return;
    }

    html.style.overflow = '';
    document.body.style.overflow = '';

    if (children) {
        children.style.overflow = '';
    }
}