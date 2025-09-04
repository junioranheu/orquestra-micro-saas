'use client';
import { CONSTS_USER, iUserInput } from '@/app/api/consts/user';
import { handleSetCookieAndLogin } from '@/app/functions/set.cookies';
import swal from '@/app/functions/swal';
import useUserContext from '@/app/hooks/contexts/useUserContext';
import useTitle from '@/app/hooks/useTitle';
import { useRouter, useSearchParams } from 'next/navigation';
import { KeyboardEvent, useEffect, useRef, useState } from 'react';
import styles from './page.module.scss';

export default function CriarConta() {

    useTitle('Criar conta');

    const searchParams = useSearchParams();
    const router = useRouter();
    const [auth, setAuth] = useUserContext();
    const [token, setToken] = useState('');

    const refButton = useRef<HTMLButtonElement>(null);

    function handleKeyDown(e: KeyboardEvent<HTMLInputElement>) {
        if (e.key === 'Enter') {
            refButton.current?.click();
        }
    }

    const [formData, setFormData] = useState<iUserInput>({
        fullName: '', email: '', password: '', inviteToken: ''
    });

    async function handleCreate() {
        if (!formData.fullName || !formData.email || !formData.password) {
            swal({ str: 'Preencha todos os campos antes de prosseguir.', icon: 'error' });
            return;
        }

        const user = {
            fullName: formData.fullName,
            email: formData.email,
            password: formData.password,
            inviteToken: formData.inviteToken
        } as iUserInput;

        try {
            await handleSetCookieAndLogin(CONSTS_USER.create, user, setAuth, router);
        } catch {
            setFormData(x => ({ ...x, password: '' }));
            return;
        }
    }

    useEffect(() => {
        const token = searchParams.get('token');

        if (token) {
            setToken(token);
        }
    }, [searchParams]);

    return (
        <section className={styles.main}>
            <h1>Criar conta {token}</h1>
        </section>
    )
}