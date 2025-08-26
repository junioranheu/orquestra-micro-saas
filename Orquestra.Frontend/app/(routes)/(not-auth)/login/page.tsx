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
import useIsIncognito from '@/app/hooks/useIsIncognito';
import useTitle from '@/app/hooks/useTitle';
import Cookies from 'js-cookie';
import Image from 'next/image';
import { useRouter } from 'next/navigation';
import { useEffect, useRef, useState } from 'react';
import styles from './page.module.scss';

interface iLoginForm {
    email: string;
    password: string;
}

export default function Login() {

    useTitle('Iniciar sessão');

    const router = useRouter();
    const [_, setAuth] = useUserContext();
    const isIncognito = useIsIncognito();

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
            const result = await Fetch.post({ url: CONSTS_AUTH.auth, body: user }) as iUser;

            if (!result) {
                return;
            }

            setAuth(result);
            Cookies.set(SYSTEM.COOKIE_NAME, JSON.stringify(result), { expires: new Date(result.refreshTokenExpirationDate), path: '/' });
            router.push(ROUTES.DASHBOARD);
        } catch {
            setFormData(x => ({ ...x, password: '' }));
            return;
        }
    }

    useEffect(() => {
        if (isIncognito) {
            swal({
                // title: 'Aviso',
                str: `Não é possível acessar o ${SYSTEM.NAME} em modo anônimo.`,
                confirmBtnText: 'OK',
                cancelBtnText: 'Saiba mais',
                confirmFunction: () => {

                },
                cancelFunction: () => {
                    swal({
                        title: 'Por que não é permitido o modo anônimo?',
                        str: 'O sistema não pode ser usado em modo anônimo ou privado porque, nesse modo, os cookies e dados de autenticação não são salvos permanentemente. ' +
                            'Isso significa que sua sessão pode ser perdida a qualquer momento, e você não conseguiria acessar suas informações de forma segura. ' +
                            '<b>Para garantir que tudo funcione corretamente, use o navegador no modo normal.</b>',
                        confirmBtnText: 'OK',
                        icon: 'info'
                    });
                },
                icon: 'error',
                allowOutsideClick: false
            });
        }
    }, [isIncognito]);

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
                        isDisabled={isIncognito}
                        handleChange={(e) => handleInputFormStateChange(e, setFormData)}
                        handleKeyDown={(e) => handleKeyDown(e)}
                    />

                    <InputMask
                        title={'Senha'}
                        objectFormData={handleGetPropName(formData, x => x.password)}
                        type='password'
                        isDisabled={isIncognito}
                        handleChange={(e) => handleInputFormStateChange(e, setFormData)}
                        handleKeyDown={(e) => handleKeyDown(e)}
                    />

                    <Button
                        label={'Entrar'}
                        handleFunction={() => handleLogin()}
                        refBtn={refButton}
                        isDisabled={isIncognito}
                    />
                </div>
            </div>
        </section>
    )
}