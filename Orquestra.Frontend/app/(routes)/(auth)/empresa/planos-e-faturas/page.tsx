'use client';
import SvgError from '@/app/assets/svg/error.svg';
import CardSimple from '@/app/components/card/simple';
import Tabs from '@/app/components/tabs';
import TemplatePageHeader from '@/app/components/template/template-page-header';
import ROUTES from '@/app/consts/routes';
import { handleGetFirstName } from '@/app/functions/get.formatUserName';
import useApiGetMe from '@/app/hooks/api/useApiGetMe';
import { useFakeLoading } from '@/app/hooks/useFakeLoader';
import useTitle from '@/app/hooks/useTitle';
import { useRouter } from 'next/navigation';
import styles from './page.module.scss';
import EmpresaUsoEPlanoTabFaturas from './tabs/faturas';
import EmpresaUsoEPlanoTabPlanos from './tabs/planos';

export default function EmpresaUsoEPlano() {

    useTitle('Planos e faturas');
    const me = useApiGetMe({});
    const router = useRouter();
    const isLoading = useFakeLoading();

    if (!me || isLoading) {
        return (
            <TemplatePageHeader title='Planos e histórico de faturas' isLoading={isLoading}>
            </TemplatePageHeader>
        )
    }

    if (me && !isLoading && !me.currentMainCompany) {
        return (
            <TemplatePageHeader title='Sem acesso'>
                <CardSimple
                    img={SvgError}
                    title={`Parado aí, ${handleGetFirstName(me?.userName)}!`}
                    description='Parece que você não está vinculado a nenhuma empresa no momento.<br/>Não perca mais tempo! Cadastre a sua própria empresa clicando no botão abaixo:'
                    buttonLabel='Cadastrar sua empresa agora mesmo'
                    buttonFunction={() => router.push(ROUTES.EMPRESA_GERENCIAR)}
                />
            </TemplatePageHeader>
        )
    }

    return (
        <section className={styles.main}>
            <Tabs
                tabs={['Planos', 'Histórico de faturas']}
                contents={[
                    <EmpresaUsoEPlanoTabPlanos me={me} key={1} />,
                    <EmpresaUsoEPlanoTabFaturas me={me} key={2} />
                ]}
                isBig={true}
            />
        </section>
    )
}