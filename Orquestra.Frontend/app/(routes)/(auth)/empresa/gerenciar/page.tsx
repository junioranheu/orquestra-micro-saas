'use client';
import iCompanySimpleOutput, { CONSTS_COMPANY } from '@/app/api/consts/company';
import SvgUserArrow from '@/app/assets/svg/user-arrow.svg';
import CardSimple from '@/app/components/card/simple';
import Icon from '@/app/components/icon';
import ROUTES from '@/app/consts/routes';
import SYSTEM from '@/app/consts/system';
import { handleGetFirstName, handleGetNameInitials } from '@/app/functions/get.formatUserName';
import useApiGetMe from '@/app/hooks/api/useApiGetMe';
import useApiRequestToSetterOnUrlChange from '@/app/hooks/useApiRequestToSetterOnUrlChange';
import { Guid } from 'guid-typescript';
import { useRouter } from 'next/navigation';
import { useState } from 'react';
import styles from './page.module.scss';

export default function EmpresaGerenciar() {

    const router = useRouter();
    const me = useApiGetMe();

    const [companies, setCompanies] = useState<iCompanySimpleOutput[]>();
    useApiRequestToSetterOnUrlChange<iCompanySimpleOutput[]>({ apiUrlRequest: `${CONSTS_COMPANY.getAllByUserId}?userId=${me?.userId ?? Guid.EMPTY}`, setter: setCompanies });

    return (
        <section className={styles.main}>
            {
                (companies && companies?.length) ? (
                    <CardSimple
                        img={SvgUserArrow}
                        title={`${handleGetFirstName(me?.userName)}, atualmente você faz parte de ${companies?.length} empresa${companies?.length > 1 ? 's' : ''}.`}
                        description={`A sua empresa principal é a <b>${me?.currentMainCompany?.name}</b>.<br/>Caso exista essa possibilidade, você pode escolher abaixo uma outra empresa para ser a sua principal.<br/>Você também pode alterar essa escolha a qualquer momento!`}
                    />
                ) : (
                    <CardSimple
                        img={SvgUserArrow}
                        title={`${handleGetFirstName(me?.userName)}, tudo parece tão vazio por aqui...`}
                        description='Aparentemente você não faz parte de nenhuma empresa.<br/>Por que não cadastra a sua agora mesmo?'
                        buttonLabel={`Cadastrar sua empresa no ${SYSTEM.NAME}`}
                        buttonFunction={() => router.push(ROUTES.EMPRESA_CADASTRAR)}
                    />
                )
            }

            <div className={styles.grid}>
                {
                    companies?.sort((a, b) => a.name.localeCompare(b.name)).map((company) => (
                        <article key={company.companyId.toString()} className={styles.card}>
                            <header className={styles.header}>
                                <div className={styles.avatar}>
                                    {
                                        company.logoUrl ? (
                                            <img src={company.logoUrl} alt={company.name} />
                                        ) : (
                                            <span>{handleGetNameInitials(company.name)}</span>
                                        )
                                    }
                                </div>

                                <div className={styles.info}>


                                    <h3>{company.name}</h3>
                                    <p className={styles.type}>{company.companyType}</p>
                                </div>

                                {
                                    me?.currentMainCompany?.companyId == company.companyId && (
                                        <span className={styles.badge}>Principal</span>
                                    )
                                }
                            </header>

                            <div className={styles.content}>
                                <p>
                                    <Icon icon='mail' size='small' /> {company.email}
                                </p>

                                {
                                    company.planStartDate && company.planEndDate && (
                                        <p>
                                            <Icon icon='calendar' size='small' />{' '}
                                            {new Date(company.planStartDate).toLocaleDateString()} →{' '}
                                            {new Date(company.planEndDate).toLocaleDateString()}
                                        </p>
                                    )
                                }

                                <p>
                                    <Icon icon='info' size='small' /> {company.companySituation}
                                </p>

                                <div className={styles.modules}>
                                    {
                                        company.modulesStr?.map((m) => (
                                            <span key={m} className={styles.module}>
                                                {m}
                                            </span>
                                        ))
                                    }
                                </div>
                            </div>
                        </article>
                    ))
                }
            </div>
        </section>
    )
}