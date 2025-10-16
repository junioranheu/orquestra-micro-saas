'use client';
import Icon from '@/app/components/icon';
import ROUTES from '@/app/consts/routes';
import SYSTEM from '@/app/consts/system';
import useTitle from '@/app/hooks/useTitle';
import Link from 'next/link';
import { MouseEvent, useState } from 'react';
import styles from './page.module.scss';

export default function LandingPage() {

    useTitle(`${SYSTEM.NAME}: ${SYSTEM.DESCRIPTION}`, false);
    const [open, setOpen] = useState<boolean>(false);

    return (
        <div className={styles.page}>
            <Header open={open} setOpen={setOpen} />

            <main className={styles.main}>
                <Hero />
                <Features />
                <Pricing />
                <CTA />
            </main>

            <Footer />
        </div>
    )
}

function Header({ open, setOpen }: { open: boolean; setOpen: (v: boolean) => void }) {

    function handleScroll(e: MouseEvent<HTMLAnchorElement>, id: string) {
        e.preventDefault();
        const el = document.getElementById(id);

        if (el) {
            el.scrollIntoView({ behavior: 'smooth' });
        }
    }

    return (
        <header className={styles.header}>
            <div className={styles.container}>
                <nav className={styles.navDesktop}>
                    <a className={styles.navLink} href='#' onClick={(e) => handleScroll(e, 'features')}>
                        Funcionalidades
                    </a>

                    <a className={styles.navLink} href='#' onClick={(e) => handleScroll(e, 'pricing')}>
                        Preços
                    </a>

                    <a className={styles.navLink} href='#' onClick={(e) => handleScroll(e, 'cta')}>
                        Contato
                    </a>
                </nav>

                <div className={styles.actionsDesktop}>
                    <Link className={styles.link} href={ROUTES.LOGIN}>
                        Entrar
                    </Link>

                    <Link className={styles.cta} href={ROUTES.CRIAR_CONTA}>
                        Criar conta
                    </Link>
                </div>

                <button
                    className={styles.menuButton}
                    aria-label='menu'
                    onClick={() => setOpen(!open)}
                    title='menu'
                >
                    <Icon icon='menu' />
                </button>
            </div>

            {/* Menu Mobile */}
            {
                open && (
                    <div className={styles.mobileMenu}>
                        <div className={styles.mobileInner}>
                            <a className={styles.mobileLink} href='#' onClick={(e) => handleScroll(e, 'features')}>
                                Funcionalidades
                            </a>

                            <a className={styles.mobileLink} href='#' onClick={(e) => handleScroll(e, 'pricing')}>
                                Preços
                            </a>

                            <a className={styles.mobileLink} href='#' onClick={(e) => handleScroll(e, 'cta')}>
                                Contato
                            </a>

                            <div className={styles.mobileActions}>
                                <Link className={styles.link} href={ROUTES.LOGIN}>
                                    Entrar
                                </Link>

                                <Link className={styles.ctaAlt} href={ROUTES.CRIAR_CONTA}>
                                    Criar conta
                                </Link>
                            </div>
                        </div>
                    </div>
                )
            }
        </header>
    )
}

function Hero() {
    return (
        <section className={styles.hero}>
            <div className={styles.container}>
                <div className={styles.heroInner} data-aos='fade-up'>
                    <div className={styles.heroIcon}>
                        <Icon icon='calendar' className={styles.iconWhite} />
                    </div>

                    <h1 className={styles.h1}>
                        <span className={styles.highlight}>{SYSTEM.NAME}</span>
                    </h1>

                    <h2 className={styles.h2}>
                        <span>{SYSTEM.DESCRIPTION}</span>
                    </h2>

                    <p className={styles.lead}>
                        Eleve a experiência dos seus clientes com nossa plataforma intuitiva, criada exclusivamente para profissionais de serviços.
                    </p>

                    <div className={styles.heroCTAs}>
                        <a href={`${ROUTES.LOGIN}`} className={styles.primaryBtn}>
                            Começar Teste Grátis
                        </a>

                        <a href={`${ROUTES.LOGIN}`} className={styles.secondaryBtn}>
                            Assistir Demo
                        </a>
                    </div>
                </div>
            </div>
        </section>
    )
}

