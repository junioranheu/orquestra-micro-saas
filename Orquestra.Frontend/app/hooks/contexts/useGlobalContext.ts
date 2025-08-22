import { GlobalContext } from '@/app/contexts/global.context';
import { Dispatch, SetStateAction, useContext } from 'react';

export function useGlobalContextIsLoading(): [boolean, Dispatch<SetStateAction<boolean>>] {
    const context = useContext(GlobalContext);
    const [isRequestLoading, setIsRequestLoading] = [context?.isLoadingContext[0], context?.isLoadingContext[1]] as [boolean, Dispatch<SetStateAction<boolean>>];

    return [isRequestLoading, setIsRequestLoading];
}