import { useEffect, useState } from 'react';

interface iWindowSize {
    width: number;
    height: number;
}

export default function useWindowSize(): iWindowSize {

    const [windowSize, setWindowSize] = useState<iWindowSize>({
        width: 0,
        height: 0,
    });

    useEffect(() => {
        function handleResize() {
            setWindowSize({
                width: window.innerWidth,
                height: window.innerHeight
            });
        }

        if (typeof window !== 'undefined') {
            window.addEventListener('resize', handleResize);

            handleResize();

            return () => { window.removeEventListener('resize', handleResize); };
        }
    }, []);

    return windowSize;

}