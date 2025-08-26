import { GlobalContext } from '@/app/contexts/global.context';
import { useContext } from 'react';

export default function useGlobalContext() {
    const context = useContext(GlobalContext);

    if (!context) {
        throw new Error('useGlobalContext');
    }

    return [context.isRequestLoading, context.setIsRequestLoading] as const;
}