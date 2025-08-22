import { CSSProperties } from 'react';
import { toast as ToastSonner } from 'sonner';

interface iProps {
    content: string;
    ms?: number;
    title?: string;
    triggerFunction?: () => void;
}

export default function toast(props: iProps) {
    const { content, ms = 5000, title = '', triggerFunction = () => null } = props;

    const isSmallScreen = window.innerWidth <= 801;

    const style = {
        background: 'var(--main)',
        borderColor: 'var(--main)',
        borderRadius: 'var(--border-radius)',
        boxShadow: '0 4px 30px rgba(0, 0, 0, 0.25)',
        color: 'var(--white)',
        userSelect: 'none'
    } as CSSProperties;

    ToastSonner(title, {
        description: `<div>${content}</div>`,
        duration: ms,
        position: isSmallScreen ? 'top-center' : 'bottom-right',
        style: style,
        className: 'toastSonner',
        action: {
            label: '✕',
            onClick: () => {
                triggerFunction && triggerFunction();
            }
        }
    });
}