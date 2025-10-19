'use client';
import { iMeSimple } from '@/app/api/consts/auth';
import { CONSTS_UTILITY, iPlanType } from '@/app/api/consts/utility';
import Icon from '@/app/components/icon';
import { iDropdownOption } from '@/app/components/input/drop-down';
import WhatsappButton from '@/app/components/whatsapp/button';
import ROUTES from '@/app/consts/routes';
import SYSTEM from '@/app/consts/system';
import { handleGetFirstName } from '@/app/functions/get.formatUserName';
import swal from '@/app/functions/swal';
import useApiGetCompanySituationEnum from '@/app/hooks/api/enums/useApiGetCompanySituationEnum';
import useApiGetMeSimple from '@/app/hooks/api/useApiGetMeSimple';
import useApiRequestToSetterOnUrlChange from '@/app/hooks/useApiRequestToSetterOnUrlChange';
import useTitle from '@/app/hooks/useTitle';
import useWindowSize from '@/app/hooks/useWindowSize';
import Tippy from '@tippyjs/react';
import Link from 'next/link';
import { useRouter } from 'next/navigation';
import { MouseEvent, useCallback, useEffect, useState } from 'react';
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
                <Pricing me={me} />
                <Testimonials />
                <CTA me={me} />

                <WhatsappButton phone={SYSTEM.PHONE_SUPPORT} />
            </main>

            <Footer />
        </div>
    )
}

function Header({ me, open, setOpen, scrolled }: { me: iMeSimple | undefined, open: boolean; setOpen: (v: boolean) => void; scrolled: boolean }) {

    const router = useRouter();
    const windowSize = useWindowSize();

    function handleScroll(e: MouseEvent<HTMLAnchorElement>, id: string) {
        e.preventDefault();
        const el = document.getElementById(id);

        if (el) {
            el.scrollIntoView({ behavior: 'smooth' });
        }

        setOpen(false);
    }

    useEffect(() => {
        if (windowSize.width > 0) {
            setOpen(false);
        }
    }, [windowSize.width, setOpen]);

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
                            <Link
                                className={styles.link}
                                href='#'
                                onClick={(e) => {
                                    e.preventDefault();
                                    swal({
                                        content: 'Você realmente deseja finalizar sua sessão?',
                                        confirmBtnText: 'Sim',
                                        confirmFunction: () => router.push(ROUTES.LOGOUT),
                                        cancelBtnText: 'Voltar',
                                        icon: 'question'
                                    });
                                }}
                            >Finalizar sessão</Link>

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

                <button className={styles.menuButton} aria-label='menu' onClick={() => setOpen(!open)}>
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

    const handlePickRandomCompanies = useCallback(() => {
        if (!companyTypeEnum || companyTypeEnum.length === 0) {
            return [];
        }

        const shuffled = [...companyTypeEnum].sort(() => 0.5 - Math.random());

        return shuffled.slice(0, 3).map((c) => ({
            label: c.label,
            value: Number(c.value)
        }));
    }, [companyTypeEnum]);

    useEffect(() => {
        setRandomCompanies(handlePickRandomCompanies());
    }, [companyTypeEnum, handlePickRandomCompanies]);

    function handleRefresh() {
        setAnimate('out');

        setTimeout(() => {
            setRandomCompanies(handlePickRandomCompanies());
            setAnimate('in');
        }, 250);
    }

    return (
        <section className={styles.hero} aria-labelledby='hero-title'>
            <div className={styles.container}>
                <div className={styles.heroInner}>
                    <div className={styles.heroBody}>
                        <div className={styles.heroBadge}>Uhu! Lançamento 🎉</div>

                        <h1 id='hero-title' className={styles.h1}>
                            {SYSTEM.NAME} — <span className={styles.highlight}>{SYSTEM.DESCRIPTION}</span>
                        </h1>

                        <p className={styles.lead}>
                            Plataforma feita pra quem presta serviço: agenda, clientes, pagamentos, integração com WhatsApp e confirmações automáticas. Tudo
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
            text: 'Marque horários automaticamente com confirmações e lembretes para reduzir faltas e otimizar sua agenda.'
        },
        {
            icon: 'users',
            title: 'Gestão de clientes',
            text: 'Tenha o histórico completo do cliente, notas rápidas e informações de contato centralizadas.'
        },
        {
            icon: 'calendar',
            title: 'Agenda multi-profissional',
            text: 'Gerencie horários de diferentes profissionais e bloqueios de forma rápida e visual.'
        },
        {
            icon: 'bell',
            title: 'Notificações automáticas',
            text: 'Envie lembretes e confirmações por e-mail e WhatsApp, mantendo sua agenda sempre organizada.'
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
                    {
                        features.map((f) => (
                            <article key={f.title} className={styles.featureCard}>
                                <div className={styles.featureIconWrap}>
                                    <Icon icon={f.icon as any} size='big' className={styles.featureIcon} />
                                </div>

                                <h3 className={styles.featureTitle}>{f.title}</h3>
                                <p className={styles.featureText}>{f.text}</p>
                            </article>
                        ))
                    }
                </div>
            </div>
        </section>
    )
}

