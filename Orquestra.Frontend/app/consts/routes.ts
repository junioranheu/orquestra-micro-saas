import SYSTEM from './system';

const ROUTES = {
    LANDING_PAGE: `/${SYSTEM.NAME.toLocaleLowerCase()}`,
    LOGIN: '/login',
    CRIAR_CONTA: '/criar-conta',
    LOGOUT: '/logout',
    USUARIO_VERIFICADO: '/usuario/verificado',
    DASHBOARD: '/dashboard'
};

export default ROUTES;