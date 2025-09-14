'use client';
import { CONSTS_AUTH } from '@/app/api/consts/auth';
import { iUserInput } from '@/app/api/consts/user';
import ImgLogo from '@/app/assets/png/logo.png';
import { CookieDefault } from '@/app/components/cookie';
import Button from '@/app/components/input/button';
import InputMask from '@/app/components/input/text';
import SYSTEM from '@/app/consts/system';
import handleGetPropName from '@/app/functions/get.propName';
import { handleSetCookieAndLogin } from '@/app/functions/set.cookies';
import { handleInputFormStateChange } from '@/app/functions/set.formState';
import swal from '@/app/functions/swal';
import useApiGetBuildVersion from '@/app/hooks/api/useApiGetBuildVersion';
import useUserContext from '@/app/hooks/contexts/useUserContext';
import useIsIncognito from '@/app/hooks/useIsIncognito';
import useTitle from '@/app/hooks/useTitle';
import Image from 'next/image';
import { useRouter } from 'next/navigation';
import { KeyboardEvent, useEffect, useRef, useState } from 'react';
import styles from './page.module.scss';

export default function Login() {

    useTitle('Iniciar sessão');

    const versionBuild = useApiGetBuildVersion();
    const router = useRouter();
    const [_, setAuth] = useUserContext();
    const isIncognito = useIsIncognito();

    const refButton = useRef<HTMLButtonElement>(null);

    function handleKeyDown(e: KeyboardEvent<HTMLInputElement>) {
        if (e.key === 'Enter') {
            refButton.current?.click();
        }
    }

    const [formData, setFormData] = useState<iUserInput>({
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
        } as iUserInput;

        try {
            await handleSetCookieAndLogin(CONSTS_AUTH.auth, user, setAuth, router);
        } catch {
            setFormData(x => ({ ...x, password: '' }));
            return;
        }
    }

    useEffect(() => {
        if (isIncognito) {
            swal({
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
                        objectFormData={handleGetPropName(formData, x => x.email ?? '')}
                        isDisabled={isIncognito}
                        handleChange={(e) => handleInputFormStateChange(e, setFormData)}
                        handleKeyDown={(e) => handleKeyDown(e)}
                    />

                    <InputMask
                        title={'Senha'}
                        objectFormData={handleGetPropName(formData, x => x.password ?? '')}
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

            <div className={styles.bottom}>
                <code>{versionBuild?.buildVersion ? `Build ${versionBuild?.buildVersion}` : ''}</code>
                <code>{versionBuild?.configuration}</code>
            </div>

            <CookieDefault />
        </section>
    )
}