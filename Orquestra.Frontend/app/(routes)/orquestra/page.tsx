'use client';
import { iMeSimple } from '@/app/api/consts/auth';
import { CONSTS_UTILITY, iPlanTypeOutput } from '@/app/api/consts/utility';
import ImgLogo from '@/app/assets/png/logo-simple.png';
import Img from '@/app/assets/png/server.png';
import Icon from '@/app/components/icon';
import { iDropdownOption } from '@/app/components/input/drop-down';
import Splash from '@/app/components/splash';
import WhatsappButton from '@/app/components/whatsapp/button';
import WhatsappHyperlink from '@/app/components/whatsapp/hyperlink';
import ROUTES from '@/app/consts/routes';
import SYSTEM from '@/app/consts/system';
import useApiGetBuildVersion from '@/app/hooks/api/useApiGetBuildVersion';
import useApiGetEnum from '@/app/hooks/api/useApiGetEnum';
import useApiGetMeSimple from '@/app/hooks/api/useApiGetMeSimple';
import useApiRequestToSetterOnUrlChange from '@/app/hooks/api/useApiRequestToSetterOnUrlChange';
import useInjectTailwindCDN from '@/app/hooks/useInjectTailwindCDN';
import useTitle from '@/app/hooks/useTitle';
import useWindowSize from '@/app/hooks/useWindowSize';
import Tippy from '@tippyjs/react';
import Image from 'next/image';
import Link from 'next/link';
import { useRouter } from 'next/navigation';
import { Fragment, MouseEvent, useCallback, useEffect, useState } from 'react';
import styles from './page.module.scss';

export default function LandingPage() {

    useTitle(`${SYSTEM.NAME}: ${SYSTEM.DESCRIPTION}`, false);
    const isTailwindReady = useInjectTailwindCDN();

    const me = useApiGetMeSimple();
    const [open, setOpen] = useState<boolean>(false);
    const [scrolled, setScrolled] = useState<boolean>(false);

    const [plans, setPlans] = useState<iPlanTypeOutput | undefined>();
    useApiRequestToSetterOnUrlChange<iPlanTypeOutput>({ apiUrlRequest: CONSTS_UTILITY.getPlanType, setter: setPlans });

    useEffect(() => {
        const onScroll = () => setScrolled(window.scrollY > 24);
        window.addEventListener('scroll', onScroll);
        return () => window.removeEventListener('scroll', onScroll);
    }, []);

    if (!isTailwindReady) {
        return (
            <Splash text={SYSTEM.NAME} isGradient={true} />
        )
    }

    return (
        <div className={`${styles.main} min-h-screen bg-gradient-to-b from-slate-50 via-white to-[var(--main-light)]`}>
            <Header me={me} open={open} setOpen={setOpen} scrolled={scrolled} />

            <main>
                <Hero me={me} />
                <Features />
                <Process />
                <Pricing me={me} plans={plans} />
                <Testimonials />
                <CTA me={me} plans={plans} />
                <WhatsappButton phone={SYSTEM.PHONE_SUPPORT} />
            </main>

            <Footer />
        </div>
    )
}

