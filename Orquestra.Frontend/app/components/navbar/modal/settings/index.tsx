import Icon from '@/app/components/icon';
import ModalGeneric, { iModalCustomPosition } from '@/app/components/modal/generic';
import ROUTES from '@/app/consts/routes';
import { handleGetNameInitials } from '@/app/functions/get.formatUserName';
import useApiGetMe from '@/app/hooks/api/useApiGetMe';
import feather from 'feather-icons';
import { useRouter } from 'next/navigation';
import { Dispatch, Fragment, JSX, SetStateAction } from 'react';
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
    return (
        <ModalGeneric
            isOpen={isOpen}
            setModalIsOpen={setModalIsOpen}
            customPosition={customPosition}
            showCloseButton={false}
            overlayColor={0}
            customStyle={{ padding: 0 }}
        >
            <ProfileMenu setModalIsOpen={setModalIsOpen} />
        </ModalGeneric>
    )
}

export function MenuItem({ icon, label, desc, badge, handleFunction }: iPropsMenuItem) {
    return (
        <button className={styles.item} type='button'>
            <span className={styles.icon}><Icon icon={icon} weight='bold' /></span>

            <div className={styles.content} onClick={() => handleFunction && handleFunction()}>
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

    function handleLogout() {
        setModalIsOpen(false);
        router.push(ROUTES.LOGOUT);
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
                {
                    (me && me?.isUserAdmOfCurrentMainCompany) && (
                        <Fragment>
                            <MenuItem icon='credit-card' label='Plano da empresa' />
                            <MenuItem icon='user' label='Usuários' badge={me?.currentMainCompany?.name} />
                            <div className={styles.separator} />
                        </Fragment>
                    )
                }

                <MenuItem icon='settings' label='Configurações' />
                <MenuItem icon='shield' label='Segurança' />

                <div className={styles.separator} />
                <MenuItem icon='log-out' label='Sair' handleFunction={() => handleLogout()} />
            </div>
        </div>
    )
}