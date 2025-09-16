import Icon from '@/app/components/icon';
import { iModalCustomPosition } from '@/app/components/modal/generic';
import ModalSettings from '@/app/components/navbar/modal/settings';
import ROUTES from '@/app/consts/routes';
import useApiGetMe from '@/app/hooks/api/useApiGetMe';
import { useOnResize } from '@/app/hooks/useOnResize';
import Tippy from '@tippyjs/react';
import { useRouter } from 'next/navigation';
import { Fragment, useState } from 'react';
import styles from './index.module.scss';

export default function Navbar() {

    const router = useRouter();
    const me = useApiGetMe();
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
                    <div className={styles.right}>
                        {
                            (me && me?.isUserAdmOfCurrentMainCompany) && (
                                <Tippy content='Entenda mais sobre o plano atual da sua empresa e explore novos'>
                                    <span onClick={() => router.push(ROUTES.EMPRESA_USO_E_PLANO)}><Icon icon='tag' weight='bold' /> Uso e plano</span>
                                </Tippy>
                            )
                        }

                        <Tippy content='Ajuda'>
                            <span onClick={() => router.push(ROUTES.ETC_AJUDA)}><Icon icon='help-circle' weight='bold' /></span>
                        </Tippy>

                        <Tippy content='Notificações'>
                            <span onClick={() => router.push(ROUTES.USUARIO_NOTIFICACOES)}><Icon icon='bell' weight='bold' /></span>
                        </Tippy>

                        <Tippy content='Gerencie seu perfil, plano, configurações e muito mais.'>
                            <span onClick={() => handleModalClick()}>
                                <Icon icon='briefcase' weight='bold' />{me && me?.currentMainCompany ? me.currentMainCompany?.name : me?.userName} <Icon icon='chevron-down' weight='bold' />
                            </span>
                        </Tippy>
                    </div>
                </div>
            </nav>

            <ModalSettings isOpen={isMenuOpen} setModalIsOpen={setIsMenuOpen} customPosition={modalPosition} />
        </Fragment>
    )
}