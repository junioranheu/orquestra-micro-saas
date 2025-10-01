import SYSTEM from './system';

const ROUTES = {
    LANDING_PAGE: `/${SYSTEM.NAME.toLocaleLowerCase()}`,
    LOGIN: '/login',
    RECUPERAR_SENHA: '/recuperar-senha',
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
    EMPRESA_GERENCIAR: '/empresa/gerenciar',
    EMPRESA_USO_E_PLANO: '/empresa/uso-e-plano',
    EMPRESA_MEMBROS: '/empresa/membros',
    EMPRESA_CLIENTES: '/empresa/clientes',
    EMPRESA_FINANCEIRO: '/empresa/financeiro',
    EMPRESA_AGENDAMENTOS: '/empresa/agendamentos',
    EMPRESA_CADASTRAR: '/empresa/cadastrar',
    DASHBOARD: '/dashboard'
};

export default ROUTES;