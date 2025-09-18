
import { iUser } from '@/app/api/consts/user';
import SYSTEM from '@/app/consts/system';
import Cookies from 'js-cookie';
import { createContext, Dispatch, ReactNode, SetStateAction, useEffect, useState } from 'react';

type UserContextType = {
    auth: iUser | null;
    setAuth: Dispatch<SetStateAction<iUser | null>>;
};

export const UserContext = createContext<UserContextType | undefined>(undefined);

export function UserProvider({ children }: { children: ReactNode }) {
    const [auth, setAuth] = useState<iUser | null>(null);

    useEffect(() => {
        const cookieAuth = Cookies.get(SYSTEM.COOKIE_NAME);

        if (cookieAuth) {
            try {
                const cookie = JSON.parse(cookieAuth);
                // console.log(cookie);

                setAuth(cookie);
            } catch (err) {
                console.error('Erro ao parsear cookie auth', err);
                setAuth(null);
            }
        }
    }, []);

    return (
        <UserContext.Provider value={{ auth, setAuth }}>
            {children}
        </UserContext.Provider>
    );
}