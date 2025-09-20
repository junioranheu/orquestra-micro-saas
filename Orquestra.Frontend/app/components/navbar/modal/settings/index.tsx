import Icon from '@/app/components/icon';
import ModalGeneric, { iModalCustomPosition } from '@/app/components/modal/generic';
import ROUTES from '@/app/consts/routes';
import { handleGetNameInitials } from '@/app/functions/get.formatUserName';
import useApiGetMe from '@/app/hooks/api/useApiGetMe';
import feather from 'feather-icons';
import { useRouter } from 'next/navigation';
import { Dispatch, Fragment, JSX, SetStateAction, useEffect, useState } from 'react';
import styles from './index.module.scss';

interface iProps {
    isOpen: boolean;
    setModalIsOpen: Dispatch<SetStateAction<boolean>>;
    customPosition: iModalCustomPosition;
}

interface iPropsMenuItem {
    icon: keyof typeof feather.icons;
    label: string;
    desc?: string;
    badge?: string;
    handleFunction?: ((param?: any) => void) | null;
}

export default function ModalSettings({ isOpen, setModalIsOpen, customPosition }: iProps) {

    const [showContent, setShowContent] = useState<boolean>(false);

    useEffect(() => {
        let timer: NodeJS.Timeout;

        if (isOpen) {
            // Abre com delay para evitar a visualização do loading do conteúdo;
            timer = setTimeout(() => setShowContent(true), 100);
        } else {
            setShowContent(false);
        }

        return () => clearTimeout(timer);
    }, [isOpen]);

    return (
        <ModalGeneric
            isOpen={isOpen}
            setModalIsOpen={setModalIsOpen}
            customPosition={customPosition}
            showCloseButton={false}
            overlayColor={0}
            style={{ padding: 0, visibility: showContent ? 'visible' : 'hidden', }}
        >
            <ProfileMenu setModalIsOpen={setModalIsOpen} />
        </ModalGeneric>
    )
}

export function MenuItem({ icon, label, desc, badge, handleFunction }: iPropsMenuItem) {
    return (
        <button className={styles.item} type='button' onClick={() => handleFunction && handleFunction()}>
            <span className={styles.icon}><Icon icon={icon} weight='bold' /></span>

            <div className={styles.content}>
                <div className={styles.labelRow}>
                    <span className={styles.label}>{label}</span>
                    {badge && <span className={styles.badge}>{badge}</span>}
                </div>

                {desc && <span className={styles.desc}>{desc}</span>}
            </div>
        </button>
    )
}

interface iPropsProfileMenu {
    setModalIsOpen: Dispatch<SetStateAction<boolean>>;
}

export function ProfileMenu({ setModalIsOpen }: iPropsProfileMenu): JSX.Element {

    const me = useApiGetMe();
    const router = useRouter();

    function handleRedirect(route: (typeof ROUTES)[keyof typeof ROUTES]) {
        setModalIsOpen(false);
        router.push(route);
    }

    return (
        <div className={styles.main} role='menu'>
            <div className={styles.header}>
                <div className={styles.avatar}>{handleGetNameInitials(me?.userName)}</div>

                <div className={styles.userInfo}>
                    <div className={styles.name}>{me?.userName}</div>
                    <div className={styles.email}>{me?.email}</div>
                </div>
            </div>

            <div className={styles.separator} />

            <div className={styles.menu}>
                <MenuItem icon='briefcase' label='Gerenciar empresas' handleFunction={() => handleRedirect(ROUTES.EMPRESA_GERENCIAR)} />

                {
                    (me && me?.isUserAdmOfCurrentMainCompany) && (
                        <Fragment>
                            <MenuItem icon='tag' label='Uso, plano e faturas' handleFunction={() => handleRedirect(ROUTES.EMPRESA_USO_E_PLANO)} />
                            <MenuItem icon='bell' label='Notificações' handleFunction={() => handleRedirect(ROUTES.USUARIO_NOTIFICACOES)} />
                            <div className={styles.separator} />
                        </Fragment>
                    )
                }

                <MenuItem icon='help-circle' label='Ajuda' handleFunction={() => handleRedirect(ROUTES.ETC_AJUDA)} />
                <MenuItem icon='shield' label='Segurança' handleFunction={() => handleRedirect(ROUTES.ETC_SEGURANCA)} />
                <MenuItem icon='settings' label='Configurações' handleFunction={() => handleRedirect(ROUTES.USUARIO_CONFIGURACOES)} />

                <div className={styles.separator} />
                <MenuItem icon='log-out' label='Sair' handleFunction={() => handleRedirect(ROUTES.LOGOUT)} />
            </div>
        </div>
    )
}