function Header({ me, open, setOpen, scrolled }: { me: iMeSimple | undefined; open: boolean; setOpen: (v: boolean) => void; scrolled: boolean; }) {

    const router = useRouter();
    const windowSize = useWindowSize();

    function handleScroll(e: MouseEvent<HTMLAnchorElement>, id: string) {
        e.preventDefault();
        document.getElementById(id)?.scrollIntoView({ behavior: 'smooth' });
        setOpen(false);
    }

    useEffect(() => {
        if (windowSize.width > 0) setOpen(false);
    }, [windowSize.width, setOpen]);

    return (
        <header className={`sticky top-0 z-50 transition-all duration-300 ${scrolled ? 'bg-white/95 backdrop-blur-lg shadow-lg border-b border-[var(--main-light)]/50' : 'bg-white/70 backdrop-blur-md'}`}>
            <div className='max-w-6xl mx-auto px-4 sm:px-6 py-3 sm:py-4 flex items-center justify-between gap-4'>
                {/* Logo */}
                <Tippy content='Voltar ao início.'>
                    <Link href={ROUTES.DASHBOARD} className='flex items-center gap-2.5 group'>
                        {/* <div className='w-10 h-10 bg-gradient-to-br from-[var(--main)] to-[var(--main-dark)] rounded-xl flex items-center justify-center text-white shadow-lg group-hover:shadow-xl transition-all group-hover:scale-105'>
                            <Icon icon='calendar' />
                        </div> */}

                        {/* <div className='hidden sm:flex flex-col'>
                            <span className='font-bold text-gray-900 bg-gradient-to-r from-[var(--main)] to-[var(--main-dark)] bg-clip-text text-transparent'>
                                {SYSTEM.NAME}
                            </span>

                            <span className='text-xs text-gray-500'>{SYSTEM.DESCRIPTION}</span>
                        </div> */}

                        <Image src={ImgLogo} alt='' width={120} height={120} priority={true} />
                    </Link>
                </Tippy>

                {/* Desktop Nav */}
                <nav className='hidden lg:flex items-center gap-1'>
                    {[
                        { label: 'Funcionalidades', id: 'features' },
                        { label: 'Como funciona', id: 'process' },
                        { label: 'Preços', id: 'pricing' },
                        { label: 'Depoimentos', id: 'testimonials' }
                    ].map((item) => (
                        <a
                            key={item.id}
                            href={`#${item.id}`}
                            onClick={(e) => handleScroll(e as any, item.id)}
                            className='px-4 py-2 text-gray-700 hover:text-[var(--main)] font-medium transition-colors relative group'
                        >
                            {item.label}
                            <span className='absolute bottom-0 left-0 w-0 h-0.5 bg-[var(--main)] group-hover:w-full transition-all duration-300' />
                        </a>
                    ))}
                </nav>

                {/* Actions */}
                <div className='hidden lg:flex items-center gap-2'>
                    {
                        me?.isAuth ? (
                            <Fragment>
                                <Link href={ROUTES.DASHBOARD} className='px-4 py-2 text-gray-700 hover:bg-gray-100 rounded-lg transition-colors font-medium'>
                                    Dashboard
                                </Link>

                                <button onClick={() => router.push(ROUTES.LOGOUT)} className='px-4 py-2 bg-red-50 text-red-600 hover:bg-red-100 rounded-lg transition-colors font-medium'>
                                    Finalizar sessão
                                </button>
                            </Fragment>
                        ) : (
                            <Fragment>
                                <Link href={ROUTES.LOGIN} className='px-4 py-2 text-gray-700 hover:bg-gray-100 rounded-lg transition-colors font-medium'>
                                    Entrar
                                </Link>

                                <Link href={ROUTES.CRIAR_CONTA} className='px-5 py-2.5 bg-gradient-to-r from-[var(--main)] to-[var(--main-dark)] text-white rounded-lg hover:shadow-lg hover:-translate-y-0.5 transition-all font-semibold'>
                                    Começar grátis
                                </Link>
                            </Fragment>
                        )
                    }
                </div>

                {/* Mobile Menu Button */}
                <button
                    onClick={() => setOpen(!open)}
                    className='lg:hidden w-10 h-10 flex items-center justify-center text-gray-700 hover:text-[var(--main)] transition-colors'
                >
                    <Icon icon={open ? 'x' : 'menu'} />
                </button>
            </div>

            {/* Mobile Menu */}
            {
                open && (
                    <div className='lg:hidden border-t border-[var(--main-light)]/50 bg-white/50 backdrop-blur-md animate-in fade-in duration-200'>
                        <div className='max-w-6xl mx-auto px-4 py-3 flex flex-col gap-2'>
                            {[
                                { label: 'Funcionalidades', id: 'features' },
                                { label: 'Como funciona', id: 'process' },
                                { label: 'Preços', id: 'pricing' },
                                { label: 'Depoimentos', id: 'testimonials' }
                            ].map((item) => (
                                <a
                                    key={item.id}
                                    href={`#${item.id}`}
                                    onClick={(e) => handleScroll(e as any, item.id)}
                                    className='px-4 py-2.5 text-gray-700 hover:text-[var(--main)] hover:bg-[var(--main-light)] rounded-lg transition-colors font-medium'
                                >
                                    {item.label}
                                </a>
                            ))}

                            <div className='flex gap-2 pt-2 border-t border-[var(--main-light)]/50'>
                                {
                                    me?.isAuth ? (
                                        <Link href={ROUTES.DASHBOARD} className='flex-1 px-4 py-2.5 text-center text-gray-700 bg-gray-100 rounded-lg font-medium'>
                                            Dashboard
                                        </Link>
                                    ) : (
                                        <Fragment>
                                            <Link href={ROUTES.LOGIN} className='flex-1 px-4 py-2.5 text-center text-gray-700 bg-gray-100 rounded-lg font-medium'>
                                                Entrar
                                            </Link>

                                            <Link
                                                href={ROUTES.CRIAR_CONTA}
                                                className='flex-1 px-4 py-2.5 text-center bg-gradient-to-r from-[var(--main)] to-[var(--main-dark)] text-white rounded-lg font-semibold'
                                            >
                                                Criar conta
                                            </Link>
                                        </Fragment>
                                    )
                                }
                            </div>
                        </div>
                    </div>
                )
            }
        </header>
    )
}

