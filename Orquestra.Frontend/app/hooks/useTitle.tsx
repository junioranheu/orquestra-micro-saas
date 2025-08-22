import SYSTEM from '@/app/consts/system';
import { useEffect } from 'react';

export default function useTitle(title: string, hasSuffix: boolean = true): void {

    useEffect(() => {
        document.title = `${title}${(hasSuffix ? ` • ${SYSTEM.NAME}` : '')}`;
    }, [title, hasSuffix]);

}