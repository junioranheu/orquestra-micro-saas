'use client';
import Tabs from '@/app/components/tabs';
import useApiGetMe from '@/app/hooks/api/useApiGetMe';
import useTitle from '@/app/hooks/useTitle';
import styles from './page.module.scss';
import UsuarioConfiguracoesTabConta from './tabs/conta';
import UsuarioConfiguracoesTabPersonalizacao from './tabs/personalizacao';

export default function UsuarioConfiguracoes() {

    useTitle('Configurações');
    const me = useApiGetMe({});

    if (!me) {
        return null;
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