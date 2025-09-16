import { useEffect } from 'react';

export function useOnResize(callback: () => void) {

    useEffect(() => {
        const handleResize = () => {
            callback();
        };

        window.addEventListener('resize', handleResize);

        return () => {
            window.removeEventListener('resize', handleResize);
        };
    }, [callback]);

}