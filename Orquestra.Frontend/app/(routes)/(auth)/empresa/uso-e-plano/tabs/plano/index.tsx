'use client';
import { iMe } from '@/app/api/consts/auth';
import { CONSTS_COMPANY } from '@/app/api/consts/company';
import { CONSTS_UTILITY, iPlanType, iPlanTypeOutput } from '@/app/api/consts/utility';
import { Fetch } from '@/app/api/fetch';
import CardSimpleWithChildren from '@/app/components/card/simple-with-children';
import swal from '@/app/functions/swal';
import useApiRequestToSetterOnUrlChange from '@/app/hooks/api/useApiRequestToSetterOnUrlChange';
import Link from 'next/link';
import { useState } from 'react';
import styles from './index.module.scss';

interface iProps {
    me: iMe;
}

export default function EmpresaUsoEPlanoTabPlano({ me }: iProps) {

    const [plans, setPlans] = useState<iPlanTypeOutput | undefined>();
    useApiRequestToSetterOnUrlChange<iPlanTypeOutput>({ apiUrlRequest: CONSTS_UTILITY.getPlanType, setter: setPlans });

    return (
        <section className={styles.main}>
            <CardSimpleWithChildren style={{ backgroundColor: 'var(--cream)' }}>
                <Plans me={me} plans={plans} />
            </CardSimpleWithChildren>
        </section>
    )
}

function Plans({ me, plans }: { me: iMe | undefined, plans: iPlanTypeOutput | undefined }) {

    const messageInvoice = 'Ao confirmar, uma nova fatura será gerada automaticamente e o plano atual, caso exista, será substituído.';

    async function handleChooseNewPlan(plan: iPlanType) {
        swal({
            content: `Você tem certeza de que deseja prosseguir com a aquisição do plano <b>${plan.planTypeDescription.toLocaleLowerCase()}</b>?
                <br/><br/>${messageInvoice}`,
            icon: 'success',
            mustConfirm: true,
            checkboxLabel: 'Li e estou de acordo',
            confirmBtnText: 'Confirmar',
            confirmFunction: async () => {
                const formDataInput = new FormData();
                formDataInput.append('CompanyId', me?.currentMainCompany?.companyId?.toString()!);
                formDataInput.append('PlanType', plan.planType.toString());

                const output = await Fetch.put({ url: CONSTS_COMPANY.updatePlanType, body: formDataInput, isFormData: true });

                if (output) {
                    swal({
                        content: `Obrigado pela sua aquisição!<br/>Agora sua empresa está com o plano <b>${plan.planTypeDescription.toLocaleLowerCase()}</b> ativo.`,
                        confirmFunction: () => window.location.reload(),
                        icon: 'success'
                    });

                    return;
                }
            },
            cancelBtnText: 'Voltar'
        });
    }

    return (
        <section className={styles.pricingSection}>
            <div className={styles.header}>
                <h2 className={styles.title}>Planos que crescem com você</h2>
                <p className={styles.subtitle}>Sem contratos. Cancele quando quiser. Lembrando que, {messageInvoice.toLowerCase()}</p>
            </div>

            <div className={styles.grid}>
                {
                    plans?.plans?.map((p, i) => {
                        const isCurrentPlan = p.planType.toString() === me?.currentMainCompany?.planType?.toString();
                        const isFree = p.planTypeName === 'Free';

                        return (
                            <div key={i} className={`${styles.cardWrapper} ${(isFree && 'notAllowed')}`}>
                                {isCurrentPlan && <div className={styles.badge}>Plano atual</div>}

                                <div className={`${styles.card} ${styles.cardNormal}`}>
                                    <h3 className={styles.planHeading}>{p.planTypeDescription}</h3>
                                    <p className={styles.planSub}>{p.description}</p>

                                    <div className={styles.priceWrap}>
                                        <span className={styles.price}>R$ {p.price}</span>
                                        {!isFree && <span className={styles.pricePerMonth}>/mês</span>}
                                    </div>

                                    <ul className={styles.perksList}>
                                        {
                                            p.perks.map((perk) => (
                                                <li key={perk} className={styles.perkItem}>
                                                    <span className={styles.perkIcon}>✓</span>
                                                    <span className={styles.perkText}>{perk}</span>
                                                </li>
                                            ))
                                        }
                                    </ul>

                                    <Link
                                        href='#'
                                        className={`${styles.ctaButton} ${(isFree || isCurrentPlan) ? styles.disabled : ''}`}
                                        onClick={(isFree || isCurrentPlan) ? undefined : () => handleChooseNewPlan(p)}
                                    >
                                        {`Escolher plano ${p.planTypeDescription.toLocaleLowerCase()}`}
                                    </Link>
                                </div>
                            </div>
                        );
                    })
                }
            </div>
        </section>
    )
}