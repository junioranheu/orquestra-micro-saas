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
    const body = document.body;
    const children = document.querySelector<HTMLElement>('.children');

    if (disable) {
        // Trava scroll do html e body;
        html.style.overflow = 'hidden';
        body.style.overflow = 'hidden';

        if (children) {
            // força overflow-y hidden;
            const style = document.createElement('style');
            style.innerHTML = `
                .children {
                    overflow-y: hidden !important;
                }
            `;

            document.head.appendChild(style);

            // força overflow-y hidden com !important;
            children.style.setProperty('overflow-y', 'hidden', 'important');
        }
    } else {
        // Libera scroll do html e body;
        html.style.overflow = '';
        body.style.overflow = '';

        if (children) {
            children.style.setProperty('overflow-y', '', 'important');
        }
    }
}