import { GlobalContext } from '@/app/contexts/global.context';
import { useContext } from 'react';

export function useIsRequestLoading() {
    const context = useContext(GlobalContext);

    if (!context) {
        throw new Error('useGlobalContext');
    }

    return [context.isRequestLoading, context.setIsRequestLoading] as const;
}