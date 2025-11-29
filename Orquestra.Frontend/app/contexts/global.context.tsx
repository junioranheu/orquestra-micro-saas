import SYSTEM from '@/app/consts/system';
import { createContext, Dispatch, ReactNode, SetStateAction, useEffect, useState } from 'react';

type GlobalContextType = {
    isRequestLoading: boolean;
    setIsRequestLoading: Dispatch<SetStateAction<boolean>>;

    showChatbot: boolean;
    setShowChatbot: Dispatch<SetStateAction<boolean>>;

    isOpenChatbot: boolean;
    setIsOpenChatbot: Dispatch<SetStateAction<boolean>>;

    showExpandedSidebar: boolean;
    setShowExpandedSidebar: Dispatch<SetStateAction<boolean>>;

    isModalGrid: boolean;
    setIsModalGrid: Dispatch<SetStateAction<boolean>>;

    showLogsDashboard: boolean;
    setShowLogsDashboard: Dispatch<SetStateAction<boolean>>;
};

export const GlobalContext = createContext<GlobalContextType | undefined>(undefined);

export function GlobalContextProvider({ children }: { children: ReactNode }) {

    const [isRequestLoading, setIsRequestLoading] = useState<boolean>(false);
    const [showChatbot, setShowChatbot] = useState<boolean>(true);
    const [isOpenChatbot, setIsOpenChatbot] = useState<boolean>(false);
    const [showExpandedSidebar, setShowExpandedSidebar] = useState<boolean>(true);
    const [isModalGrid, setIsModalGrid] = useState<boolean>(true);
    const [showLogsDashboard, setShowLogsDashboard] = useState<boolean>(true);

    useEffect(() => {
        const valueChatbot = localStorage.getItem(SYSTEM.LOCAL_STORAGE_SHOW_CHATBOT);
        setShowChatbot(valueChatbot === null ? true : valueChatbot === 'true');

        const valueExpandedSidebar = localStorage.getItem(SYSTEM.LOCAL_STORAGE_SHOW_EXPANDED_SIDEBAR);
        setShowExpandedSidebar(valueExpandedSidebar === null ? true : valueExpandedSidebar === 'true');

        const valueModalGrid = localStorage.getItem(SYSTEM.LOCAL_STORAGE_IS_MODAL_GRID);
        setIsModalGrid(valueModalGrid === null ? true : valueModalGrid === 'true');

        const valueShowLogsDashboard = localStorage.getItem(SYSTEM.LOCAL_STORAGE_SHOW_LOGS_DASHBOARD);
        setShowLogsDashboard(valueShowLogsDashboard === null ? true : valueShowLogsDashboard === 'true');
    }, []);

    return (
        <GlobalContext.Provider value={{
            isRequestLoading, setIsRequestLoading,
            showChatbot, setShowChatbot,
            isOpenChatbot, setIsOpenChatbot,
            showExpandedSidebar, setShowExpandedSidebar,
            isModalGrid, setIsModalGrid,
            showLogsDashboard, setShowLogsDashboard
        }}>
            {children}
        </GlobalContext.Provider>
    )
}