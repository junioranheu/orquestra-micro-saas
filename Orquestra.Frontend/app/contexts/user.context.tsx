import iUser from '@/app/api/consts/user';
import SYSTEM from '@/app/consts/system';
import Cookies from 'js-cookie';
import { createContext, ReactNode, useEffect, useState } from 'react';

type UserContextType = {
    auth: iUser | null;
    setAuth: React.Dispatch<React.SetStateAction<iUser | null>>;
};

export const UserContext = createContext<UserContextType | undefined>(undefined);

export function UserProvider({ children }: { children: ReactNode }) {
    const [auth, setAuth] = useState<iUser | null>(null);

    useEffect(() => {
        const cookieAuth = Cookies.get(SYSTEM.COOKIE_NAME);

        if (cookieAuth) {
            try {
                setAuth(JSON.parse(cookieAuth));
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