'use client';
import { useEffect, useState } from 'react';

export default function useInjectTailwindCDN() {

    const [isTailwindReady, setIsTailwindReady] = useState<boolean>(false);

    useEffect(() => {
        if (!document.getElementById('tailwind-play')) {
            const s = document.createElement('script');
            s.id = 'tailwind-play';
            s.src = 'https://cdn.tailwindcss.com';
            s.async = false;
            document.head.appendChild(s);
        }

        const timer = setTimeout(() => setIsTailwindReady(true), 1000);

        return () => clearTimeout(timer);
    }, []);

    return isTailwindReady;

}