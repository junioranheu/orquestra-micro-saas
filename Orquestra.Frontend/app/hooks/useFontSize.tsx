import SYSTEM from '@/app/consts/system';
import { useEffect } from 'react';

export default function useFontSize(): void {

    useEffect(() => {
        const stored = localStorage.getItem(SYSTEM.LOCAL_STORAGE_USER_FONT_SIZE);
        const size = stored ? Number(stored) : 100;

        document.documentElement.style.setProperty('--user-font-size', `${size}%`);
        document.documentElement.style.fontSize = `${size}%`;
    }, []);

}