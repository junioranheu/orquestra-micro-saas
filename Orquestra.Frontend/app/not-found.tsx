'use client';
import '@/app/styles/globals.scss';
import { useEffect, useState } from 'react';
import LayoutTemplateOne from './components/layout/template-one';
import { HANKEN } from './fonts/fonts';
import useTitle from './hooks/useTitle';

export default function NotFound() {

    useTitle('Página não encontrada');
    const [path, setPath] = useState<string>('');

    useEffect(() => {
        setPath(window.location.pathname);
    }, []);

    return (
        <div className={HANKEN.className}>
            <LayoutTemplateOne
                svg='error'
                code='#404'
                title='Página não encontrada'
                description={path ? `A página "${path}" que você tentou acessar não existe.` : 'A página que você tentou acessar não existe.'}
                showSupportContact={true}
            />
        </div>
    )
}