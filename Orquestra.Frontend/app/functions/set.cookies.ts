'use client';
import { CONSTS_AUTH } from '@/app/api/consts/auth';
import { CONSTS_USER, iUser, iUserInput } from '@/app/api/consts/user';
import { Fetch } from '@/app/api/fetch';
import ROUTES from '@/app/consts/routes';
import SYSTEM from '@/app/consts/system';
import Cookies from 'js-cookie';
import { AppRouterInstance } from 'next/dist/shared/lib/app-router-context.shared-runtime';
import { Dispatch, SetStateAction } from 'react';

interface iSetProps {
    type: 'auth' | 'create';
    user: iUserInput;
    setAuth: (value: any) => void;
    router: AppRouterInstance;
    setIsRequestLoading?: Dispatch<SetStateAction<boolean>>;
}

export async function handleSetCookieAndLogin({ type, user, setAuth, router, setIsRequestLoading }: iSetProps) {
    const url = type === 'create' ? CONSTS_USER.create : CONSTS_AUTH.auth;
    const result = await Fetch.post({ url: url, body: user, setIsRequestLoading: setIsRequestLoading }) as iUser;
    // console.log(type, result);

    if (!result) {
        throw new Error();
    }

    if (type === 'create') {
        return;
    }

    setAuth(result);
    Cookies.set(SYSTEM.COOKIE_AUTH_FRONT, JSON.stringify(result), { expires: new Date(result.refreshTokenExpirationDate), path: '/' });
    router.push(ROUTES.DASHBOARD);
}

interface iRemoveProps {
    setAuth: (value: any) => void;
    router: AppRouterInstance;
}

export async function handleRemoveCookieAndLogout({ setAuth, router }: iRemoveProps) {
    await Fetch.delete({ url: CONSTS_AUTH.logout });
    Cookies.remove(SYSTEM.COOKIE_AUTH_FRONT, { path: '/' });
    setAuth(null);
    router.push(ROUTES.LOGIN);
}