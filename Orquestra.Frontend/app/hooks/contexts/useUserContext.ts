import { UserContext } from '@/app/contexts/user.context';
import { useContext } from 'react';

export default function useUserContext() {
    const context = useContext(UserContext);

    if (!context) {
        throw new Error('useUserContext');
    }

    return [context.auth, context.setAuth] as const;
}