import iUser from '@/app/api/consts/user';
import { Dispatch, JSX, ReactNode, SetStateAction, createContext, useEffect, useState } from 'react';

interface iContext {
    authContext: [auth: iUser | null, setAuth: Dispatch<SetStateAction<iUser | null>>];
}

const _item = '_auth';
export const UserContext = createContext<iContext | null>(null);

export function UserProvider({ children }: { children: ReactNode }): JSX.Element {

    const [auth, setAuth] = useState<iUser | null>(null);

    useEffect(() => {
        setAuth(null);
    }, []);

    return (
        <UserContext.Provider value={{
            authContext: [auth, setAuth]
        }}>
            {children}
        </UserContext.Provider>
    );
}