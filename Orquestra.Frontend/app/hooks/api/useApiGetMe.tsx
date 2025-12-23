import { CONSTS_AUTH, iMe } from '@/app/api/consts/auth';
import { Fetch } from '@/app/api/fetch';
import ROUTES from '@/app/consts/routes';
import SYSTEM from '@/app/consts/system';
import { DATE_STYLE, handleFormatDate } from '@/app/functions/format.date';
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

            const result = await Fetch.get({ url: CONSTS_AUTH.me }) as iMe;
            // console.log(result);     

            const conditions = [
                result?.currentMainCompany?.companySituation?.toString() === SYSTEM.COMPANY_SITUATION_PENDING_PAYMENT.toString(),
                pathname !== ROUTES.EMPRESA_USO_E_PLANO,
                pathname !== ROUTES.EMPRESA_GERENCIAR,
                pathname !== ROUTES.EMPRESA_VERIFICADA,
                pathname !== ROUTES.LOGOUT
            ];

            if (conditions.every(x => x)) {
                const planEndDate = result.currentMainCompany?.planEndDate;
                const formattedDate = planEndDate ? handleFormatDate(planEndDate, DATE_STYLE.DIA_MES_ANO) : null;

                swal({
                    content: result?.isUserAdmOfCurrentMainCompany
                        ? `A sua empresa <b>não tem nenhum plano ativo</b>.${formattedDate ? ` O último plano expirou em ${formattedDate}.` : ''}<br/><br/>Ative um plano para regularizar a pendência.`
                        : `A empresa <b>não tem nenhum plano ativo</b> e não tem nenhum plano ativo.${formattedDate ? ` O último plano expirou em ${formattedDate}.` : ''}<br/><br/>Peça para um administrador regularizar a pendência ativando um plano.`,
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