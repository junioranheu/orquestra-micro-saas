import ContentLoaderText from '@/app/components/content-loader/text';
import Icon from '@/app/components/icon';
import { iModalCustomPosition } from '@/app/components/modal/generic';
import ModalMenu from '@/app/components/navbar/modal/menu';
import ROUTES from '@/app/consts/routes';
import SYSTEM from '@/app/consts/system';
import { handleGetFirstName } from '@/app/functions/get.formatUserName';
import useApiGetMe from '@/app/hooks/api/useApiGetMe';
import { useOnResize } from '@/app/hooks/useOnResize';
import Tippy from '@tippyjs/react';
import { useRouter } from 'next/navigation';
import { Fragment, useState } from 'react';
import styles from './index.module.scss';

export default function Navbar() {

    const router = useRouter();
    const me = useApiGetMe({});

    const [isMenuOpen, setIsMenuOpen] = useState<boolean>(false);
    const [modalPosition, setModalPosition] = useState<iModalCustomPosition>({});

    function handleModalClick() {
        setIsMenuOpen(true);

        setTimeout(() => {
            handleGetPosition();
        }, 100);
    }

    function handleGetPosition() {
        const element = document.getElementById('modal-settings');
        let height = 327;

        if (element) {
            height = element.getBoundingClientRect().height / 2;
        }

        setModalPosition({ top: height + 79, left: window.innerWidth - 192 } as iModalCustomPosition);
    }

    useOnResize(() => {
        if (isMenuOpen) {
            handleGetPosition();
        }
    });

    return (
        <Fragment>
            <nav className={styles.nav}>
                <div className={styles.inner}>
                    <div className={styles.left}>
                    </div>

                    <div className={`${styles.right} ${SYSTEM.ANIMATE}`}>
                        <Tippy content='Gerencie os dados e informações de suas empresas cadastradas.'>
                            <span className={me?.currentMainCompany ? '' : styles.effect} onClick={() => router.push(ROUTES.EMPRESA_GERENCIAR)}>
                                {
                                    me?.currentMainCompany ? (
                                        <Fragment>
                                            <Icon icon='briefcase' weight='bold' /><span>Gerenciar empresas</span>
                                        </Fragment>
                                    ) : (
                                        <Fragment>
                                            <Icon icon='plus-circle' weight='bold' /><span>Cadastrar sua empresa</span>
                                        </Fragment>
                                    )
                                }
                            </span>
                        </Tippy>

                        {
                            (me && me?.isUserAdmOfCurrentMainCompany) && (
                                <Tippy content='Entenda mais sobre o plano atual da sua empresa e explore novos; também, consulte suas faturas.'>
                                    <span className={styles.hideIfSmall} onClick={() => router.push(ROUTES.EMPRESA_USO_E_PLANO)}><Icon icon='tag' weight='bold' /><span className={styles.hideIfSmall}>Plano e faturas</span></span>
                                </Tippy>
                            )
                        }

                        <Tippy content='Notificações'>
                            <span onClick={() => router.push(ROUTES.USUARIO_NOTIFICACOES)}><Icon icon='bell' weight='bold' /></span>
                        </Tippy>

                        <Tippy content='Gerencie seu perfil, plano, configurações e muito mais.'>
                            <span onClick={() => handleModalClick()}>
                                <span className={styles.hideIfSmall}>
                                    <ContentLoaderText text={(me && me?.currentMainCompany) ? me?.currentMainCompany?.name : `Olá, ${me?.userName ? handleGetFirstName(me?.userName) : ''}`} />
                                </span>

                                <Icon icon='chevron-down' weight='bold' />
                            </span>
                        </Tippy>
                    </div>
                </div>
            </nav>

            <ModalMenu isOpen={isMenuOpen} setIsModalOpen={setIsMenuOpen} customPosition={modalPosition} me={me} />
        </Fragment>
    )
}