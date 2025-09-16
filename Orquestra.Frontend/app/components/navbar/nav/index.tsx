import Icon from '@/app/components/icon';
import { iModalCustomPosition } from '@/app/components/modal/generic';
import ModalSettings from '@/app/components/navbar/modal/settings';
import ROUTES from '@/app/consts/routes';
import handleGetModalClickPosition from '@/app/functions/get.modalClickPosition';
import useApiGetMe from '@/app/hooks/api/useApiGetMe';
import Tippy from '@tippyjs/react';
import { useRouter } from 'next/navigation';
import { Fragment, MouseEvent, useState } from 'react';
import styles from './index.module.scss';

export default function Navbar() {

    const router = useRouter();
    const me = useApiGetMe();
    const [isMenuOpen, setIsMenuOpen] = useState<boolean>(false);
    const [modalClickPosition, setModalClickPosition] = useState<iModalCustomPosition>({});

    function handleModalClick(e: MouseEvent<HTMLSpanElement>) {
        setModalClickPosition(handleGetModalClickPosition(e, 270, 70));
        setIsMenuOpen(true);
    }

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
                            <span onClick={(e) => handleModalClick(e)}>
                                <Icon icon='briefcase' weight='bold' />{me && me?.currentMainCompany ? me.currentMainCompany?.name : 'Olá, tudo bem?'} <Icon icon='chevron-down' weight='bold' />
                            </span>
                        </Tippy>
                    </div>
                </div>
            </nav>

            <ModalSettings
                isOpen={isMenuOpen}
                setModalIsOpen={setIsMenuOpen}
                customPosition={modalClickPosition}
            />
        </Fragment>
    )
}