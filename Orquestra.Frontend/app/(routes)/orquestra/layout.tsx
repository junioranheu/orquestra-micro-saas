import ROUTES from '@/app/consts/routes';
import SYSTEM from '@/app/consts/system';
import { Metadata } from 'next';
import { ReactNode } from 'react';
import LayoutClient from './layout-client';

const PAGE_TITLE = `${SYSTEM.NAME} — Agendamento profissional sem complicações`;

const PAGE_DESCRIPTION = 'Agendamentos inteligentes, gestão de clientes, confirmações automáticas por WhatsApp e muito mais. Plataforma simples e poderosa para profissionais e empresas.';

export const metadata: Metadata = {
    metadataBase: new URL(SYSTEM.URL_BASE!),

    title: PAGE_TITLE,

    description: PAGE_DESCRIPTION,

    keywords: [
        'agendamento online',
        'gestão de clientes',
        'agenda profissional',
        'confirmação por WhatsApp',
        'sistema de agendamento',
        'SaaS',
        SYSTEM.NAME
    ],

    authors: [
        {
            name: SYSTEM.AUTHOR,
            url: SYSTEM.URL_GITHUB
        }
    ],

    creator: SYSTEM.AUTHOR,

    openGraph: {
        type: 'website',
        locale: 'pt_BR',

        siteName: SYSTEM.NAME,

        title: PAGE_TITLE,

        description: 'Agendamentos inteligentes, gestão de clientes, confirmações automáticas por WhatsApp. Tudo em uma plataforma simples e poderosa.',

        images: [
            {
                url: '/og-image.png',
                width: 1200,
                height: 630,
                alt: `${SYSTEM.NAME} — ${SYSTEM.DESCRIPTION}`
            }
        ]
    },

    twitter: {
        card: 'summary_large_image',

        title: PAGE_TITLE,

        description: 'Agendamentos inteligentes, gestão de clientes, confirmações automáticas por WhatsApp.',

        images: ['/og-image.png']
    },

    robots: {
        index: true,
        follow: true,

        googleBot: {
            index: true,
            follow: true,
            'max-video-preview': -1,
            'max-image-preview': 'large',
            'max-snippet': -1
        }
    },

    alternates: {
        canonical: ROUTES.LANDING_PAGE
    }
};

export default function RootLayout({ children }: Readonly<{ children: ReactNode }>) {

    const jsonLd = {
        '@context': 'https://schema.org',

        '@type': 'SoftwareApplication',

        name: SYSTEM.NAME,

        description: 'Agendamentos inteligentes, gestão de clientes, confirmações automáticas por WhatsApp e muito mais.',

        applicationCategory: 'BusinessApplication',

        operatingSystem: 'Web',

        offers: {
            '@type': 'AggregateOffer',
            priceCurrency: 'BRL',
            lowPrice: '0',
            offerCount: '3'
        },

        author: {
            '@type': 'Person',
            name: SYSTEM.AUTHOR,
            url: SYSTEM.URL_GITHUB
        }
    };

    return (
        <html lang='pt-BR'>
            <head>
                <script
                    type='application/ld+json'
                    dangerouslySetInnerHTML={{ __html: JSON.stringify(jsonLd) }}
                />
            </head>

            <LayoutClient>
                {children}
            </LayoutClient>
        </html>
    )
}