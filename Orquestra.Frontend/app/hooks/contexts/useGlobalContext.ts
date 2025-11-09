import { GlobalContext } from '@/app/contexts/global.context';
import { useContext } from 'react';

export function useIsRequestLoading() {

    const context = useContext(GlobalContext);

    if (!context) {
        throw new Error('useGlobalContext');
    }

    return [context.isRequestLoading, context.setIsRequestLoading] as const;

}

export function useShowChatbot() {

    const context = useContext(GlobalContext);

    if (!context) {
        throw new Error('useGlobalContext');
    }

    return [context.showChatbot, context.setShowChatbot] as const;

}

export function useIsOpenChatbot() {

    const context = useContext(GlobalContext);

    if (!context) {
        throw new Error('useGlobalContext');
    }

    return [context.isOpenChatbot, context.setIsOpenShowChatbot] as const;

}

export function useShowExpandedSidebar() {

    const context = useContext(GlobalContext);

    if (!context) {
        throw new Error('useGlobalContext');
    }

    return [context.showExpandedSidebar, context.setShowExpandedSidebar] as const;

}