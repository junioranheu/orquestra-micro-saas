import { CSSProperties } from 'react';
import { toast as ToastSonner } from 'sonner';

interface iProps {
    title?: string;
    content: string;
    ms?: number;
    triggerFunction?: () => void;
    isClosable?: boolean;
    customPosition?: 'top-left' | 'top-right' | 'bottom-left' | 'bottom-right';
}

export default function toast(props: iProps): number {

    const { title = '', content, ms = 5000, triggerFunction = () => null, isClosable = true, customPosition } = props;
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
        position: customPosition ? customPosition : isSmallScreen ? 'top-left' : 'bottom-right',
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