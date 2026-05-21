import { iMe } from '@/app/api/consts/auth';
import { TIPPY_CHATBOT } from '@/app/components/chat-bot';
import ContentLoaderText from '@/app/components/content-loader/text';
import Icon from '@/app/components/icon';
import { iModalCustomPosition } from '@/app/components/modal/generic';
import ModalMenu from '@/app/components/navbar/modal/menu';
import Tippy from '@/app/components/tool-tip';
import ROUTES from '@/app/consts/routes';
import SYSTEM from '@/app/consts/system';
import { handleTruncateText } from '@/app/functions/format.url';
import { useIsOpenChatbot, useShowChatbot } from '@/app/hooks/contexts/useGlobalContext';
import { useOnResize } from '@/app/hooks/useOnResize';
import { useRouter } from 'next/navigation';
import { Fragment, useState } from 'react';
import styles from './index.module.scss';

interface iProps {
    me: iMe | undefined;
}

export default function Navbar({ me }: iProps) {

    const router = useRouter();

    const [isMenuOpen, setIsMenuOpen] = useState<boolean>(false);
    const [modalPosition, setModalPosition] = useState<iModalCustomPosition>({});

    const [showChatbot,] = useShowChatbot();
    const [, setIsOpenChatbot] = useIsOpenChatbot();

    function handleModalClick() {
        setIsMenuOpen(true);
        setIsOpenChatbot(false);

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

        setModalPosition({ top: height + 60, left: window.innerWidth - 192 } as iModalCustomPosition);
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
                        {
                            me?.currentMainCompany ? (
                                <Tippy content='Gerencie os dados e informações de suas empresas cadastradas.'>
                                    <span onClick={() => router.push(ROUTES.EMPRESA_GERENCIAR)}>
                                        <Icon icon='briefcase' weight='bold' /><span>Gerenciar empresas</span>
                                    </span>
                                </Tippy>
                            ) : (
                                <Tippy content={`Cadastre agora mesmo sua empresa para começar a usar o ${SYSTEM.NAME}.`}>
                                    <span className={styles.effect} onClick={() => router.push(ROUTES.EMPRESA_GERENCIAR)}>
                                        <Icon icon='plus-circle' weight='bold' /><span>Cadastrar sua empresa</span>
                                    </span>
                                </Tippy>
                            )
                        }

                        {
                            (me && me?.isUserAdmOfCurrentMainCompany) && (
                                <Tippy content='Entenda mais sobre o plano atual da sua empresa e explore novos; também, consulte suas faturas.'>
                                    <span onClick={() => router.push(ROUTES.EMPRESA_USO_E_PLANO)}><Icon icon='tag' weight='bold' /><span className={styles.hideIfSmall}>Planos e faturas</span></span>
                                </Tippy>
                            )
                        }

                        <Tippy content='Notificações'>
                            <span onClick={() => router.push(ROUTES.EMPRESA_NOTIFICACOES)}><Icon icon='bell' weight='bold' /></span>
                        </Tippy>

                        {
                            showChatbot && (
                                <Tippy content={TIPPY_CHATBOT}>
                                    <span onClick={() => { setIsOpenChatbot(true); setIsMenuOpen(false); }}><Icon icon='message-square' weight='bold' /></span>
                                </Tippy>
                            )
                        }

                        <Tippy content='Gerencie seu perfil, plano, configurações e muito mais.'>
                            <span onClick={() => handleModalClick()}>
                                <span className={styles.hideIfSmall}>
                                    <ContentLoaderText content={(me && me?.currentMainCompany) ? handleTruncateText(me?.currentMainCompany?.name, 20) : 'Menu'} />
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