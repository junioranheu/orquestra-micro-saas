'use client';
import { CONSTS_AUTH } from '@/app/api/consts/auth';
import { iUserInput } from '@/app/api/consts/user';
import Img1 from '@/app/assets/abstract/1.webp';
import Img2 from '@/app/assets/abstract/2.webp';
import Img3 from '@/app/assets/abstract/3.webp';
import Img4 from '@/app/assets/abstract/4.webp';
import Carousel from '@/app/components/carousel';
import { CookieDefault } from '@/app/components/cookie';
import Divider from '@/app/components/divider';
import Button from '@/app/components/input/button';
import InputMask from '@/app/components/input/text';
import ROUTES from '@/app/consts/routes';
import SYSTEM from '@/app/consts/system';
import handleGetPropName from '@/app/functions/get.propName';
import { handleSetCookieAndLogin } from '@/app/functions/set.cookies';
import { handleInputFormStateChange } from '@/app/functions/set.formState';
import swal from '@/app/functions/swal';
import useApiGetBuildVersion from '@/app/hooks/api/useApiGetBuildVersion';
import { useIsRequestLoading } from '@/app/hooks/contexts/useGlobalContext';
import useUserContext from '@/app/hooks/contexts/useUserContext';
import useIsIncognito from '@/app/hooks/useIsIncognito';
import useTitle from '@/app/hooks/useTitle';
import Link from 'next/link';
import { useRouter } from 'next/navigation';
import { KeyboardEvent, useEffect, useRef, useState } from 'react';
import styles from './page.module.scss';

export default function Login() {

    useTitle('Iniciar sessão');

    const versionBuild = useApiGetBuildVersion();
    const router = useRouter();
    const [_, setAuth] = useUserContext();
    const [isRequestLoading, setIsRequestLoading] = useIsRequestLoading();
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
            await handleSetCookieAndLogin({ url: CONSTS_AUTH.auth, user: user, setAuth: setAuth, router: router });
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
            <div className={styles.wrapper}>
                <div className={styles.left}>
                    <div className={styles.form}>
                        <div className={styles.welcome}>
                            <span>Bem-vindo ao {SYSTEM.NAME}</span>
                        </div>

                        <div className={styles.flex}>
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
                                label={'Iniciar sessão'}
                                handleFunction={() => handleLogin()}
                                refBtn={refButton}
                                isDisabled={isIncognito || isRequestLoading}
                                style={{ height: '3rem', fontWeight: '600', boxShadow: 'var(--box-shadow)' }}
                            />

                            <Link className={styles.forget} href={ROUTES.RECUPERAR_SENHA}>Esqueci minha senha</Link>
                        </div>

                        <Divider text='Não tem uma conta?' />

                        <div className={styles.flex}>
                            <Button
                                label='Crie uma conta agora mesmo'
                                handleFunction={() => router.push(ROUTES.CRIAR_CONTA)}
                                isDisabled={isIncognito || isRequestLoading}
                                classes='btn-secondary'
                                style={{ height: '3rem', fontWeight: '600' }}
                            />
                        </div>
                    </div>
                </div>

                <div className={styles.right}>
                    <CarouselLogin alignCaptionToRight={true} />
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

interface iCarouselLoginProps {
    alignCaptionToRight: boolean;
}

export function CarouselLogin({ alignCaptionToRight }: iCarouselLoginProps) {
    return (
        <Carousel
            items={[
                { image: Img1, caption: 'Agende seus compromissos em segundos' },
                { image: Img2, caption: 'Seu negócio afinado como uma orquestra' },
                { image: Img3, caption: 'Organize sua agenda sem dor de cabeça' },
                { image: Img4, caption: 'Mais tempo pra você, menos tempo no caos' }
            ]}
            autoSlideInterval={7500}
            mustShuffle={true}
            mustHideButtonsIfSmallScreen={true}
            alignCaptionToRight={alignCaptionToRight}
        />
    )
}