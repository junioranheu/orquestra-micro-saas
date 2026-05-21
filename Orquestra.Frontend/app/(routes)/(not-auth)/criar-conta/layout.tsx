import ROUTES from '@/app/consts/routes';
import SYSTEM from '@/app/consts/system';
import { Metadata } from 'next';
import { ReactNode } from 'react';

const PAGE_TITLE = `Criar conta — ${SYSTEM.NAME}`;
const PAGE_DESCRIPTION = `Crie sua conta gratuita no ${SYSTEM.NAME} e comece a organizar seus agendamentos profissionais em minutos. Sem cartão de crédito necessário.`;

export const metadata: Metadata = {
    metadataBase: new URL(SYSTEM.URL_BASE!),

    title: PAGE_TITLE,

    description: PAGE_DESCRIPTION,

    robots: {
        index: true,
        follow: true
    },

    openGraph: {
        type: 'website',
        locale: 'pt_BR',
        siteName: SYSTEM.NAME,

        title: PAGE_TITLE,

        description: `Crie sua conta gratuita no ${SYSTEM.NAME} e comece a organizar seus agendamentos profissionais em minutos.`,

        images: [
            {
                url: '/og-image.png',
                width: 1200,
                height: 630,
                alt: `${SYSTEM.NAME} — Criar conta`
            }
        ]
    },

    alternates: {
        canonical: ROUTES.CRIAR_CONTA
    }
};

export default function CriarContaLayout({ children }: Readonly<{ children: ReactNode }>) {
    return children;
}