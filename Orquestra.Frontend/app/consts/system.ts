import { Guid } from 'guid-typescript';

const SYSTEM = {
    NAME: 'Orquestra',
    DESCRIPTION: 'Harmonia na gestão do seu negócio',
    AUTHOR: '@junioranheu',
    COLOR: '#f0fcdc',
    MASCOT: 'Maestro',

    EMAIL_SUPPORT: 'orquestra.saas@gmail.com',
    PHONE_SUPPORT: '12982716339',
    URL_GITHUB: 'https://github.com/junioranheu',
    URL_LINKEDIN: 'https://www.linkedin.com/in/junioranheu/',

    COOKIE_AUTH_FRONT: 'COOKIE_AUTH_FRONT',
    LOCAL_STORAGE_FONT_SIZE: 'LOCAL_STORAGE_FONT_SIZE',
    LOCAL_STORAGE_SHOW_MASCOT: 'LOCAL_STORAGE_SHOW_MASCOT',
    LOCAL_STORAGE_SHOW_EXPANDED_SIDEBAR: 'LOCAL_STORAGE_SHOW_EXPANDED_SIDEBAR',
    LOCAL_STORAGE_SHOW_CHATBOT: 'LOCAL_STORAGE_SHOW_CHATBOT',
    LOCAL_STORAGE_THEME: 'LOCAL_STORAGE_THEME',
    LOCAL_STORAGE_USER_FONT_SIZE: 'LOCAL_STORAGE_USER_FONT_SIZE',

    ANIMATE: 'animate__animated animate__fadeIn',
    ANIMATE_SLOW: 'animate__animated animate__fadeIn animate__slow',
    ANIMATE_FAST: 'animate__animated animate__fadeIn animate__faster',
    ANIMATE_FADE_IN_RIGHT_FAST: 'animate__animated animate__fadeInRight animate__fast',
    ANIMATE_DELAY_1s: 'animate__animated animate__fadeIn animate__fast animate__delay-1s',
    ANIMATE_DELAY_0_5s: 'animate__animated animate__fadeIn animate__fast animate__delay-0_5s',
    ANIMATE_DELAY_FASTER: 'animate__animated animate__fadeIn animate__fast animate__delay-faster',
    ANIMATE_PULSE_INFINITE: 'animate__animated animate__pulse animate__slower animate__infinite',

    EMPTY_GUID: Guid.parse('00000000-0000-0000-0000-000000000000'),
    EMPTY_DATE: new Date(-62135596800000),

    WARN_FILL_OBLIGATORY_FIELDS: 'Preencha todos os campos obrigatórios (*) antes de prosseguir com esta ação.'
};

export default SYSTEM;