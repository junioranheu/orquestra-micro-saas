import { CSSProperties } from 'react';
import { toast as ToastSonner } from 'sonner';

interface iProps {
    content: string;
    ms?: number;
    title?: string;
    triggerFunction?: () => void;
    isClosable?: boolean;
}

export default function toast(props: iProps): number {

    const { content, ms = 5000, title = '', triggerFunction = () => null, isClosable = true } = props;
    const isSmallScreen = window.innerWidth <= 600;

    const style = {
        background: 'var(--black)',
        borderColor: 'var(--black)',
        borderRadius: 'var(--border-radius)',
        boxShadow: '0 4px 30px rgba(0, 0, 0, 0.25)',
        userSelect: 'none'
    } as CSSProperties;

    const toastId = ToastSonner(title, {
        description: content,
        duration: isClosable ? ms : Infinity,
        position: isSmallScreen ? 'top-left' : 'bottom-right',
        style: style,
        className: 'toastSonner',
        ...(isClosable && {
            action: {
                label: '✕',
                onClick: () => triggerFunction()
            }
        })
    });

    return Number(toastId);
}