import ROUTES from '@/app/consts/routes';
import SYSTEM from '@/app/consts/system';
import { Metadata } from 'next';
import { ReactNode } from 'react';

const PAGE_TITLE = `Entrar — ${SYSTEM.NAME}`;
const PAGE_DESCRIPTION = `Acesse sua conta no ${SYSTEM.NAME} e gerencie seus agendamentos, clientes e equipe de forma simples e rápida.`;

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

        description: `Acesse sua conta no ${SYSTEM.NAME} e gerencie seus agendamentos, clientes e equipe.`,

        images: [
            {
                url: '/og-image.png',
                width: 1200,
                height: 630,
                alt: `${SYSTEM.NAME} — Login`
            }
        ]
    },

    alternates: {
        canonical: ROUTES.LOGIN
    }
};

export default function LoginLayout({ children }: Readonly<{ children: ReactNode }>) {
    return children;
}