import { handleCheckHasPermission } from '@/app/functions/check.permission';
import useUserContext from '@/app/hooks/contexts/useUserContext';
import { useRouter } from 'next/navigation';
import { useEffect, useState } from 'react';

export default function useCheckHasPermission(rolesRequired: string[]): boolean {

    const router = useRouter();
    const [auth, _] = useUserContext();
    const [hasPermission, setHasPermission] = useState<boolean>(false);

    const [loading, setLoading] = useState<boolean>(true);

    useEffect(() => {
        setLoading(false);
    }, [auth?.isAuth]);

    useEffect(() => {
        if (!loading) {
            const hasPermission = handleCheckHasPermission(router, rolesRequired);
            // console.log('hasPermission', hasPermission);

            setHasPermission(hasPermission);
        }
    }, [loading, router, rolesRequired]);

    return hasPermission;

}