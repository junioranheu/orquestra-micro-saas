'use client';
import Icon from '@/app/components/icon/icon';
import ROUTES from '@/app/consts/routes';
import SYSTEM from '@/app/consts/system';
import useTitle from '@/app/hooks/useTitle';
import { useRouter } from 'next/navigation';
import { MouseEvent, useState } from 'react';

export default function LandingPage() {

    useTitle(`${SYSTEM.NAME}: ${SYSTEM.DESCRIPTION}`, false);
    const [open, setOpen] = useState<boolean>(false);

    return (
        <div className='min-h-screen font-inter text-gray-800 bg-white'>
            <Header open={open} setOpen={setOpen} />

            <main>
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

    const router = useRouter();

    function handleScroll(e: MouseEvent<HTMLAnchorElement>, id: string) {
        e.preventDefault();
        const el = document.getElementById(id);

        if (el) {
            el.scrollIntoView({ behavior: 'smooth' });
        }
    }

    return (
        <header className='sticky top-0 z-50 bg-white shadow-sm'>
            <div className='container mx-auto px-6 py-4 flex items-center justify-between'>
                <div className='flex items-center space-x-2'>
                    <div className='w-10 h-10 rounded-full bg-[#a4dcb9] flex items-center justify-center'>
                        <Icon icon='calendar' size='small' />
                    </div>

                    <span className='text-xl font-semibold'>{SYSTEM.NAME}</span>
                </div>

                <nav className='hidden md:flex space-x-8'>
                    <a className='text-gray-600 hover:text-[#a4dcb9] transition-all duration-300' href='#' onClick={(e) => handleScroll(e, 'features')}>
                        Funcionalidades
                    </a>

                    <a className='text-gray-600 hover:text-[#a4dcb9] transition-all duration-300' href='#' onClick={(e) => handleScroll(e, 'pricing')}>
                        Preços
                    </a>

                    <a className='text-gray-600 hover:text-[#a4dcb9] transition-all duration-300' href='#' onClick={(e) => handleScroll(e, 'cta')}>
                        Contato
                    </a>
                </nav>

                <div className='hidden md:flex items-center space-x-4'>
                    <a className='text-gray-600 hover:text-[#a4dcb9] transition-all duration-300' href='#' onClick={() => router.push(ROUTES.LOGIN)}>
                        Entrar
                    </a>

                    <a className='px-4 py-2 rounded-full bg-[#a4dcb9] text-white hover:bg-[#6aa87d] transition-all duration-300' href='#' onClick={() => router.push(ROUTES.CRIAR_CONTA)}>
                        Criar conta
                    </a>
                </div>

                <button
                    className='md:hidden'
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
                    <div className='md:hidden border-t border-gray-100'>
                        <div className='px-6 py-4 space-y-3'>
                            <a className='block text-gray-700' href='#' onClick={(e) => handleScroll(e, 'features')}>
                                Funcionalidades
                            </a>

                            <a className='block text-gray-700' href='#' onClick={(e) => handleScroll(e, 'pricing')}>
                                Preços
                            </a>

                            <a className='block text-gray-700' href='#' onClick={(e) => handleScroll(e, 'cta')}>
                                Contato
                            </a>

                            <div className='pt-3 flex gap-3'>
                                <a className='text-gray-700' href='#' onClick={() => router.push(ROUTES.LOGIN)}>
                                    Entrar
                                </a>

                                <a className='px-4 py-2 rounded-full bg-[#a4dcb9] text-white hover:bg-[#6aa87d]' href='#' onClick={() => router.push(ROUTES.CRIAR_CONTA)}>
                                    Criar conta
                                </a>
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
        <section className='py-32 bg-gradient-to-b from-[#d1f2e0] to-white'>
            <div className='container mx-auto px-6 text-center'>
                <div className='max-w-3xl mx-auto' data-aos='fade-up'>
                    <div className='w-20 h-20 rounded-full bg-[#a4dcb9] flex items-center justify-center mx-auto mb-8'>
                        <Icon icon='calendar' className='text-white' />
                    </div>

                    <h1 className='text-5xl md:text-6xl font-bold mb-6 leading-tight'>
                        <span className='text-[#6aa87d]'>{SYSTEM.NAME}</span>
                    </h1>

                    <h2 className='text-5xl md:text-6xl font-bold mb-6 leading-tight'>
                        <span>{SYSTEM.DESCRIPTION}</span>
                    </h2>

                    <p className='text-xl text-gray-600 mb-10 max-w-2xl mx-auto'>
                        Eleve a experiência dos seus clientes com nossa plataforma intuitiva, criada exclusivamente para profissionais de serviços.
                    </p>

                    <div className='flex justify-center space-x-4'>
                        <a href={`${ROUTES.LOGIN}`} className='px-8 py-4 rounded-full bg-[#a4dcb9] text-white hover:bg-[#6aa87d] transition-all duration-300 text-lg font-medium shadow-lg hover:shadow-xl transform hover:-translate-y-1'>
                            Começar Teste Grátis
                        </a>

                        <a href={`${ROUTES.LOGIN}`} className='px-8 py-4 rounded-full border-2 border-gray-200 hover:border-[#a4dcb9] text-gray-700 hover:text-[#a4dcb9] transition-all duration-300 text-lg font-medium'>
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
        <section id='features' className='py-24'>
            <div className='container mx-auto px-6'>
                <div className='text-center mb-16' data-aos='fade-up'>
                    <h2 className='text-4xl font-bold mb-4'>Funcionalidades Poderosas</h2>
                    <p className='text-gray-600 max-w-2xl mx-auto'>
                        Tudo que você precisa para gerenciar agendamentos, clientes e pagamentos em um só lugar.
                    </p>
                </div>

                <div className='grid md:grid-cols-3 gap-12'>
                    <div className='p-6 rounded-2xl bg-white shadow-md hover:shadow-xl transition-all' data-aos='fade-up' data-aos-delay='100'>
                        <Icon icon='clock' size='big' className='w-10 h-10 text-[#a4dcb9] mb-4' />
                        <h3 className='text-xl font-semibold mb-2'>Agendamento Inteligente</h3>
                        <p className='text-gray-600'>
                            Gerencie horários automaticamente com lembretes para clientes e equipe.
                        </p>
                    </div>

                    <div className='p-6 rounded-2xl bg-white shadow-md hover:shadow-xl transition-all' data-aos='fade-up' data-aos-delay='200'>
                        <Icon icon='users' size='big' className='w-10 h-10 text-[#a4dcb9] mb-4' />
                        <h3 className='text-xl font-semibold mb-2'>Gestão de Clientes</h3>
                        <p className='text-gray-600'>
                            Tenha o histórico completo dos seus clientes em um só lugar.
                        </p>
                    </div>

                    <div className='p-6 rounded-2xl bg-white shadow-md hover:shadow-xl transition-all' data-aos='fade-up' data-aos-delay='300'>
                        <Icon icon='credit-card' size='big' className='w-10 h-10 text-[#a4dcb9] mb-4' />
                        <h3 className='text-xl font-semibold mb-2'>Pagamentos Integrados</h3>
                        <p className='text-gray-600'>
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
        <section id='pricing' className='py-24 bg-gray-50'>
            <div className='container mx-auto px-6'>
                <div className='text-center mb-16' data-aos='fade-up'>
                    <h2 className='text-4xl font-bold mb-4'>Planos e Preços</h2>
                    <p className='text-gray-600'>Escolha o plano que melhor se adapta ao seu negócio</p>
                </div>

                <div className='grid md:grid-cols-3 gap-8'>
                    <div className='p-8 bg-white rounded-2xl shadow-md hover:shadow-xl transition-all' data-aos='fade-up' data-aos-delay='100'>
                        <h3 className='text-2xl font-semibold mb-4'>Básico</h3>
                        <p className='text-gray-600 mb-6'>Ideal para profissionais individuais</p>
                        <p className='text-4xl font-bold mb-6'>R$29<span className='text-lg'>/mês</span></p>
                        <ul className='space-y-3 text-gray-600 mb-6'>
                            <li>✔️ 50 agendamentos/mês</li>
                            <li>✔️ Notificações por e-mail</li>
                            <li>✔️ Suporte básico</li>
                        </ul>
                        <a href='#' className='w-full block text-center px-6 py-3 rounded-full bg-[#a4dcb9] text-white hover:bg-[#6aa87d] transition-all'>
                            Escolher plano
                        </a>
                    </div>

                    <div className='p-8 bg-white rounded-2xl shadow-lg border-2 border-[#a4dcb9]' data-aos='fade-up' data-aos-delay='200'>
                        <h3 className='text-2xl font-semibold mb-4'>Profissional</h3>
                        <p className='text-gray-600 mb-6'>Perfeito para equipes pequenas</p>
                        <p className='text-4xl font-bold mb-6'>R$79<span className='text-lg'>/mês</span></p>
                        <ul className='space-y-3 text-gray-600 mb-6'>
                            <li>✔️ Agendamentos ilimitados</li>
                            <li>✔️ Notificações por SMS</li>
                            <li>✔️ Suporte prioritário</li>
                        </ul>
                        <a href='#' className='w-full block text-center px-6 py-3 rounded-full bg-[#6aa87d] text-white hover:bg-[#4d7a5b] transition-all'>
                            Escolher plano
                        </a>
                    </div>

                    <div className='p-8 bg-white rounded-2xl shadow-md hover:shadow-xl transition-all' data-aos='fade-up' data-aos-delay='300'>
                        <h3 className='text-2xl font-semibold mb-4'>Empresarial</h3>
                        <p className='text-gray-600 mb-6'>Para grandes empresas</p>
                        <p className='text-4xl font-bold mb-6'>R$199<span className='text-lg'>/mês</span></p>
                        <ul className='space-y-3 text-gray-600 mb-6'>
                            <li>✔️ Tudo do Profissional</li>
                            <li>✔️ Integrações avançadas</li>
                            <li>✔️ Suporte dedicado</li>
                        </ul>
                        <a href='#' className='w-full block text-center px-6 py-3 rounded-full bg-[#a4dcb9] text-white hover:bg-[#6aa87d] transition-all'>
                            Escolher plano
                        </a>
                    </div>
                </div>
            </div>
        </section>
    );
}

function CTA() {

    const router = useRouter();

    return (
        <section id='cta' className='py-24 bg-[#a4dcb9] text-white text-center'>
            <div className='container mx-auto px-6' data-aos='zoom-in'>
                <h2 className='text-4xl font-bold mb-6'>Pronto para organizar sua agenda?</h2>
                <p className='text-lg mb-8'>Experimente o {SYSTEM.NAME} gratuitamente por 14 dias</p>
                <a className='px-8 py-4 rounded-full bg-white text-[#6aa87d] font-medium hover:bg-gray-100 transition-all' href='#' onClick={() => router.push(ROUTES.LOGIN)}>
                    Criar conta grátis
                </a>
            </div>
        </section>
    );
}

function Footer() {
    return (
        <footer className='py-12 bg-gray-900 text-gray-400'>
            <div className='container mx-auto px-6 text-center'>
                <p>© {new Date().getFullYear()} {SYSTEM.NAME}. Todos os direitos reservados.</p>
            </div>
        </footer>
    );
}