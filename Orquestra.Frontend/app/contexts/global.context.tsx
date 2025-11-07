import SYSTEM from '@/app/consts/system';
import { createContext, Dispatch, ReactNode, SetStateAction, useEffect, useState } from 'react';

type GlobalContextType = {
    isRequestLoading: boolean;
    setIsRequestLoading: Dispatch<SetStateAction<boolean>>;
    showExpandedSidebar: boolean;
    setShowExpandedSidebar: Dispatch<SetStateAction<boolean>>;
};

export const GlobalContext = createContext<GlobalContextType | undefined>(undefined);

export function GlobalContextProvider({ children }: { children: ReactNode }) {

    const [isRequestLoading, setIsRequestLoading] = useState<boolean>(false);
    const [showExpandedSidebar, setShowExpandedSidebar] = useState<boolean>(false);

    useEffect(() => {
        const valueExpandedSidebar = localStorage.getItem(SYSTEM.LOCAL_STORAGE_SHOW_EXPANDED_SIDEBAR);
        setShowExpandedSidebar(valueExpandedSidebar === null ? true : valueExpandedSidebar === 'true');
    }, []);

    return (
        <GlobalContext.Provider value={{
            isRequestLoading, setIsRequestLoading,
            showExpandedSidebar, setShowExpandedSidebar
        }}>
            {children}
        </GlobalContext.Provider>
    )
}