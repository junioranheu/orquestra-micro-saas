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
    USUARIO_SENHA_REDEFINIDA: '/usuario/senha-redefinida',
    USUARIO_CONFIGURACOES: '/usuario/configuracoes',
    USUARIO_NOTIFICACOES: '/usuario/notificacoes',
    ETC_AJUDA: '/ajuda',
    ETC_SEGURANCA: '/seguranca',
    ETC_PRIVACIDADE: '/privacidade',
    ETC_TERMOS_DE_USO: '/termos-de-uso',
    EMPRESA_GERENCIAR: '/empresa/gerenciar',
    EMPRESA_VERIFICADA: '/empresa/verificada',
    EMPRESA_USO_E_PLANO: '/empresa/uso-e-plano',
    EMPRESA_COLABORADORES: '/empresa/colaboradores',
    EMPRESA_CLIENTES: '/empresa/clientes',
    EMPRESA_FINANCEIRO: '/empresa/financeiro',
    EMPRESA_FOLLOW_UP: '/empresa/follow-up',
    EMPRESA_NOTA_FISCAL: '/empresa/nota-fiscal',
    EMPRESA_INTEGRACAO_WHATSAPP: '/empresa/integracao-whatsapp',
    EMPRESA_AGENDAMENTOS: '/empresa/agenda',
    DASHBOARD: '/dashboard'
};

export default ROUTES;