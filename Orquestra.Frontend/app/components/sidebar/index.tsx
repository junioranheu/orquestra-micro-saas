import ChatBot from '@/app/components/chat-bot';
import Icon from '@/app/components/icon';
import { MODULES } from '@/app/consts/modules';
import ROUTES from '@/app/consts/routes';
import SYSTEM from '@/app/consts/system';
import { PACIFICO } from '@/app/fonts/fonts';
import { handleCheckShowElement } from '@/app/functions/check.permission';
import useApiGetMe from '@/app/hooks/api/useApiGetMe';
import { useShowChatbot, useShowExpandedSidebar } from '@/app/hooks/contexts/useGlobalContext';
import Tippy from '@tippyjs/react';
import feather from 'feather-icons';
import { usePathname, useRouter } from 'next/navigation';
import { Fragment, useEffect, useRef, useState } from 'react';
import styles from './index.module.scss';

interface iMenuItem {
    label: string;
    description: string;
    icon: keyof typeof feather.icons;
    route: (typeof ROUTES)[keyof typeof ROUTES];
    hasAccess: boolean;
}

interface iMenuGroup {
    label: string;
    items: iMenuItem[];
}

export default function Sidebar() {

    const router = useRouter();
    const pathname = usePathname();
    const me = useApiGetMe({});

    const [active, setActive] = useState<string>('');
    const [showExpandedSidebar,] = useShowExpandedSidebar();
    const [showChatbot,] = useShowChatbot();

    const [openPopover, setOpenPopover] = useState<string | null>(null);
    const [popoverPos, setPopoverPos] = useState<{ top: number; left: number }>({ top: 0, left: 0 });
    const popoverRef = useRef<HTMLDivElement>(null);

    function handleClickOutside(e: MouseEvent) {
        if (popoverRef.current && !popoverRef.current.contains(e.target as Node)) {
            setOpenPopover(null);
        }
    }

    useEffect(() => {
        setActive(pathname);
        document.addEventListener('mousedown', handleClickOutside);
        return () => document.removeEventListener('mousedown', handleClickOutside);
    }, [pathname]);

    const MENU_GROUPS = [
        {
            label: 'Geral',
            items: [
                { label: 'Início', description: `Visão geral e estatísticas rápidas do ${SYSTEM.NAME}.`, icon: 'home', route: ROUTES.DASHBOARD, hasAccess: true },
                // { label: 'Empresas', description: 'Gerencie os dados e informações de suas empresas cadastradas.', icon: 'briefcase', route: ROUTES.EMPRESA_GERENCIAR, hasAccess: handleCheckShowElement({ me, rolesRequired: [] }) },
                // ...((me && me?.isUserAdmOfCurrentMainCompany) ? [
                //     { label: 'Plano e faturas', description: 'Visualize o plano atual e gerencie suas faturas.', icon: 'tag', route: ROUTES.EMPRESA_USO_E_PLANO, hasAccess: handleCheckShowElement({ me, rolesRequired: [] }) },
                // ] : []),
                { label: 'Configurações', description: 'Personalize o sistema, altere informações da conta e troque sua senha.', icon: 'settings', route: ROUTES.USUARIO_CONFIGURACOES, hasAccess: handleCheckShowElement({ me, rolesRequired: [] }) },
            ]
        },
        {
            label: 'Operacional',
            items: [
                { label: 'Agenda', description: 'Gerencie todos os agendamentos.', icon: 'calendar', route: ROUTES.EMPRESA_AGENDAMENTOS, hasAccess: handleCheckShowElement({ me, rolesRequired: [MODULES.Scheduling] }) },
                { label: 'Colaboradores', description: 'Controle os usuários e profissionais da empresa.', icon: 'users', route: ROUTES.EMPRESA_COLABORADORES, hasAccess: handleCheckShowElement({ me, rolesRequired: [] }) },
                { label: 'Clientes', description: 'Gerencie informações e histórico dos clientes.', icon: 'user-check', route: ROUTES.EMPRESA_CLIENTES, hasAccess: handleCheckShowElement({ me, rolesRequired: [] }) },
                { label: 'Follow-up', description: 'Acompanhe retornos e contatos com clientes.', icon: 'repeat', route: ROUTES.EMPRESA_FOLLOW_UP, hasAccess: handleCheckShowElement({ me, rolesRequired: [MODULES.CostumerFollowUp] }) },
                { label: 'Orçamento', description: 'Crie e acompanhe propostas de serviço.', icon: 'file', route: ROUTES.EMPRESA_ORCAMENTO, hasAccess: handleCheckShowElement({ me, rolesRequired: [MODULES.Quote] }) },
                { label: 'Ordem de serviço', description: 'Gerencie execuções e status dos serviços.', icon: 'tool', route: ROUTES.EMPRESA_ORDEM_DE_SERVICO, hasAccess: handleCheckShowElement({ me, rolesRequired: [MODULES.ServiceOrder] }) },
                { label: 'Estoque', description: 'Controle produtos e materiais disponíveis.', icon: 'package', route: ROUTES.EMPRESA_ESTOQUE, hasAccess: handleCheckShowElement({ me, rolesRequired: [MODULES.Inventory] }) },
            ]
        },
        {
            label: 'Integração',
            items: [
                { label: 'WhatsApp', description: 'Configure e envie mensagens automáticas via WhatsApp.', icon: 'message-circle', route: ROUTES.EMPRESA_INTEGRACAO_WHATSAPP, hasAccess: handleCheckShowElement({ me, rolesRequired: [MODULES.IntegrationWhatsApp] }) }
            ]
        },
        {
            label: 'Financeiro',
            items: [
                { label: 'Nota fiscal', description: 'Emita e gerencie notas fiscais eletrônicas.', icon: 'file-text', route: ROUTES.EMPRESA_NOTA_FISCAL, hasAccess: handleCheckShowElement({ me, rolesRequired: [MODULES.Invoice] }) },
                { label: 'Gestão financeira', description: 'Acompanhe receitas, despesas e veja se a empresa teve lucro ou prejuízo.', icon: 'dollar-sign', route: ROUTES.EMPRESA_FINANCEIRO, hasAccess: handleCheckShowElement({ me, rolesRequired: [MODULES.Sales] }) }
            ]
        },
        {
            label: 'Sistema',
            items: [
                { label: 'Logs', description: 'Visualize registros e auditorias do sistema.', icon: 'terminal', route: ROUTES.LOGS, hasAccess: handleCheckShowElement({ me, rolesRequired: [], mustBeSystemAdmin: true }) }
            ]
        }
    ] as iMenuGroup[];

    function handleGroupClick(e: React.MouseEvent, groupLabel: string) {
        const rect = (e.currentTarget as HTMLElement).getBoundingClientRect();
        setPopoverPos({ top: rect.top, left: rect.right + 0 }); // Posição ao lado;
        setOpenPopover(openPopover === groupLabel ? null : groupLabel);
    }

    return (
        <Fragment>
            <aside className={`${styles.sidebar} notSelectable`}>
                <h1>{showExpandedSidebar}</h1>

                <div className={styles.brand}>
                    <Icon icon='calendar' weight='bold' />
                    <span className={PACIFICO.className}>{SYSTEM.NAME}</span>
                </div>

                <nav className={SYSTEM.ANIMATE_DELAY_0_5s}>
                    {
                        MENU_GROUPS?.map((group, gIndex) => {
                            const visibleItems = group.items.filter(x => x.hasAccess);

                            if (visibleItems.length === 0) {
                                return null;
                            }

                            const isExpanded = group.label === 'Geral' || group.label === 'Sistema' || showExpandedSidebar;

                            return (
                                <div key={gIndex} className={styles.group}>
                                    <div
                                        className={styles.groupHeader}
                                        onClick={isExpanded ? undefined : (e) => handleGroupClick(e, group.label)}
                                    >
                                        <span className={styles.groupLabel}>{group.label}</span>
                                        {!isExpanded && <Icon icon='chevron-right' />}
                                    </div>

                                    {
                                        isExpanded && (
                                            <ul>
                                                {
                                                    visibleItems.map((item, index) => (
                                                        <Tippy key={index} content={item.description} placement='right'>
                                                            <li
                                                                className={active === item.route ? styles.active : ''}
                                                                onClick={() => router.push(item.route)}
                                                            >
                                                                <Icon icon={item.icon} />
                                                                <span>{item.label}</span>
                                                            </li>
                                                        </Tippy>
                                                    ))
                                                }
                                            </ul>
                                        )
                                    }
                                </div>
                            );
                        })
                    }
                </nav>


                {
                    openPopover && (
                        <div
                            ref={popoverRef}
                            className={styles.popover}
                            style={{ top: popoverPos.top, left: popoverPos.left }}
                        >
                            {
                                MENU_GROUPS.find(g => g.label === openPopover)?.items.filter(x => x.hasAccess).map((item, index) => (
                                    <Tippy key={index} content={item.description} placement='right'>
                                        <div
                                            className={`${styles.popoverItem} ${active === item.route ? styles.active : ''}`}
                                            onClick={() => { router.push(item.route); setOpenPopover(null); }}
                                        >
                                            <Icon icon={item.icon} />
                                            <span>{item.label}</span>
                                        </div>
                                    </Tippy>
                                ))
                            }
                        </div>
                    )
                }
            </aside>

            {
                showChatbot && (
                    <ChatBot me={me} showButtonAbsolute={false} />
                )
            }
        </Fragment>
    )
}