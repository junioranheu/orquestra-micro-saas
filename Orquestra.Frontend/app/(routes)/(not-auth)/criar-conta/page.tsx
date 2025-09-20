'use client';
import { CarouselLogin } from '@/app/(routes)/(not-auth)/login/page';
import styles from '@/app/(routes)/(not-auth)/login/page.module.scss';
import { iUserInput } from '@/app/api/consts/user';
import Divider from '@/app/components/divider';
import Button from '@/app/components/input/button';
import InputMask from '@/app/components/input/text';
import ROUTES from '@/app/consts/routes';
import SYSTEM from '@/app/consts/system';
import { handleGetFirstName } from '@/app/functions/get.formatUserName';
import handleGetPropName from '@/app/functions/get.propName';
import { handleSetCookieAndLogin } from '@/app/functions/set.cookies';
import { handleInputFormStateChange } from '@/app/functions/set.formState';
import swal from '@/app/functions/swal';
import { useIsRequestLoading } from '@/app/hooks/contexts/useGlobalContext';
import useUserContext from '@/app/hooks/contexts/useUserContext';
import useIsIncognito from '@/app/hooks/useIsIncognito';
import useTitle from '@/app/hooks/useTitle';
import { useRouter } from 'next/navigation';
import { KeyboardEvent, useEffect, useRef, useState } from 'react';

export default function CriarConta() {

    useTitle('Criar conta');

    const router = useRouter();
    const [auth, setAuth] = useUserContext();
    const [isRequestLoading, setIsRequestLoading] = useIsRequestLoading();
    const isIncognito = useIsIncognito({ mustShowModalIfIncognito: true });
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
        fullName: '', email: '', password: ''
    });

    async function handleCreate() {
        setIsRequestLoading(true);

        if (!formData.fullName || !formData.email || !formData.password) {
            swal({ str: 'Preencha todos os campos antes de prosseguir.', icon: 'error' });
            setIsRequestLoading(false);
            return;
        }

        const user = {
            fullName: formData.fullName,
            email: formData.email,
            password: formData.password,
            inviteToken: token
        } as iUserInput;

        try {
            await handleSetCookieAndLogin({ type: 'create', user: user, setAuth: setAuth, router: router });
            router.push(ROUTES.LOGIN);

            swal({
                str: `${handleGetFirstName(formData.fullName)}, você foi registrado com sucesso!</br>Antes de fazer seu primeiro acesso, por favor, <b>valide sua conta usando o e-mail que foi enviado para ${formData.email}</b>.`,
                icon: 'success'
            });
        } catch {
            setFormData(x => ({ ...x, password: '' }));
            setIsRequestLoading(false);
            return;
        }
    }

    return (
        <section className={styles.main}>
            <div className={`${styles.wrapper} ${styles.invert}`}>
                <div className={styles.left}>
                    <div className={styles.form}>
                        <div className={styles.welcome}>
                            <span>Cadastre-se agora no {SYSTEM.NAME}</span>
                        </div>

                        <div className={styles.flex}>
                            <InputMask
                                title='Nome completo'
                                objectFormData={handleGetPropName(formData, x => x.fullName ?? '')}
                                isDisabled={isIncognito}
                                handleChange={(e) => handleInputFormStateChange(e, setFormData)}
                                handleKeyDown={(e) => handleKeyDown(e)}
                            />

                            <InputMask
                                title='E-mail'
                                objectFormData={handleGetPropName(formData, x => x.email ?? '')}
                                isDisabled={isIncognito}
                                handleChange={(e) => handleInputFormStateChange(e, setFormData)}
                                handleKeyDown={(e) => handleKeyDown(e)}
                            />

                            <InputMask
                                title='Senha'
                                objectFormData={handleGetPropName(formData, x => x.password ?? '')}
                                type='password'
                                isDisabled={isIncognito}
                                handleChange={(e) => handleInputFormStateChange(e, setFormData)}
                                handleKeyDown={(e) => handleKeyDown(e)}
                            />

                            <Button
                                label='Criar conta'
                                handleFunction={() => handleCreate()}
                                refBtn={refButton}
                                isDisabled={isIncognito || isRequestLoading}
                                style={{ height: '3rem', fontWeight: '600', boxShadow: 'var(--box-shadow)' }}
                            />
                        </div>

                        <Divider text='Já tem uma conta?' />

                        <div className={styles.flex}>
                            <Button
                                label='Entre agora mesmo'
                                handleFunction={() => router.push(ROUTES.LOGIN)}
                                isDisabled={isIncognito || isRequestLoading}
                                isStyleSimple={true}
                                style={{ height: '3rem', fontWeight: '600' }}
                            />
                        </div>
                    </div>
                </div>

                <div className={styles.right}>
                    <CarouselLogin alignCaptionToRight={false} />
                </div>
            </div>
        </section>
    )
}