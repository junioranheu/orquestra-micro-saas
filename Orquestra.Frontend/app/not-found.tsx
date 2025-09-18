'use client';
import '@/app/styles/globals.scss';
import { usePathname } from 'next/navigation';
import LayoutTemplateOne from './components/layout/template-one';
import { HANKEN } from './fonts/fonts';
import useTitle from './hooks/useTitle';

export default function NotFound() {

    useTitle('Página não encontrada');
    const pathname = usePathname();

    return (
        <div className={HANKEN.className}>
            <LayoutTemplateOne
                svg='error'
                code='#404'
                title='Página não encontrada'
                description={`A página "${pathname}" que você tentou acessar não existe (ou está de férias).`}
                showSupportContact={true}
            />
        </div>
    )
}