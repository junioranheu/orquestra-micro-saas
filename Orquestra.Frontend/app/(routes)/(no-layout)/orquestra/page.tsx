'use client';
import SYSTEM from '@/app/consts/system';
import useTitle from '@/app/hooks/useTitle';
import AOS from 'aos';
import 'aos/dist/aos.css';
import feather from 'feather-icons';
import { MouseEvent, useEffect, useState } from 'react';

export default function LandingPage() {

    useTitle(SYSTEM.NAME);
    const [open, setOpen] = useState<boolean>(false);

    useEffect(() => {
        AOS.init({ duration: 800, easing: 'ease-in-out', once: true });
        feather.replace();
    }, [])

    return (
        <div className='min-h-screen font-inter text-gray-800 bg-white'>
            <Header open={open} setOpen={setOpen} />

            <main>
                <Hero />
                <Features />
                <Testimonials />
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

            setTimeout(() => {
                AOS.refresh();
            }, 200);
        }
    }

    return (
        <header className='sticky top-0 z-50 bg-white shadow-sm'>
            <div className='container mx-auto px-6 py-4 flex items-center justify-between'>
                <div className='flex items-center space-x-2'>
                    <div className='w-10 h-10 rounded-full bg-[#a4dcb9] flex items-center justify-center'>
                        <i data-feather='calendar' className='text-white'></i>
                    </div>

                    <span className='text-xl font-semibold'>Orquestra</span>
                </div>

                <nav className='hidden md:flex space-x-8'>
                    <a className='text-gray-600 hover:text-[#a4dcb9] transition-all duration-300' href='#' onClick={(e) => handleScroll(e, 'features')}>
                        Features
                    </a>

                    <a className='text-gray-600 hover:text-[#a4dcb9] transition-all duration-300' href='#' onClick={(e) => handleScroll(e, 'pricing')}>
                        Pricing
                    </a>

                    <a className='text-gray-600 hover:text-[#a4dcb9] transition-all duration-300' href='#' onClick={(e) => handleScroll(e, 'testimonials')}>
                        Testimonials
                    </a>

                    <a className='text-gray-600 hover:text-[#a4dcb9] transition-all duration-300' href='#' onClick={(e) => handleScroll(e, 'cta')}>
                        Contact
                    </a>
                </nav>

                <div className='hidden md:flex items-center space-x-4'>
                    <a className='text-gray-600 hover:text-[#a4dcb9] transition-all duration-300' href='#'>
                        Log in
                    </a>

                    <a className='px-4 py-2 rounded-full bg-[#a4dcb9] text-white hover:bg-[#6aa87d] transition-all duration-300' href='#'>
                        Sign up
                    </a>
                </div>

                <button
                    className='md:hidden'
                    aria-label='menu'
                    onClick={() => setOpen(!open)}
                    title='menu'
                >
                    <i data-feather='menu'></i>
                </button>
            </div>

            {/* Mobile menu */}
            {
                open && (
                    <div className='md:hidden border-t border-gray-100'>
                        <div className='px-6 py-4 space-y-3'>
                            <a className='block text-gray-700' href='#' onClick={(e) => handleScroll(e, 'features')}>
                                Features
                            </a>

                            <a className='block text-gray-700' href='#' onClick={(e) => handleScroll(e, 'pricing')}>
                                Pricing
                            </a>

                            <a className='block text-gray-700' href='#' onClick={(e) => handleScroll(e, 'testimonials')}>
                                Testimonials
                            </a>

                            <a className='block text-gray-700' href='#' onClick={(e) => handleScroll(e, 'cta')}>
                                Contact
                            </a>

                            <div className='pt-3 flex gap-3'>
                                <a className='text-gray-700' href='#'>
                                    Log in
                                </a>
                                <a className='px-4 py-2 rounded-full bg-[#a4dcb9] text-white hover:bg-[#6aa87d]' href='#'>
                                    Sign up
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
                        <i data-feather='calendar' className='text-white text-2xl'></i>
                    </div>

                    <h1 className='text-5xl md:text-6xl font-bold mb-6 leading-tight'>
                        Professional <span className='text-[#a4dcb9]'>Appointment Management</span> Made Simple
                    </h1>

                    <p className='text-xl text-gray-600 mb-10 max-w-2xl mx-auto'>
                        Elevate your client experience with our intuitive platform designed exclusively for service
                        professionals.
                    </p>

                    <div className='flex justify-center space-x-4'>
                        <a href='#' className='px-8 py-4 rounded-full bg-[#a4dcb9] text-white hover:bg-[#6aa87d] transition-all duration-300 text-lg font-medium shadow-lg hover:shadow-xl transform hover:-translate-y-1'>
                            Start Free Trial
                        </a>
                        <a href='#' className='px-8 py-4 rounded-full border-2 border-gray-200 hover:border-[#a4dcb9] text-gray-700 hover:text-[#a4dcb9] transition-all duration-300 text-lg font-medium'>
                            Watch Demo
                        </a>
                    </div>
                </div>
            </div>
        </section>
    )
}

function Features() {
    const cards = [
        {
            icon: 'clock',
            title: 'Smart Scheduling',
            text: 'Automated reminders and intelligent time slot suggestions to optimize your calendar.',
            delay: 100,
        },
        {
            icon: 'users',
            title: 'Client Management',
            text: 'Comprehensive client profiles with history, preferences, and notes all in one place.',
            delay: 200,
        },
        {
            icon: 'bar-chart-2',
            title: 'Performance Insights',
            text: 'Detailed analytics to track your business growth and identify opportunities.',
            delay: 300,
        }
    ];

    return (
        <section className='py-20 bg-gray-50' id='features'>
            <div className='container mx-auto px-6'>
                <div className='text-center mb-16' data-aos='fade-up'>
                    <h2 className='text-3xl font-bold mb-4'>Powerful Features</h2>
                    <p className='text-gray-600 max-w-2xl mx-auto'>Designed to simplify your workflow and enhance client experience</p>
                </div>

                <div className='grid md:grid-cols-3 gap-8'>
                    {
                        cards.map((c) => (
                            <div
                                key={c.title}
                                className='bg-white p-8 rounded-xl shadow-sm hover:shadow-md transition-all duration-500'
                                data-aos='fade-up'
                                data-aos-delay={c.delay}
                            >
                                <div className='w-12 h-12 rounded-full bg-[#d1f2e0] flex items-center justify-center mb-4'>
                                    <i data-feather={c.icon} className='text-[#a4dcb9]'></i>
                                </div>
                                <h3 className='text-xl font-semibold mb-3'>{c.title}</h3>
                                <p className='text-gray-600'>{c.text}</p>
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
        <section className='py-20' id='testimonials'>
            <div className='container mx-auto px-6'>
                <div className='text-center mb-16' data-aos='fade-up'>
                    <h2 className='text-3xl font-bold mb-4'>Trusted by Professionals</h2>
                    <p className='text-gray-600 max-w-2xl mx-auto'>Join thousands of professionals who transformed their business</p>
                </div>

                <div className='grid md:grid-cols-2 gap-8'>
                    <div className='bg-white p-8 rounded-xl shadow-sm' data-aos='fade-right'>
                        <div className='flex items-center mb-4'>
                            <div className='w-12 h-12 rounded-full bg-gray-200 mr-4 overflow-hidden'>
                                <img src='http://static.photos/people/200x200/1' alt='Sarah Johnson' className='w-full h-full object-cover' />
                            </div>

                            <div>
                                <h4 className='font-semibold'>Sarah Johnson</h4>
                                <p className='text-gray-500 text-sm'>Dental Clinic Owner</p>
                            </div>
                        </div>

                        <p className='text-gray-600'>'Orquestra reduced our no-shows by 70% with automated reminders. The interface is so intuitive that my entire staff adopted it within days.'</p>
                    </div>

                    <div className='bg-white p-8 rounded-xl shadow-sm' data-aos='fade-left'>
                        <div className='flex items-center mb-4'>
                            <div className='w-12 h-12 rounded-full bg-gray-200 mr-4 overflow-hidden'>
                                <img src='http://static.photos/people/200x200/2' alt='Michael Chen' className='w-full h-full object-cover' />
                            </div>

                            <div>
                                <h4 className='font-semibold'>Michael Chen</h4>
                                <p className='text-gray-500 text-sm'>Consulting Firm</p>
                            </div>
                        </div>

                        <p className='text-gray-600'>'I've tried many scheduling tools, but Orquestra stands out with its elegant design and powerful features. It's become indispensable to my practice.'</p>
                    </div>
                </div>
            </div>
        </section>
    )
}

function Pricing() {
    return (
        <section className='py-20 bg-gray-50' id='pricing'>
            <div className='container mx-auto px-6'>
                <div className='text-center mb-16' data-aos='fade-up'>
                    <h2 className='text-3xl font-bold mb-4'>Simple Pricing</h2>
                    <p className='text-gray-600 max-w-2xl mx-auto'>Choose the plan that fits your needs</p>
                </div>

                <div className='grid md:grid-cols-3 gap-8 max-w-5xl mx-auto'>
                    <div className='bg-white p-8 rounded-xl shadow-sm' data-aos='fade-up' data-aos-delay='100'>
                        <h3 className='text-xl font-semibold mb-2'>Starter</h3>
                        <p className='text-gray-600 mb-6'>Perfect for solo professionals</p>
                        <div className='text-4xl font-bold mb-6'>$19<span className='text-lg text-gray-500'>/month</span></div>

                        <ul className='space-y-3 mb-8'>
                            <li className='flex items-center'><i data-feather='check' className='text-[#a4dcb9] mr-2'></i><span>Up to 100 appointments/month</span></li>
                            <li className='flex items-center'><i data-feather='check' className='text-[#a4dcb9] mr-2'></i><span>Email reminders</span></li>
                            <li className='flex items-center'><i data-feather='check' className='text-[#a4dcb9] mr-2'></i><span>Basic analytics</span></li>
                        </ul>

                        <a className='block w-full py-3 text-center rounded-full border border-gray-300 hover:border-[#a4dcb9] text-gray-700 hover:text-[#a4dcb9] transition-all duration-300' href='#'>Get Started</a>
                    </div>

                    <div className='bg-white p-8 rounded-xl shadow-lg border-2 border-[#a4dcb9] relative' data-aos='fade-up'>
                        <div className='absolute top-0 right-0 bg-[#a4dcb9] text-white px-4 py-1 rounded-bl-lg rounded-tr-lg text-sm font-medium'>Most Popular</div>
                        <h3 className='text-xl font-semibold mb-2'>Professional</h3>
                        <p className='text-gray-600 mb-6'>For growing practices</p>
                        <div className='text-4xl font-bold mb-6'>$39<span className='text-lg text-gray-500'>/month</span></div>

                        <ul className='space-y-3 mb-8'>
                            <li className='flex items-center'><i data-feather='check' className='text-[#a4dcb9] mr-2'></i><span>Unlimited appointments</span></li>
                            <li className='flex items-center'><i data-feather='check' className='text-[#a4dcb9] mr-2'></i><span>SMS & email reminders</span></li>
                            <li className='flex items-center'><i data-feather='check' className='text-[#a4dcb9] mr-2'></i><span>Advanced analytics</span></li>
                            <li className='flex items-center'><i data-feather='check' className='text-[#a4dcb9] mr-2'></i><span>Client portal</span></li>
                        </ul>

                        <a className='block w-full py-3 text-center rounded-full bg-[#a4dcb9] text-white hover:bg-[#6aa87d] transition-all duration-300' href='#'>Get Started</a>
                    </div>

                    <div className='bg-white p-8 rounded-xl shadow-sm' data-aos='fade-up' data-aos-delay='100'>
                        <h3 className='text-xl font-semibold mb-2'>Enterprise</h3>
                        <p className='text-gray-600 mb-6'>For large organizations</p>
                        <div className='text-4xl font-bold mb-6'>$99<span className='text-lg text-gray-500'>/month</span></div>

                        <ul className='space-y-3 mb-8'>
                            <li className='flex items-center'><i data-feather='check' className='text-[#a4dcb9] mr-2'></i><span>Unlimited appointments</span></li>
                            <li className='flex items-center'><i data-feather='check' className='text-[#a4dcb9] mr-2'></i><span>Multi-location support</span></li>
                            <li className='flex items-center'><i data-feather='check' className='text-[#a4dcb9] mr-2'></i><span>API access</span></li>
                            <li className='flex items-center'><i data-feather='check' className='text-[#a4dcb9] mr-2'></i><span>Dedicated support</span></li>
                        </ul>

                        <a className='block w-full py-3 text-center rounded-full border border-gray-300 hover:border-[#a4dcb9] text-gray-700 hover:text-[#a4dcb9] transition-all duration-300' href='#'>Contact Sales</a>
                    </div>
                </div>
            </div>
        </section>
    )
}

function CTA() {
    return (
        <section className='py-20 bg-[#d1f2e0]' id='cta'>
            <div className='container mx-auto px-6 text-center' data-aos='fade-up'>
                <h2 className='text-3xl font-bold mb-6'>Ready to transform your appointment management?</h2>
                <p className='text-gray-700 mb-8 max-w-2xl mx-auto'>Join thousands of professionals who save hours every week with Orquestra.</p>
                <a className='inline-block px-8 py-3 rounded-full bg-[#a4dcb9] text-white hover:bg-[#6aa87d] transition-all duration-300' href='#'>Start Your Free Trial</a>
            </div>
        </section>
    )
}

function Footer() {
    return (
        <footer className='bg-gray-900 text-white py-12'>
            <div className='container mx-auto px-6'>
                <div className='grid md:grid-cols-4 gap-8'>
                    <div>
                        <div className='flex items-center space-x-2 mb-4'>
                            <div className='w-8 h-8 rounded-full bg-[#a4dcb9] flex items-center justify-center'>
                                <i data-feather='calendar' className='text-white text-sm'></i>
                            </div>
                            <span className='text-lg font-semibold'>Orquestra</span>
                        </div>
                        <p className='text-gray-400'>The elegant solution for professional appointment management.</p>
                    </div>

                    <div>
                        <h4 className='text-lg font-semibold mb-4'>Product</h4>
                        <ul className='space-y-2'>
                            <li><a className='text-gray-400 hover:text-white transition-all duration-300' href='#'>Features</a></li>
                            <li><a className='text-gray-400 hover:text-white transition-all duration-300' href='#'>Pricing</a></li>
                            <li><a className='text-gray-400 hover:text-white transition-all duration-300' href='#'>Integrations</a></li>
                            <li><a className='text-gray-400 hover:text-white transition-all duration-300' href='#'>Updates</a></li>
                        </ul>
                    </div>

                    <div>
                        <h4 className='text-lg font-semibold mb-4'>Company</h4>
                        <ul className='space-y-2'>
                            <li><a className='text-gray-400 hover:text-white transition-all duration-300' href='#'>About</a></li>
                            <li><a className='text-gray-400 hover:text-white transition-all duration-300' href='#'>Careers</a></li>
                            <li><a className='text-gray-400 hover:text-white transition-all duration-300' href='#'>Blog</a></li>
                            <li><a className='text-gray-400 hover:text-white transition-all duration-300' href='#'>Press</a></li>
                        </ul>
                    </div>

                    <div>
                        <h4 className='text-lg font-semibold mb-4'>Connect</h4>
                        <ul className='space-y-2'>
                            <li><a className='text-gray-400 hover:text-white transition-all duration-300' href='#'>Contact</a></li>
                            <li><a className='text-gray-400 hover:text-white transition-all duration-300' href='#'>Help Center</a></li>
                            <li><a className='text-gray-400 hover:text-white transition-all duration-300' href='#'>Community</a></li>
                            <li className='flex space-x-4 mt-4'>
                                <a className='text-gray-400 hover:text-white transition-all duration-300' href='#'><i data-feather='twitter'></i></a>
                                <a className='text-gray-400 hover:text-white transition-all duration-300' href='#'><i data-feather='facebook'></i></a>
                                <a className='text-gray-400 hover:text-white transition-all duration-300' href='#'><i data-feather='instagram'></i></a>
                                <a className='text-gray-400 hover:text-white transition-all duration-300' href='#'><i data-feather='linkedin'></i></a>
                            </li>
                        </ul>
                    </div>
                </div>

                <div className='border-t border-gray-800 mt-12 pt-8 flex flex-col md:flex-row justify-between items-center'>
                    <p className='text-gray-500 text-sm mb-4 md:mb-0'>© 2023 Orquestra. All rights reserved.</p>
                    <div className='flex space-x-6'>
                        <a className='text-gray-500 hover:text-white text-sm transition-all duration-300' href='#'>Privacy Policy</a>
                        <a className='text-gray-500 hover:text-white text-sm transition-all duration-300' href='#'>Terms of Service</a>
                        <a className='text-gray-500 hover:text-white text-sm transition-all duration-300' href='#'>Cookie Policy</a>
                    </div>
                </div>
            </div>
        </footer>
    )
}