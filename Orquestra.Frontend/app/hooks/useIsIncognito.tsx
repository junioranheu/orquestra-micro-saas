import detectIncognito from 'detectincognitojs';
import { useEffect, useState } from 'react';

export default function useIsIncognito() {

    const [isIncognito, setIsIncognito] = useState(false);

    useEffect(() => {
        detectIncognito().then((result) => {
            setIsIncognito(result.isPrivate);
        }).catch(() => {
            setIsIncognito(false);
        });
    }, []);

    return isIncognito;
}