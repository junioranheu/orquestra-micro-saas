'use client';
import { iMeSimple } from '@/app/api/consts/auth';
import Icon from '@/app/components/icon';
import { iDropdownOption } from '@/app/components/input/drop-down';
import ROUTES from '@/app/consts/routes';
import SYSTEM from '@/app/consts/system';
import { handleGetFirstName } from '@/app/functions/get.formatUserName';
import useApiGetCompanySituationEnum from '@/app/hooks/api/enums/useApiGetCompanySituationEnum';
import useApiGetMeSimple from '@/app/hooks/api/useApiGetMeSimple';
import useTitle from '@/app/hooks/useTitle';
import Tippy from '@tippyjs/react';
import Link from 'next/link';
import { MouseEvent, useEffect, useState } from 'react';
import styles from './page.module.scss';

export default function LandingPage() {

    useTitle(`${SYSTEM.NAME}: ${SYSTEM.DESCRIPTION}`, false);
    const me = useApiGetMeSimple();

    const [open, setOpen] = useState<boolean>(false);
    const [scrolled, setScrolled] = useState(false);

    useEffect(() => {
        const onScroll = () => setScrolled(window.scrollY > 24);
        onScroll();
        window.addEventListener('scroll', onScroll);

        return () => window.removeEventListener('scroll', onScroll);
    }, []);

    return (
        <div className={styles.page}>
            <Header me={me} open={open} setOpen={setOpen} scrolled={scrolled} />

            <main className={styles.main}>
                <Hero me={me} />
                <Features />
                <Pricing />
                <Testimonials />
                <CTA me={me} />
            </main>

            <Footer />
        </div>
    )
}

