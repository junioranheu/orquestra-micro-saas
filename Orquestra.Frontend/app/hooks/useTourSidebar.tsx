import { useTour } from '@reactour/tour';
import { useEffect } from 'react';

export const CONST_TOUR = 'tour';

interface iProps {
    delayMs?: number;
}

export function useTourSidebar({ delayMs = 1150 }: iProps) {

    const { setIsOpen } = useTour();

    useEffect(() => {
        if (typeof window === 'undefined') {
            return;
        }

        const params = new URLSearchParams(window.location.search);

        // Só exibe o tour caso exista o param na url (?tour=true);
        if (params.get(CONST_TOUR) === 'true') {
            setTimeout(() => {
                setIsOpen(true);
            }, delayMs);
        }
    }, [delayMs, setIsOpen]);

}