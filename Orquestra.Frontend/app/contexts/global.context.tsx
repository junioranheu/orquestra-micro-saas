import { createContext, Dispatch, ReactNode, SetStateAction, useState } from 'react';

type GlobalContextType = {
    isRequestLoading: boolean;
    setIsRequestLoading: Dispatch<SetStateAction<boolean>>;
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