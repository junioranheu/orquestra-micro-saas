'use client';
import { CONSTS_COMPANY_INVOICE, iCompanyInvoice } from '@/app/api/consts/company-invoice';
import { Fetch } from '@/app/api/fetch';
import Tabs from '@/app/components/tabs';
import swal from '@/app/functions/swal';
import toast from '@/app/functions/toast';
import useApiGetMe from '@/app/hooks/api/useApiGetMe';
import useTitle from '@/app/hooks/useTitle';
import styles from './page.module.scss';
import EmpresaUsoEPlanoTabFaturas from './tabs/faturas';
import EmpresaUsoEPlanoTabPlano from './tabs/plano';

export default function EmpresaUsoEPlano() {

    useTitle('Plano e faturas');

    const me = useApiGetMe({});

    async function handlePay(e: iCompanyInvoice) {
        const pendingPayment = 1; // De acordo com o back-end (CompanySituationEnum);

        if (e.companyInvoiceSituation.toString() !== pendingPayment.toString()) {
            swal({
                content: 'Apenas faturas <b>pendentes</b> podem ser pagas.',
                icon: 'warning'
            });

            return;
        }

        const schedule = await Fetch.put({ url: `${CONSTS_COMPANY_INVOICE.pay}/${e.companyInvoiceId}` });

        if (schedule) {
            swal({
                content: 'Fatura foi paga com sucesso.',
                confirmFunction: () => window.location.reload(),
                icon: 'success'
            });

            return;
        }

        toast({ content: 'Não foi possível pagar esta fatura. Tente novamente mais tarde.' });
    }

    if (!me) {
        return null;
    }

    return (
        <section className={styles.main}>
            <Tabs
                tabs={['Planos', 'Histórico de faturas']}
                contents={[
                    <EmpresaUsoEPlanoTabPlano me={me} handlePay={handlePay} key={1} />,
                    <EmpresaUsoEPlanoTabFaturas me={me} handlePay={handlePay} key={2} />
                ]}
            />
        </section>
    )
}