'use client';
import { CONSTS_AUTH } from '@/app/api/consts/auth';
import { iUser, iUserInput } from '@/app/api/consts/user';
import { Fetch } from '@/app/api/fetch';
import ROUTES from '@/app/consts/routes';
import SYSTEM from '@/app/consts/system';
import Cookies from 'js-cookie';
import { AppRouterInstance } from 'next/dist/shared/lib/app-router-context.shared-runtime';
import { Dispatch, SetStateAction } from 'react';

interface iSetProps {
    url: string;
    user: iUserInput;
    setAuth: (value: any) => void;
    router: AppRouterInstance;
    setIsRequestLoading?: Dispatch<SetStateAction<boolean>>;
}

export async function handleSetCookieAndLogin({ url, user, setAuth, router, setIsRequestLoading }: iSetProps) {
    const result = await Fetch.post({ url: url, body: user, setIsRequestLoading: setIsRequestLoading }) as iUser;

    if (!result) {
        return;
    }

    setAuth(result);
    Cookies.set(SYSTEM.COOKIE_NAME, JSON.stringify(result), { expires: new Date(result.refreshTokenExpirationDate), path: '/' });
    router.push(ROUTES.DASHBOARD);
}

interface iRemoveProps {
    setAuth: (value: any) => void;
    router: AppRouterInstance;
}

export async function handleRemoveCookieAndLogout({ setAuth, router }: iRemoveProps) {
    await Fetch.delete({ url: CONSTS_AUTH.logout });
    Cookies.remove(SYSTEM.COOKIE_NAME, { path: '/' });
    setAuth(null);
    router.push(ROUTES.LOGIN);
}