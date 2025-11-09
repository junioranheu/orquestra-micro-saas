import SYSTEM from '@/app/consts/system';
import { createContext, Dispatch, ReactNode, SetStateAction, useEffect, useState } from 'react';

type GlobalContextType = {
    isRequestLoading: boolean;
    setIsRequestLoading: Dispatch<SetStateAction<boolean>>;

    showChatbot: boolean;
    setShowChatbot: Dispatch<SetStateAction<boolean>>;

    isOpenChatbot: boolean;
    setIsOpenShowChatbot: Dispatch<SetStateAction<boolean>>;

    showExpandedSidebar: boolean;
    setShowExpandedSidebar: Dispatch<SetStateAction<boolean>>;
};

export const GlobalContext = createContext<GlobalContextType | undefined>(undefined);

export function GlobalContextProvider({ children }: { children: ReactNode }) {

    const [isRequestLoading, setIsRequestLoading] = useState<boolean>(false);
    const [showChatbot, setShowChatbot] = useState<boolean>(true);
    const [isOpenChatbot, setIsOpenShowChatbot] = useState<boolean>(false);
    const [showExpandedSidebar, setShowExpandedSidebar] = useState<boolean>(true);

    useEffect(() => {
        const valueChatbot = localStorage.getItem(SYSTEM.LOCAL_STORAGE_SHOW_CHATBOT);
        setShowChatbot(valueChatbot === null ? true : valueChatbot === 'true');

        const valueExpandedSidebar = localStorage.getItem(SYSTEM.LOCAL_STORAGE_SHOW_EXPANDED_SIDEBAR);
        setShowExpandedSidebar(valueExpandedSidebar === null ? true : valueExpandedSidebar === 'true');
    }, []);

    return (
        <GlobalContext.Provider value={{
            isRequestLoading, setIsRequestLoading,
            showChatbot, setShowChatbot,
            isOpenChatbot, setIsOpenShowChatbot,
            showExpandedSidebar, setShowExpandedSidebar
        }}>
            {children}
        </GlobalContext.Provider>
    )
}