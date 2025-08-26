import { createContext, Dispatch, ReactNode, useState } from 'react';

type GlobalContextType = {
    isRequestLoading: boolean;
    setIsRequestLoading: Dispatch<React.SetStateAction<boolean>>;
};

export const GlobalContext = createContext<GlobalContextType | undefined>(undefined);

export function GlobalContextProvider({ children }: { children: ReactNode }) {
    const [isRequestLoading, setIsRequestLoading] = useState(false);

    return (
        <GlobalContext.Provider value={{ isRequestLoading, setIsRequestLoading }}>
            {children}
        </GlobalContext.Provider>
    );
}