import { useEffect } from 'react';

export default function useDisableScroll(disable: boolean = true): void {

    useEffect(() => {
        let childrenStyleTag: HTMLStyleElement | null = null;

        function handleDisableScroll(disable: boolean = true) {
            const html = document.documentElement;
            const body = document.body;
            const children = document.querySelector<HTMLElement>('.children');

            if (disable) {
                html.style.overflow = 'hidden';
                body.style.overflow = 'hidden';

                if (children && !childrenStyleTag) {
                    childrenStyleTag = document.createElement('style');
                    childrenStyleTag.innerHTML = `
                .children {
                    overflow-y: hidden !important;
                }
            `;
                    document.head.appendChild(childrenStyleTag);
                }

                children?.style.setProperty('overflow-y', 'hidden', 'important');
            } else {
                html.style.overflow = '';
                body.style.overflow = '';

                if (children) {
                    children.style.setProperty('overflow-y', '', 'important');
                }

                if (childrenStyleTag) {
                    childrenStyleTag.remove();
                    childrenStyleTag = null;
                }
            }
        }

        handleDisableScroll(disable);

        return () => {
            handleDisableScroll(false);
        }
    }, [disable]);

}