import iUsuario from '@/app/api/consts/usuario';
import { Dispatch, JSX, ReactNode, SetStateAction, createContext, useEffect, useState } from 'react';

interface iContext {
    authContext: [auth: iUsuario | null, setAuth: Dispatch<SetStateAction<iUsuario | null>>];
}

const _item = '_auth';
export const UserContext = createContext<iContext | null>(null);

export function UserProvider({ children }: { children: ReactNode }): JSX.Element {

    const [auth, setAuth] = useState<iUsuario | null>(null);

    useEffect(() => {
        setAuth(Auth?.get() ?? null);
    }, []);

    return (
        <UserContext.Provider value={{
            authContext: [auth, setAuth]
        }}>
            {children}
        </UserContext.Provider>
    );
}

export const Auth = {
    set(data: iUsuario): void {
        const user = {
            userId: data.userId,
            fullName: data.fullName,
            email: data.email,
            isAuth: true
        } as iUsuario;

        const parsedData = JSON.stringify(user);
        localStorage.setItem(_item, parsedData);
    },

    get(): iUsuario | null {
        if (typeof window !== 'undefined') {
            const data = localStorage.getItem(_item);

            if (!data) {
                return null;
            }

            const dataJson = JSON.parse(data);
            return dataJson;
        }

        return null;
    },

    delete(): void {
        localStorage.removeItem(_item);
    }
}