function Header({ me, open, setOpen, scrolled }: { me: iMeSimple | undefined, open: boolean; setOpen: (v: boolean) => void; scrolled: boolean }) {

    function handleScroll(e: MouseEvent<HTMLAnchorElement>, id: string) {
        e.preventDefault();
        const el = document.getElementById(id);

        if (el) {
            el.scrollIntoView({ behavior: 'smooth' });
        }

        setOpen(false);
    }

    return (
        <header className={`${styles.header} ${scrolled ? styles.headerShadow : ''}`}>
            <div className={styles.container}>
                <div className={styles.brand}>
                    <div className={styles.logoCircle} aria-hidden>
                        <Icon icon='calendar' />
                    </div>

                    <div className={styles.brandText}>
                        <span className={styles.brandName}>{SYSTEM.NAME}</span>

                        {
                            me?.isAuth ? (
                                <small className={styles.brandTag}>{handleGetFirstName(me?.userName)}, já encontrou {SYSTEM.DESCRIPTION.toLocaleLowerCase()}? :&#41;</small>
                            ) : (
                                <small className={styles.brandTag}>{SYSTEM.DESCRIPTION}</small>
                            )
                        }
                    </div>
                </div>

                <nav className={styles.navDesktop} aria-label='Main navigation'>
                    <Link className={styles.navLink} href='#' onClick={(e) => handleScroll(e, 'features')}>
                        Funcionalidades
                    </Link>

                    <Link className={styles.navLink} href='#' onClick={(e) => handleScroll(e, 'pricing')}>
                        Preços
                    </Link>

                    <Link className={styles.navLink} href='#' onClick={(e) => handleScroll(e, 'cta')}>
                        Contato
                    </Link>
                </nav>

                {
                    me?.isAuth ? (
                        <div className={styles.actionsDesktop}>
                            <Link className={styles.cta} href={ROUTES.DASHBOARD}>
                                Voltar ao início
                            </Link>
                        </div>
                    ) : (
                        <div className={styles.actionsDesktop}>
                            <Link className={styles.link} href={ROUTES.LOGIN}>
                                Entrar
                            </Link>

                            <Link className={styles.cta} href={ROUTES.CRIAR_CONTA}>
                                Criar conta
                            </Link>
                        </div>
                    )
                }

                <button className={styles.menuButton} aria-label='menu' onClick={() => setOpen(!open)} title='menu'>
                    <Icon icon={open ? 'x' : 'menu'} />
                </button>
            </div>

            {
                open && (
                    <div className={styles.mobileMenu} role='dialog' aria-modal='true'>
                        <div className={styles.mobileInner}>
                            <Link className={styles.mobileLink} href='#' onClick={(e) => handleScroll(e, 'features')}>
                                Funcionalidades
                            </Link>

                            <Link className={styles.mobileLink} href='#' onClick={(e) => handleScroll(e, 'pricing')}>
                                Preços
                            </Link>

                            <Link className={styles.mobileLink} href='#' onClick={(e) => handleScroll(e, 'cta')}>
                                Contato
                            </Link>

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

function Hero({ me }: { me: iMeSimple | undefined }) {

    const companyTypeEnum = useApiGetCompanySituationEnum({ enumName: 'CompanyTypeEnum' });
    const [randomCompanies, setRandomCompanies] = useState<iDropdownOption[]>([]);
    const [animate, setAnimate] = useState<'in' | 'out'>('in');

    function handlePickRandomCompanies() {
        if (!companyTypeEnum || companyTypeEnum.length === 0) return [];

        const shuffled = [...companyTypeEnum].sort(() => 0.5 - Math.random());
        return shuffled.slice(0, 3).map((c) => ({
            label: c.label,
            value: Number(c.value)
        }));
    }

    useEffect(() => {
        setRandomCompanies(handlePickRandomCompanies());
    }, [companyTypeEnum]);

    function handleRefresh() {
        setAnimate('out');

        setTimeout(() => {
            setRandomCompanies(handlePickRandomCompanies());
            setAnimate('in');
        }, 250);
    }

    return (
        <section className={styles.hero} aria-labelledby="hero-title">
            <div className={styles.container}>
                <div className={styles.heroInner}>
                    <div className={styles.heroBody}>
                        <div className={styles.heroBadge}>Uhu! Lançamento 🎉</div>

                        <h1 id="hero-title" className={styles.h1}>
                            {SYSTEM.NAME} — <span className={styles.highlight}>{SYSTEM.DESCRIPTION}</span>
                        </h1>

                        <p className={styles.lead}>
                            Plataforma feita pra quem presta serviço: agenda, clientes, pagamentos e confirmações automático. Tudo
                            centralizado — sem gambiarra.
                        </p>

                        {
                            me?.isAuth ? (
                                <div className={styles.heroCTAs}>
                                    <Link href={ROUTES.DASHBOARD} className={styles.primaryBtn}>
                                        Ir para o dashboard
                                    </Link>
                                    <Link href={ROUTES.ETC_AJUDA} className={styles.secondaryBtn}>
                                        Central de ajuda
                                    </Link>
                                </div>
                            ) : (
                                <div className={styles.heroCTAs}>
                                    <Link href={ROUTES.CRIAR_CONTA} className={styles.primaryBtn}>
                                        Começar teste grátis
                                    </Link>
                                    <Link href={ROUTES.ETC_AJUDA} className={styles.secondaryBtn}>
                                        Central de ajuda
                                    </Link>
                                </div>
                            )
                        }

                        <div className={styles.trustRow}>
                            <span>Usado por</span>
                            <div className={styles.logos} aria-hidden>
                                {
                                    randomCompanies.map((c) => (
                                        <div
                                            key={c.value.toString()}
                                            className={`${styles.pill} ${animate === 'out' ? styles.fadeOut : styles.fadeIn}`}
                                        >
                                            {c.label}
                                        </div>
                                    ))
                                }
                            </div>

                            <button onClick={handleRefresh} className={styles.refreshBtn}>
                                Ver mais
                            </button>
                        </div>
                    </div>
                </div>
            </div>
        </section>
    )
}

function Features() {

    const features = [
        {
            icon: 'clock',
            title: 'Agendamento inteligente',
            text: 'Regras, buffers e lembretes automáticos — menos no-show, mais agenda cheia.'
        },
        {
            icon: 'users',
            title: 'CRM leve',
            text: 'Ficha completa do cliente, histórico e notas rápidas.'
        },
        {
            icon: 'credit-card',
            title: 'Pagamentos automatizados',
            text: 'Cobrança, parcelamento e integração com gateways.'
        },
        {
            icon: 'calendar',
            title: 'Agenda multi-profissional',
            text: 'Gerencie horários, agenda por sala e bloqueios em segundos.'
        },
        {
            icon: 'bell',
            title: 'Notificações & SMS',
            text: 'Lembretes configuráveis por e-mail, SMS e WhatsApp.'
        }
    ];

    return (
        <section id='features' className={styles.featuresSection}>
            <div className={styles.container}>
                <div className={styles.sectionHeader}>
                    <h2 className={styles.sectionTitle}>Funcionalidades poderosas</h2>
                    <p className={styles.sectionSubtitle}>Tudo que você precisa pra rodar um serviço profissional sem complicação.</p>
                </div>

                <div className={styles.featuresGrid}>
                    {features.map((f, i) => (
                        <article key={f.title} className={styles.featureCard}>
                            <div className={styles.featureIconWrap}>
                                <Icon icon={f.icon as any} size='big' className={styles.featureIcon} />
                            </div>
                            <h3 className={styles.featureTitle}>{f.title}</h3>
                            <p className={styles.featureText}>{f.text}</p>
                        </article>
                    ))}
                </div>
            </div>
        </section>
    )
}

function Pricing() {

    const plans = [
        { id: 'basic', name: 'Básico', price: '29', suffix: '/mês', desc: 'Freelancers e autônomos', perks: ['50 agendamentos/mês', 'Notificações por e-mail', 'Suporte básico'] },
        { id: 'pro', name: 'Profissional', price: '79', suffix: '/mês', desc: 'Pequenas equipes', perks: ['Agendamentos ilimitados', 'Notificações por SMS', 'Suporte prioritário'], featured: true },
        { id: 'enterprise', name: 'Empresarial', price: '199', suffix: '/mês', desc: 'Grandes operações', perks: ['Integrações avançadas', 'Suporte dedicado', 'SLA customizável'] }
    ];

    return (
        <section id='pricing' className={styles.pricingSection}>
            <div className={styles.container}>
                <div className={styles.sectionHeader}>
                    <h2 className={styles.sectionTitle}>Planos e preços</h2>
                    <p className={styles.sectionSubtitle}>Cresça sem surpresas — periodicidade mensal com upgrade fácil.</p>
                </div>

                <div className={styles.pricingGrid}>
                    {
                        plans.map((p) => (
                            <div key={p.id} className={`${styles.pricingCard} ${p.featured ? styles.featured : ''}`}>
                                {p.featured && <div className={styles.badge}>Mais popular</div>}
                                <h3 className={styles.pricingTitle}>{p.name}</h3>
                                <p className={styles.pricingSubtitle}>{p.desc}</p>
                                <p className={styles.price}>R${p.price}<span className={styles.priceSuffix}>{p.suffix}</span></p>

                                <ul className={styles.featuresList}>
                                    {
                                        p.perks.map((perk) => (
                                            <li key={perk}>✔️ {perk}</li>
                                        ))
                                    }
                                </ul>

                                <Link href='#' className={`${styles.fullBtn} ${p.featured ? styles.fullBtnFeatured : ''}`}>
                                    Escolher plano
                                </Link>
                            </div>
                        ))
                    }
                </div>
            </div>
        </section>
    )
}

function Testimonials() {
    return (
        <section className={styles.testimonialsSection} aria-label='Depoimentos'>
            <div className={styles.container}>
                <div className={styles.sectionHeader}>
                    <h2 className={styles.sectionTitle}>O que profissionais dizem</h2>
                    <p className={styles.sectionSubtitle}>Feedback real de quem usa no dia a dia.</p>
                </div>

                <div className={styles.testimonialGrid}>
                    <blockquote className={styles.testimonialCard}>
                        <p>'Reduzi o no-show em 40% e finalmente tenho tempo livre pra focar no que importa.'</p>
                        <footer>- Dra. Carla, Clínica Odonto</footer>
                    </blockquote>

                    <blockquote className={styles.testimonialCard}>
                        <p>'Interface limpa, suporte rápido e migração simples — recomendo.'</p>
                        <footer>- Studio Beleza</footer>
                    </blockquote>
                </div>
            </div>
        </section>
    )
}

function CTA({ me }: { me: iMeSimple | undefined }) {
    return (
        <section id='cta' className={styles.ctaSection}>
            <div className={styles.container}>
                <div className={styles.ctaInner}>
                    <div>
                        <h2 className={styles.sectionTitleLight}>Pronto para organizar sua agenda?</h2>
                        <p className={styles.ctaText}>Comece um teste gratuito e veja o impacto em uma semana.</p>
                    </div>

                    {
                        me?.isAuth ? (
                            <Link className={styles.ctaButton} href={ROUTES.DASHBOARD}>
                                Voltar ao início
                            </Link>
                        ) : (
                            <Link className={styles.ctaButton} href={ROUTES.CRIAR_CONTA}>
                                Criar conta
                            </Link>
                        )
                    }
                </div>
            </div>
        </section>
    )
}

function Footer() {
    return (
        <footer className={styles.footer}>
            <div className={styles.container}>
                <div className={styles.footerInner}>
                    <div>
                        <strong>{SYSTEM.NAME}.</strong>
                        <p className={styles.footerSmall}>{SYSTEM.DESCRIPTION}.</p>
                    </div>

                    <div className={styles.footerRight}>
                        <p>Todos os direitos reservados © {new Date().getFullYear()}</p>

                        <div className={styles.icons}>
                            <Tippy content='Contatar suporte'>
                                <Link
                                    href='#'
                                    onClick={(e) => {
                                        e.preventDefault();
                                        window.location.href = `mailto:${SYSTEM.EMAIL_SUPPORT}`;
                                    }}
                                >
                                    <Icon icon='mail' color='var(--gray-dark)' className='contrastOnHover' />
                                </Link>
                            </Tippy>

                            <Tippy content='GitHub'>
                                <Link href={SYSTEM.URL_GITHUB} target='_blank' rel='noopener noreferrer'>
                                    <Icon icon='github' color='var(--gray-dark)' className='contrastOnHover' />
                                </Link>
                            </Tippy>

                            <Tippy content='LinkedIn'>
                                <Link href={SYSTEM.URL_LINKEDIN} target='_blank' rel='noopener noreferrer'>
                                    <Icon icon='linkedin' color='var(--gray-dark)' className='contrastOnHover' />
                                </Link>
                            </Tippy>
                        </div>
                    </div>
                </div>
            </div>
        </footer>
    )
}