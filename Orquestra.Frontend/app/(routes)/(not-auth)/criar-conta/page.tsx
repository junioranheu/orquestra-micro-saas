'use client';
import { CarouselLogin } from '@/app/(routes)/(not-auth)/login/page';
import styles from '@/app/(routes)/(not-auth)/login/page.module.scss';
import { iUserInput } from '@/app/api/consts/user';
import { CONSTS_UTILITY, iPlanTypeOutput } from '@/app/api/consts/utility';
import ImgLogo from '@/app/assets/png/logo.png';
import Divider from '@/app/components/divider';
import Button from '@/app/components/input/button';
import { iDropdownOption } from '@/app/components/input/drop-down';
import InputMask from '@/app/components/input/text';
import ROUTES from '@/app/consts/routes';
import SYSTEM from '@/app/consts/system';
import { handleCheckIsSupportedBrowser } from '@/app/functions/check.browser';
import { handleGetFirstName } from '@/app/functions/get.formatUserName';
import handleGetPropName from '@/app/functions/get.propName';
import { handleSetCookieAndLogin } from '@/app/functions/set.cookies';
import { handleSetDropdownOption } from '@/app/functions/set.formState';
import swal from '@/app/functions/swal';
import useApiGetEnum from '@/app/hooks/api/useApiGetEnum';
import useApiRequestToSetterOnUrlChange from '@/app/hooks/api/useApiRequestToSetterOnUrlChange';
import { useIsRequestLoading } from '@/app/hooks/contexts/useGlobalContext';
import useUserContext from '@/app/hooks/contexts/useUserContext';
import useIsIncognito from '@/app/hooks/useIsIncognito';
import useTitle from '@/app/hooks/useTitle';
import dynamic from 'next/dynamic';
import Image from 'next/image';
import { useRouter } from 'next/navigation';
import { Dispatch, KeyboardEvent, SetStateAction, useEffect, useRef, useState } from 'react';
const Dropdown = dynamic(() => import('@/app/components/input/drop-down').then(x => x.default), { ssr: false });

export default function CriarConta() {

    useTitle('Criar conta');

    const router = useRouter();
    const [, setAuth] = useUserContext();
    const [isRequestLoading, setIsRequestLoading] = useIsRequestLoading();

    const isIncognito = useIsIncognito({ mustShowModalIfIncognito: true });

    const [plans, setPlans] = useState<iPlanTypeOutput | undefined>();
    useApiRequestToSetterOnUrlChange<iPlanTypeOutput>({ apiUrlRequest: CONSTS_UTILITY.getPlanType, setter: setPlans });

    const [token, setToken] = useState('');

    const refButton = useRef<HTMLButtonElement>(null);

    useEffect(() => {
        if (typeof window === 'undefined') {
            return;
        }

        const params = new URLSearchParams(window.location.search);
        const token = params.get('token');

        if (token) {
            swal({
                content: `Finalize o seu cadastro para aceitar o convite e participar do ${SYSTEM.NAME}.`,
                icon: 'info'
            });

            setToken(token);
        }
    }, []);

    function handleKeyDown(e: KeyboardEvent<HTMLInputElement>) {
        if (e.key === 'Enter') {
            refButton.current?.click();
        }
    }

    const [formData, setFormData] = useState<iUserInput>({
        fullName: '', email: '', password: '', recoverPasswordQuestion: '', recoverPasswordAnswer: ''
    });

    const recoverPasswordQuestionEnum = useApiGetEnum({ enumName: 'RecoverPasswordQuestionEnum' });
    const setRecoverPasswordQuestion = handleSetDropdownOption(formData, setFormData, handleGetPropName(formData, x => x.recoverPasswordQuestion!)[1]) as Dispatch<SetStateAction<iDropdownOption[]>>;

    async function handleCreate() {
        const isSupported = handleCheckIsSupportedBrowser();

        if (!isSupported) {
            return;
        }

        setIsRequestLoading(true);

        if (!formData.fullName || !formData.email || !formData.password || !formData.recoverPasswordQuestion || !formData.recoverPasswordAnswer) {
            swal({ content: SYSTEM.WARN_FILL_OBLIGATORY_FIELDS, icon: 'error' });
            setIsRequestLoading(false);
            return;
        }

        // @ts-expect-error: dynamic;
        const recoverPasswordQuestion = formData.recoverPasswordQuestion?.value;

        const user = {
            fullName: formData.fullName,
            email: formData.email,
            password: formData.password,
            recoverPasswordQuestion: recoverPasswordQuestion,
            recoverPasswordAnswer: formData.recoverPasswordAnswer,
            inviteToken: token
        } as iUserInput;

        try {
            await handleSetCookieAndLogin({ type: 'create', user: user, setAuth: setAuth, router: router });
            setIsRequestLoading(false);
            router.push(ROUTES.LOGIN);

            if (!token) {
                swal({
                    content: `${handleGetFirstName(formData.fullName)}, seu registro foi concluído com sucesso!</br></br>
                        Confirme seu e-mail clicando no link enviado para <b>${formData.email}</b>.`,
                    icon: 'success',
                    mustConfirm: true,
                    checkboxLabel: 'Entendi. Validarei minha conta via e-mail'
                });
            } else {
                swal({
                    content: `${handleGetFirstName(formData.fullName)}, você foi registrado com sucesso!`,
                    icon: 'success'
                });
            }
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
                            <Image src={ImgLogo} alt='' priority={true} />
                            {/* <span>Cadastre-se agora no {SYSTEM.NAME}</span> */}
                        </div>

                        <div className={styles.flex}>
                            <InputMask
                                title='Nome completo'
                                fieldName='fullName'
                                formData={formData}
                                setFormData={setFormData}
                                isDisabled={isIncognito}
                                isObligatory={true}
                                handleKeyDown={handleKeyDown}
                            />

                            <div className={styles.flexRow}>
                                <InputMask
                                    title='E-mail'
                                    fieldName='email'
                                    formData={formData}
                                    setFormData={setFormData}
                                    isDisabled={isIncognito}
                                    isObligatory={true}
                                    handleKeyDown={handleKeyDown}
                                />

                                <InputMask
                                    title='Senha'
                                    fieldName='password'
                                    type='password'
                                    formData={formData}
                                    setFormData={setFormData}
                                    isDisabled={isIncognito}
                                    isObligatory={true}
                                    handleKeyDown={handleKeyDown}
                                />
                            </div>

                            <div className={styles.flexRow}>
                                <Dropdown
                                    title='Pergunta de segurança'
                                    options={recoverPasswordQuestionEnum ?? []} selectedOption={recoverPasswordQuestionEnum?.find(x => x.value.toString() === formData.recoverPasswordQuestion?.toString())}
                                    setSelectedOption={setRecoverPasswordQuestion}
                                    isDisabled={isIncognito}
                                    isObligatory={true}
                                    isSearchable={false}
                                />

                                <InputMask
                                    title='Resposta de segurança'
                                    fieldName='recoverPasswordAnswer'
                                    formData={formData}
                                    setFormData={setFormData}
                                    isDisabled={isIncognito}
                                    isObligatory={true}
                                    handleKeyDown={handleKeyDown}
                                />
                            </div>

                            <Button
                                label={(plans?.planDurationDaysFree ? `Criar conta grátis por ${plans?.planDurationDaysFree} dias` : 'Criar conta grátis')}
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