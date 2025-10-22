'use client';
import Tabs from '@/app/components/tabs';
import useApiGetMe from '@/app/hooks/api/useApiGetMe';
import useTitle from '@/app/hooks/useTitle';
import styles from './page.module.scss';
import UsuarioConfiguracoesTabInfos from './tabs/infos';

export default function UsuarioConfiguracoes() {

    useTitle('Configurações');
    const me = useApiGetMe({});

    if (!me) {
        return null;
    }

    return (
        <section className={styles.main}>
            <Tabs
                tabs={[`Informações xxx ${me?.userName}`,]}
                contents={[
                    <UsuarioConfiguracoesTabInfos me={me} key={1} />,
                ]}
            />
        </section>
    )
}