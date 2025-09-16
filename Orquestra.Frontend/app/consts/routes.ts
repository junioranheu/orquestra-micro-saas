import SYSTEM from './system';

const ROUTES = {
    LANDING_PAGE: `/${SYSTEM.NAME.toLocaleLowerCase()}`,
    LOGIN: '/login',
    CRIAR_CONTA: '/criar-conta',
    LOGOUT: '/logout',
    USUARIO_VERIFICADO: '/usuario/verificado',
    USUARIO_CONFIGURACOES: '/usuario/configuracoes',
    USUARIO_NOTIFICACOES: '/usuario/notificacoes',
    ETC_AJUDA: '/etc/ajuda',
    ETC_SEGURANCA: '/etc/seguranca',
    EMPRESA_USO_E_PLANO: '/empresa/uso-e-plano',
    EMPRESA_USUARIOS: '/empresa/usuarios',
    DASHBOARD: '/dashboard'
};

export default ROUTES;