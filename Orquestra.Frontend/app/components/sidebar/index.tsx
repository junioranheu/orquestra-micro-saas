import Icon from '@/app/components/icon';
import { MODULES } from '@/app/consts/modules';
import ROUTES from '@/app/consts/routes';
import SYSTEM from '@/app/consts/system';
import { PACIFICO } from '@/app/fonts/fonts';
import { handleCheckShowElement } from '@/app/functions/check.permission';
import useApiGetMe from '@/app/hooks/api/useApiGetMe';
import Tippy from '@tippyjs/react';
import feather from 'feather-icons';
import { usePathname, useRouter } from 'next/navigation';
import { useEffect, useState } from 'react';
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

    const menuGroups = [
        {
            label: 'Geral',
            items: [
                { label: 'Início', description: `Visão geral e estatísticas rápidas do ${SYSTEM.NAME}.`, icon: 'home', route: ROUTES.DASHBOARD, hasAccess: true },
                { label: 'Colaboradores', description: 'Controle os usuários e profissionais da empresa.', icon: 'users', route: ROUTES.EMPRESA_COLABORADORES, hasAccess: handleCheckShowElement({ me, rolesRequired: [] }) }
            ]
        },
        {
            label: 'Operacional',
            items: [
                { label: 'Agenda', description: 'Gerencie todos os agendamentos.', icon: 'calendar', route: ROUTES.EMPRESA_AGENDAMENTOS, hasAccess: handleCheckShowElement({ me, rolesRequired: [MODULES.Scheduling] }) },
                { label: 'Clientes', description: 'Gerencie informações e histórico dos clientes.', icon: 'user-check', route: ROUTES.EMPRESA_CLIENTES, hasAccess: handleCheckShowElement({ me, rolesRequired: [] }) },
                { label: 'Configurações', description: 'Personalize o sistema, altere informações da conta e troque sua senha.', icon: 'settings', route: ROUTES.USUARIO_CONFIGURACOES, hasAccess: handleCheckShowElement({ me, rolesRequired: [] }) },
                { label: 'Follow-up', description: 'Acompanhe retornos e contatos com clientes.', icon: 'repeat', route: ROUTES.EMPRESA_FOLLOW_UP, hasAccess: handleCheckShowElement({ me, rolesRequired: [MODULES.CostumerFollowUp] }) },
                { label: 'Orçamento', description: 'Crie e acompanhe propostas de serviço.', icon: 'file', route: ROUTES.EMPRESA_ORCAMENTO, hasAccess: handleCheckShowElement({ me, rolesRequired: [MODULES.Quote] }) },
                { label: 'OS', description: 'Gerencie execuções e status dos serviços.', icon: 'tool', route: ROUTES.EMPRESA_ORDEM_DE_SERVICO, hasAccess: handleCheckShowElement({ me, rolesRequired: [MODULES.ServiceOrder] }) },
                { label: 'Estoque', description: 'Controle produtos e materiais disponíveis.', icon: 'package', route: ROUTES.EMPRESA_ESTOQUE, hasAccess: handleCheckShowElement({ me, rolesRequired: [MODULES.Inventory] }) }
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
                { label: 'Financeiro', description: 'Acompanhe receitas, despesas e veja se a empresa teve lucro ou prejuízo.', icon: 'dollar-sign', route: ROUTES.EMPRESA_FINANCEIRO, hasAccess: handleCheckShowElement({ me, rolesRequired: [MODULES.Sales] }) }
            ]
        },
        {
            label: 'Sistema',
            items: [
                { label: 'Logs', description: 'Visualize registros e auditorias do sistema.', icon: 'terminal', route: ROUTES.LOGS, hasAccess: handleCheckShowElement({ me, rolesRequired: [], mustBeSystemAdmin: true }) }
            ]
        }
    ] as iMenuGroup[];

    useEffect(() => {
        setActive(pathname);
    }, [pathname]);

    return (
        <aside className={`${styles.sidebar} notSelectable`}>
            <div className={styles.brand}>
                <Icon icon='calendar' weight='bold' />
                <span className={PACIFICO.className}>{SYSTEM.NAME}</span>
            </div>

            <nav>
                {
                    menuGroups.map((group, gIndex) => {
                        const visibleItems = group.items.filter(x => x.hasAccess);

                        if (visibleItems.length === 0) {
                            return null;
                        }

                        return (
                            <div key={gIndex} className={styles.group}>
                                <span className={styles.groupLabel}>{group.label}</span>

                                <ul>
                                    {
                                        visibleItems.map((item, index) => (
                                            <Tippy key={index} content={item.description} placement='bottom'>
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
                            </div>
                        );
                    })
                }
            </nav>
        </aside>
    )
}