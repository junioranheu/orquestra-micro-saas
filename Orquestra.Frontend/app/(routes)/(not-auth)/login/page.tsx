'use client';
import { CONSTS_AUTH } from '@/app/api/consts/auth';
import { Fetch } from '@/app/api/fetch';
import ImgLoading from '@/app/assets/gif/loading.gif';
import ImgLogo from '@/app/assets/png/logo.png';
import Button from '@/app/components/input/button/button';
import InputMask from '@/app/components/input/text/input.mask';
import ROUTES from '@/app/consts/routes';
import SYSTEM from '@/app/consts/system';
import handleGetPropName from '@/app/functions/get.propName';
import { handleInputFormStateChange } from '@/app/functions/set.formState';
import swal from '@/app/functions/swal';
import useUserContext from '@/app/hooks/contexts/useUserContext';
import useTitle from '@/app/hooks/useTitle';
import Image from 'next/image';
import { useRouter } from 'next/navigation';
import { useEffect, useRef, useState } from 'react';
import styles from './page.module.scss';

interface iLoginForm {
    email: string;
    password: string;
}

export default function Login() {

    useTitle('Login');

    const router = useRouter();
    const [auth, setAuth] = useUserContext();
    const [btnLoginDisabled, setBtnLoginDisabled] = useState<boolean>(true);

    const refButton = useRef<HTMLButtonElement>(null);

    function handleKeyDown(e: React.KeyboardEvent<HTMLInputElement>) {
        if (e.key === 'Enter') {
            refButton.current?.click();
        }
    }

    const [formData, setFormData] = useState<iLoginForm>({
        email: '', password: ''
    });

    useEffect(() => {
        const timeoutId = setTimeout(() => {
            setBtnLoginDisabled(false);
        }, 1500);

        // console.log(isAuthenticated);

        if (auth?.isAuth) {
            router.push(ROUTES.DASHBOARD);
        }

        return () => clearTimeout(timeoutId);
    }, [router, auth?.isAuth]);

    async function handleLogin() {
        if (!formData.email || !formData.password) {
            swal({ str: 'Preencha todos os campos antes de prosseguir.', icon: 'error' });
            return;
        }

        const user = {
            email: formData.email,
            password: formData.password
        } as iLoginForm;

        const result = await Fetch.post(CONSTS_AUTH.auth, user);
        console.log(result);

        // if (result) {
        //     Auth.set(result);
        //     setAuth(result);

        //     setTimeout(() => {
        //         router.push(ROUTES.DASHBOARD);
        //     }, 250);

        //     return;
        // }

        // swal({ str: 'E-mail ou senha incorretos.', icon: 'error' });
        // setFormData(x => ({ ...x, password: '' }));
    }

    async function handleXD() {
        console.clear();
        const result = await Fetch.get(CONSTS_AUTH.test);
        console.log(CONSTS_AUTH.test, result);
    }

    if (auth?.isAuth) {
        return (
            <section className={styles.loading}>
                <h3 dangerouslySetInnerHTML={{ __html: 'Carregando' }} />
                <Image src={ImgLoading} alt={SYSTEM.NAME} width={50} priority={true} unoptimized={true} />
            </section>
        )
    }

    return (
        <section className={styles.main}>
            <div className={styles.form}>
                <picture className={styles.icon} title={SYSTEM.NAME}>
                    <Image src={ImgLogo} alt={SYSTEM.NAME} priority={true} width={120} height={120} />
                </picture>

                <div className={styles.welcome}>
                    <span>Bem-vindo ao {SYSTEM.NAME}</span>
                </div>

                <div className={styles.flex}>
                    <InputMask
                        title={'E-mail'}
                        objectFormData={handleGetPropName(formData, x => x.email)}
                        handleChange={(e) => handleInputFormStateChange(e, setFormData)}
                        handleKeyDown={(e) => handleKeyDown(e)}
                    />

                    <InputMask
                        title={'Senha'}
                        objectFormData={handleGetPropName(formData, x => x.password)}
                        handleChange={(e) => handleInputFormStateChange(e, setFormData)}
                        type='password'
                        handleKeyDown={(e) => handleKeyDown(e)}
                    />

                    <Button
                        label={'Entrar'}
                        handleFunction={() => handleLogin()}
                        isDisabled={btnLoginDisabled}
                    />

                    <Button
                        label={'XD'}
                        handleFunction={() => handleXD()}
                        isDisabled={btnLoginDisabled}
                    />
                </div>
            </div>
        </section>
    )
}