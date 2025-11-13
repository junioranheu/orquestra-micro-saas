import { iMe } from '@/app/api/consts/auth';
import { MODULES } from '@/app/consts/modules';
import ROUTES from '@/app/consts/routes';
import SYSTEM from '@/app/consts/system';
import { handleCheckShowElement } from '@/app/functions/check.permission';
import feather from 'feather-icons';
import { useEffect, useState } from 'react';

interface iProps {
    me: iMe | undefined;
}

export interface iTourGroup {
    selector: string;
    content: string;
}

interface iUseMenuGroupsReturn {
    MENU_GROUPS: iMenuGroup[];
    TOUR_STEPS: iTourGroup[];
}

export interface iMenuGroup {
    label: string;
    items: {
        id: string;
        label: string;
        description: string;
        icon: keyof typeof feather.icons;
        route: (typeof ROUTES)[keyof typeof ROUTES];
        hasAccess: boolean;
    }[];
}

export function useMenuGroups({ me }: iProps): iUseMenuGroupsReturn {

    const [menuGroups, setMenuGroups] = useState<iMenuGroup[]>([]);
    const [tourSteps, setTourSteps] = useState<iTourGroup[]>([]);

    useEffect(() => {
        const MENU_GROUPS: iMenuGroup[] = [
            {
                label: 'Geral',
                items: [
                    { id: 'inicio', label: 'Início', description: `Visão geral e estatísticas rápidas do ${SYSTEM.NAME}.`, icon: 'home', route: ROUTES.DASHBOARD, hasAccess: true },
                    { id: 'configuracoes', label: 'Configurações', description: 'Personalize o sistema, altere informações da conta e troque sua senha.', icon: 'settings', route: ROUTES.USUARIO_CONFIGURACOES, hasAccess: handleCheckShowElement({ me, rolesRequired: [] }) },
                ]
            },
            {
                label: 'Operacional',
                items: [
                    { id: 'agenda', label: 'Agenda', description: 'Gerencie todos os agendamentos.', icon: 'calendar', route: ROUTES.EMPRESA_AGENDAMENTOS, hasAccess: handleCheckShowElement({ me, rolesRequired: [MODULES.Scheduling] }) },
                    { id: 'colaboradores', label: 'Colaboradores', description: 'Controle os usuários e profissionais da empresa.', icon: 'users', route: ROUTES.EMPRESA_COLABORADORES, hasAccess: handleCheckShowElement({ me, rolesRequired: [] }) },
                    { id: 'clientes', label: 'Clientes', description: 'Gerencie informações e histórico dos clientes.', icon: 'user-check', route: ROUTES.EMPRESA_CLIENTES, hasAccess: handleCheckShowElement({ me, rolesRequired: [] }) },
                    { id: 'acompanhamento', label: 'Acompanhamento', description: 'Acompanhe retornos e contatos com clientes.', icon: 'repeat', route: ROUTES.EMPRESA_ACOMPANHAMENTO, hasAccess: handleCheckShowElement({ me, rolesRequired: [MODULES.CostumerFollowUp] }) },
                    { id: 'orcamento', label: 'Orçamento', description: 'Crie e acompanhe propostas de serviço.', icon: 'file', route: ROUTES.EMPRESA_ORCAMENTO, hasAccess: handleCheckShowElement({ me, rolesRequired: [MODULES.Quote] }) },
                    { id: 'ordem-de-servico', label: 'Ordem de serviço', description: 'Gerencie execuções e status dos serviços.', icon: 'tool', route: ROUTES.EMPRESA_ORDEM_DE_SERVICO, hasAccess: handleCheckShowElement({ me, rolesRequired: [MODULES.ServiceOrder] }) },
                    { id: 'estoque', label: 'Estoque', description: 'Controle produtos e materiais disponíveis.', icon: 'package', route: ROUTES.EMPRESA_ESTOQUE, hasAccess: handleCheckShowElement({ me, rolesRequired: [MODULES.Inventory] }) },
                ]
            },
            {
                label: 'Integração',
                items: [
                    { id: 'whatsapp', label: 'WhatsApp', description: 'Configure e envie mensagens automáticas via WhatsApp.', icon: 'message-circle', route: ROUTES.EMPRESA_INTEGRACAO_WHATSAPP, hasAccess: handleCheckShowElement({ me, rolesRequired: [MODULES.IntegrationWhatsApp] }) }
                ]
            },
            {
                label: 'Financeiro',
                items: [
                    { id: 'nota-fiscal', label: 'Nota fiscal', description: 'Emita e gerencie notas fiscais eletrônicas.', icon: 'file-text', route: ROUTES.EMPRESA_NOTA_FISCAL, hasAccess: handleCheckShowElement({ me, rolesRequired: [MODULES.Invoice] }) },
                    { id: 'gestao-financeira', label: 'Gestão financeira', description: 'Acompanhe receitas, despesas e veja se a empresa teve lucro ou prejuízo.', icon: 'dollar-sign', route: ROUTES.EMPRESA_FINANCEIRO, hasAccess: handleCheckShowElement({ me, rolesRequired: [MODULES.Sales] }) }
                ]
            },
            {
                label: 'Sistema',
                items: [
                    { id: 'logs', label: 'Logs', description: 'Visualize registros e auditorias do sistema.', icon: 'terminal', route: ROUTES.LOGS, hasAccess: handleCheckShowElement({ me, rolesRequired: [], mustBeSystemAdmin: true }) }
                ]
            }
        ];

        const TOUR_STEPS = MENU_GROUPS.flatMap(group =>
            group.items.filter(item => item.hasAccess).map(item => ({
                selector: `#${item.id}`,
                content: item.description || item.label
            }))
        );

        const activeGroups = MENU_GROUPS.filter(group =>
            group.items.some(item => item.hasAccess)
        );

        setMenuGroups(MENU_GROUPS);
        setTourSteps(activeGroups.length > 1 ? TOUR_STEPS : []);
    }, [me]);

    return {
        MENU_GROUPS: menuGroups,
        TOUR_STEPS: tourSteps
    };

}