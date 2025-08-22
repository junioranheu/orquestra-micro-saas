import { Dispatch, JSX, ReactNode, SetStateAction, createContext, useState } from 'react';

interface iContext {
    isLoadingContext: [isRequestLoading: boolean, setIsRequestLoading: Dispatch<SetStateAction<boolean>>];
}

export const GlobalContext = createContext<iContext | null>(null);

export function GlobalContextProvider({ children }: { children: ReactNode }): JSX.Element {

    const [isRequestLoading, setIsRequestLoading] = useState<boolean>(false);


    return (
        <GlobalContext.Provider value={{
            isLoadingContext: [isRequestLoading, setIsRequestLoading]
        }}>
            {children}
        </GlobalContext.Provider>
    )
}