function Pricing({ me }: { me: iMeSimple | undefined }) {

    const [plans, setPlans] = useState<iPlanType | undefined>();
    useApiRequestToSetterOnUrlChange<iPlanType>({ apiUrlRequest: CONSTS_UTILITY.getPlanType, setter: setPlans });

    return (
        <section id='pricing' className={styles.pricingSection}>
            <div className={styles.container}>
                <div className={styles.sectionHeader}>
                    <h2 className={styles.sectionTitle}>Planos e preços</h2>
                    <p className={styles.sectionSubtitle}>Cresça sem surpresas — periodicidade mensal com upgrade fácil.</p>
                </div>

                <div className={styles.pricingGrid}>
                    {
                        plans?.plans?.map((p, i) => (
                            <div key={i} className={`${styles.pricingCard} ${p.planTypeName === 'Basic' ? styles.featured : ''}`}>
                                {
                                    p.planTypeName === 'Basic' && <div className={styles.badge}>Mais popular</div>
                                }

                                <h3 className={styles.pricingTitle}>{p.planTypeDescription}</h3>
                                <p className={styles.pricingSubtitle}>{p.description}</p>
                                <p className={styles.price}>
                                    R$ {p.price}

                                    {
                                        p.planTypeName !== 'Free' && <span className={styles.priceSuffix}>/mês</span>
                                    }
                                </p>

                                <ul className={styles.featuresList}>
                                    {
                                        p.perks.map((perk) => (
                                            <li key={perk}>✔️ {perk}</li>
                                        ))
                                    }
                                </ul>

                                {
                                    me?.isAuth ? (
                                        <Link href={`${ROUTES.EMPRESA_USO_E_PLANO}?plan=${p.planTypeName?.toLocaleLowerCase()}`} className={styles.fullBtn}>
                                            Escolher plano {p.planTypeDescription.toLocaleLowerCase()}
                                        </Link>
                                    ) : (
                                        <Link href={ROUTES.LOGIN} className={styles.fullBtn}>
                                            Entrar para escolher o plano {p.planTypeDescription.toLocaleLowerCase()}
                                        </Link>
                                    )
                                }
                            </div>
                        ))
                    }
                </div>
            </div>
        </section>
    )
}

function Testimonials() {

    const testimonials = [
        {
            text: 'Reduzi o no-show em 40% e finalmente tenho tempo livre pra focar no que importa.',
            author: 'Dra. Izabelle',
            company: 'Point do Sorriso'
        },
        {
            text: 'Interface limpa, suporte rápido e migração simples — recomendo.',
            author: 'Equipe',
            company: 'Studio Beleza'
        },
        {
            text: `A agenda do ${SYSTEM.NAME} salvou minha rotina. Nunca mais marquei paciente em cima do outro!`,
            author: 'Dr. Jailson Mendes',
            company: 'Orto Orange'
        }
    ]

    const [visible, setVisible] = useState(2)

    useEffect(() => {
        const timer = setTimeout(() => setVisible(3), 1000)
        return () => clearTimeout(timer)
    }, [])

    return (
        <section className={styles.testimonialsSection}>
            <div className={styles.container}>
                <header className={styles.sectionHeader}>
                    <h2 className={styles.sectionTitle}>O que profissionais dizem</h2>
                    <p className={styles.sectionSubtitle}>Feedback real de quem usa o Orquestra no dia a dia.</p>
                </header>

                <div className={styles.testimonialGrid}>
                    {
                        testimonials.slice(0, visible).map((t, i) => (
                            <blockquote key={i} className={styles.testimonialCard}>
                                <p className={styles.quote}>“{t.text}”</p>

                                <footer>
                                    <strong>{t.author}</strong>
                                    <span>{t.company}</span>
                                </footer>
                            </blockquote>
                        ))
                    }
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
                        <h2>Pronto para organizar sua agenda?</h2>
                        <p>Comece um teste gratuito e veja o impacto em uma semana.</p>
                    </div>

                    {
                        me?.isAuth ? (
                            <Link className={styles.ctaButton} href={ROUTES.DASHBOARD}>
                                Voltar ao dashboard
                            </Link>
                        ) : (
                            <Link className={styles.ctaButton} href={ROUTES.CRIAR_CONTA}>
                                Criar conta agora mesmo
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