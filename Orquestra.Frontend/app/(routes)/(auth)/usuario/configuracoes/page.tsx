'use client';
import Tabs from '@/app/components/tabs';
import TemplatePageHeader from '@/app/components/template/template-page-header';
import useApiGetMe from '@/app/hooks/api/useApiGetMe';
import { useFakeLoading } from '@/app/hooks/useFakeLoader';
import useTitle from '@/app/hooks/useTitle';
import styles from './page.module.scss';
import UsuarioConfiguracoesTabConta from './tabs/conta';
import UsuarioConfiguracoesTabPersonalizacao from './tabs/personalizacao';

export default function UsuarioConfiguracoes() {

    useTitle('Configurações');
    const me = useApiGetMe({});
    const isLoading = useFakeLoading();

    if (!me || isLoading) {
        return (
            <TemplatePageHeader title='Configurações' isLoading={isLoading}>
            </TemplatePageHeader>
        )
    }

    return (
        <section className={styles.main}>
            <Tabs
                tabs={['Personalização', 'Configurações da conta']}
                contents={[
                    <UsuarioConfiguracoesTabPersonalizacao key={1} />,
                    <UsuarioConfiguracoesTabConta me={me} key={2} />
                ]}
                isBig={true}
            />
        </section>
    )
}