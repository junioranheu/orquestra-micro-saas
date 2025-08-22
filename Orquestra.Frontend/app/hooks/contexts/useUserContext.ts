import iUsuario from '@/app/api/consts/user';
import { UserContext } from '@/app/contexts/user.context';
import { Dispatch, SetStateAction, useContext } from 'react';

export default function useUserContext(): [iUsuario | null, Dispatch<SetStateAction<iUsuario | null>>] {
    const context = useContext(UserContext);
    const [auth, setAuth] = [context?.authContext[0], context?.authContext[1]] as [iUsuario | null, Dispatch<SetStateAction<iUsuario | null>>];

    return [auth, setAuth];
}