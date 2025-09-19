'use client';
import { HELP_TOPICS, iAjudaTopico } from '@/app/(routes)/(no-layout)/(etc)/ajuda/page';
import styles from '@/app/(routes)/(no-layout)/(etc)/ajuda/page.module.scss';
import ImgMeditation from '@/app/assets/webp/meditation.webp';
import SYSTEM from '@/app/consts/system';
import handleNormalizeUrl, { handleNormalizeHtml } from '@/app/functions/format.url';
import useTitle from '@/app/hooks/useTitle';
import Image from 'next/image';
import { useRouter, useSearchParams } from 'next/navigation';
import { useEffect, useState } from 'react';

export default function AjudaBusca() {

    useTitle('Central de ajuda');
    const router = useRouter();
    const searchParams = useSearchParams();
    const query = searchParams.get('q');

    const [filteredTopics, setFilteredTopics] = useState<iAjudaTopico[]>();

    useEffect(() => {
        if (query) {
            const filtered = HELP_TOPICS.filter(x =>
                handleNormalizeUrl(handleNormalizeHtml(x?.topic)) === query.toString()
            ) as iAjudaTopico[];

            setFilteredTopics(filtered);
        }
    }, [query]);

    return (
        <section className={styles.main}>
            <div className={styles.hero}>
                <span>Central de ajuda</span>

                <div className={SYSTEM.ANIMATE_PULSE_INFINITE}>
                    <Image src={ImgMeditation} alt='' priority={true} />
                </div>
            </div>

            {/* <AjudaSearchInput key={q?.toString()} /> */}

            {/* <div>
                <span className='title'>Resultado da busca</span>
                <br />
                <span className={styles.textoPequeno}> {listaAjudasItens?.length ?? 0} {listaAjudasItens?.length === 1 ? 'resultado' : 'resultados'}</span>
            </div>
            <span className='margem2'></span>

            <AjudaListaItens
                listaAjudasItens={listaAjudasItens}
                queryBuscada={queryBuscada}
                isMargemTop={false}
                isExibirTopico={true}
            /> */}
        </section>
    )
}