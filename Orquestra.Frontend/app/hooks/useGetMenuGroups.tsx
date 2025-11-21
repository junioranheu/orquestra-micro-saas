import { iMe } from '@/app/api/consts/auth';
import { TIPPY_CHATBOT } from '@/app/components/chat-bot';
import ROUTES from '@/app/consts/routes';
import SYSTEM from '@/app/consts/system';
import { MODULE_ENUM } from '@/app/enums/modulesEnum';
import { handleCheckShowElement } from '@/app/functions/check.permission';
import feather from 'feather-icons';
import { usePathname, useRouter } from 'next/navigation';
import { useEffect, useState } from 'react';
import { useIsOpenChatbot, useShowChatbot } from './contexts/useGlobalContext';

interface iProps {
    me: iMe | undefined;
}

export interface iMenuGroup {
    label: string;
    items: {
        id: string;
        label: string;
        description: string;
        icon: keyof typeof feather.icons;
        route?: (typeof ROUTES)[keyof typeof ROUTES];
        onClick?: () => void;
        hasAccess: boolean;
    }[];
}

export function useMenuGroups({ me }: iProps): iMenuGroup[] {

    const router = useRouter();
    const pathname = usePathname();
    const [showChatbot,] = useShowChatbot();
    const [, setIsOpenChatbot] = useIsOpenChatbot();
    const [menu, setMenu] = useState<iMenuGroup[]>([]);

    useEffect(() => {
        const MENU_GROUPS: iMenuGroup[] = [
            {
                label: 'Geral',
                items: [
                    { id: 'inicio', label: 'Início', description: `Visão geral e estatísticas rápidas do ${SYSTEM.NAME}.`, icon: 'home', route: ROUTES.DASHBOARD, hasAccess: true },
                    { id: 'configuracoes', label: 'Configurações', description: 'Personalize o sistema, altere informações da conta e troque sua senha.', icon: 'settings', route: ROUTES.USUARIO_CONFIGURACOES, hasAccess: true },
                    // @ts-expect-error: wtf;
                    ...(showChatbot ? [{ id: 'chatbot', label: 'Chatbot', description: TIPPY_CHATBOT, icon: 'message-square', onClick: () => setIsOpenChatbot(true), hasAccess: true }] : []),
                ]
            },
            {
                label: 'Operacional',
                items: [
                    { id: 'agenda', label: 'Agenda', description: 'Gerencie todos os agendamentos.', icon: 'calendar', route: ROUTES.EMPRESA_AGENDAMENTOS, hasAccess: handleCheckShowElement({ me, modulesRequired: [MODULE_ENUM.Scheduling] }) },
                    { id: 'colaboradores', label: 'Colaboradores', description: 'Controle os usuários e profissionais da empresa.', icon: 'users', route: ROUTES.EMPRESA_COLABORADORES, hasAccess: handleCheckShowElement({ me, modulesRequired: [MODULE_ENUM.Member] }) },
                    { id: 'clientes', label: 'Clientes', description: 'Gerencie informações e histórico dos clientes.', icon: 'user-check', route: ROUTES.EMPRESA_CLIENTES, hasAccess: handleCheckShowElement({ me, modulesRequired: [MODULE_ENUM.Client] }) },
                    { id: 'acompanhamentos', label: 'Acompanhamentos', description: 'Acompanhe retornos e contatos com clientes.', icon: 'repeat', route: ROUTES.EMPRESA_ACOMPANHAMENTO, hasAccess: handleCheckShowElement({ me, modulesRequired: [MODULE_ENUM.CostumerFollowUp] }) },
                    { id: 'orcamentos', label: 'Orçamentos', description: 'Crie e acompanhe propostas de serviço.', icon: 'file', route: ROUTES.EMPRESA_ORCAMENTO, hasAccess: handleCheckShowElement({ me, modulesRequired: [MODULE_ENUM.Quote] }) },
                    { id: 'ordem-de-servico', label: 'Ordens de serviço', description: 'Gerencie execuções e status dos serviços.', icon: 'tool', route: ROUTES.EMPRESA_ORDEM_DE_SERVICO, hasAccess: handleCheckShowElement({ me, modulesRequired: [MODULE_ENUM.ServiceOrder] }) },
                    { id: 'estoque', label: 'Estoque', description: 'Controle produtos e materiais disponíveis.', icon: 'package', route: ROUTES.EMPRESA_ESTOQUE, hasAccess: handleCheckShowElement({ me, modulesRequired: [MODULE_ENUM.Inventory] }) },
                ]
            },
            {
                label: 'Integração',
                items: [
                    { id: 'whatsapp', label: 'WhatsApp', description: 'Configure e envie mensagens automáticas via WhatsApp.', icon: 'message-circle', route: ROUTES.EMPRESA_INTEGRACAO_WHATSAPP, hasAccess: handleCheckShowElement({ me, modulesRequired: [MODULE_ENUM.IntegrationWhatsApp] }) }
                ]
            },
            {
                label: 'Financeiro',
                items: [
                    { id: 'nota-fiscal', label: 'Nota fiscal', description: 'Emita e gerencie notas fiscais eletrônicas.', icon: 'file-text', route: ROUTES.EMPRESA_NOTA_FISCAL, hasAccess: handleCheckShowElement({ me, modulesRequired: [MODULE_ENUM.Invoice] }) },
                    { id: 'gestao-financeira', label: 'Gestão financeira', description: 'Acompanhe receitas, despesas e veja se a empresa teve lucro ou prejuízo.', icon: 'dollar-sign', route: ROUTES.EMPRESA_FINANCEIRO, hasAccess: handleCheckShowElement({ me, modulesRequired: [MODULE_ENUM.Sales] }) }
                ]
            },
            {
                label: 'Sistema',
                items: [
                    { id: 'logs', label: 'Logs', description: 'Visualize registros e auditorias do sistema.', icon: 'terminal', route: ROUTES.LOGS, hasAccess: handleCheckShowElement({ me, modulesRequired: [], mustBeSystemAdmin: true }) },
                    { id: 'logoff', label: 'Finalizar sessão', description: `Encerre sua sessão atual no ${SYSTEM.NAME}.`, icon: 'log-out', onClick: () => router.push(ROUTES.LOGOUT), hasAccess: true },
                ]
            }
        ];

        setMenu(MENU_GROUPS);
    }, [me, pathname, showChatbot, setIsOpenChatbot, router]);

    return menu;

}