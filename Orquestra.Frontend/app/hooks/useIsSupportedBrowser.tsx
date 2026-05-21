import { handleCheckIsSupportedBrowser } from '@/app/functions/check.browser';
import { useEffect, useState } from 'react';

export default function useIsSupportedBrowser() {

    const [isSupported, setIsSupported] = useState(true);

    useEffect(() => {
        setIsSupported(handleCheckIsSupportedBrowser());
    }, []);

    return isSupported;
}