function Hero({ me }: { me: iMeSimple | undefined }) {

    const versionBuild = useApiGetBuildVersion();
    const companyTypeEnum = useApiGetEnum({ enumName: 'CompanyTypeEnum' });

    const [companies, setCompanies] = useState<iDropdownOption[]>([]);
    const [animate, setAnimate] = useState<'in' | 'out'>('in');

    const pickRandom = useCallback(() => {
        if (!companyTypeEnum?.length) {
            return [];
        }

        return [...companyTypeEnum].sort(() => 0.5 - Math.random()).slice(0, 3).map((c) => ({ label: c.label, value: Number(c.value) }));
    }, [companyTypeEnum]);

    useEffect(() => {
        setCompanies(pickRandom());
    }, [companyTypeEnum, pickRandom]);

    const refresh = () => {
        setAnimate('out');

        setTimeout(() => {
            setCompanies(pickRandom());
            setAnimate('in');
        }, 250);
    };

    return (
        <section className='relative px-4 sm:px-6 py-16 sm:py-24 max-w-6xl mx-auto overflow-hidden'>
            <div className='relative z-10'>
                <div className='flex flex-col lg:flex-row items-center gap-8'>
                    <div className='w-full lg:w-[70%]'>
                        <div className='inline-flex items-center gap-2 mb-6 px-3 py-1.5 bg-[var(--main-light)] border border-[var(--main-light)] rounded-full'>
                            <span className='w-2 h-2 bg-[var(--main)] rounded-full animate-pulse' />
                            <span className='text-sm font-semibold text-[var(--main-dark)]'>{SYSTEM.NAME} {versionBuild?.buildVersion}</span>
                        </div>

                        <h1 className='text-4xl sm:text-5xl lg:text-6xl font-black text-gray-900 mb-6 leading-tight'>
                            Sua agenda profissional{' '}
                            <span className='bg-gradient-to-r from-[var(--main)] via-[var(--main)] to-[var(--main-dark)] bg-clip-text text-transparent'>
                                sem complicações
                            </span>
                        </h1>

                        <p className='text-lg sm:text-xl text-gray-600 max-w-2xl mb-8 leading-relaxed'>
                            Agendamentos inteligentes, gestão de clientes, confirmações automáticas por WhatsApp e etc. Tudo em uma plataforma
                            simples e poderosa.
                        </p>

                        <div className='flex flex-col sm:flex-row gap-3 mb-12'>
                            {
                                me?.isAuth ? (
                                    <Fragment>
                                        <Link href={ROUTES.DASHBOARD} className='px-8 py-3.5 bg-gradient-to-r from-[var(--main)] to-[var(--main-dark)] text-white rounded-lg font-bold hover:shadow-xl hover:-translate-y-1 transition-all inline-flex items-center justify-center gap-2 group'>
                                            Ir para dashboard
                                            <Icon icon='arrow-right' className='group-hover:translate-x-1 transition-transform' />
                                        </Link>

                                        <Link href={ROUTES.ETC_AJUDA} className='px-8 py-3.5 bg-white border-2 border-gray-200 text-gray-700 rounded-lg font-bold hover:border-[var(--main-light)] hover:bg-[var(--main-light)] transition-all inline-flex items-center justify-center'>
                                            Central de ajuda
                                        </Link>
                                    </Fragment>
                                ) : (
                                    <Fragment>
                                        <Link href={ROUTES.CRIAR_CONTA} className='px-8 py-3.5 bg-gradient-to-r from-[var(--main)] to-[var(--main-dark)] text-white rounded-lg font-bold hover:shadow-xl hover:-translate-y-1 transition-all inline-flex items-center justify-center gap-2 group'>
                                            Começar teste grátis
                                            <Icon icon='arrow-right' className='group-hover:translate-x-1 transition-transform' />
                                        </Link>

                                        <Link href={ROUTES.ETC_AJUDA} className='px-8 py-3.5 bg-white border-2 border-gray-200 text-gray-700 rounded-lg font-bold hover:border-[var(--main-light)] hover:bg-[var(--main-light)] transition-all inline-flex items-center justify-center'>
                                            Ver documentação
                                        </Link>
                                    </Fragment>
                                )
                            }
                        </div>

                        <div className='space-y-3'>
                            <p className='text-sm font-semibold text-gray-500 uppercase tracking-wide'>O {SYSTEM.NAME} pode ser usado por...</p>

                            <div className='flex flex-wrap gap-2'>
                                {
                                    companies.map((c) => (
                                        <div key={c.value.toString()} className={`px-3.5 py-2 bg-white/60 backdrop-blur-sm border border-[var(--main-light)]/80 rounded-full text-sm font-medium text-gray-700 transition-all duration-300 ${animate === 'out' ? 'opacity-0 -translate-y-2' : 'opacity-100 translate-y-0'}`}>
                                            {c.label}
                                        </div>
                                    ))
                                }
                            </div>

                            <button onClick={refresh} className='text-sm font-semibold text-[var(--main)] hover:text-[var(--main-dark)] transition-colors'>
                                Ver mais →
                            </button>
                        </div>
                    </div>

                    <div className='hidden lg:flex lg:w-[30%] justify-center items-center'>
                        <figure className='max-w-[28rem] w-full pointer-events-none select-none'>
                            <Image src={Img} alt='' priority={true} />
                        </figure>
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
            title: 'Agendamento fácil',
            desc: 'Crie, edite e visualize seus horários em poucos cliques, sem complicação.'
        },
        {
            icon: 'users',
            title: 'Gestão de clientes',
            desc: 'Tenha o controle de quem marcou, com histórico básico e dados organizados.'
        },
        {
            icon: 'calendar',
            title: 'Vários profissionais',
            desc: 'Gerencie as agendas da sua equipe em um só lugar, de forma simples e rápida.'
        },
        {
            icon: 'bell',
            title: 'Lembretes por WhatsApp',
            desc: 'Envie lembretes automáticos e reduza faltas sem precisar sair da plataforma.'
        }
    ];

    return (
        <section id='features' className='px-4 sm:px-6 py-16 sm:py-24 max-w-6xl mx-auto'>
            {/* Section Header */}
            <div className='text-center mb-12 sm:mb-16'>
                <h2 className='text-3xl sm:text-4xl lg:text-5xl font-black text-gray-900 mb-4'>Funcionalidades simples e poderosas</h2>
                <p className='text-lg text-gray-600 max-w-2xl mx-auto'>Tudo que você precisa para rodar um serviço profissional sem complicações</p>
            </div>

            {/* Features Grid */}
            <div className='grid md:grid-cols-2 gap-6 sm:gap-8'>
                {
                    features.map((f, i) => (
                        <div key={i} className='group p-6 sm:p-8 bg-white/70 backdrop-blur-sm border border-[var(--main-light)]/80 rounded-2xl hover:bg-white hover:border-[var(--main-light)] hover:shadow-xl transition-all duration-300 hover:-translate-y-1'>
                            <div className='w-16 h-16 bg-gradient-to-br from-[var(--main-light)] to-[var(--main-light)]/80 rounded-xl flex items-center justify-center mb-5 group-hover:from-[var(--main-light)] group-hover:to-[var(--main-light)] transition-colors'>
                                <Icon icon={f.icon as any} className='text-[var(--main)]' />
                            </div>

                            <h3 className='text-xl font-bold text-gray-900 mb-3'>{f.title}</h3>
                            <p className='text-gray-600 leading-relaxed'>{f.desc}</p>
                        </div>
                    ))
                }
            </div>
        </section>
    )
}

