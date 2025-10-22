import { iMe } from '@/app/api/consts/auth';
import Icon from '@/app/components/icon';
import ModalGeneric, { iModalCustomPosition } from '@/app/components/modal/generic';
import ROUTES from '@/app/consts/routes';
import { handleGetFirstName, handleGetNameInitials } from '@/app/functions/get.formatUserName';
import useWindowSize from '@/app/hooks/useWindowSize';
import feather from 'feather-icons';
import { useRouter } from 'next/navigation';
import { Dispatch, Fragment, JSX, SetStateAction, useEffect, useState } from 'react';
import styles from './index.module.scss';

interface iProps {
    isOpen: boolean;
    setIsModalOpen: Dispatch<SetStateAction<boolean>>;
    customPosition: iModalCustomPosition;
    me: iMe | undefined;
}

interface iPropsMenuItem {
    icon: keyof typeof feather.icons;
    label: string;
    desc?: string;
    badge?: string;
    handleFunction?: ((param?: any) => void) | null;
}

export default function ModalMenu({ isOpen, setIsModalOpen, customPosition, me }: iProps) {

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
            setIsModalOpen={setIsModalOpen}
            customPosition={customPosition}
            showCloseButton={false}
            overlayColor={0}
            style={{ padding: 0, visibility: showContent ? 'visible' : 'hidden' }}
        >
            <ProfileMenu setIsModalOpen={setIsModalOpen} me={me} />
        </ModalGeneric>
    )
}

export function MenuItem({ icon, label, desc, badge, handleFunction }: iPropsMenuItem) {
    return (
        <button className={styles.item} type='button' onClick={() => handleFunction && handleFunction()}>
            <span className={styles.icon}><Icon icon={icon} weight='normal' /></span>

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
    setIsModalOpen: Dispatch<SetStateAction<boolean>>;
    me: iMe | undefined;
}

export function ProfileMenu({ setIsModalOpen, me }: iPropsProfileMenu): JSX.Element {

    const router = useRouter();
    const windowSize = useWindowSize();

    function handleRedirect(route: (typeof ROUTES)[keyof typeof ROUTES]) {
        setIsModalOpen(false);
        router.push(route);
    }

    return (
        <div className={styles.main} role='menu'>
            <div className={styles.header}>
                <div className={styles.avatar}>{handleGetNameInitials(me?.userName)}</div>

                <div className={styles.userInfo}>
                    <div className={styles.name}>{handleGetFirstName(me?.userName)}</div>
                    <div className={styles.email}>{me?.email}</div>
                </div>
            </div>

            <div className={styles.separator} />

            <div className={styles.menu}>
                <MenuItem icon='briefcase' label='Gerenciar empresas' handleFunction={() => handleRedirect(ROUTES.EMPRESA_GERENCIAR)} />

                {
                    (me && me?.isUserAdmOfCurrentMainCompany) && (
                        <Fragment>
                            <MenuItem icon='tag' label='Plano e faturas' handleFunction={() => handleRedirect(ROUTES.EMPRESA_USO_E_PLANO)} />
                            <MenuItem icon='bell' label='Notificações' handleFunction={() => handleRedirect(ROUTES.USUARIO_NOTIFICACOES)} />
                            <div className={styles.separator} />
                        </Fragment>
                    )
                }

                <MenuItem icon='help-circle' label='Central de ajuda' handleFunction={() => handleRedirect(ROUTES.ETC_AJUDA)} />

                {
                    windowSize?.height > 900 && (
                        <Fragment>
                            <MenuItem icon='file' label='Termos de uso' handleFunction={() => handleRedirect(ROUTES.ETC_TERMOS_DE_USO)} />
                            <MenuItem icon='lock' label='Privacidade' handleFunction={() => handleRedirect(ROUTES.ETC_PRIVACIDADE)} />
                            <MenuItem icon='shield' label='Segurança' handleFunction={() => handleRedirect(ROUTES.ETC_SEGURANCA)} />
                            <MenuItem icon='smile' label='Página de apresentação' handleFunction={() => handleRedirect(ROUTES.LANDING_PAGE)} />
                        </Fragment>
                    )
                }

                <div className={styles.separator} />
                <MenuItem icon='settings' label='Configurações' handleFunction={() => handleRedirect(ROUTES.USUARIO_CONFIGURACOES)} />
                <MenuItem icon='log-out' label='Finalizar sessão' handleFunction={() => handleRedirect(ROUTES.LOGOUT)} />
            </div>
        </div>
    )
}