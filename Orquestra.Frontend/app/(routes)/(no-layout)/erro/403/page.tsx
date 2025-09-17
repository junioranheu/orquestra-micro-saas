'use client';
import Icon from '@/app/components/icon';
import ROUTES from '@/app/consts/routes';
import SYSTEM from '@/app/consts/system';
import useTitle from '@/app/hooks/useTitle';
import styles from './page.module.scss';

export default function Erro403() {

    useTitle('Acesso negado');

    return (
        <main className={styles.container}>
            <div className={styles.inner}>
                <div className={styles.iconWrapper}>
                    <svg
                        className={styles.errorIcon}
                        xmlns='http://www.w3.org/2000/svg'
                        viewBox='0 0 24 24'
                        fill='none'
                        stroke='currentColor'
                        strokeWidth={1.5}
                        strokeLinecap='round'
                        strokeLinejoin='round'
                    >
                        <circle cx='12' cy='12' r='10' />
                        <line x1='4.93' y1='4.93' x2='19.07' y2='19.07' />
                    </svg>
                </div>

                <h1 className={styles.code}>#403</h1>
                <h2 className={styles.title}>Accesso negado</h2>

                <p className={styles.description}>
                    Você não tem permissão para acessar este recurso. Por favor, verifique suas credenciais ou
                    entre em contato com o suporte caso acredite que isto seja um erro.
                </p>

                <div className={styles.actions}>
                    <a href={ROUTES.DASHBOARD} className={styles.primary}>
                        <Icon icon='home' />
                        Voltar para o início
                    </a>

                    <a href={`mailto:${SYSTEM.EMAIL_SUPPORT}`} className='btn-secondary'>
                        <Icon icon='mail' />
                        Contatar suporte
                    </a>
                </div>
            </div>
        </main>
    )
}