function Process() {

    const steps = [
        { num: '01', title: 'Cadastre seus serviços', desc: 'Defina o que você oferece, duração e horários disponíveis.' },
        { num: '02', title: 'Agende', desc: 'Insira manualmente os horários marcados pelos seus clientes.' },
        { num: '03', title: 'Visualize sua agenda', desc: 'Acompanhe tudo em um painel simples e organizado.' },
        { num: '04', title: 'Atualize quando precisar', desc: 'Edite, cancele ou reagende com poucos cliques.' }
    ];

    return (
        <section id='process' className='px-4 sm:px-6 py-16 sm:py-24 bg-gradient-to-r from-[var(--main-light)]/50 to-[var(--main-light)]/30'>
            <div className='max-w-6xl mx-auto'>
                {/* Section Header */}
                <div className='text-center mb-12 sm:mb-16'>
                    <h2 className='text-3xl sm:text-4xl lg:text-5xl font-black text-gray-900 mb-4'>Como funciona</h2>
                    <p className='text-lg text-gray-600 max-w-2xl mx-auto'>4 passos simples para você começar</p>
                </div>

                {/* Steps Grid */}
                <div className='grid md:grid-cols-2 lg:grid-cols-4 gap-4 sm:gap-6'>
                    {
                        steps.map((step, i) => (
                            <div key={i} className='group'>
                                <div className='p-6 sm:p-8 bg-white/80 backdrop-blur-sm border border-[var(--main-light)]/80 rounded-xl hover:bg-white hover:shadow-lg transition-all duration-300'>
                                    <div className='text-4xl font-black text-transparent bg-gradient-to-r from-[var(--main)] to-[var(--main-dark)] bg-clip-text mb-4'>
                                        {step.num}
                                    </div>
                                    <h4 className='text-lg font-bold text-gray-900 mb-2'>{step.title}</h4>
                                    <p className='text-gray-600 text-sm leading-relaxed'>{step.desc}</p>
                                </div>

                                {
                                    i < steps.length - 1 && (
                                        <div className='hidden lg:flex justify-center items-center py-4'>
                                            <div className='text-[var(--main-light)] text-2xl group-hover:text-[var(--main)] transition-colors'>→</div>
                                        </div>
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

function Pricing({ me, plans }: { me: iMeSimple | undefined, plans: iPlanTypeOutput | undefined }) {
    return (
        <section id='pricing' className='px-4 sm:px-6 py-16 sm:py-24 max-w-6xl mx-auto'>
            {/* Section Header */}
            <div className='text-center mb-12 sm:mb-16'>
                <h2 className='text-3xl sm:text-4xl lg:text-5xl font-black text-gray-900 mb-4'>Planos que crescem com você</h2>
                <p className='text-lg text-gray-600 max-w-2xl mx-auto'>Sem contratos. Cancele quando quiser.</p>
            </div>

            {/* Pricing Grid */}
            <div className='grid md:grid-cols-3 gap-6 sm:gap-8'>
                {
                    plans?.plans?.map((p, i) => (
                        <div key={i} className={`relative group transition-all duration-300 ${p.planTypeName === 'Basic' ? 'md:scale-105 md:z-10' : 'hover:-translate-y-1'}`}>
                            {
                                p.planTypeName === 'Basic' && (
                                    <div className='absolute -top-4 left-8 px-4 py-1.5 bg-gradient-to-r from-[var(--main)] to-[var(--main-dark)] text-white text-sm font-bold rounded-full'>
                                        Ideal
                                    </div>
                                )
                            }

                            <div className={`h-full p-8 rounded-2xl border transition-all duration-300 ${p.planTypeName === 'Basic' ? 'bg-gradient-to-br from-white to-[var(--main-light)]/50 border-[var(--main-light)]/30 shadow-xl' : 'bg-white/70 backdrop-blur-sm border-[var(--main-light)]/80 hover:bg-white hover:border-[var(--main-light)] hover:shadow-lg'}`}>
                                <h3 className='text-2xl font-bold text-gray-900 mb-2'>{p.planTypeDescription}</h3>
                                <p className='text-gray-600 text-sm mb-6'>{p.description}</p>

                                <div className='mb-6'>
                                    <span className='text-4xl font-black text-transparent bg-gradient-to-r from-[var(--main)] to-[var(--main-dark)] bg-clip-text'>
                                        R$ {p.price}
                                    </span>

                                    {p.planTypeName !== 'Free' && <span className='text-gray-600 ml-2'>/mês</span>}
                                </div>

                                <ul className='space-y-3 mb-8'>
                                    {
                                        p.perks.map((perk) => (
                                            <li key={perk} className='flex items-center gap-3 text-gray-700'>
                                                <span className='w-5 h-5 rounded-full bg-[var(--main-light)] flex items-center justify-center text-[var(--main)] font-bold text-xs'>✓</span>
                                                <span className='text-sm'>{perk}</span>
                                            </li>
                                        ))
                                    }
                                </ul>

                                <Link href={me?.isAuth ? ROUTES.EMPRESA_USO_E_PLANO : ROUTES.LOGIN} className='block w-full py-3 px-4 text-center font-bold rounded-lg transition-all duration-300 text-white bg-gradient-to-r from-[var(--main)] to-[var(--main-dark)] hover:shadow-lg hover:-translate-y-0.5'>
                                    {`Escolher plano ${p.planTypeDescription.toLocaleLowerCase()}`}
                                </Link>
                            </div>
                        </div>
                    ))
                }
            </div>
        </section>
    );
}

function Testimonials() {

    const testimonials = [
        {
            text: 'Reduzi meus no-shows em 40%. O sistema de confirmação automática mudou meu negócio completamente.',
            author: 'Dra. Izabelle',
            company: 'Point do Sorriso',
            emoji: '👩‍⚕️'
        },
        {
            text: 'Interface limpa, suporte ágil e migração super fácil. Recomendo de verdade.',
            author: 'Equipe Pitoco Beleza',
            company: 'Pitoco Beleza',
            emoji: '💄'
        },
        {
            text: 'Nunca mais marquei paciente em cima do outro. Finalmente tenho controle total.',
            author: 'Dr. Jailson Mendes',
            company: 'Orto Orange',
            emoji: '😁'
        }
    ];

    const [visible, setVisible] = useState<number>(2);

    useEffect(() => {
        const timer = setTimeout(() => setVisible(3), 800);
        return () => clearTimeout(timer);
    }, []);

    return (
        <section id='testimonials' className='px-4 sm:px-6 py-16 sm:py-24 max-w-6xl mx-auto'>
            {/* Section Header */}
            <div className='text-center mb-12 sm:mb-16'>
                <h2 className='text-3xl sm:text-4xl lg:text-5xl font-black text-gray-900 mb-4'>O que profissionais dizem</h2>
                <p className='text-lg text-gray-600 max-w-2xl mx-auto'>Histórias reais de quem usa a plataforma no dia a dia</p>
            </div>

            {/* Testimonials Grid */}
            <div className='grid md:grid-cols-3 gap-6 sm:gap-8'>
                {
                    testimonials.slice(0, visible).map((t, i) => (
                        <div key={i} className='p-6 sm:p-8 bg-white/70 backdrop-blur-sm border border-[var(--main-light)]/80 rounded-2xl hover:bg-white hover:shadow-lg hover:-translate-y-1 transition-all duration-300 animate-in fade-in'>
                            <p className='text-gray-700 leading-relaxed mb-6 text-lg italic'>&quot;{t.text}&quot;</p>
                            <div className='flex items-center gap-3'>
                                <span className='text-3xl'>{t.emoji}</span>

                                <div>
                                    <p className='font-bold text-gray-900'>{t.author}</p>
                                    <p className='text-sm text-gray-600'>{t.company}</p>
                                </div>
                            </div>
                        </div>
                    ))
                }
            </div>
        </section>
    )
}

function CTA({ me, plans }: { me: iMeSimple | undefined, plans: iPlanTypeOutput | undefined }) {
    return (
        <section className='px-4 sm:px-6 py-12 sm:py-16 max-w-6xl mx-auto'>
            <div className='relative overflow-hidden px-6 sm:px-12 py-12 sm:py-16 rounded-3xl bg-gradient-to-r from-[var(--main)] via-[var(--main)] to-[var(--main-dark)] shadow-2xl'>
                {/* Background Pattern */}
                <div className='absolute inset-0 opacity-10'>
                    <div className='absolute top-0 right-0 w-72 h-72 bg-white rounded-full blur-3xl' />
                    <div className='absolute bottom-0 left-0 w-96 h-96 bg-white rounded-full blur-3xl' />
                </div>

                {/* Content */}
                <div className='relative z-10 text-center'>
                    <h2 className='text-3xl sm:text-4xl lg:text-5xl font-black text-white mb-4 leading-tight'>Pronto para organizar sua agenda?</h2>
                    <p className='text-lg sm:text-xl text-[var(--main-light)] mb-8 max-w-2xl mx-auto'>
                        {
                            me?.isAuth ? `Bem-vindo de volta, ${me?.userName}! Organize sua agenda e veja seus próximos agendamentos.` : `Teste gratuitamente por ${plans?.planDurationDaysFree ?? '-'} dias. Sem cartão de crédito necessário.`
                        }
                    </p>

                    <Link href={me?.isAuth ? ROUTES.DASHBOARD : ROUTES.CRIAR_CONTA} className='inline-flex items-center gap-2 px-8 py-4 bg-white text-[var(--main-dark)] font-bold rounded-lg hover:shadow-2xl hover:-translate-y-1 transition-all text-lg'>
                        {me?.isAuth ? 'Acessar dashboard' : 'Começar agora'}
                        <Icon icon='arrow-right' className='w-5 h-5' />
                    </Link>
                </div>
            </div>
        </section>
    )
}

function Footer() {
    return (
        <footer className='bg-gradient-to-b from-gray-900 to-black text-gray-400 py-12 sm:py-16'>
            <div className='max-w-6xl mx-auto px-4 sm:px-6'>
                <div className='grid md:grid-cols-4 gap-8 mb-8 pb-8 border-b border-gray-800'>
                    {/* Brand */}
                    <div>
                        <div className='flex items-center gap-2 mb-3'>
                            <div className='w-8 h-8 bg-gradient-to-br from-[var(--main)] to-[var(--main-dark)] rounded-lg flex items-center justify-center text-white'>
                                <Icon icon='calendar' />
                            </div>

                            <span className='font-bold text-white'>{SYSTEM.NAME}</span>
                        </div>

                        <p className='text-sm text-gray-500'>{SYSTEM.DESCRIPTION}</p>
                    </div>

                    {/* Product */}
                    <div>
                        <p className='font-semibold text-white mb-3 text-sm uppercase tracking-wide'>Plataforma {SYSTEM.NAME}</p>

                        <ul className='space-y-2'>
                            <li>
                                <Link href={ROUTES.DASHBOARD} className='hover:text-[var(--main-light)] transition-colors text-sm'>Início</Link>
                            </li>

                            <li>
                                <Link href={ROUTES.ETC_AJUDA} className='hover:text-[var(--main-light)] transition-colors text-sm'>Central de ajuda</Link>
                            </li>
                        </ul>
                    </div>

                    {/* Workaround */}
                    <div className='hidden lg:flex' />

                    {/* Social */}
                    <div className='ml-auto'>
                        <p className='font-semibold text-white mb-3 text-sm uppercase tracking-wide'>Links úteis</p>

                        <div className='flex gap-3'>
                            <Tippy content='Contatar suporte via WhatsApp.'>
                                <span>
                                    <WhatsappHyperlink showIcon={true} className='w-10 h-10 rounded-lg bg-gray-800 hover:bg-[var(--main)] flex items-center justify-center text-gray-400 hover:text-white transition-all' />
                                </span>
                            </Tippy>

                            <Tippy content='Contatar suporte via e-mail.'>
                                <a
                                    href={`mailto:${SYSTEM.EMAIL_SUPPORT}`}
                                    className='w-10 h-10 rounded-lg bg-gray-800 hover:bg-[var(--main)] flex items-center justify-center text-gray-400 hover:text-white transition-all'
                                >
                                    <Icon icon='mail' />
                                </a>
                            </Tippy>

                            <Tippy content={`GitHub ${SYSTEM.AUTHOR}.`}>
                                <a
                                    href={SYSTEM.URL_GITHUB}
                                    target='_blank'
                                    rel='noopener noreferrer'
                                    className='w-10 h-10 rounded-lg bg-gray-800 hover:bg-[var(--main)] flex items-center justify-center text-gray-400 hover:text-white transition-all'
                                >
                                    <Icon icon='github' />
                                </a>
                            </Tippy>

                            <Tippy content={`LinkedIn ${SYSTEM.AUTHOR}.`}>
                                <a
                                    href={SYSTEM.URL_LINKEDIN}
                                    target='_blank'
                                    rel='noopener noreferrer'
                                    className='w-10 h-10 rounded-lg bg-gray-800 hover:bg-[var(--main)] flex items-center justify-center text-gray-400 hover:text-white transition-all'
                                >
                                    <Icon icon='linkedin' />
                                </a>
                            </Tippy>
                        </div>
                    </div>
                </div>

                {/* Bottom */}
                <div className='flex flex-col sm:flex-row justify-between items-center gap-4 text-sm text-gray-500'>
                    <p>© {new Date().getFullYear()} {SYSTEM.NAME}. Todos os direitos reservados.</p>

                    <div className='flex gap-6'>
                        <Link href={ROUTES.ETC_SEGURANCA} className='hover:text-[var(--main-light)] transition-colors'>Segurança</Link>
                        <Link href={ROUTES.ETC_PRIVACIDADE} className='hover:text-[var(--main-light)] transition-colors'>Privacidade</Link>
                        <Link href={ROUTES.ETC_TERMOS_DE_USO} className='hover:text-[var(--main-light)] transition-colors'>Termos</Link>
                    </div>
                </div>
            </div>
        </footer>
    )
}