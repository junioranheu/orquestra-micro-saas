'use client';
import iMe, { CONSTS_AUTH } from '@/app/api/consts/auth';
import { CONSTS_COMPANY, iCalculatePriceModuleCompanyOutput } from '@/app/api/consts/company';
import { CONSTS_LOG } from '@/app/api/consts/log';
import { Fetch } from '@/app/api/fetch';
import CalendarSimple from '@/app/components/calendar/simple';
import Button from '@/app/components/input/button';
import { DATE_STYLE, handleFormatDate } from '@/app/functions/format.date';
import useApiGetMe from '@/app/hooks/api/useApiGetMe';
import { useIsRequestLoading } from '@/app/hooks/contexts/useGlobalContext';
import useUserContext from '@/app/hooks/contexts/useUserContext';
import useTitle from '@/app/hooks/useTitle';
import { useState } from 'react';
import styles from './page.module.scss';

export default function UsuarioConfiguracoes() {

    useTitle('Configurações');

    const [auth,] = useUserContext();
    const [isRequestLoading, setIsRequestLoading] = useIsRequestLoading();
    const me = useApiGetMe();
    const [modules, setModules] = useState<iCalculatePriceModuleCompanyOutput[]>([]);

    async function handleXD() {
        console.clear();
        const test = await Fetch.get({ url: CONSTS_AUTH.me }) as iMe;
        console.log(CONSTS_AUTH.me, test);

        const modules = await Fetch.get({ url: `${CONSTS_COMPANY.getModulesInfo}?companyId=${test.currentMainCompany.companyId}` }) as iCalculatePriceModuleCompanyOutput[];
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
            <h3>{me && me.currentMainCompany?.name}</h3>
            <h3>{me && me.currentMainCompany?.color}</h3>
            {auth && <h3>Refresh token válido até {handleFormatDate(auth?.refreshTokenExpirationDate, DATE_STYLE.DETALHADO)}</h3>}

            <br />
            <Button label={'/me'} handleFunction={() => handleXD()} isDisabled={isRequestLoading} />
            <br />
            <Button label={'/log'} handleFunction={() => handleLog()} isStyleSimple={true} isDisabled={isRequestLoading} />
            <br />

            <div>
                {
                    modules?.map((m, index) => (
                        <div key={index}>
                            <h2>{m.moduleStr}</h2>
                            <p>Já possui: {m.companyAlreadyHasThisModule ? 'Sim' : 'Não'}</p>
                            <p>Preço original: R$ {m.originalPrice.toFixed(2)}</p>
                            <p>Desconto: {m.discountPercentage}%</p>
                            <p>Preço com desconto: R$ {m.discountedPrice.toFixed(2)}</p>
                            <p>Preço proporcional: R$ {m.proportionalPrice.toFixed(2)}</p>
                        </div>
                    ))
                }
            </div>

            <hr />
            <br />
            <h1>Saco wea</h1>
            <br />
            <h1>Saco wea</h1>
            <br />
            <h1>Saco wea</h1>
            <br />
            <h1>Saco wea</h1>
            <br />
            <h1>Saco wea</h1>
            <br />
            <h1>Saco wea</h1>
            <br />
            <h1>Saco wea</h1>
            <br />
            <h1>Saco wea</h1>
            <br />
            <h1>Saco wea</h1>
            <br />
            <h1>Saco wea</h1>
            <br />
            <h1>Saco wea</h1>
            <br />
            <h1>Saco wea</h1>
            <br />
            <h1>Saco wea</h1>
            <br />

            <CalendarSimple isReadOnly={false} disablePastDays={true} />
        </section>
    )
}