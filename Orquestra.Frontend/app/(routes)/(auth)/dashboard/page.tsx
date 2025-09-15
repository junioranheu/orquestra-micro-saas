'use client';
import iMe, { CONSTS_AUTH } from '@/app/api/consts/auth';
import { CONSTS_COMPANY, iCalculatePriceModuleCompanyOutput } from '@/app/api/consts/company';
import { CONSTS_LOG } from '@/app/api/consts/log';
import { Fetch } from '@/app/api/fetch';
import Button from '@/app/components/input/button';
import ROUTES from '@/app/consts/routes';
import { DATE_STYLE, handleFormatDate } from '@/app/functions/format.date';
import useApiGetMe from '@/app/hooks/api/useApiGetMe';
import { useIsRequestLoading } from '@/app/hooks/contexts/useGlobalContext';
import useUserContext from '@/app/hooks/contexts/useUserContext';
import useTitle from '@/app/hooks/useTitle';
import { useRouter } from 'next/navigation';
import { useState } from 'react';
import styles from './page.module.scss';

export default function Dashboard() {

    useTitle('Dashboard');

    const router = useRouter();
    const [auth, setAuth] = useUserContext();
    const [isRequestLoading, setIsRequestLoading] = useIsRequestLoading();
    const me = useApiGetMe();
    const [modules, setModules] = useState<iCalculatePriceModuleCompanyOutput[]>([]);

    function handleLogout() {
        router.push(ROUTES.LOGOUT);
    }

    async function handleXD() {
        console.clear();
        const test = await Fetch.get({ url: CONSTS_AUTH.me }) as iMe;
        console.log(CONSTS_AUTH.me, test);

        const modules = await Fetch.get({ url: `${CONSTS_COMPANY.getModulesInfo}?companyId=${test.currentMainCompany.companyId}`, setIsRequestLoading: setIsRequestLoading }) as iCalculatePriceModuleCompanyOutput[];
        setModules(modules);
        console.log('modules', modules);
    }

    async function handleLog() {
        console.clear();
        const result = await Fetch.get({ url: CONSTS_LOG.get, setIsRequestLoading: setIsRequestLoading });
        console.log(CONSTS_LOG.get, result);
    }

    return (
        <section className={styles.main}>
            <h1>Olá... {auth?.fullName}</h1>
            {me && me.currentMainCompany?.name}
            {auth && <h2>Refresh token válido até {handleFormatDate(auth?.refreshTokenExpirationDate, DATE_STYLE.DETALHADO)}</h2>}

            <br />
            <Button label={'/me'} handleFunction={() => handleXD()} />
            <br />
            <Button label={'/log'} handleFunction={() => handleLog()} />
            <br />
            <Button label={'Logout'} handleFunction={() => handleLogout()} />
            <br />
            <div className="space-y-4">
                {
                    modules?.map((m, index) => (
                        <div key={index} className="p-4 rounded-lg shadow bg-white">
                            <h2 className="text-lg font-semibold">{m.moduleStr}</h2>
                            <p>Já possui: {m.companyAlreadyHasThisModule ? "Sim" : "Não"}</p>
                            <p>Preço original: R$ {m.originalPrice.toFixed(2)}</p>
                            <p>Desconto: {m.discountPercentage}%</p>
                            <p>Preço com desconto: R$ {m.discountedPrice.toFixed(2)}</p>
                            <p>Preço proporcional: R$ {m.proportionalPrice.toFixed(2)}</p>
                        </div>
                    ))
                }
            </div>
            <br />
            <h1>a</h1>
            <br />
            <h1>a</h1>
            <br />
            <h1>a</h1>
            <br />
            <h1>a</h1>
            <br />
            <h1>a</h1>
            <br />
            <h1>a</h1>
            <br />
            <h1>a</h1>
            <br />
            <h1>a</h1>
            <br />
            <h1>a</h1>
            <br />
            <h1>a</h1>
            <br />
            <h1>a</h1>
            <br />
            <h1>a</h1>
            <br />
            <h1>a</h1>
            <br />
            <h1>a</h1>
            <br />
            <h1>a</h1>
            <br />
            <h1>a</h1>
            <br />
            <h1>a</h1>
            <br />
            <h1>a</h1>
            <br />
            <h1>a</h1>
            <br />
            <h1>a</h1>
            <br />
            <h1>a</h1>
            <br />
            <h1>a</h1>
            <br />
            <h1>a</h1>
            <br />
            <h1>a</h1>
            <br />
            <h1>a</h1>
            <br />
            <h1>a</h1>
            <br />
            <h1>a</h1>
            <br />
        </section>
    )
}