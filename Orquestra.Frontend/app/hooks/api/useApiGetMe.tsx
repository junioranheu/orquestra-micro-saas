import { CONSTS_AUTH, iMe } from '@/app/api/consts/auth';
import { Fetch } from '@/app/api/fetch';
import ROUTES from '@/app/consts/routes';
import swal from '@/app/functions/swal';
import { usePathname, useRouter } from 'next/navigation';
import { useEffect, useState } from 'react';

interface iProps {
    isFetch?: boolean;
    trigger?: Date | undefined;
}

export default function useApiGetMe({ isFetch = true, trigger = undefined }: iProps): iMe | undefined {

    const [me, setMe] = useState<iMe>();
    const router = useRouter();
    const pathname = usePathname();

    useEffect(() => {
        async function handleFetch() {
            const pendingPayment = 1; // De acordo com o back-end (CompanySituationEnum);
            const result = await Fetch.get({ url: CONSTS_AUTH.me }) as iMe;
            // console.log(result);     

            // Verificar situação da empresa;
            if (
                result?.currentMainCompany?.companySituation?.toString() === pendingPayment.toString() &&
                (pathname !== ROUTES.EMPRESA_USO_E_PLANO && pathname !== ROUTES.EMPRESA_GERENCIAR && pathname !== ROUTES.EMPRESA_VERIFICADA && pathname !== ROUTES.LOGOUT)
            ) {
                swal({
                    content: result?.isUserAdmOfCurrentMainCompany ? 'A situação atual da empresa é <b>pendente de pagamento</b>. Isso significa que existe pelo menos uma fatura em aberto. Por favor, regularize essa pendência antes de continuar.' : 'A situação atual da empresa é <b>pendente de pagamento</b>. Isso significa que existe pelo menos uma fatura em aberto. Por favor, peça para um administrador regularizar essa pendência antes de continuar.',
                    confirmBtnText: result?.isUserAdmOfCurrentMainCompany ? 'Regularizar pendência' : 'Voltar',
                    confirmFunction: result?.isUserAdmOfCurrentMainCompany ? () => router.push(ROUTES.EMPRESA_USO_E_PLANO) : () => router.push(ROUTES.EMPRESA_GERENCIAR),
                    icon: 'error'
                });
            }

            setMe(result);
        }

        if (isFetch) {
            handleFetch();
        }
    }, [isFetch, trigger, pathname, router]);

    return me;

}