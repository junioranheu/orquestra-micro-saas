import { useEffect, useState } from 'react';

export function useFakeLoading(min = 1000, max = 1250) {

    const [isLoading, setIsLoading] = useState<boolean>(true);

    useEffect(() => {
        const randomDelay = Math.floor(Math.random() * (max - min + 1)) + min;

        const timer = setTimeout(() => {
            setIsLoading(false);
        }, randomDelay);

        return () => clearTimeout(timer);
    }, [min, max]);

    return isLoading;

}