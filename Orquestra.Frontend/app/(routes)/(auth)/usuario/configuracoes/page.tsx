'use client';
import Tabs from '@/app/components/tabs';
import useApiGetMe from '@/app/hooks/api/useApiGetMe';
import useTitle from '@/app/hooks/useTitle';
import styles from './page.module.scss';
import UsuarioConfiguracoesTabEtc from './tabs/etc';
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
                tabs={['Configurações da conta', 'Personalização']}
                contents={[
                    <UsuarioConfiguracoesTabEtc me={me} key={1} />,
                    <UsuarioConfiguracoesTabPersonalizacao key={2} />,
                ]}
                isBig={true}
            />
        </section>
    )
}