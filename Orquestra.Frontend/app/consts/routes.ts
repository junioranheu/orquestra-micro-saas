import SYSTEM from './system';

const ROUTES = {
    LANDING_PAGE: `/${SYSTEM.NAME.toLocaleLowerCase()}`,
    LOGIN: '/login',
    CRIAR_CONTA: '/criar-conta',
    LOGOUT: '/logout',
    LOGS: '/logs',
    ERRO_403: '/erro/403',
    ERRO_404: '/erro/404',
    USUARIO_VERIFICADO: '/usuario/verificado',
    USUARIO_CONFIGURACOES: '/usuario/configuracoes',
    USUARIO_NOTIFICACOES: '/usuario/notificacoes',
    ETC_AJUDA: '/ajuda',
    ETC_SEGURANCA: '/seguranca',
    ETC_PRIVACIDADE: '/privacidade',
    ETC_TERMOS_DE_USO: '/termos-de-uso',
    EMPRESA_GERENCIAR: '/empresa/gerenciar',
    EMPRESA_USO_E_PLANO: '/empresa/uso-e-plano',
    EMPRESA_MEMBROS: '/empresa/membros',
    EMPRESA_CLIENTES: '/empresa/clientes',
    EMPRESA_FINANCEIRO: '/empresa/financeiro',
    EMPRESA_AGENDAMENTOS: '/empresa/agendamentos',
    DASHBOARD: '/dashboard'
};

export default ROUTES;