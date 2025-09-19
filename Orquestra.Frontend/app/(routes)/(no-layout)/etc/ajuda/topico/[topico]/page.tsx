'use client';
import AjudaListRows from '@/app/(routes)/(no-layout)/etc/ajuda/item/list-rows';
import { HELP_TOPICS, iAjudaTopico } from '@/app/(routes)/(no-layout)/etc/ajuda/page';
import Seta from '@/app/components/svg/seta/seta';
import ROUTES from '@/app/consts/routes';
import handleNormalizeUrl, { handleNormalizeHtml } from '@/app/functions/format.url';
import useTitle from '@/app/hooks/useTitle';
import { useParams, useRouter } from 'next/navigation';
import { useEffect, useState } from 'react';
import styles from './page.module.scss';

export default function AjudaTopico() {

    useTitle('Central de ajuda');
    const router = useRouter();
    const params = useParams();
    const query = params.topico;

    const [filteredTopic, setFilteredTopic] = useState<iAjudaTopico>();
    const [queryNormalized, setQueryNormalized] = useState<string>('');

    useEffect(() => {
        if (query) {
            const filtered = HELP_TOPICS.find(x =>
                handleNormalizeUrl(handleNormalizeHtml(x?.topic)) === query.toString()
            ) as iAjudaTopico;

            setFilteredTopic(filtered);
            setQueryNormalized(filtered?.topic ?? '');
        }
    }, [query]);

    return (
        <section className={styles.main}>
            <div className={`${styles.back} contrastOnHover`} onClick={() => router.push(ROUTES.ETC_AJUDA)}>
                <Seta />
                <span className='pointer'>Voltar à central de ajuda</span>
            </div>

            <div>
                <div className={styles.title} dangerouslySetInnerHTML={{ __html: queryNormalized }} />
            </div>

            <AjudaListRows filteredTopic={filteredTopic} />
        </section>
    )
}