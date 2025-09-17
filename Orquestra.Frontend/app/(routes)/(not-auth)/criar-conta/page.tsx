'use client';
import { CONSTS_USER, iUserInput } from '@/app/api/consts/user';
import { handleSetCookieAndLogin } from '@/app/functions/set.cookies';
import swal from '@/app/functions/swal';
import useUserContext from '@/app/hooks/contexts/useUserContext';
import useTitle from '@/app/hooks/useTitle';
import { useRouter } from 'next/navigation';
import { KeyboardEvent, useEffect, useRef, useState } from 'react';
import styles from './page.module.scss';

export default function CriarConta() {

    useTitle('Criar conta');

    const router = useRouter();
    const [auth, setAuth] = useUserContext();
    const [token, setToken] = useState('');

    const refButton = useRef<HTMLButtonElement>(null);

    useEffect(() => {
        const params = new URLSearchParams(window.location.search);
        const token = params.get('token');

        if (token) {
            setToken(token);
        }
    }, []);

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
            await handleSetCookieAndLogin({ url: CONSTS_USER.create, user: user, setAuth: setAuth, router: router });
        } catch {
            setFormData(x => ({ ...x, password: '' }));
            return;
        }
    }

    return (
        <section className={styles.main}>
            <h1>Criar conta {token}</h1>
        </section>
    )
}