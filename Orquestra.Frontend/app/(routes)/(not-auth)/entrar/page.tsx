'use client';
import { CONSTS_AUTH } from '@/app/api/consts/auth';
import iUser from '@/app/api/consts/user';
import { Fetch } from '@/app/api/fetch';
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
import Cookies from 'js-cookie';
import Image from 'next/image';
import { useRouter } from 'next/navigation';
import { useRef, useState } from 'react';
import styles from './page.module.scss';

interface iLoginForm {
    email: string;
    password: string;
}

export default function Login() {

    useTitle('Login');

    const router = useRouter();
    const [_, setAuth] = useUserContext();

    const refButton = useRef<HTMLButtonElement>(null);

    function handleKeyDown(e: React.KeyboardEvent<HTMLInputElement>) {
        if (e.key === 'Enter') {
            refButton.current?.click();
        }
    }

    const [formData, setFormData] = useState<iLoginForm>({
        email: '', password: ''
    });

    async function handleLogin() {
        if (!formData.email || !formData.password) {
            swal({ str: 'Preencha todos os campos antes de prosseguir.', icon: 'error' });
            return;
        }

        const user = {
            email: formData.email,
            password: formData.password
        } as iLoginForm;

        try {
            const result = await Fetch.post(CONSTS_AUTH.auth, user) as iUser;

            if (!result) {
                return;
            }

            setAuth(result);
            Cookies.set(SYSTEM.COOKIE_NAME, JSON.stringify(result), { expires: new Date(result.tokenExpirationDate), path: '/' });
            router.push(ROUTES.DASHBOARD);
        } catch {
            setFormData(x => ({ ...x, password: '' }));
            return;
        }
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
                        refBtn={refButton}
                    />
                </div>
            </div>
        </section>
    )
}