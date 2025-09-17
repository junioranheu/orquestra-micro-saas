import SYSTEM from './system';

const ROUTES = {
    LANDING_PAGE: `/${SYSTEM.NAME.toLocaleLowerCase()}`,
    LOGIN: '/login',
    RECUPERAR_SENHA: '/recuperar-senha',
    CRIAR_CONTA: '/criar-conta',
    LOGOUT: '/logout',
    ERRO_403: '/erro/403',
    USUARIO_VERIFICADO: '/usuario/verificado',
    USUARIO_CONFIGURACOES: '/usuario/configuracoes',
    USUARIO_NOTIFICACOES: '/usuario/notificacoes',
    ETC_AJUDA: '/etc/ajuda',
    ETC_SEGURANCA: '/etc/seguranca',
    EMPRESA_GERENCIAR: '/empresa/gerenciar',
    EMPRESA_USO_E_PLANO: '/empresa/uso-e-plano',
    EMPRESA_MEMBROS: '/empresa/membros',
    EMPRESA_CLIENTES: '/empresa/clientes',
    EMPRESA_FINANCEIRO: '/empresa/financeiro',
    EMPRESA_AGENDAMENTOS: '/empresa/agendamentos',
    DASHBOARD: '/dashboard'
};

export default ROUTES;