import iUser from '@/app/api/consts/user';
import { UserContext } from '@/app/contexts/user.context';
import { Dispatch, SetStateAction, useContext } from 'react';

export default function useUserContext(): [iUser | null, Dispatch<SetStateAction<iUser | null>>] {
    const context = useContext(UserContext);
    const [auth, setAuth] = [context?.authContext[0], context?.authContext[1]] as [iUser | null, Dispatch<SetStateAction<iUser | null>>];

    return [auth, setAuth];
}