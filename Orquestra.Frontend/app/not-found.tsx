'use client';
import '@/app/styles/globals.scss';
import { useEffect, useState } from 'react';
import UpNav from './components/navbar/up-nav';
import LayoutTemplateOne from './components/template/template-one';
import { HANKEN } from './fonts/fonts';
import useTitle from './hooks/useTitle';

export default function NotFound() {

    useTitle('Página não encontrada');
    const [path, setPath] = useState<string>('');

    useEffect(() => {
        setPath(window.location.pathname);
    }, []);

    return (
        <section className={HANKEN.className}>
            <UpNav />

            <section style={{ background: 'var(--gradient)' }}>
                <LayoutTemplateOne
                    svg='error'
                    code='#404'
                    title='Página não encontrada'
                    description={path ? `A página ${(path.includes("/404") ? "" : `"${path}"`)} que você tentou acessar não existe.` : 'A página que você tentou acessar não existe.'}
                    showSupportContact={true}
                    isCentralized={true}
                />
            </section>
        </section>
    )
}