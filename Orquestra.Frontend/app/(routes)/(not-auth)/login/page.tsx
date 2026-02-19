'use client';
import { iUserInput } from '@/app/api/consts/user';
import Img1 from '@/app/assets/abstract/1.webp';
import Img2 from '@/app/assets/abstract/2.webp';
import Img3 from '@/app/assets/abstract/3.webp';
import Img4 from '@/app/assets/abstract/4.webp';
import ImgLogo from '@/app/assets/png/logo.png';
import Carousel from '@/app/components/carousel';
import { CookieDefault } from '@/app/components/cookie';
import Divider from '@/app/components/divider';
import Icon from '@/app/components/icon';
import Button from '@/app/components/input/button';
import InputMask from '@/app/components/input/text';
import ROUTES from '@/app/consts/routes';
import SYSTEM from '@/app/consts/system';
import { handleCheckIsSupportedBrowser } from '@/app/functions/check.browser';
import { handleSetCookieAndLogin } from '@/app/functions/set.cookies';
import swal from '@/app/functions/swal';
import useApiGetBuildVersion from '@/app/hooks/api/useApiGetBuildVersion';
import { useIsRequestLoading } from '@/app/hooks/contexts/useGlobalContext';
import useUserContext from '@/app/hooks/contexts/useUserContext';
import useIsIncognito from '@/app/hooks/useIsIncognito';
import useTitle from '@/app/hooks/useTitle';
import Tippy from '@tippyjs/react';
import Image from 'next/image';
import Link from 'next/link';
import { useRouter } from 'next/navigation';
import { Fragment, KeyboardEvent, useRef, useState } from 'react';
import LoginModalRecuperarSenha from './modal/recuperar-senha';
import styles from './page.module.scss';

export default function Login() {

    useTitle('Iniciar sessão');

    const versionBuild = useApiGetBuildVersion();
    const router = useRouter();
    const [, setAuth] = useUserContext();

    const [isRequestLoading, setIsRequestLoading] = useIsRequestLoading();
    const [isModalRecuperarSenhaOpen, setIsModalRecuperarSenhaOpen] = useState<boolean>(false);

    const isIncognito = useIsIncognito({ mustShowModalIfIncognito: true });
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
        const isSupported = handleCheckIsSupportedBrowser();

        if (!isSupported) {
            return;
        }

        setIsRequestLoading(true);

        if (!formData.email || !formData.password) {
            swal({ content: SYSTEM.WARN_FILL_OBLIGATORY_FIELDS, icon: 'error' });
            setIsRequestLoading(false);
            return;
        }

        const user = {
            email: formData.email,
            password: formData.password
        } as iUserInput;

        try {
            await handleSetCookieAndLogin({ type: 'auth', user: user, setAuth: setAuth, router: router });
        } catch {
            setFormData(x => ({ ...x, password: '' }));
            setIsRequestLoading(false);
            return;
        }
    }

    return (
        <Fragment>
            <section className={styles.main}>
                <div className={styles.wrapper}>
                    <div className={styles.left}>
                        <div className={styles.form}>
                            <div className={styles.welcome}>
                                <Image src={ImgLogo} alt='' priority={true} />
                                {/* <span>Bem-vindo ao {SYSTEM.NAME}</span> */}
                            </div>

                            <div className={styles.flex}>
                                <InputMask
                                    title='E-mail'
                                    fieldName='email'
                                    formData={formData}
                                    setFormData={setFormData}
                                    isDisabled={isIncognito}
                                    handleKeyDown={handleKeyDown}
                                />

                                <InputMask
                                    title='Senha'
                                    fieldName='password'
                                    type='password'
                                    formData={formData}
                                    setFormData={setFormData}
                                    isDisabled={isIncognito}
                                    handleKeyDown={handleKeyDown}
                                />

                                <Button
                                    label={'Iniciar sessão'}
                                    handleFunction={() => handleLogin()}
                                    refBtn={refButton}
                                    isDisabled={isIncognito || isRequestLoading}
                                    style={{ fontWeight: '600', boxShadow: 'var(--box-shadow)' }}
                                    isBig={true}
                                />

                                {
                                    !isIncognito && <Link className={styles.forget} href='#' onClick={() => setIsModalRecuperarSenhaOpen(true)}>Esqueci minha senha</Link>
                                }
                            </div>

                            <Divider text='Não tem uma conta?' />

                            <div className={styles.flex}>
                                <Button
                                    label='Crie uma conta agora mesmo'
                                    handleFunction={() => router.push(ROUTES.CRIAR_CONTA)}
                                    isDisabled={isIncognito || isRequestLoading}
                                    styleType='transparent'
                                    isBig={true}
                                    style={{ fontWeight: '600' }}
                                />
                            </div>
                        </div>
                    </div>

                    <div className={styles.right}>
                        <CarouselLogin alignCaptionToRight={true} />
                    </div>
                </div>

                <div className={styles.bottom}>
                    <div className={styles.link}>
                        <Tippy content={`Sobre o ${SYSTEM.NAME}.`} placement='right'>
                            <Link href={ROUTES.LANDING_PAGE}>
                                <Icon icon='smile' color='var(--gray-dark)' className='contrastOnHover' />
                            </Link>
                        </Tippy>
                    </div>

                    <div className={styles.build}>
                        <code>{versionBuild?.buildVersion ? `Build ${versionBuild?.buildVersion}` : ''}</code>
                        <code>{versionBuild?.configuration}</code>
                    </div>
                </div>

                <CookieDefault />
            </section>

            <LoginModalRecuperarSenha
                isModalOpen={isModalRecuperarSenhaOpen}
                setIsModalOpen={setIsModalRecuperarSenhaOpen}
            />
        </Fragment>
    )
}

interface iCarouselLoginProps {
    alignCaptionToRight: boolean;
}

export function CarouselLogin({ alignCaptionToRight }: iCarouselLoginProps) {
    return (
        <Carousel
            items={[
                { image: Img1, caption: 'Agende seus compromissos e eventos em segundos' },
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