function Features() {
    return (
        <section id='features' className={styles.featuresSection}>
            <div className={styles.container}>
                <div className={styles.sectionHeader} data-aos='fade-up'>
                    <h2 className={styles.sectionTitle}>Funcionalidades Poderosas</h2>
                    <p className={styles.sectionSubtitle}>
                        Tudo que você precisa para gerenciar agendamentos, clientes e pagamentos em um só lugar.
                    </p>
                </div>

                <div className={styles.featuresGrid}>
                    <div className={styles.featureCard} data-aos='fade-up' data-aos-delay='100'>
                        <Icon icon='clock' size='big' className={styles.featureIcon} />
                        <h3 className={styles.featureTitle}>Agendamento Inteligente</h3>
                        <p className={styles.featureText}>
                            Gerencie horários automaticamente com lembretes para clientes e equipe.
                        </p>
                    </div>

                    <div className={styles.featureCard} data-aos='fade-up' data-aos-delay='200'>
                        <Icon icon='users' size='big' className={styles.featureIcon} />
                        <h3 className={styles.featureTitle}>Gestão de Clientes</h3>
                        <p className={styles.featureText}>
                            Tenha o histórico completo dos seus clientes em um só lugar.
                        </p>
                    </div>

                    <div className={styles.featureCard} data-aos='fade-up' data-aos-delay='300'>
                        <Icon icon='credit-card' size='big' className={styles.featureIcon} />
                        <h3 className={styles.featureTitle}>Pagamentos Integrados</h3>
                        <p className={styles.featureText}>
                            Receba pagamentos online de forma simples e segura.
                        </p>
                    </div>
                </div>
            </div>
        </section>
    );
}

function Pricing() {
    return (
        <section id='pricing' className={styles.pricingSection}>
            <div className={styles.container}>
                <div className={styles.sectionHeader} data-aos='fade-up'>
                    <h2 className={styles.sectionTitle}>Planos e Preços</h2>
                    <p className={styles.sectionSubtitle}>Escolha o plano que melhor se adapta ao seu negócio</p>
                </div>

                <div className={styles.pricingGrid}>
                    <div className={styles.pricingCard} data-aos='fade-up' data-aos-delay='100'>
                        <h3 className={styles.pricingTitle}>Básico</h3>
                        <p className={styles.pricingSubtitle}>Ideal para profissionais individuais</p>
                        <p className={styles.price}>R$29<span className={styles.priceSuffix}>/mês</span></p>
                        <ul className={styles.featuresList}>
                            <li>✔️ 50 agendamentos/mês</li>
                            <li>✔️ Notificações por e-mail</li>
                            <li>✔️ Suporte básico</li>
                        </ul>
                        <a href='#' className={styles.fullBtn}>
                            Escolher plano
                        </a>
                    </div>

                    <div className={`${styles.pricingCard} ${styles.featured}`} data-aos='fade-up' data-aos-delay='200'>
                        <h3 className={styles.pricingTitle}>Profissional</h3>
                        <p className={styles.pricingSubtitle}>Perfeito para equipes pequenas</p>
                        <p className={styles.price}>R$79<span className={styles.priceSuffix}>/mês</span></p>
                        <ul className={styles.featuresList}>
                            <li>✔️ Agendamentos ilimitados</li>
                            <li>✔️ Notificações por SMS</li>
                            <li>✔️ Suporte prioritário</li>
                        </ul>
                        <a href='#' className={styles.fullBtnAlt}>
                            Escolher plano
                        </a>
                    </div>

                    <div className={styles.pricingCard} data-aos='fade-up' data-aos-delay='300'>
                        <h3 className={styles.pricingTitle}>Empresarial</h3>
                        <p className={styles.pricingSubtitle}>Para grandes empresas</p>
                        <p className={styles.price}>R$199<span className={styles.priceSuffix}>/mês</span></p>
                        <ul className={styles.featuresList}>
                            <li>✔️ Tudo do Profissional</li>
                            <li>✔️ Integrações avançadas</li>
                            <li>✔️ Suporte dedicado</li>
                        </ul>
                        <a href='#' className={styles.fullBtn}>
                            Escolher plano
                        </a>
                    </div>
                </div>
            </div>
        </section>
    );
}

function CTA() {
    return (
        <section id='cta' className={styles.ctaSection}>
            <div className={styles.container} data-aos='zoom-in'>
                <h2 className={styles.sectionTitleLight}>Pronto para organizar sua agenda?</h2>
                <p className={styles.ctaText}>Entre no {SYSTEM.NAME} agora mesmo</p>

                <Link className={styles.ctaButton} href={ROUTES.CRIAR_CONTA}>
                    Criar conta
                </Link>
            </div>
        </section>
    );
}

function Footer() {
    return (
        <footer className={styles.footer}>
            <div className={styles.container}>
                <p>© {new Date().getFullYear()} {SYSTEM.NAME}. Todos os direitos reservados.</p>
            </div>
        </footer>
    );
}